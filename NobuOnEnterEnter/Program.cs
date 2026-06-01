using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace NobuOnEnterEnter
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Load and set saved language BEFORE creating form
            string savedLanguage = Settings.Default.Language;
            if (string.IsNullOrEmpty(savedLanguage))
            {
                savedLanguage = "zh-TW"; // Default to Traditional Chinese
                Settings.Default.Language = savedLanguage;
                Settings.Default.Save();
            }

            // Set the culture
            SetCulture(savedLanguage);

            Application.Run(new NobuEnterEnter());
        }

        public static void SetCulture(string cultureName)
        {
            CultureInfo culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            // Also set default culture for new threads
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
        }
    }
}