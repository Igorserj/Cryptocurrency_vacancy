using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Cryptocurrency
{
    public sealed partial class Calculator : Page
    {
        public ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public List<Currency> Currencies { get; set; }
        public Calculator()
        {
            this.InitializeComponent();
            Client();
            Text_populating();
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
                for (int j = 0; j < 3; j++) results[j] = Parsing(i, j, jsonObject)[0].ToString();
                Currencies.Add(new Currency { Name = results[0], Symbol = results[1], Price = double.Parse(results[2]) });
            }
        }
        public void Text_populating()
        {
            currencyName.Text = localSettings.Values["Name"] as string;
            Symbol.Text = localSettings.Values["Symbol"] as string;
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
            if (calculatorGrid.SelectedIndex != -1)
            {
                Currency currency = new Currency();
                foreach (var obj in calculatorGrid.SelectedItems)
                {
                    currency = obj as Currency;
                    PriceConversion.Text = "Price coversion: " + ((double)currency.Price / (double)(localSettings.Values["Price"])).ToString() + " " + (localSettings.Values["Symbol"] as string);
                }
            }
            else
            {
                //toDetails.IsEnabled = false;
                //toCalculator.IsEnabled = false;
            }
        }
        private void menuThemeClicked(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["themeSetting"] = App.Current.RequestedTheme == ApplicationTheme.Light ? 1 : 0;
            DoMajorAppReconfiguration();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
