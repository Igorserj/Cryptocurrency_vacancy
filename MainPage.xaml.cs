using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cryptocurrency
{
    public class Currency
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
    }

    public class FullCurrency : Currency
    {
        public double Volume { get; set; }
        public double Change { get; set; }
        public string Website { get; set; }
    }

    public sealed partial class MainPage : Page
    {
        public ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public List<Currency> Currencies { get; set; }

        public MainPage()
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

            for (int i = 0; i < 10; i++)
            {
                string[] results = new string[3];
                for (int j = 0; j < 3; j++) results[j] = Parsing(i, j, jsonObject)[0].ToString();
                Currencies.Add(new Currency { Name = results[0], Symbol = results[1], Price = float.Parse(results[2]) });
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

        private void view_click(object sender, RoutedEventArgs e)
        {
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
