using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cryptocurrency
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Client();
        }
        private void Client()
        {
            var client = new RestClient("https://api.coincap.io/v2/assets");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            var json = JsonConvert.SerializeObject(response.Content, Formatting.Indented);

            var jsonObject = JObject.Parse(response.Content);
            JArray items = (JArray)jsonObject["data"];
            int length = items.Count;

            List<TextBlock> textBlocks = new List<TextBlock>();
            for (int i = 0; i < length; i++)
            {
                var newJsonArray = jsonObject.SelectTokens("data[" + i + "].priceUsd");
                var result = JsonConvert.SerializeObject(newJsonArray, Newtonsoft.Json.Formatting.Indented);

                textBlocks.Add(new TextBlock());
                textBlocks[i].Text = result.ToString();
                textBlocks[i].Opacity = 1;
                //SomeText.Text = result.ToString();
                //textBlock = new TextBlock();
            }


            //var jsonArray = JArray.Parse(result);
            //newJsonArray = jsonArray.SelectTokens("$..bitcoin");
            //result = JsonConvert.SerializeObject(newJsonArray, Newtonsoft.Json.Formatting.Indented);

            //var data = (JObject)JsonConvert.DeserializeObject(result);
            //var timeZone = data["priceUsd"].Value<JObject>();
            //JArray jArr = (JArray)JsonConvert.DeserializeObject(result);
            //int count = result.Count;
        }
    }
}
