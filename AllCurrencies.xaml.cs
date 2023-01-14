using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cryptocurrency
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AllCurrencies : Page
    {
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
                foreach (var obj in currencyGrid.SelectedItems)
                {
                    currency = obj as Currency;
                    selectedCurrencyName.Text = currency.Name.ToString();
                    string path = "ms-appx:///Assets/CurrencyIcon/" + currency.Name.ToString() + ".png";
                    currencyImage.Source = new BitmapImage(new Uri(path));
                }
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

        private void SearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            this.Frame.Navigate(typeof(AllCurrencies), args.QueryText);
        }
    }
}
