using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotTelegramm
{

    /* {"ok":true,
         "result":[{"update_id":851444546,
             "message":{"message_id":5,
                 "from":{"id":1043241084,
                     "is_bot":false,
                         "first_name":"\u041f\u0430\u0439\u0440\u0430\u0432",
                             "username":"payrav_97",
                                "language_code":"ru"},
                             "chat":{ "id":1043241084,
                         "first_name":"\u041f\u0430\u0439\u0440\u0430\u0432",
                     "username":"payrav_97",
                  "type":"private"},
              "date":1610814001,
             "text":"/start",
     "entities":[{"offset":0,"length":6,
     "type":"bot_command"}]}}]}*/

    class TelegrammMessage
    {
        /// <summary>
        /// Все ли хорошо
        /// </summary>
        public bool ok { get; set; }
        /// <summary>
        /// Результат сообщения
        /// </summary>
        public Result[] result { get; set; }
    }

    public class Result
    {
        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public int update_id { get; set; }
        /// <summary>
        /// Сообщения
        /// </summary>
        public Message message { get; set; }
    }

    public class Message
    {
        /// <summary>
        ///Id Сообщения 
        /// </summary>
        public int message_id { get; set; }
        /// <summary>
        /// От кого сообщения
        /// </summary>
        public From from { get; set; }
        /// <summary>
        /// Описание чата
        /// </summary>
        public Chat chat { get; set; }
        /// <summary>
        /// Дата в формате юникс
        /// </summary>
        public int date { get; set; }
        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string text { get; set; }
        public Entity[] entities { get; set; }
    }

    public class From
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }

    public class Chat
    {
        /// <summary>
        /// Иденитификатор чата
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string first_name { get; set; }
        /// <summary>
        /// Имя в телеграмме
        /// </summary>
        public string username { get; set; }
        public string type { get; set; }
    }

    public class Entity
    {
        public int offset { get; set; }
        public int length { get; set; }
        public string type { get; set; }
    }

}


