/*   Copyright 2023 Mathias Lund-Hansen
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using Microsoft.Win32;
using VlsmCalculator;

namespace VLSMCalculator.ViewModel
{
    /// <summary> View model for the icon theme. Doesn't change if pinned due to WPF limitations. </summary>
    public class IconThemeViewModel : INotifyPropertyChanged
    {
        /// <summary> Gets or sets the icon source from the view. </summary>
        private ImageSource _iconSource;
        
        /// <summary> Used in the view to set the icon source. </summary>
        public ImageSource IconSource
        {
            get { return _iconSource; }
            set
            {
                _iconSource = value;
                OnPropertyChanged();
            }
        }

        /// <summary> Sets the icon theme and changes it if the theme changes. </summary>
        public IconThemeViewModel()
        {
            SetApplicationIconBasedOnTheme();

            Microsoft.Win32.SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        /// <summary> When the user changes the theme, the icon will change as well. </summary>
        private void OnUserPreferenceChanged(object sender, Microsoft.Win32.UserPreferenceChangedEventArgs e)
        {
            if (e.Category == Microsoft.Win32.UserPreferenceCategory.General)
            {
                SetApplicationIconBasedOnTheme();
            }
        }

        /// <summary> Sets the icon based on the theme. </summary>
        public void SetApplicationIconBasedOnTheme()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            int? appsUseLightTheme = key.GetValue("AppsUseLightTheme") as int?;

            var iconPath = (appsUseLightTheme.Value == 1) ? "Resources/icon_dark.ico" : "Resources/icon_light.ico";

            Uri iconUri = new Uri(System.IO.Path.GetFullPath(iconPath), UriKind.RelativeOrAbsolute);
            IconSource = new BitmapImage(iconUri);
        }

        /// <summary> Event handler for the PropertyChanged event. Used to notify the view of changes. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> The method that notifies the view of changes. </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
