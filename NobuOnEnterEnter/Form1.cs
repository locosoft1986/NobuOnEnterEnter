using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NobuOnEnterEnter.NobuEnterEnter;
using static System.Windows.Forms.DataFormats;

namespace NobuOnEnterEnter
{
    public partial class NobuEnterEnter : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // Store window handles and titles
        private List<WindowInfo> capturedWindows = new List<WindowInfo>();
        // Windows message constants
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;
        private ComponentResourceManager resources;
        private bool isInitializing = true;
        public NobuEnterEnter()
        {
            InitializeComponent();
            InitializeLanguageComboBox();
            resources = new ComponentResourceManager(typeof(NobuEnterEnter));

            isInitializing = false;
            // Disable remove button initially
            removeWindow.Enabled = false;
            startStopOneWindow.Enabled = false;
            startStopOneWindow.Text = resources.GetString("startStopOneWindow.Text");

            delayInMS.Minimum = 300;  // Minimum 300ms
            delayInMS.Maximum = 5000; // Maximum 5 seconds
            delayInMS.Value = 500;    // Default 500ms
            delayInMS.Increment = 100;

            // Subscribe to ListBox selection changed event
            windowList.SelectedIndexChanged += ListBoxWindows_SelectedIndexChanged;
        }

        public static bool IsRunningAsAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            Console.WriteLine($"Running as Administrator: {isAdmin}");
            return isAdmin;
        }
        private async void addWindow_Click(object sender, EventArgs e)
        {
            // Disable button
            addWindow.Enabled = false;
            string originalText = addWindow.Text;

            try
            {
                // Countdown from 10 to 1
                for (int i = 10; i >= 1; i--)
                {
                    string prefixTranslation = resources.GetString("waitWindowSelection.Text");
                    addWindow.Text = $"{prefixTranslation} [{i}s]";
                    await Task.Delay(1000); // Wait 1 second
                }

                // Capture the foreground window
                IntPtr windowHandle = GetForegroundWindow();

                if (windowHandle != IntPtr.Zero)
                {
                    string windowTitle = GetWindowTitle(windowHandle);

                    if (!string.IsNullOrEmpty(windowTitle))
                    {
                        // Check if window already exists
                        bool exists = capturedWindows.Exists(w => w.Handle == windowHandle);

                        if (!exists)
                        {
                            // Add to list
                            WindowInfo windowInfo = new WindowInfo
                            {
                                Handle = windowHandle,
                                Title = windowTitle
                            };

                            capturedWindows.Add(windowInfo);

                            // Update ListBox
                            windowList.Items.Add(windowInfo);
                        }
                        else
                        {
                            MessageBox.Show(resources.GetString("Modal.DuplicateWindow.Content"),
                                resources.GetString("Modal.DuplicateWindow.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(resources.GetString("Modal.Error.Content.CanNotGetWindowTitle"),
                            resources.GetString("Modal.Error.Title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show(resources.GetString("Modal.Error.NotDetectAnyWindow.Content"),
                        resources.GetString("Modal.Error.Title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    resources.GetString("Modal.Error.Title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable button and restore text
                addWindow.Text = originalText;
                addWindow.Enabled = true;
            }
        }



        private void removeWindow_Click(object sender, EventArgs e)
        {
            if (windowList.SelectedIndex != -1)
            {
                int index = windowList.SelectedIndex;
                WindowInfo windowInfo = capturedWindows[index];
                if (capturedWindows[index].IsRunning)
                {
                    StopSendingKeys(windowInfo);
                }

                capturedWindows.RemoveAt(index);
                windowList.Items.RemoveAt(index);
            }
        }

        private void startAllWindow_Click(object sender, EventArgs e)
        {
            // Stop all running windows
            foreach (var window in capturedWindows)
            {
                if (!window.IsRunning)
                {
                    StartSendingKeys(window);
                }
            }
            // Refresh ListBox to show [Running] prefix
            RefreshListBox();
        }

        private void startStopOneWindow_Click(object sender, EventArgs e)
        {
            if (windowList.SelectedIndex == -1)
                return;

            WindowInfo selectedWindow = capturedWindows[windowList.SelectedIndex];

            if (selectedWindow.IsRunning)
            {
                // Stop
                StopSendingKeys(selectedWindow);
            }
            else
            {
                // Start
                StartSendingKeys(selectedWindow);
            }

            // Update button states
            UpdateButtonStates();

            // Refresh ListBox to show [Running] prefix
            RefreshListBox();
        }

        private void stopAllWindow_Click(object sender, EventArgs e)
        {
            // Stop all running windows
            foreach (var window in capturedWindows)
            {
                if (window.IsRunning)
                {
                    StopSendingKeys(window);
                }
            }
            // Refresh ListBox to show [Running] prefix
            RefreshListBox();
            UpdateButtonStates();
        }

        private void ListBoxWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
        private void UpdateButtonStates()
        {
            // Enable remove button only when an item is selected
            bool hasSelection = (windowList.SelectedIndex != -1);

            if (hasSelection)
            {
                WindowInfo selectedWindow = capturedWindows[windowList.SelectedIndex];

                // Update remove button
                removeWindow.Enabled = true;

                // Update start/stop button based on selected window's state
                startStopOneWindow.Enabled = true;

                if (selectedWindow.IsRunning)
                {
                    startStopOneWindow.Text = resources.GetString("stopSelectedWindow.Text");
                    startStopOneWindow.BackColor = Color.LightCoral;

                    // Show current interval for running window
                    delayInMS.Value = selectedWindow.Interval;
                }
                else
                {
                    startStopOneWindow.Text = resources.GetString("startStopOneWindow.Text");
                    startStopOneWindow.BackColor = SystemColors.Control;
                }
            }
            else
            {
                // No selection
                removeWindow.Enabled = false;
                startStopOneWindow.Enabled = false;
                startStopOneWindow.Text = resources.GetString("startStopOneWindow.Text");
                startStopOneWindow.BackColor = SystemColors.Control;
            }

        }

        // Helper method to get window title
        private string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length == 0) return string.Empty;

            StringBuilder builder = new StringBuilder(length + 1);
            GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        private async void StartSendingKeys(WindowInfo windowInfo)
        {
            if (windowInfo.IsRunning)
                return;

            // Check if window still exists
            if (!IsWindowVisible(windowInfo.Handle))
            {
                MessageBox.Show(resources.GetString("Modal.WindowNotFound.Content"),
                   resources.GetString("Modal.Warning.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Update window state
            windowInfo.IsRunning = true;
            windowInfo.Interval = (int)delayInMS.Value;
            windowInfo.CancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = windowInfo.CancellationTokenSource.Token;

            try
            {
                await Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        SendEnterKey(windowInfo.Handle);
                        await Task.Delay(windowInfo.Interval, token);
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                // Normal cancellation
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Clean up when task ends
                windowInfo.IsRunning = false;
                windowInfo.CancellationTokenSource?.Dispose();
                windowInfo.CancellationTokenSource = null;

                // Update UI if this window is still selected
                if (windowList.SelectedIndex != -1 &&
                    capturedWindows[windowList.SelectedIndex] == windowInfo)
                {
                    this.Invoke(new Action(() =>
                    {
                        UpdateButtonStates();
                        RefreshListBox();
                    }));
                }
            }
        }

        private void StopSendingKeys(WindowInfo windowInfo)
        {
            if (!windowInfo.IsRunning)
                return;

            // Cancel the task
            windowInfo.CancellationTokenSource?.Cancel();

            // State will be cleaned up in the finally block of StartSendingKeys
        }
        private void InitializeLanguageComboBox()
        {
            // Clear existing items
            cmbLanguage.Items.Clear();

            // Add language options
            cmbLanguage.Items.Add(new LanguageItem("繁體中文", "zh-TW"));
            cmbLanguage.Items.Add(new LanguageItem("日本語", "ja-JP"));

            // Load saved language or default to Traditional Chinese
            string savedLanguage = Settings.Default.Language;
            if (string.IsNullOrEmpty(savedLanguage))
            {
                savedLanguage = "zh-TW"; // Default to Traditional Chinese
            }

            // Set the selected language
            SelectLanguage(savedLanguage);

            // Attach event handler
            cmbLanguage.SelectedIndexChanged += cmbLanguage_SelectedIndexChanged;
        }

        private void ChangeLanguage(string cultureName)
        {
            // Save preference
            Settings.Default.Language = cultureName;
            Settings.Default.Save();

            // Set culture
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            // Restart application
            Application.Restart();
            Environment.Exit(0);
        }

        private void SelectLanguage(string cultureCode)
        {
            for (int i = 0; i < cmbLanguage.Items.Count; i++)
            {
                LanguageItem item = (LanguageItem)cmbLanguage.Items[i];
                if (item.CultureCode == cultureCode)
                {
                    cmbLanguage.SelectedIndex = i;
                    break;
                }
            }
        }
        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Skip if initializing
            if (isInitializing) return;

            if (cmbLanguage.SelectedItem is LanguageItem selectedLanguage)
            {
                ChangeLanguage(selectedLanguage.CultureCode);
            }
        }
        private void RefreshListBox()
        {
            // Refresh ListBox display to update [Running] prefix
            int selectedIndex = windowList.SelectedIndex;
            windowList.Items.Clear();

            foreach (var window in capturedWindows)
            {
                windowList.Items.Add(window);
            }

            // Restore selection
            if (selectedIndex >= 0 && selectedIndex < windowList.Items.Count)
            {
                windowList.SelectedIndex = selectedIndex;
            }
        }


        private void SendEnterKey(IntPtr hWnd)
        {
            bool isWind = IsWindow(hWnd);
            Console.WriteLine($"Is Valid window handle: {IsWindow(hWnd)}");
            if (hWnd == IntPtr.Zero)
                return;

            bool result1 = false;
            bool result2 = false;
            try
            {
                // Method 1: Using PostMessage (works for most applications)
                result1 = PostMessage(hWnd, WM_KEYDOWN, VK_RETURN, 0x001C001);
                Console.WriteLine($"WM_KEYDOWN posted: {result1}");

                if (!result1)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Error code1: {error}");
                }
                Thread.Sleep(50); // Small delay between key down and up
                result2 = PostMessage(hWnd, WM_KEYUP, VK_RETURN, unchecked((int)0xC01C0001));
                Console.WriteLine($"WM_KEYUP posted: {result2}");

                if (!result2)
                {
                    int error = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Error code2: {error}");
                }

                //Alternative Method 2: Using SendMessage (more reliable but slower)
                //SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_RETURN, 0x001C001);
                //Thread.Sleep(100);
                //SendMessage(hWnd, WM_KEYUP, (IntPtr)VK_RETURN, unchecked((int)0xC01C0001));
            }
            catch (Exception ex)
            {
                // Log error but don't stop the loop
                System.Diagnostics.Debug.WriteLine($"Error sending key: {ex.Message}");
            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop all running windows
            foreach (var window in capturedWindows)
            {
                if (window.IsRunning)
                {
                    StopSendingKeys(window);
                }
            }

            base.OnFormClosing(e);
        }

        public class WindowInfo
        {
            public IntPtr Handle { get; set; }
            public string Title { get; set; }

            public bool IsRunning { get; set; }
            public CancellationTokenSource CancellationTokenSource { get; set; }
            public int Interval { get; set; }
            public WindowInfo()
            {
                IsRunning = false;
                CancellationTokenSource = null;
                Interval = 300;
            }
            public override string ToString()
            {
                string RunningState = IsRunning ? $" (起動中, {Interval}ms)" : "";
                return $"{Title}{RunningState}";
            }
        }

        private class LanguageItem
        {
            public string DisplayName { get; set; }
            public string CultureCode { get; set; }

            public LanguageItem(string displayName, string cultureCode)
            {
                DisplayName = displayName;
                CultureCode = cultureCode;
            }

            public override string ToString()
            {
                return DisplayName;
            }
        }

        private void NobuEnterEnter_Load(object sender, EventArgs e)
        {
            if (!IsRunningAsAdmin())
            {
                MessageBox.Show(resources.GetString("Modal.NoAdminWarning.Content"),
                                resources.GetString("Modal.Warning.Title"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }
    }
}
