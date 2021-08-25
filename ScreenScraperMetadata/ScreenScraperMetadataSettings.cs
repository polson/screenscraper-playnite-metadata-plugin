using System.Collections.Generic;
using System.Windows.Controls;
using Playnite.SDK;

namespace ScreenScraperMetadata
{
    public class ScreenScraperMetadataSettings : ISettings
    {
        private readonly ScreenScraperMetadata plugin;

        // Parameterless constructor must exist if you want to use LoadPluginSettings method.
        public string Username { get; set; } = string.Empty;

        public string RegionPreferences { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string SelectedBackgroundPreference { get; set; }
        
        public bool ShouldUseGameLogosAsIcon { get; set; }

        public enum BackgroundPreferenceEnum
        {
            Fanart,
            Screenshot,
            PreferFanart,
            PreferScreenshot
        }
        
        public Dictionary<BackgroundPreferenceEnum, string> BackgroundPreferencesWithCaptions { get; } = new()
            {
                {BackgroundPreferenceEnum.Fanart, "Fanart"},
                {BackgroundPreferenceEnum.Screenshot, "Screenshot"},
                {BackgroundPreferenceEnum.PreferFanart, "Both (Prefer fanart)"},
                {BackgroundPreferenceEnum.PreferScreenshot, "Both (Prefer screenshot)"}
            };
        
        public BackgroundPreferenceEnum BackgroundPreference
        {
            get; set;
        }

        public ScreenScraperMetadataSettings()
        {
        }

        public ScreenScraperMetadataSettings(ScreenScraperMetadata plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<ScreenScraperMetadataSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                Username = savedSettings.Username;
                RegionPreferences = savedSettings.RegionPreferences;
                Password = savedSettings.Password;
                SelectedBackgroundPreference = savedSettings.SelectedBackgroundPreference;
                BackgroundPreference = savedSettings.BackgroundPreference;
                ShouldUseGameLogosAsIcon = savedSettings.ShouldUseGameLogosAsIcon;
            }
        }

        public void BeginEdit()
        {
            GetPasswordView().Password = Password;
            // Code executed when settings view is opened and user starts editing values.
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            Password = GetPasswordView().Password;
            plugin.SavePluginSettings(this);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }

        private PasswordBox GetPasswordView()
        {
            return (PasswordBox)plugin.GetSettingsView(false).FindName("PasswordInput")!;
        }
    }
}