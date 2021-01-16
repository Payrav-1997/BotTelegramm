using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotTelegramm
{
    public partial class MainForm : Form
    {
        private string token = "1420711938:AAHpHyW0n7hhSRSqxU2xGUzOiTc3TUVjHL8";
        private string BaleUrl = "https://api.telegram.org/bot";

        WebClient client;

        public MainForm()
        {
            InitializeComponent();
            Init();
        }
        private void Init()
        {
            client = new WebClient();
            // InitProxy();
            string s =  client.DownloadString(BaleUrl + token+ "/getUpdates?offset=851444544");
            TelegrammClass
            textBoxLog.Text = s;

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
