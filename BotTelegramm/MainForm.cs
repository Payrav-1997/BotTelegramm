using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Windows.Forms;

namespace BotTelegramm
{
    public partial class MainForm : Form
    {
        private string token = "1420711938:AAHpHyW0n7hhSRSqxU2xGUzOiTc3TUVjHL8";
        private string BaseUrl = "https://api.telegram.org/bot";

        WebClient client;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            client = new WebClient();
            string s =  client.DownloadString(BaseUrl + token+ "/getUpdates?offset=851444544");
            textBoxLog.Text = s;
            TelegrammMessage telegrammMessage = JsonConvert.DeserializeObject<TelegrammMessage>(s);
            textBoxLog.Text += "\r\n" + new DateTime(1970, 1, 1).AddSeconds(telegrammMessage.result[0].message.date) + "Сообщение: " + telegrammMessage.result[0].message.text;

            string address = BaseUrl + token + "/sendMessage";
            NameValueCollection collection = new NameValueCollection();
            collection.Add("chat_id", telegrammMessage.result[0].message.chat.id.ToString());
            collection.Add("text", "Программный привет от бота");
            client.UploadValues(address,collection);
            // InitProxy();

        }
        //Соединение через proxy Сервер..
        private void InitProxy()
        {
            WebProxy proxy = new WebProxy("192.168.0.1",8080);
            //Отпавляет данные 
            proxy.Credentials = new NetworkCredential("admin", "pass");
            client.Proxy = proxy;
        }
    }
}
