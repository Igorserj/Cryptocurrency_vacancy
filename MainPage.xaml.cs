using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cryptocurrency
{
    public class Currency
    {
        public string Usds { get; set; }
        public string Names { get; set; }
    }

    public sealed partial class MainPage : Page
    {
        public List<Currency> Currencies { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Client();
        }


        private void Client()
        {
            Currencies = new List<Currency>();
            var client = new RestClient("https://api.coincap.io/v2/assets");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var json = JsonConvert.SerializeObject(response.Content, Formatting.Indented);

            var jsonObject = JObject.Parse(response.Content);
            JArray items = (JArray)jsonObject["data"];
            int length = items.Count;

            //StringCollection usds = new StringCollection();
            for (int i = 0; i < length; i++)
            {
                var priceArray = jsonObject.SelectTokens("data[" + i + "].priceUsd");
                var result = JsonConvert.SerializeObject(priceArray, Newtonsoft.Json.Formatting.Indented);

                var nameArray = jsonObject.SelectTokens("data[" + i + "].id");
                var result2 = JsonConvert.SerializeObject(nameArray, Newtonsoft.Json.Formatting.Indented);
                //usds.Add(result.ToString());
                Currencies.Add(new Currency { Usds = result.ToString(), Names = result2.ToString() });

            }
        }
    }
}
