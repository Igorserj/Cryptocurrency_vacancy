using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cryptocurrency
{
    public sealed partial class AllCurrencies : Page
    {
        public ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public List<Currency> Currencies { get; set; }
        public AllCurrencies()
        {
            this.InitializeComponent();

            Client();
        }


        public void Client()
        {
            Currencies = new List<Currency>();
            var client = new RestClient("https://api.coincap.io/v2/assets");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var jsonObject = JObject.Parse(response.Content);
            int length = LengthCalc(jsonObject);

            for (int i = 0; i < length; i++)
            {
                string[] results = new string[3];
                for (int j = 0; j < 3; j++)
                {
                    if ((localSettings.Values["Search"] as string) != null)
                    {
                        string pattern = (@"\w*" + (localSettings.Values["Search"] as string) + @"\w*");
                        if (Regex.IsMatch(Parsing(i, j, jsonObject)[0].ToString(), pattern, RegexOptions.IgnoreCase))
                        {
                            results[0] = Parsing(i, 0, jsonObject)[0].ToString();
                            results[1] = Parsing(i, 1, jsonObject)[0].ToString();
                            results[2] = Parsing(i, 2, jsonObject)[0].ToString();
                            j = 3;
                        }
                    }
                    else results[j] = Parsing(i, j, jsonObject)[0].ToString();
                }
                if (results[0] != null && results[1] != null && results[2] != null)
                {
                    Currencies.Add(new Currency { Name = results[0], Symbol = results[1], Price = float.Parse(results[2]) });
                }
            }
        }
        private int LengthCalc(dynamic jsonObject)
        {
            JArray items = (JArray)jsonObject["data"];
            return items.Count;
        }
        private dynamic Parsing(int i, int j, dynamic jsonObject)
        {
            var priceArray = j == 0 ? jsonObject.SelectTokens("data[" + i + "].name") : j == 1 ? jsonObject.SelectTokens("data[" + i + "].symbol") : jsonObject.SelectTokens("data[" + i + "].priceUsd");
            return JValue.Parse(JsonConvert.SerializeObject(priceArray, Newtonsoft.Json.Formatting.Indented));
        }
        private async void DoMajorAppReconfiguration()
        {
            AppRestartFailureReason result =
                await CoreApplication.RequestRestartAsync("");
        }

        private void selectingIndex(object sender, SelectionChangedEventArgs e)
        {
            if (currencyGrid.SelectedIndex != -1)
            {
                Currency currency = new Currency();
                toDetails.IsEnabled = true;
                foreach (var obj in currencyGrid.SelectedItems)
                {
                    currency = obj as Currency;
                    selectedCurrencyName.Text = currency.Name.ToString();
                    string path = "ms-appx:///Assets/CurrencyIcon/" + currency.Name.ToString() + ".png";
                    currencyImage.Source = new BitmapImage(new Uri(path));
                }
            }
            else toDetails.IsEnabled = false;
        }

        private void menuThemeClicked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["themeSetting"] = App.Current.RequestedTheme == ApplicationTheme.Light ? 1 : 0;
            DoMajorAppReconfiguration();
        }

        private void return_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            localSettings.Values["Search"] = searchBox.QueryText;
            this.Frame.Navigate(typeof(AllCurrencies));
        }
        private void toDetails_click(object sender, RoutedEventArgs e)
        {
            Currency currency = new Currency();
            foreach (var obj in currencyGrid.SelectedItems)
            {
                currency = obj as Currency;
                localSettings.Values["Name"] = currency.Name.ToString();
            }
            this.Frame.Navigate(typeof(Details));
        }
        private void toCalculator_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Calculator));
        }
    }
}
