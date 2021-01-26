using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace BotTelegramm
{
    public partial class MainForm : Form
    {
        private string token = "1420711938:AAHpHyW0n7hhSRSqxU2xGUzOiTc3TUVjHL8";
        private string BaseUrl = "https://api.telegram.org/bot";
        private long LastUpdateID = 0; 
        private string fileLog = "BotLog.log";

        WebClient client;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            client = new WebClient();
            WriteLog("Авторизация....");
            timerGetUpdates.Enabled = true;
            
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

        private void SendMessage(long chat_id,string message)
        {
            string address = BaseUrl + token + "/sendMessage";
            NameValueCollection collection = new NameValueCollection();
            collection.Add("chat_id",chat_id.ToString());
            collection.Add("text", message);
            client.UploadValues(address, collection);
        }

        private void timerGetUpdates_Tick(object sender, EventArgs e)
        {
            GetUpdates();
        }
        private void GetUpdates()
        {
            string s = client.DownloadString(BaseUrl + token + "/getUpdates?offset=" + (LastUpdateID + 1));
            TelegrammMessage telegrammMessage = JsonConvert.DeserializeObject<TelegrammMessage>(s);
            if (!telegrammMessage.ok || telegrammMessage.result.Length == 0) return;
            foreach (Result result in telegrammMessage.result)
            {
                LastUpdateID = result.update_id;
                SendAnswer(result.message.chat.id, result.message.text);
                WriteLog(result.message.from.first_name + "(" + result.message.from.username + "): " + result.message.text + Environment.NewLine);
            
            }
            
        }
        private void SendAnswer(long chat_id,string message)
        {
            string answer = "";
            switch (message.ToLower())
            {
                case "/start": answer = "Я твой бот,знаешь что я умею? /help"; break;
                case "лог111": answer = RetLog();break;
                case "/help": answer = 
 @"Добро пожаловать в помощь нашего телеграмм Бота.
Ниже представлены все поддерживаемые команды


/start - самое начало!
/help  - помощь
Лог    - логи";break;

                default: answer = "Вы мне написали  " + "" + message + ',' + "но я не знаю что ответит :(";
                    break;
            }
            SendMessage(chat_id, answer);
        }
        private void WriteLog(string text)
        {
            text = DateTime.Now + " " + text + Environment.NewLine;
            textBoxLog.Text += text;
            File.AppendAllText(fileLog,text);


        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WriteLog("Остановка БОТА!");
        }
        private string RetLog()
        {
            string answer = "";
            if (File.Exists(fileLog))
            {
                string[] readingFileLog = File.ReadAllLines(fileLog);
                foreach (string log in readingFileLog)
                {
                    answer += log + Environment.NewLine;
                }
            }
            return answer;
        }
    }
}
