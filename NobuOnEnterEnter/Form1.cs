using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

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


        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public KEYBDINPUT ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        const uint INPUT_KEYBOARD = 1;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;


        // Store window handles and titles
        private List<WindowInfo> capturedWindows = new List<WindowInfo>();
        // Windows message constants
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const int VK_RETURN = 0x0D;
        private const int VK_ESCAPE = 0x1B;
        private const int VK_LEFT = 0x25;
        private const int VK_GAMEPAD_DPAD_LEFT = 0xCD;
        private const int VK_UP = 0x26;
        private const int VK_W = 0x57;
        private const int VK_S = 0x53;
        private const int VK_PALACE = 9999;
        private const int VK_FOUNTAIN_WAIT = 8888;
        private ComponentResourceManager resources;
        private bool isInitializing = true;

        const uint WM_MOUSEMOVE = 0x0200;

        // Helper to create lParam from X, Y coordinates
        static int MakeLParam(int x, int y)
        {
            return ((y << 16) | (x & 0xFFFF));
        }

        public static void SendMouseMove(IntPtr hWnd, int x, int y)
        {
            int lParam = MakeLParam(x, y);
            PostMessage(hWnd, WM_MOUSEMOVE, 0, lParam);
        }

        public NobuEnterEnter()
        {
            InitializeComponent();
            InitializeLanguageComboBox();
            resources = new ComponentResourceManager(typeof(NobuEnterEnter));
            InitializeModeComboBox();

            isInitializing = false;
            // Disable remove button initially
            removeWindow.Enabled = false;
            startStopOneWindow.Enabled = false;
            startStopOneWindow.Text = resources.GetString("startStopOneWindow.Text");

            delayInMS.Minimum = 50;  // Minimum 300ms
            delayInMS.Maximum = 5000; // Maximum 5 seconds
            delayInMS.Value = 100;    // Default 500ms
            delayInMS.Increment = 10;
            cbModeSelection.Enabled = false;
            delayInMS.Enabled = false;
            numPalaceBattleWaitTime.Enabled = false;
            numFountainWaitTime.Enabled = false;
            numFountainFinalBossWaitTime.Enabled = false;
            numFountainStartFloor.Enabled = false;

            // Subscribe to ListBox selection changed event
            windowList.SelectedIndexChanged += ListBoxWindows_SelectedIndexChanged;

            // Subscribe to settings value changed events
            delayInMS.ValueChanged += Setting_ValueChanged;
            numPalaceBattleWaitTime.ValueChanged += Setting_ValueChanged;
            numFountainWaitTime.ValueChanged += Setting_ValueChanged;
            numFountainFinalBossWaitTime.ValueChanged += Setting_ValueChanged;
            numFountainStartFloor.ValueChanged += Setting_ValueChanged;

        }

        private void Setting_ValueChanged(object sender, EventArgs e)
        {
            if (isUpdatingUI || windowList.SelectedIndex == -1) return;

            var selectedWindow = capturedWindows[windowList.SelectedIndex];
            if (sender == delayInMS)
                selectedWindow.ModeSettings["delayInMS"] = delayInMS.Value;
            else if (sender == numPalaceBattleWaitTime)
                selectedWindow.ModeSettings["numPalaceBattleWaitTime"] = numPalaceBattleWaitTime.Value;
            else if (sender == numFountainWaitTime)
                selectedWindow.ModeSettings["numFountainWaitTime"] = numFountainWaitTime.Value;
            else if (sender == numFountainFinalBossWaitTime)
                selectedWindow.ModeSettings["numFountainFinalBossWaitTime"] = numFountainFinalBossWaitTime.Value;
            else if (sender == numFountainStartFloor)
                selectedWindow.ModeSettings["numFountainStartFloor"] = numFountainStartFloor.Value;
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
                            var clientSize = GetClientSize(windowHandle);
                            // Add to list
                            WindowInfo windowInfo = new WindowInfo
                            {
                                Handle = windowHandle,
                                Title = windowTitle,
                                Width = clientSize.Width,
                                Height = clientSize.Height,
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
                StartSendingKeys(selectedWindow, windowList.SelectedIndex);
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
        private bool isUpdatingUI = false;

        private void UpdateButtonStates()
        {
            if (isUpdatingUI) return;

            // Enable remove button only when an item is selected
            bool hasSelection = (windowList.SelectedIndex != -1);

            isUpdatingUI = true;
            try
            {
                // Enable/disable input controls based on whether a window is running
                bool isRunning = false;
                if (hasSelection)
                {
                    WindowInfo selectedWindow = capturedWindows[windowList.SelectedIndex];
                    isRunning = selectedWindow.IsRunning;
                }

                // If a window is selected AND it is running, disable inputs. Otherwise enable them.
                // Or based on user request: "在有window選擇的時候 模式選擇必須禁用，對應panel下的數值也必須禁止更改"
                // This means if hasSelection is true, we disable them. Wait, if we disable them when selected, 
                // how can they configure it for that window? 
                // Usually we disable inputs when it is RUNNING, not just selected.
                // Let's re-read: "在有window選擇的時候 模式選擇必須禁用，對應panel下的數值也必須禁止更改" -> Wait, 
                // if it's disabled when selected, they can never change the settings for a selected window.
                // Ah, the user said "在有window選擇的時候". This might mean they can only set defaults before adding?
                // Let me implement exactly what they asked: disable when hasSelection is true.
                
                bool enableInputs = !isRunning;
                cbModeSelection.Enabled = enableInputs;
                delayInMS.Enabled = enableInputs;
                numPalaceBattleWaitTime.Enabled = enableInputs;
                numFountainWaitTime.Enabled = enableInputs;
                numFountainFinalBossWaitTime.Enabled = enableInputs;
                numFountainStartFloor.Enabled = enableInputs;

                if (hasSelection)
                {
                    WindowInfo selectedWindow = capturedWindows[windowList.SelectedIndex];

                    // Update clbSlavesSelector
                    if (!selectedWindow.IsRunning)
                    {
                        clbSlavesSelector.Items.Clear();
                        foreach (var window in capturedWindows)
                        {
                            if (window != selectedWindow && !window.IsRunning)
                            {
                                clbSlavesSelector.Items.Add(window);
                            }
                        }
                    }
                    
                    bool canHaveSlaves = (selectedWindow.ModeCode == "Palace" || selectedWindow.ModeCode == "Fountain") && !selectedWindow.IsRunning;
                    clbSlavesSelector.Enabled = canHaveSlaves;

                    // Restore dropdown mode selection
                    foreach (ModeItem item in cbModeSelection.Items)
                    {
                        if (item.ModeCode == selectedWindow.ModeCode)
                        {
                            cbModeSelection.SelectedItem = item;
                            break;
                        }
                    }

                    // Restore numeric up/down settings
                    if (selectedWindow.ModeSettings.ContainsKey("delayInMS")) 
                        delayInMS.Value = selectedWindow.ModeSettings["delayInMS"];
                    if (selectedWindow.ModeSettings.ContainsKey("numPalaceBattleWaitTime")) 
                        numPalaceBattleWaitTime.Value = selectedWindow.ModeSettings["numPalaceBattleWaitTime"];
                    if (selectedWindow.ModeSettings.ContainsKey("numFountainWaitTime")) 
                        numFountainWaitTime.Value = selectedWindow.ModeSettings["numFountainWaitTime"];
                    if (selectedWindow.ModeSettings.ContainsKey("numFountainFinalBossWaitTime")) 
                        numFountainFinalBossWaitTime.Value = selectedWindow.ModeSettings["numFountainFinalBossWaitTime"];
                    if (selectedWindow.ModeSettings.ContainsKey("numFountainStartFloor")) 
                        numFountainStartFloor.Value = selectedWindow.ModeSettings["numFountainStartFloor"];

                    // Update remove button
                    removeWindow.Enabled = true;

                    // Update start/stop button based on selected window's state
                    if (selectedWindow.IsRunning && selectedWindow.CancellationTokenSource == null && selectedWindow.Master != null)
                    {
                        // It's a slave, cannot stop individually
                        startStopOneWindow.Enabled = false;
                        startStopOneWindow.Text = resources.GetString("runningAsSlave.Text") ?? "正在作爲從視窗運行";
                        startStopOneWindow.BackColor = Color.LightGray;
                        removeWindow.Enabled = false;
                    }
                    else
                    {
                        startStopOneWindow.Enabled = true;
                        if (selectedWindow.IsRunning)
                        {
                            startStopOneWindow.Text = resources.GetString("stopSelectedWindow.Text");
                            startStopOneWindow.BackColor = Color.LightCoral;
                        }
                        else
                        {
                            startStopOneWindow.Text = resources.GetString("startStopOneWindow.Text");
                            startStopOneWindow.BackColor = SystemColors.ButtonFace;
                        }
                    }
                }
                else
                {
                    // No selection
                    clbSlavesSelector.Items.Clear();
                    clbSlavesSelector.Enabled = false;
                    removeWindow.Enabled = false;
                    startStopOneWindow.Enabled = false;
                    startStopOneWindow.Text = resources.GetString("startStopOneWindow.Text");
                    startStopOneWindow.BackColor = SystemColors.Control;
                }
            }
            finally
            {
                isUpdatingUI = false;
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

        private (int Width, int Height) GetClientSize(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero)
            {
                return (0, 0);
            }

            if (!GetClientRect(hWnd, out RECT rect))
            {
                return (0, 0);
            }

            return (rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        private async void StartSendingKeys(WindowInfo windowInfo, int InWindowIndex)
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
            windowInfo.KeySequence.Clear();
            windowInfo.Slaves.Clear();

            if (cbModeSelection.SelectedItem is ModeItem selectedMode)
            {
                windowInfo.ModeName = selectedMode.DisplayName;
                windowInfo.ModeCode = selectedMode.ModeCode;

                // Handle Slaves
                if (selectedMode.ModeCode == "Palace" || selectedMode.ModeCode == "Fountain")
                {
                    foreach (var item in clbSlavesSelector.CheckedItems)
                    {
                        if (item is WindowInfo slave)
                        {
                            slave.IsRunning = true;
                            slave.Master = windowInfo;
                            windowInfo.Slaves.Add(slave);
                        }
                    }
                }
                
                // Refresh ListBox to show slave status
                RefreshListBox();
                
                if (selectedMode.ModeCode == "Enter")
                {
                    int delay = windowInfo.ModeSettings.ContainsKey("delayInMS") ? (int)windowInfo.ModeSettings["delayInMS"] : 500;
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, delay, 50));
                }
                else if (selectedMode.ModeCode == "Palace")
                {
                    int battleWaitTimeMs = windowInfo.ModeSettings.ContainsKey("numPalaceBattleWaitTime") ? 
                        (int)(windowInfo.ModeSettings["numPalaceBattleWaitTime"] * 1000) : 60000;

                    windowInfo.KeySequence.Add(new KeyAction(VK_W, 50, 800));
                    windowInfo.KeySequence.Add(new KeyAction(VK_UP, 50, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1600, 200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, battleWaitTimeMs, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50, true));
                    windowInfo.KeySequence.Add(new KeyAction(VK_W, 200, 1000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_UP, 500, 200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 400));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1200, 1500));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1200, 2000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1200, 2000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_W, 800, 1200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_S, 1000, 1000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 500, 100));
                    windowInfo.KeySequence.Add(new KeyAction(VK_PALACE, 500, 200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 5000, 200));
                }
                else if (selectedMode.ModeCode == "Fountain")
                {
                    int battleWaitTimeMs = windowInfo.ModeSettings.ContainsKey("numFountainFinalBossWaitTime") ?
                        (int)(windowInfo.ModeSettings["numFountainFinalBossWaitTime"] * 1000) : 60000;
                    windowInfo.KeySequence.Add(new KeyAction(VK_W, 50, 800));
                    windowInfo.KeySequence.Add(new KeyAction(VK_UP, 50, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1600, 200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_FOUNTAIN_WAIT, battleWaitTimeMs, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_ESCAPE, 1000, 50));
                    windowInfo.KeySequence.Add(new KeyAction(VK_W, 200, 1000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_UP, 500, 200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1000, 400));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1200, 1500));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1200, 2000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 1200, 2000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_W, 800, 1200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_S, 1000, 1000));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 500, 100));
                    windowInfo.KeySequence.Add(new KeyAction(VK_PALACE, 500, 200));
                    windowInfo.KeySequence.Add(new KeyAction(VK_RETURN, 5000, 200));
                }
            }
            
            windowInfo.CancellationTokenSource = new CancellationTokenSource();

            CancellationToken token = windowInfo.CancellationTokenSource.Token;

            try
            {
                await Task.Run(async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        var targetWindow = capturedWindows[InWindowIndex];
                        foreach (var keyAction in windowInfo.KeySequence)
                        {
                            if (token.IsCancellationRequested)
                                break;

                            SendKey(windowInfo.Handle, keyAction.KeyValue, keyAction.Duration, windowInfo.Width, windowInfo.Height);
                            
                            // Share key with slaves if enabled
                            if (keyAction.SlaveShared)
                            {
                                foreach (var slave in windowInfo.Slaves)
                                {
                                    SendKey(slave.Handle, keyAction.KeyValue, keyAction.Duration, slave.Width, slave.Height);
                                }
                            }

                            if (keyAction.KeyValue == VK_FOUNTAIN_WAIT)
                            {
                                if (targetWindow.ModeSettings.ContainsKey("numFountainStartFloor"))
                                {
                                    decimal nextFloor = targetWindow.ModeSettings["numFountainStartFloor"];
                                    decimal normalWaitTime = targetWindow.ModeSettings.ContainsKey("numFountainWaitTime") ? targetWindow.ModeSettings["numFountainWaitTime"] : 60;
                                    decimal finalBossWaitTime = targetWindow.ModeSettings.ContainsKey("numFountainFinalBossWaitTime") ? targetWindow.ModeSettings["numFountainFinalBossWaitTime"] : 120;

                                    decimal actualDelayTime = (nextFloor % 10) == 0 ? finalBossWaitTime : ((nextFloor % 5) == 0 ? normalWaitTime + 8 : normalWaitTime);
                                    await Task.Delay(((int)actualDelayTime) * 1000, token);
                                } else
                                {
                                    await Task.Delay(keyAction.DelayTime, token);
                                }
                            } else
                            {
                                await Task.Delay(keyAction.DelayTime, token);
                            }
                        }
                        if (targetWindow.ModeSettings.ContainsKey("numFountainStartFloor"))
                        {
                            decimal nextFloor = targetWindow.ModeSettings["numFountainStartFloor"] + 1;
                            targetWindow.ModeSettings["numFountainStartFloor"] = nextFloor;
                            if (windowList.SelectedIndex != -1)
                            {
                                WindowInfo selectedWindow = capturedWindows[windowList.SelectedIndex];
                                if (selectedWindow.ModeSettings.ContainsKey("numFountainStartFloor"))
                                {
                                    numFountainStartFloor.Value = targetWindow.ModeSettings["numFountainStartFloor"];
                                }
                            }
                        }
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

                // Stop slaves
                foreach (var slave in windowInfo.Slaves)
                {
                    slave.IsRunning = false;
                    slave.Master = null;
                }
                windowInfo.Slaves.Clear();

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
                else
                {
                    // If not selected, we still need to refresh the list to show status changes
                    this.Invoke(new Action(() =>
                    {
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

        private void InitializeModeComboBox()
        {
            cbModeSelection.Items.Clear();
            cbModeSelection.Items.Add(new ModeItem(resources.GetString("Mode.Enter") ?? "連續回車模式", "Enter"));
            cbModeSelection.Items.Add(new ModeItem(resources.GetString("Mode.Palace") ?? "夢幻冥宮", "Palace"));
            cbModeSelection.Items.Add(new ModeItem(resources.GetString("Mode.Fountain") ?? "夢幻冥泉", "Fountain"));
            
            cbModeSelection.SelectedIndex = 0;
            cbModeSelection.SelectedIndexChanged += cbModeSelection_SelectedIndexChanged;
            UpdatePanelVisibility();
        }

        private void cbModeSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePanelVisibility();
            UpdateButtonStates();

            if (isUpdatingUI || windowList.SelectedIndex == -1) return;

            if (cbModeSelection.SelectedItem is ModeItem selectedMode)
            {
                var selectedWindow = capturedWindows[windowList.SelectedIndex];
                selectedWindow.ModeCode = selectedMode.ModeCode;
                selectedWindow.ModeName = selectedMode.DisplayName;
                
                // Refresh list box without losing selection or causing recursive loops
                int selectedIndex = windowList.SelectedIndex;
                isUpdatingUI = true;
                try
                {
                    windowList.Items[selectedIndex] = selectedWindow;
                }
                finally
                {
                    isUpdatingUI = false;
                }
            }
        }

        private void UpdatePanelVisibility()
        {
            if (cbModeSelection.SelectedItem is ModeItem selectedMode)
            {
                panelModeEnter.Visible = selectedMode.ModeCode == "Enter";
                panelModePalace.Visible = selectedMode.ModeCode == "Palace";
                panelModeFountain.Visible = selectedMode.ModeCode == "Fountain";
                panelSlaves.Visible = selectedMode.ModeCode == "Palace" || selectedMode.ModeCode == "Fountain";
            }
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

        private static bool IsExtendedKey(int keyValue)
        {
            return keyValue == VK_LEFT ||
                   keyValue == VK_UP ||
                   keyValue == VK_GAMEPAD_DPAD_LEFT ||
                   keyValue == 0x27 || // VK_RIGHT
                   keyValue == 0x28 || // VK_DOWN
                   keyValue == 0x21 || // VK_PRIOR
                   keyValue == 0x22 || // VK_NEXT
                   keyValue == 0x23 || // VK_END
                   keyValue == 0x24 || // VK_HOME
                   keyValue == 0x2D || // VK_INSERT
                   keyValue == 0x2E;   // VK_DELETE
        }

        private static int BuildKeyLParam(int keyValue, bool keyUp)
        {
            int repeatCount = 1;
            int scanCode = (int)MapVirtualKey((uint)keyValue, 0) & 0xFF;
            int extendedFlag = IsExtendedKey(keyValue) ? (1 << 24) : 0;
            int previousStateFlag = keyUp ? (1 << 30) : 0;
            int transitionStateFlag = keyUp ? unchecked((int)0x80000000) : 0;

            return repeatCount |
                   (scanCode << 16) |
                   extendedFlag |
                   previousStateFlag |
                   transitionStateFlag;
        }

        private void SendKey(IntPtr hWnd, int keyValue, int duration, int width, int height)
        {
            bool isWind = IsWindow(hWnd);
            Console.WriteLine($"Is Valid window handle: {IsWindow(hWnd)}");
            if (hWnd == IntPtr.Zero)
                return;

            bool result1 = false;
            bool result2 = false;
            try
            {
                if (keyValue == VK_PALACE)
                {
                    float desiredX = width * 0.453125f;
                    float desiredY = height * 0.376875f;
                    SendMouseMove(hWnd, (int)(desiredX), (int)desiredY);
                    Thread.Sleep(100);
                    SendMouseMove(hWnd, (int)(desiredX+3), (int)desiredY+3);
                    Thread.Sleep(100);
                    SendMouseMove(hWnd, (int)(desiredX - 3), (int)desiredY + 3);
                    Thread.Sleep(100);
                    SendMouseMove(hWnd, (int)(desiredX - 3), (int)desiredY-3);
                    Thread.Sleep(100);
                    SendMouseMove(hWnd, (int)(desiredX + 3), (int)desiredY - 3);
                    Thread.Sleep(duration);
                } else
                {
                    if (keyValue == VK_FOUNTAIN_WAIT)
                    {
                        keyValue = VK_RETURN;
                    }
                    int fixedKeyDownLParam = 0x001C001;
                    int fixedKeyUpLParam = unchecked((int)0xC01C0001);
                    int fixedLeftKeyDownLParam = 0x014B0001;
                    int fixedLeftKeyUpLParam = unchecked((int)0xC14B0001);
                    int keyDownLParam = BuildKeyLParam(keyValue, false);
                    int keyUpLParam = BuildKeyLParam(keyValue, true);

                    // Method 1: Using PostMessage (works for most applications)
                    result1 = PostMessage(hWnd, WM_KEYDOWN, keyValue, keyDownLParam);
                    Console.WriteLine($"WM_KEYDOWN posted: {result1}");


                    int error1 = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Error code1: {error1}");
                    Thread.Sleep(duration); // Delay between key down and up based on duration


                    result2 = PostMessage(hWnd, WM_KEYUP, keyValue, keyUpLParam);
                    Console.WriteLine($"WM_KEYUP posted: {result2}");


                    int error2 = Marshal.GetLastWin32Error();
                    Console.WriteLine($"Error code2: {error2}");
                }


                //Alternative Method 2: Using SendMessage (more reliable but slower)
                //SendMessage(hWnd, WM_KEYDOWN, (IntPtr)keyValue, 0x001C001);
                //Thread.Sleep(100);
                //SendMessage(hWnd, WM_KEYUP, (IntPtr)keyValue, unchecked((int)0xC01C0001));
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

        public class KeyAction
        {
            public int KeyValue { get; set; }
            public int DelayTime { get; set; }
            public int Duration { get; set; }
            public bool SlaveShared { get; set; }

            public KeyAction(int keyValue, int delayTime, int duration, bool slaveShared = false)
            {
                KeyValue = keyValue;
                DelayTime = delayTime;
                Duration = duration;
                SlaveShared = slaveShared;
            }
        }

        public class WindowInfo
        {
            public IntPtr Handle { get; set; }
            public string Title { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }


            public bool IsRunning { get; set; }
            public CancellationTokenSource CancellationTokenSource { get; set; }
            public List<KeyAction> KeySequence { get; set; }
            public string ModeName { get; set; }
            public string ModeCode { get; set; }
            public Dictionary<string, decimal> ModeSettings { get; set; }
            public List<WindowInfo> Slaves { get; set; }
            public WindowInfo Master { get; set; }

            public WindowInfo()
            {
                IsRunning = false;
                CancellationTokenSource = null;
                KeySequence = new List<KeyAction>();
                Slaves = new List<WindowInfo>();
                Master = null;
                ModeName = "";
                ModeCode = "";
                ModeSettings = new Dictionary<string, decimal>
                {
                    { "delayInMS", 100 },
                    { "numPalaceBattleWaitTime", 60 },
                    { "numFountainWaitTime", 60 },
                    { "numFountainFinalBossWaitTime", 120 },
                    { "numFountainStartFloor", 1 }
                };
            }
            public override string ToString()
            {
                string RunningState = "";
                if (IsRunning)
                {
                    if (CancellationTokenSource != null)
                        RunningState = $" (啓動中, {ModeName})";
                    else if (Master != null)
                        RunningState = $" (作為從視窗運行中)";
                    else
                        RunningState = " (啓動中)";
                }
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

        private class ModeItem
        {
            public string DisplayName { get; set; }
            public string ModeCode { get; set; }

            public ModeItem(string displayName, string modeCode)
            {
                DisplayName = displayName;
                ModeCode = modeCode;
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
