using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Cryptocurrency
{
    public class Markets
    {
        public string Symbol { get; set; }
        public string Exchange { get; set; }
        public double Price { get; set; }
        public double Volume { get; set; }
    }

    public sealed partial class Details : Page
    {
        public ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public List<FullCurrency> Currencies { get; set; }
        public List<Markets> MarketsList { get; set; }
        public Details()
        {
            this.InitializeComponent();
            Client();
            //Thread.Sleep(100);
            Text_populating();
        }

        public void Client()
        {
            Currencies = new List<FullCurrency>();
            MarketsList = new List<Markets>();

            var client = new RestClient("https://api.coincap.io/v2/assets");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var jsonObject = JObject.Parse(response.Content);

            int length = LengthCalc(jsonObject);

            var client2 = new RestClient("https://api.coincap.io/v2/assets/bitcoin/markets");
            client2.Timeout = -1;
            var request2 = new RestRequest(Method.GET);
            IRestResponse response2 = client2.Execute(request2);
            var marketsJsonObject = JObject.Parse(response2.Content);

            int marketsLength = LengthCalc(marketsJsonObject);

            for (int i = 0; i < length; i++)
            {
                string[] results = new string[6];
                string[] marketResults = new string[4];
                for (int j = 0; j < 6; j++)
                {
                    if ((localSettings.Values["Name"] as string) != null)
                    {
                        if ((localSettings.Values["Name"] as string) == Parsing(i, 0, jsonObject)[0].ToString())
                        {
                            results[j] = Parsing(i, j, jsonObject)[0].ToString();
                            if (j == 1)
                            {
                                for (int k = 0; k < marketsLength; k++)
                                {
                                    if (results[j] == MarketsParsing(k, 0, marketsJsonObject)[0].ToString()) {
                                        for (int l = 0; l < 4; l++) marketResults[l] = MarketsParsing(k, l, marketsJsonObject)[0].ToString();
                                        MarketsList.Add(new Markets { Symbol = marketResults[0], Exchange = marketResults[1], Price = double.Parse(marketResults[2]), Volume = double.Parse(marketResults[3]) });
                                    }
                                }
                            }
                        }
                    }
                }
                if (results[1] != null && results[2] != null && results[3] != null && results[4] != null && results[5] != null)
                {
                    localSettings.Values["Price"] = results[2];
                    localSettings.Values["Volume"] = results[3];
                    localSettings.Values["Change"] = results[4];
                    localSettings.Values["Website"] = results[5];
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
            var array = j == 0 ? jsonObject.SelectTokens("data[" + i + "].name") : j == 1 ? jsonObject.SelectTokens("data[" + i + "].symbol") : j == 2 ? jsonObject.SelectTokens("data[" + i + "].priceUsd") : j == 3 ? jsonObject.SelectTokens("data[" + i + "].volumeUsd24Hr") : j == 4 ? jsonObject.SelectTokens("data[" + i + "].changePercent24Hr") : jsonObject.SelectTokens("data[" + i + "].explorer");
            return JValue.Parse(JsonConvert.SerializeObject(array, Newtonsoft.Json.Formatting.Indented));
        }
        private dynamic MarketsParsing(int k, int l, dynamic jsonObject)
        {
            var marketArray = l == 0 ? jsonObject.SelectTokens("data[" + k + "].quoteSymbol") : l == 1 ? jsonObject.SelectTokens("data[" + k + "].exchangeId") : l == 2 ? jsonObject.SelectTokens("data[" + k + "].priceUsd") : jsonObject.SelectTokens("data[" + k + "].volumeUsd24Hr");
            return JValue.Parse(JsonConvert.SerializeObject(marketArray, Newtonsoft.Json.Formatting.Indented));
        }

        public void Text_populating()
        {
            currencyName.Text = localSettings.Values["Name"] as string;
            currencyPrice.Text = "Price USD: $" + localSettings.Values["Price"] as string;
            currencyVolume.Text = "Volume USD: $" + localSettings.Values["Volume"] as string;
            currencyChange.Text = "Change: " + (localSettings.Values["Change"] as string) + "%";
            currencyWebsite.NavigateUri = new Uri(localSettings.Values["Website"] as string, UriKind.Absolute);
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
