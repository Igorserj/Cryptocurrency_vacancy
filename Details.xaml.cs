using System;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cryptocurrency
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Details : Page
    {
        public ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public Details()
        {
            this.InitializeComponent();
            currencyName.Text = localSettings.Values["Name"] as string;
            currencyPrice.Text = "Price: $" + (localSettings.Values["Price"] as string);

        }
        private async void DoMajorAppReconfiguration()
        {
            AppRestartFailureReason result =
                await CoreApplication.RequestRestartAsync("");
        }

        private void menuThemeClicked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["themeSetting"] = App.Current.RequestedTheme == ApplicationTheme.Light ? 1 : 0;
            DoMajorAppReconfiguration();
        }
    }
}
