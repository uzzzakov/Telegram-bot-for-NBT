using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;

namespace Bot
{
    class Program
    {
        private static string _token = "YOUR_TOKEN_FROM_BOT_FATHER";
        private static TelegramBotClient _client;
        private static double n;
        private static int month;
        private static double percent;
        private static string capitalization;
        private static string admin = "admin_email";
        private static string bot = "bot_email";
        private static string password = "bot_email_password";
        private static string username;
        private static string number;
        private static int flag = 0;
        private static string banki = "";
        private static string mdo = "";
        private static string mko = "";
        private static string mkf = "";

        [Obsolete]
        static void Main(string[] args)
        {
            _client = new TelegramBotClient(_token) { Timeout = TimeSpan.FromSeconds(10)};
            _client.StartReceiving();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Бот запущен");
            Console.ForegroundColor = ConsoleColor.White;

            string fileName = @"docs/banki.txt";
            IEnumerable<string> lines = File.ReadLines(fileName);
            banki += String.Join(Environment.NewLine, lines) + "\n";

            string fileName2 = @"docs/mdo.txt";
            IEnumerable<string> lines2 = File.ReadLines(fileName2);
            mdo += String.Join(Environment.NewLine, lines2) + "\n";

            string fileName3 = @"docs/mko.txt";
            IEnumerable<string> lines3 = File.ReadLines(fileName3);
            mko += String.Join(Environment.NewLine, lines3) + "\n";

            string fileName4 = @"docs/mkf.txt";
            IEnumerable<string> lines4 = File.ReadLines(fileName4);
            mkf += String.Join(Environment.NewLine, lines4) + "\n";

            _client.OnMessage += StartHandler;
            Console.ReadLine();
            _client.StopReceiving();
        }
        [Obsolete]
        private async static void StartHandler(object sender, MessageEventArgs e)
        {
            try
            {
                if (e.Message != null && e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && !string.IsNullOrEmpty(e.Message.Text))
                {
                    //start
                    if (e.Message.Text.Contains("/start"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Национальный банк Таджикистана приветствует Вас и предлагает воспользоваться чат-ботом \"виртуальный помощник\"", replyMarkup: GetButtons());
                        _client.OnMessage += OnMessageHandler;
                    }
                }
            }
            catch (Exception ex)
            {
                await _client.SendTextMessageAsync(e.Message.Chat.Id, ex.ToString());
            }
        }

        [Obsolete]
        private static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= StartHandler;
            try
            {
                if (e.Message != null && e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text && !string.IsNullOrEmpty(e.Message.Text))
                {
                    //Курс валюты
                    if (e.Message.Text.Contains("Курс валюты"))
                    {
                        try
                        {
                            string url = "https://tajikexchangecourses.herokuapp.com/nbt";
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                            string response;
                            using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                response = streamReader.ReadToEnd();
                            }

                            Currency currency = JsonConvert.DeserializeObject<Currency>(response);
                            //DateTime date = DateTime.Now;
                            DateTimeOffset date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Utc);
                            date = date.ToOffset(TimeSpan.FromHours(5));

                            await _client.SendTextMessageAsync(e.Message.Chat.Id, $"Официальные курсы валют к TJS на \n{date.ToString("dd.MM.yyyy")}\n\n1 USD = {currency.USD} TJS\n1 EUR = {currency.EUR} TJS\n1 RUB = {currency.RUB} TJS", replyMarkup: GetButtons());
                        }
                        catch (Exception ex)
                        {
                            await _client.SendTextMessageAsync(e.Message.Chat.Id, ex.ToString(), replyMarkup: GetButtons());
                        }

                    }

                    //Телефон доверия НБТ
                    else if (e.Message.Text.Contains("Телефон доверия НБТ"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Телефон доверия Национального Банка Таджикистана +992446001520", replyMarkup: GetButtons());
                    }

                    //Кредитные организации
                    else if (e.Message.Text.Contains("Кредитные организации"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Выберите вид кредитных организаций из нижнего меню, чтобы просмотреть телефоны доверия:", replyMarkup: GetButtons4());
                    }
                    else if (e.Message.Text.Contains("Банки"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, banki, replyMarkup: GetButtons4());
                    }
                    else if (e.Message.Text.Contains("Микродепозитные организации"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, mdo, replyMarkup: GetButtons4());
                    }
                    else if (e.Message.Text.Contains("Микрокредитные организации"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, mko, replyMarkup: GetButtons4());
                    }
                    else if (e.Message.Text.Contains("Микрокредитные фонды"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, mkf, replyMarkup: GetButtons4());
                    }
                    else if (e.Message.Text.Contains("На главную"))
                    {
                        flag = 0;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Вы вышли на главное меню чат-бота:", replyMarkup: GetButtons());
                    }

                    //Написать на почту
                    else if (e.Message.Text.Contains("Обращение в НБТ"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, $"Ваше ФИО:");
                        _client.OnMessage += MessageNameHandler;
                    }

                    //FAQ
                    else if (e.Message.Text.Contains("Часто задаваемые вопросы"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Часто задаваемые вопросы", replyMarkup: GetButtons5());
                    }
                    else if (e.Message.Text.Contains("Бaнкoвские кaрты"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Бaнкoвские кaрты", replyMarkup: FAQBKButtons());
                    }
                    else if (e.Message.Text.Contains("Как я могу получить банковскую карту?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Физическое лицо может получить банковскую карту, обратившись в любую кредитную финансовую организацию, осуществляющую эмиссию и эквайринг банковских платёжных карт.", replyMarkup: FAQBKButtons());
                    }
                    else if (e.Message.Text.Contains("Какие существуют виды банковских карт?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Различаются следующие виды банковских платёжных карт: а) в зависимости от происхождения денежных средств, находящихся на счёте: дебетовая карта, посредством которой держатель карты может распоряжаться денежными средствами, депонированными в кредитной финансовой организации или в рамках лимита овердрафта, установленного договором о выдаче карты;  кредитная карта, посредством которой держатель может распоряжаться денежными средствами, предоставленными кредитной финансовой организацией в форме кредитной линии; предоплаченная карта, посредством которой держатель имеет право выполнять операции в пределах предварительно внесённых средств и которые учитываются на консолидированном карт-счёте кредитной финансовой организации; б) в зависимости от принадлежности к платёжной системе: локальные карты (внутренние), которые обслуживаются только в пределах терминальной сети кредитной финансовой организации; национальные карты, которые обслуживаются в пределах терминальной сети участников Национальной платежной системы «Корти милли»; международные карты, которые обслуживаются в пределах терминальной сети кредитных финансовых организаций, являющимися членами международной платёжной системы, на территории Республики Таджикистан и за его приделами; кобейджинговая карта (кобейдж карта) – банковская платежная карта, выпущенная эмитентом в рамках взаимодействия двух платежных систем. (постановление Правления НБТ от 29.04.2019,№39)", replyMarkup: FAQBKButtons());
                    }
                    else if (e.Message.Text.Contains("Какие операции я могу осуществить посредством банковской карты?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "На территории Республики Таджикистан, посредством банковских платёжных карт, могут быть осуществлены следующие операции: оплата товаров и услуг, налогов, пошлин и других платежей, включая посредством сети Интернет; снятие наличных денег в банкоматах и в пунктах выдачи и/или приёма наличных денежных средств и банковских платежных агентов; перевод денежных средств между двумя держателями банковских платёжных карт. (постановление Правления НБТ от 29.04.2019,№39", replyMarkup: FAQBKButtons());
                    }
                    else if (e.Message.Text.Contains("В каких случаях у меня могут изъять или блокировать банковскую платежную карту?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Кредитная финансовая организация вправе блокировать или изъять банковскую платёжную карту в случаях: получения уведомления от держателя карты об утере, краже или несанкционированном использовании карты;  неисполнения держателем карты своих обязательств, предусмотренных Договором о выдаче карты; нарушения правил пользования картой; окончания срока действия карты;  расторжения Договора о выдаче карты", replyMarkup: FAQBKButtons());
                    }

                    else if (e.Message.Text.Contains("Вaлютые oпeрaции"))
                    {
                        flag = 2;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Вaлютые oпeрaции", replyMarkup: FAQVOButtons());
                    }
                    else if (e.Message.Text.Contains("1️⃣") && flag == 2)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Вaлютые oпeрaции\nСтраница 1", replyMarkup: FAQVOButtons());
                    }
                    else if (e.Message.Text.Contains("Можно ли оплачивать покупки иностранной валютой?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Национальная валюта – «сомони» является единственным и исключительно законным платежным средством в обращении на территории Республики Таджикистан. Использование иностранной валюты в обращении и при осуществлении расчётов на территории Республики Таджикистан запрещено За исключением случаев, описанных в Инструкции №168. (Статья 2, пункты а-г)", replyMarkup: FAQVOButtons());
                    }
                    else if (e.Message.Text.Contains("Где я могу приобрести иностранную валюту?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "В финансовых кредитных организациях и их филиалах", replyMarkup: FAQVOButtons());
                    }
                    else if (e.Message.Text.Contains("Почему на товарах нет цен в иностранной валюте?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "На территории Таджикистана все ценники, тарифы и единицы указываются только в национальной валюте - сомони", replyMarkup: FAQVOButtons());
                    }
                    else if (e.Message.Text.Contains("Имеются ли ограничения на ввоз валюты в Республику Таджикистан?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Ограничений нет.	Ввоз в Республику Таджикистан наличной национальной валюты и ценных бумаг, выраженных в национальной валюте, осуществляется без ограничений", replyMarkup: FAQVOButtons());
                    }
                    else if (e.Message.Text.Contains("2️⃣") && flag == 2)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Вaлютые oпeрaции\nСтраница 2", replyMarkup: FAQVOButtons2());
                    }
                    else if (e.Message.Text.Contains("Какую сумму в иностранной валюте при вывозе мне придётся декларировать?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "От 3001 до 10 000 долларов США", replyMarkup: FAQVOButtons2());
                    }
                    else if (e.Message.Text.Contains("Какую сумму в иностранной валюте я могу вывезти из Республики Таджикистан без предоставления документов, подтверждающих источник происхождения валютных ценностей?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "До 3000 долларов США", replyMarkup: FAQVOButtons2());
                    }
                    else if (e.Message.Text.Contains("При какой сумме в иностранной валюте при вывозе мне придётся предоставлять документы, подтверждающие источник происхождения валютных ценностей?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "От 3000 долларов США", replyMarkup: FAQVOButtons2());
                    }
                    else if (e.Message.Text.Contains("Какие документы являются основанием для вывоза валютных ценностей из Республики Таджикистан?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Официальные документы, подтверждающие  покупку валютных ценностей в финансовых кредитных организациях", replyMarkup: FAQVOButtons2());
                    }

                    else if (e.Message.Text.Contains("Дeнeжные пeрeвoды"))
                    {
                        flag = 3;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Дeнeжные пeрeвoды", replyMarkup: FAQDPButtons());
                    }
                    else if (e.Message.Text.Contains("1️⃣") && flag == 3)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Дeнeжные пeрeвoды\nСтраница 1", replyMarkup: FAQDPButtons());
                    }
                    else if (e.Message.Text.Contains("Могу ли я получить или отправить деньги из РТ?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да	Физические лица могут осуществлять переводные операции только для перевода или получения денежных средств, не связанных с коммерческой деятельностью (предпринимательством или инвестированием).", replyMarkup: FAQDPButtons());
                    }
                    else if (e.Message.Text.Contains("Как мне получить или отправить деньги, если они связаны с коммерческой деятельностью?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Только посредством банковских счетов, открытых ими в уполномоченных банках.", replyMarkup: FAQDPButtons());
                    }
                    else if (e.Message.Text.Contains("Каким образом я могу получить или отправить деньги?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "При предоставлении документа удостоверяющего личность", replyMarkup: FAQDPButtons());
                    }
                    else if (e.Message.Text.Contains("В какой валюте я могу получить денежный перевод?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "В любой валюте кроме российский рублей", replyMarkup: FAQDPButtons());
                    }
                    else if (e.Message.Text.Contains("2️⃣") && flag == 3)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Дeнeжные пeрeвoды\nСтраница 2", replyMarkup: FAQDPButtons2());
                    }
                    else if (e.Message.Text.Contains("Можно ли перевести валюту, которая была ранее ввезена в Республику Таджикистан?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да, можно	Резиденты (их представители) могут перевести валюту, которую ранее была переведена, ввезена или переслана в Республику Таджикистан в сумме, указанной в таможенной декларации или в другом подтверждающем их перевод или пересылку документ, без указания конкретного назначения.", replyMarkup: FAQDPButtons2());
                    }
                    else if (e.Message.Text.Contains("Какую сумму можно перевести из Республики Таджикистан?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "До 96 250 сомони в один день", replyMarkup: FAQDPButtons2());
                    }
                    else if (e.Message.Text.Contains("Под каким назначением можно перевести деньги из Республики Таджикистан?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Перевод из Республики Таджикистан за рубеж валюту, имеющую социальный некоммерческий характер", replyMarkup: FAQDPButtons2());
                    }
                    else if (e.Message.Text.Contains("Как мне перевести валюту из Республики Таджикистан, если я не гражданин этой страны?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Перевод валюты осуществляется при условии предоставления нерезидентами подтверждающего документа об источнике наличной валюты.", replyMarkup: FAQDPButtons2());
                    }

                    else if (e.Message.Text.Contains("Дeпoзиты"))
                    {
                        flag = 4;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Дeпoзиты", replyMarkup: FAQDButtons());
                    }
                    else if (e.Message.Text.Contains("1️⃣") && flag == 4)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Дeпoзиты\nСтраница 1", replyMarkup: FAQDButtons());
                    }
                    else if (e.Message.Text.Contains("Возмещаются ли проценты по депозитам, если кредитная организация объявлена банкротом?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Нет	Проценты по сбережениям не возмещаются Фондом.(Фонд страхования сбережений физических лиц)", replyMarkup: FAQDButtons());
                    }
                    else if (e.Message.Text.Contains("Сколько денег мне возместит Государство?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "В национальной валюте не более 27 500 сомони. В иностранной валюте - не более 19 250 сомони", replyMarkup: FAQDButtons());
                    }
                    else if (e.Message.Text.Contains("Какие проценты выплачиваются если снять вклад досрочно?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Проценты выплачиваются согласно условиям договора", replyMarkup: FAQDButtons());
                    }
                    else if (e.Message.Text.Contains("Что с процентами, если я не требую возврата вклада по истечению срока?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Проценты выплачиваются согласно условиям договора", replyMarkup: FAQDButtons());
                    }
                    else if (e.Message.Text.Contains("2️⃣") && flag == 4)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Дeпoзиты\nСтраница 2", replyMarkup: FAQDButtons2());
                    }
                    else if (e.Message.Text.Contains("На какую сумму я могу рассчитывать, если имею несколько вкладов в национальной валюте при страховом случае?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Страховое возмещение выплачивается в размере не более 27 500 сомони", replyMarkup: FAQDButtons2());
                    }
                    else if (e.Message.Text.Contains("На какую сумму я могу рассчитывать, если имею несколько вкладов в иностранной валюте при страховом случае?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Страховое возмещение выплачивается в размере не более 19 250 сомони", replyMarkup: FAQDButtons2());
                    }
                    else if (e.Message.Text.Contains("На какую сумму я могу рассчитывать, если имею несколько вкладов в национальной и иностранной валюте при страховом случае?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Размер страхового возмещения для сбережений не должен превышать 27 500 сомони", replyMarkup: FAQDButtons2());
                    }
                    else if (e.Message.Text.Contains("Что мне делать, если у меня несколько вкладов в разных организациях при страховом случае?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Размер страхового возмещения исчисляется в отношении каждой кредитной организации", replyMarkup: FAQDButtons2());
                    }
                    else if (e.Message.Text.Contains("3️⃣") && flag == 4)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Дeпoзиты\nСтраница 3", replyMarkup: FAQDButtons3());
                    }
                    else if (e.Message.Text.Contains("Как возмещаются сбережения, размещённые в национальной валюте при страховом случае?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Возмещаются в национальной валюте", replyMarkup: FAQDButtons3());
                    }
                    else if (e.Message.Text.Contains("Как возмещаются сбережения, размещённые в иностранной валюте при страховом случае?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Возмещаются в национальной валюте", replyMarkup: FAQDButtons3());
                    }

                    else if (e.Message.Text.Contains("▶ Далее"))
                    {
                        flag = 0;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Часто задаваемые вопросы", replyMarkup: GetButtons51());
                    }

                    else if (e.Message.Text.Contains("Крeдитнoe бюрo"))
                    {
                        flag = 5;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитнoe бюрo", replyMarkup: FAQKBButtons());
                    }
                    else if (e.Message.Text.Contains("1️⃣") && flag == 5)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитнoe бюрo\nСтраница 1", replyMarkup: FAQKBButtons());
                    }
                    else if (e.Message.Text.Contains("Что такое кредитное бюро?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Бюро кредитных историй  оказывает услуги по организации, обработке, хранению кредитных историй и предоставлению кредитных отчетов", replyMarkup: FAQKBButtons());
                    }
                    else if (e.Message.Text.Contains("Могу ли я получить информацию о себе от кредитного бюро и платна ли она?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да, можете. Один раз в год на бесплатной основе и в любое другое время на платной основе Вы можете получить кредитный отчет о себе", replyMarkup: FAQKBButtons());
                    }
                    else if (e.Message.Text.Contains("Отправляют ли информацию обо мне в кредитное бюро без согласия моего согласия?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Нет. Уведомление субъекта кредитной истории о предоставлении сведений о нем в Бюро кредитных историй и его согласие на выдачу кредитного отчета пользователям кредитного отчета оформляется в письменном виде.", replyMarkup: FAQKBButtons());
                    }
                    else if (e.Message.Text.Contains("Сколько лет может храниться информация в кредитном бюро?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "5 лет. Бюро кредитных историй обеспечивает хранение информации о кредитной истории в течение пяти лет со дня получения последней информации", replyMarkup: FAQKBButtons());
                    }
                    else if (e.Message.Text.Contains("2️⃣") && flag == 5)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитнoe бюрo\nСтраница 2", replyMarkup: FAQKBButtons2());
                    }
                    else if (e.Message.Text.Contains("Кто несёт ответственность за искажение информации о клиентах в кредитном бюро?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Бюро кредитных историй. За искажение информации, полученной от поставщика информации, Бюро кредитных историй несет ответственность в соответствии с законодательством Республики Таджикистан", replyMarkup: FAQKBButtons2());
                    }
                    else if (e.Message.Text.Contains("Кто может увидеть мою кредитную историю?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Любое юридическое лицо или индивидуальный предприниматель", replyMarkup: FAQKBButtons2());
                    }
                    else if (e.Message.Text.Contains("Ведёт ли кредитное бюро «чёрный» список неплательщиков?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Нет, не ведёт", replyMarkup: FAQKBButtons2());
                    }
                    else if (e.Message.Text.Contains("Почему кредитное бюро передает информацию обо мне как о «неплательщике»?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Каждая кредитная организация передает информацию о наличии или отсутствии просроченных сумм и количестведней просрочки в кредитное бюро, соответственно, эта информация отражается в кредитной истории.В результате этого, при Вашем обращении за получением кредита, эта информация может быть рассмотрена потенциальным кредитором негативно и шансы на успешное получение кредита могут быть снижены.При этом, обращаем Ваше внимание, что если у Вас были просроченные платежи, то информация о количестве дней просрочки будет отражена в кредитном отчете даже после погашения кредита.", replyMarkup: FAQKBButtons2());
                    }

                    else if (e.Message.Text.Contains("Крeдитныe oпeрaции"))
                    {
                        flag = 6;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитныe oпeрaции", replyMarkup: FAQKOButtons());
                    }
                    else if (e.Message.Text.Contains("1️⃣") && flag == 6)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитныe oпeрaции\nСтраница 1", replyMarkup: FAQKOButtons());
                    }
                    else if (e.Message.Text.Contains("Сколько времени понадобится, чтобы получить одобрение на получение кредита?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "От 1-го до 5-ти рабочих дней. Это зависит от кредитного продукта, а также как быстро вы предоставите всю необходимую информацию и документы. В целом, с момента подачи всех документов и информации, это займет в среднем 5 рабочих дней", replyMarkup: FAQKOButtons());
                    }
                    else if (e.Message.Text.Contains("Могу ли я получить кредит в долларах США?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да. Клиенты могут получить кредиты в долларах США для всех кредитных продуктов.", replyMarkup: FAQKOButtons());
                    }
                    else if (e.Message.Text.Contains("Какие виды залога принимаются?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Принимаются различные виды залогового обеспечения, такие как золотые ювелирные изделия, предметы домашнего обихода, оборудование, скот, материально-производственные запасы, наличие депозитного счета в банке и т.д.", replyMarkup: FAQKOButtons());
                    }
                    else if (e.Message.Text.Contains("Как начисляются проценты по кредитам?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Проценты начисляются только на остаток задолженности по кредиту", replyMarkup: FAQKOButtons());
                    }
                    else if (e.Message.Text.Contains("2️⃣") && flag == 6)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитныe oпeрaции\nСтраница 2", replyMarkup: FAQKOButtons2());
                    }
                    else if (e.Message.Text.Contains("Что такое аннуитетные платежи?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "В соответствии с аннуитетными платежами, ваш кредит будет выплачиваться равными долями в течение срока действия кредита. Сумма платежа включает в себя часть основной суммы и начисленных процентов на суммы кредита.", replyMarkup: FAQKOButtons2());
                    }
                    else if (e.Message.Text.Contains("Что такое дифференцированные платежи?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "В случае дифференцированных платежей, сумма основного долга делится на равные части пропорционально сроку кредитования,  проценты начисляются на остаток суммы.", replyMarkup: FAQKOButtons2());
                    }
                    else if (e.Message.Text.Contains("Что такое эффективная процентная ставка?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Годовая эффективная процентная ставка - это процентное выражение, согласно которому процент по кредиту начисляется с учётом всех выплат клиента по обслуживанию кредита кредитной организации.", replyMarkup: FAQKOButtons2());
                    }
                    else if (e.Message.Text.Contains("Где я могу досрочно погасить свой кредит?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Клиент может погасить свой кредит посредством перевода денег из других банков или с помощью банкоматов / терминалов самообслуживания, установленных в филиалах, а также в кассовых узлах кредитных организаций.", replyMarkup: FAQKOButtons2());
                    }
                    else if (e.Message.Text.Contains("3️⃣") && flag == 6)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитныe oпeрaции\nСтраница 3", replyMarkup: FAQKOButtons3());
                    }
                    else if (e.Message.Text.Contains("Могу ли я передать сумму погашения кредитному специалисту или сотруднику Банка?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Вы никогда не должны давать деньги кредитным специалистам или другим сотрудникам банка. Клиенты должны погашать свои кредиты непосредственно в кассах банков/кредитных организаций и их филиалов.", replyMarkup: FAQKOButtons3());
                    }
                    else if (e.Message.Text.Contains("Может ли третье лицо осуществить выплату кредита за клиента? Что для этого потребуется?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да. Человек, осуществляющий выплату кредита, должен знать правильное имя клиента и номер его счета.", replyMarkup: FAQKOButtons3());
                    }
                    else if (e.Message.Text.Contains("Что будет, если я забуду погасить кредит вовремя?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Кредитная организация взимает штраф в соответствии с кредитным договором", replyMarkup: FAQKOButtons3());
                    }
                    else if (e.Message.Text.Contains("Могу ли я полностью или частично погасить свой кредит? Если да, то буду ли я оштрафован или наказан за это?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да, вы можете. Никакого штрафа или наказания кредитная организация за это не назначает.", replyMarkup: FAQKOButtons3());
                    }
                    else if (e.Message.Text.Contains("4️⃣") && flag == 6)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитныe oпeрaции\nСтраница 4", replyMarkup: FAQKOButtons4());
                    }
                    else if (e.Message.Text.Contains("Мне отказали в выдаче кредита без объяснения причин. Почему?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Кредитная организация имеет право отказаться от выдачи кредита. Но по желанию клиента кредитная организация может предоставить ему письменный или устный ответ причины отказа.", replyMarkup: FAQKOButtons4());
                    }
                    else if (e.Message.Text.Contains("Если мой предыдущий запрос кредита был отклонен, можно ли ожидать положительного решения при подаче заявления на новый кредит?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да, это возможно", replyMarkup: FAQKOButtons4());
                    }
                    else if (e.Message.Text.Contains("Что происходит с погашением кредита в случае смерти клиента?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "В случае смерти клиента, его/ее долг переходит к его/ее преемнику в соответствии с законодательством Республики Таджикистан. В других случаях обязательство передается поручителю.", replyMarkup: FAQKOButtons4());
                    }
                    else if (e.Message.Text.Contains("Почему, кроме кредитных специалистов, нас проверяют ещё и другие сотрудники кредитной организации?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Они не проверяют Вас, вместо этого они проверяют эффективность работы кредитных специалистов кредитной организации.", replyMarkup: FAQKOButtons4());
                    }
                    else if (e.Message.Text.Contains("5️⃣") && flag == 6)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитныe oпeрaции\nСтраница5", replyMarkup: FAQKOButtons5());
                    }
                    else if (e.Message.Text.Contains("Могу ли я выплатить долг в национальной валюте, даже если мой долг получен в долларах США?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Да, вы можете сделать это на основе курса кредитной организации на дату погашения, если позволяет валютная позиция кредитной организации.", replyMarkup: FAQKOButtons5());
                    }
                    else if (e.Message.Text.Contains("Предоставляет ли кредитная организация кредит нерезидентам?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Нет, не предоставляет, кроме как беженцам из Афганистана, которые имеют красную карту, предоставленную правительством и которым нужен бизнес-кредит на сумму до 1000 долларов США.", replyMarkup: FAQKOButtons5());
                    }
                    else if (e.Message.Text.Contains("Какие другие услуги предлагает кредитная организация, кроме кредитов?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Кредитная организация предлагает различные депозитные продукты, расчетно-кассовое обслуживание, банковские карты, денежные переводы (внутренние и международные услуги денежных переводов), банковские гарантии и услуги обмена валют.", replyMarkup: FAQKOButtons5());
                    }
                    else if (e.Message.Text.Contains("Если у меня возникнут проблемы с филиалом или ЦБО, к кому я должен обратиться?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Вы должны позвонить на номер горячей линии кредитной организации, а также написать на её электронный адрес", replyMarkup: FAQKOButtons5());
                    }
                    else if (e.Message.Text.Contains("6️⃣") && flag == 6)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Крeдитныe oпeрaции\nСтраница 6", replyMarkup: FAQKOButtons6());
                    }
                    else if (e.Message.Text.Contains("В чем заключается ответственность поручителя?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Поручитель берет на себя полную ответственность и обязанность погасить кредит от имени заемщика в случае, если заемщик не выполнит свои обязательства перед кредитной организацией в соответствии с условиями кредитного договора.", replyMarkup: FAQKOButtons6());
                    }
                    else if (e.Message.Text.Contains("Почему клиент должен открыть депозитный счет, если он заинтересован только в получении кредита?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Кредитная организация использует депозитный счет в качестве расчетного счета клиента для всех операций, связанных с их кредитами в кредитной организации.", replyMarkup: FAQKOButtons6());
                    }
                    else if (e.Message.Text.Contains("Как получить справку об отсутствии задолженности?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Справку об отсутствии задолженности Вы можете заказать в любое время после исполнения обязательств по кредиту в полном объеме.", replyMarkup: FAQKOButtons6());
                    }

                    else if (e.Message.Text.Contains("Oбрaщeниe грaждaн"))
                    {
                        flag = 7;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Oбрaщeниe грaждaн", replyMarkup: FAQOGButtons());
                    }
                    else if (e.Message.Text.Contains("1️⃣") && flag == 7)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Oбрaщeниe грaждaн\nСтраница 1", replyMarkup: FAQOGButtons());
                    }
                    else if (e.Message.Text.Contains("Какие обращения рассматриваются в отделе обращений граждан?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Просьба, жалоба, предложение, запрос", replyMarkup: FAQOGButtons());
                    }
                    else if (e.Message.Text.Contains("Кто имеет право на подачу обращения?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Любой человек (физическое или юридическое лицо) имеет право на подачу обращения в отдел рассмотрения обращений граждан, по вопросу, имеющему к нему непосредственное отношение.", replyMarkup: FAQOGButtons());
                    }
                    else if (e.Message.Text.Contains("Могу ли я обратиться к вам от имени другого человека?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Существует возможность подать обращение и от имени другого человека, с приложением к просьбе соответствующей доверенности.", replyMarkup: FAQOGButtons());
                    }
                    else if (e.Message.Text.Contains("Каким образом можно подать жалобу?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Физические и юридические лица вправе обращаться как в устной форме, так в письменной и электронной форме", replyMarkup: FAQOGButtons());
                    }
                    else if (e.Message.Text.Contains("2️⃣") && flag == 7)
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Oбрaщeниe грaждaн\nСтраница 2", replyMarkup: FAQOGButtons2());
                    }
                    else if (e.Message.Text.Contains("Какие обращения не будут рассматриваться отделом обращений граждан?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "- Письменные и электронные обращения, в которых не указаны фамилия, имя, отчество физического лица, сведения о месте его проживания или полное наименование юридического лица, адрес его места расположения, или сведения не соответствуют действительности, а также представлены без подписи (цифровой электронной подписи).\n- В котором содержатся нецензурные либо оскорбительные выражения, угрозы жизни, здоровью и имуществу.\n- В случае, если текст письменных обращений не поддается чтению.", replyMarkup: FAQOGButtons2());
                    }
                    else if (e.Message.Text.Contains("Существует ли возможность подачи обращения в устной форме?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Физические и юридические лица вправе обращаться в соответствующие органы и организации в устной форме.", replyMarkup: FAQOGButtons2());
                    }
                    else if (e.Message.Text.Contains("Что мне следует указать в обращении?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Фамилию, имя, отчество, адрес места жительства, суть обращения", replyMarkup: FAQOGButtons2());
                    }
                    else if (e.Message.Text.Contains("Каков период времени рассмотрения обращения?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "- Обращения рассматриваются в течение 30 дней, обращения, не требующие дополнительного изучения и исследования, рассматриваются в течение 15 дней со дня регистрации.\n- В исключительных случаях, руководитель соответствующего органа и организации вправе продлить срок рассмотрения обращения не более 30 дней о чем должен информировать заявителя в течение 3 дней.", replyMarkup: FAQOGButtons2());
                    }

                    else if (e.Message.Text.Contains("Oткрытиe счeтoв"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Oткрытиe счeтoв", replyMarkup: FAQOSButtons());
                    }
                    else if (e.Message.Text.Contains("Как можно открыть счет в банке?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Для того, чтобы открыть банковский счет, необходимо обратиться в кредитную финансовую организацию. Банковские счета для клиентов кредитных организаций открываются в национальной и или иностранной валютах в следующих видах: - депозитные счета; - депозитные счета до востребования; - срочные депозитные счета; - корреспондентские счета кредитных организаций.", replyMarkup: FAQOSButtons());
                    }
                    else if (e.Message.Text.Contains("Какие  документы необходимы для открытия счета? (физическое лицо"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Для открытия банковских счетов физические лица представляют в кредитную финансовую организацию следующие документы: - заявление на открытие банковского счета; - документ, удостоверяющий личность физического лица (паспорт);", replyMarkup: FAQOSButtons());
                    }
                    else if (e.Message.Text.Contains("Нужно ли платить комиссию за открытие счета?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "За открытие банковских счетов кредитные организации взимают комиссионное вознаграждение и исламские кредитные организации могут взимать комиссионное вознаграждение в соответствии с утвержденными в установленном порядке тарифами.", replyMarkup: FAQOSButtons());
                    }
                    else if (e.Message.Text.Contains("В каких случаях закрывается банковский счет?"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Банковские счета кредитных финансовых организаций закрываются в следующих случаях: заявлению владельца банковского счета в порядке и в сроки, определяемые договором банковского счета и/или договором банковского вклада; по решению органа, создавшего предприятие, объединение, организацию или учреждение либо органа обладающего таким правом; при расторжении договора банковского счета; требованию уполномоченной кредитной финансовой организации Республики Таджикистан в случае, когда сумма денежных средств в банковском счете или банковского вклада не превышает предусмотренный минимальный размер, указанный в заключенном договоре; также в иных случаях предусмотренных действующим законодательством.", replyMarkup: FAQOSButtons());
                    }

                    else if (e.Message.Text.Contains("◀ Назад"))
                    {
                        flag = 0;
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Часто задаваемые вопросы", replyMarkup: GetButtons5());
                    }



                    //Банковские продукты
                    else if (e.Message.Text.Contains("Банковские продукты"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Банковские продукты:", replyMarkup: GetButtons3());
                    }
                    else if (e.Message.Text.Contains("Кредиты"))
                    {
                        string imagePath = null;
                        imagePath = Path.Combine(Environment.CurrentDirectory, "docs/credit.png");
                        using (var stream = File.OpenRead(imagePath))
                        {
                            await _client.SendPhotoAsync(
                                e.Message.Chat.Id,
                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                caption: "Кредит - это предоставление денежных средств банком в долг на условиях возвратности.\nВ первую очередь о кредите стоит знать четыре его основных свойства. Это возвратность – заемщик может взять определенную сумму, однако при этом он берет на себя обязательства их вернуть. Платность – каким бы выгодным ни был кредит – это всегда услуга со стороны банка, и за нее потребуется платить. Срочность – при оформлении кредита строго оговариваются сроки, в которые заемщик будет его отдавать. Дифференцированность – особый подход в каждой отдельной ситуации.",
                                replyMarkup: GetInlineButton("www.fingram.tj/credits"));
                        }
                    }
                    else if (e.Message.Text.Contains("Депозиты"))
                    {
                        string imagePath = null;
                        imagePath = Path.Combine(Environment.CurrentDirectory, "docs/Deposits.jpg");
                        using (var stream = File.OpenRead(imagePath))
                        {
                            await _client.SendPhotoAsync(
                                e.Message.Chat.Id,
                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                caption: "Банковский вклад (или банковский депозит) — сумма денег, переданная лицом кредитному учреждению с целью получить доход в виде процентов, образующихся в ходе финансовых операций с вкладом.",
                                replyMarkup: GetInlineButton("www.fingram.tj/deposits"));
                        }
                    }
                    else if (e.Message.Text.Contains("Денежные переводы"))
                    {
                        string imagePath = null;
                        imagePath = Path.Combine(Environment.CurrentDirectory, "docs/Online-Money-Transfer-Software.jpg");
                        using (var stream = File.OpenRead(imagePath))
                        {
                            await _client.SendPhotoAsync(
                                e.Message.Chat.Id,
                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                caption: "Денежный перевод — это перевод (движение) денежных средств от отправителя к получателю с помощью операторов платежных систем через национальные или международные платежные системы с целью зачисления денежных средств на счет получателя или выдачи ему их в наличной форме. В структуре денежного перевода всегда присутствует отправитель, получатель и посредник — оператор платежной системы, взимающий за свои услуги определённую плату.",
                                replyMarkup: GetInlineButton("www.fingram.tj/moneytransfer"));
                        }
                    }
                    else if (e.Message.Text.Contains("Банковские карты"))
                    {
                        string imagePath = null;
                        imagePath = Path.Combine(Environment.CurrentDirectory, "docs/cards.png");
                        using (var stream = File.OpenRead(imagePath))
                        {
                            await _client.SendPhotoAsync(
                                e.Message.Chat.Id,
                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                caption: "Банковская карта - пластиковая карта, обычно привязанная к одному или нескольким расчётным счетам в банке. Используется для оплаты товаров и услуг, в том числе через Интернет, с использованием бесконтактной технологии, совершения переводов, а также снятия наличных.",
                                replyMarkup: GetInlineButton("www.fingram.tj/cards"));
                        }
                    }
                    else if (e.Message.Text.Contains("Интернет / Мобильный банкинг"))
                    {
                        string imagePath = null;
                        imagePath = Path.Combine(Environment.CurrentDirectory, "docs/MBanking.jpg");
                        using (var stream = File.OpenRead(imagePath))
                        {
                            await _client.SendPhotoAsync(
                                e.Message.Chat.Id,
                                new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream),
                                caption: "Интернет/Мобильный-банкинг — это общее название технологий дистанционного банковского обслуживания, а также доступ к счетам и операциям (по ним), предоставляющийся в любое время и с любого устройства, имеющего доступ в Интернет.",
                                replyMarkup: GetInlineButton("www.fingram.tj/ibank"));
                        }
                    }

                    //Кредитный калькулятор
                    else if (e.Message.Text.Contains("Кредитный калькулятор"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите желаемую сумму кредита цифрами: (Например, 10000)");
                        _client.OnMessage += GetNHandler;
                    }

                    //Депозитный калькулятор
                    else if (e.Message.Text.Contains("Депозитный калькулятор"))
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите желаемую сумму вклада цифрами: (Например, 10000)");
                        _client.OnMessage += GetN2Handler;
                    }

                    else
                    {
                        await _client.SendTextMessageAsync(e.Message.Chat.Id, "Я вас не понимаю 😕\nИспользуйте кнопки ниже:", replyMarkup: GetButtons());
                    }
                }
                else
                {
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, "Неправильный ввод! Вы можете выбрать из меню:", replyMarkup: GetButtons());
                }
            }
            catch (Exception ex)
            {
                await _client.SendTextMessageAsync(e.Message.Chat.Id, ex.ToString());
            }

        }
        
        [Obsolete]
        private async static void MessageNameHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= OnMessageHandler;

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                username = e.Message.Text;
            }
            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Ваш номер телефона:");
            _client.OnMessage -= MessageNameHandler;
            _client.OnMessage += MessageNumberHandler;
        }
        [Obsolete]
        private async static void MessageNumberHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= MessageNameHandler;

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                number = e.Message.Text;
            }
            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите, что вы хотите отправить нам на почту: ");
            _client.OnMessage -= MessageNumberHandler;
            _client.OnMessage += SendMessageHandler;
        }

        [Obsolete]
        private async static void SendMessageHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= MessageNumberHandler;
            string text = e.Message.Text;
            text += $"\n\nОтправитель: {username}\n\nТелефон: {number}";

            MailAddress fromAddress = new MailAddress(bot, "БОТ");
            MailAddress toAddress = new MailAddress(admin, "АДМИН");
            MailMessage message = new MailMessage(fromAddress, toAddress);
            message.Body = text;
            message.Subject = "Письмо с телеграм бота FinGramTJBot";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(bot, password);

            smtpClient.Send(message);

            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Спасибо! Ваше письмо успешно отправлено.", replyMarkup: GetButtons());

            _client.OnMessage -= SendMessageHandler;
            _client.OnMessage += OnMessageHandler;
        }

        [Obsolete]
        private static async void GetNHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= OnMessageHandler;
            if (!Double.TryParse(e.Message.Text, out n)) n = -1;
            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите срок кредита цифрами (в месяцах): (Например, 12)");
            _client.OnMessage -= GetNHandler;
            _client.OnMessage += GetMonthHandler;
        }

        [Obsolete]
        private static async void GetMonthHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= GetNHandler;
            if (!Int32.TryParse(e.Message.Text, out month)) month = -1;
            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите годовую процентную ставку цифрами: (Например, 30)");
            _client.OnMessage -= GetMonthHandler;
            _client.OnMessage += GetProcentHandler;
        }

        [Obsolete]
        private static async void GetProcentHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= GetMonthHandler;
            if (!Double.TryParse(e.Message.Text, out percent)) percent = -1;

            if (n != -1 && month != -1 && percent != -1)
            {
                double ps = percent / (100 * 12);
                double pp = -1 * (month);
                double q = Math.Pow((1 + ps), pp);
                double o = 1 - q;
                double ans = n * (ps / o);
                ans = Math.Round(ans, 2);
                await _client.SendTextMessageAsync(e.Message.Chat.Id, $"Ежемесячный взнос: {ans} сомони.\n" +
                    $"Сумма за весь период: {Math.Round((ans * month), 2)} сомони.", replyMarkup: GetButtons());
            }

            else
                await _client.SendTextMessageAsync(e.Message.Chat.Id, "Проверьте правильность введенных вами данных и попробуй заново.", replyMarkup: GetButtons());
            _client.OnMessage -= GetProcentHandler;
            _client.OnMessage += OnMessageHandler;
        }

        [Obsolete]
        private static async void GetN2Handler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= OnMessageHandler;
            if (!Double.TryParse(e.Message.Text, out n)) n = -1;
            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите срок вклада цифарми (в месяцах): (Например, 12)");
            _client.OnMessage -= GetN2Handler;
            _client.OnMessage += GetMonth2Handler;
        }

        [Obsolete]
        private static async void GetMonth2Handler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= GetN2Handler;
            if (!Int32.TryParse(e.Message.Text, out month)) month = -1;
            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Введите годовую процентную ставку цифрами: (Например, 30)");
            _client.OnMessage -= GetMonth2Handler;
            _client.OnMessage += GetProcent2Handler;
        }

        [Obsolete]
        private static async void GetProcent2Handler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= GetMonth2Handler;
            if (!Double.TryParse(e.Message.Text, out percent)) percent = -1;
            await _client.SendTextMessageAsync(e.Message.Chat.Id, "Выберите вариант вклада: \n(\"Без капитализации\" - ежемесячно получаете проценты по вкладу, а в конце срока сумму вклада; \"С капитализацией\" - и проценты и сумму вклада получаете в конце срока).", replyMarkup: GetButtons2());
            _client.OnMessage -= GetProcent2Handler;
            _client.OnMessage += GetCapitalizationHandler;
        }

        [Obsolete]
        private static async void GetCapitalizationHandler(object sender, MessageEventArgs e)
        {
            _client.OnMessage -= GetProcent2Handler;
            if (e.Message.Text == "Без капитализации") capitalization = "Без капитализации";
            else if (e.Message.Text == "С капитализацией") capitalization = "С капитализацией";
            else capitalization = null;
            if (n != -1 && month != -1 && percent != -1 && capitalization != null)
            {
                double ans;
                //await _client.SendTextMessageAsync(e.Message.Chat.Id, "В РАЗРАБОТКЕ 😉", replyMarkup: GetButtons());
                if (capitalization == "Без капитализации")
                {
                    ans = (n * percent * month / 12) / 100;
                    double p = (ans * 12) / 100;
                    ans = Math.Round(ans - p, 2);
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, $"Начисленные проценты (с учетом НДС): {ans} сомони.\n" +
                        $"Общая сумма: {Math.Round((ans + n), 2)} сомони.", replyMarkup: GetButtons());
                }
                if (capitalization == "С капитализацией")
                {
                    double N = percent / 100;
                    double a = 1 + (N / 12);
                    double o = Math.Pow(a, month);
                    ans = n * o;
                    ans -= n;
                    double p = (ans * 12) / 100;
                    ans = Math.Round(ans - p, 2);
                    await _client.SendTextMessageAsync(e.Message.Chat.Id, $"Начисленные проценты (с учетом НДС): {ans} сомони.\n" +
                        $"Общая сумма: {Math.Round((ans + n), 2)} сомони.", replyMarkup: GetButtons());
                }
            }
                
            else
                await _client.SendTextMessageAsync(e.Message.Chat.Id, "Проверьте правильность введенных вами данных и попробуй заново.", replyMarkup: GetButtons());
            _client.OnMessage -= GetCapitalizationHandler;
            _client.OnMessage += OnMessageHandler;
        }

        // main buttons
        private static IReplyMarkup GetButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "💳 Банковские продукты" }, new KeyboardButton { Text = "🏦 Кредитные организации" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "🧮 Кредитный калькулятор" }, new KeyboardButton { Text = "🧮 Депозитный калькулятор" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "💵 Курс валюты" }, new KeyboardButton { Text = "☎️ Телефон доверия НБТ" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "📧 Обращение в НБТ" }, new KeyboardButton { Text = "⁉ Часто задаваемые вопросы" } }
                },
                ResizeKeyboard = true
                //,OneTimeKeyboard = true
            };
        }

        // depos buttons
        private static IReplyMarkup GetButtons2()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Без капитализации" }, new KeyboardButton { Text = "С капитализацией" } }
                },
                ResizeKeyboard = true
                //,OneTimeKeyboard = true
            };
        }

        // credit organization buttons
        private static IReplyMarkup GetButtons4()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Банки" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "Микродепозитные организации" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Микрокредитные организации" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Микрокредитные фонды" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
                //,OneTimeKeyboard = true
            };
        }
        
        // FAQ buttons p.1
        private static IReplyMarkup GetButtons5()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Бaнкoвские кaрты" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Вaлютые oпeрaции" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Дeнeжные пeрeвoды" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Дeпoзиты" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "🏠 На главную" }, new KeyboardButton { Text = "▶ Далее" } }
                },
                ResizeKeyboard = true
                //,OneTimeKeyboard = true
            };
        }
        // FAQ buttons p.2
        private static IReplyMarkup GetButtons51()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Крeдитнoe бюрo" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Крeдитныe oпeрaции" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Oбрaщeниe грaждaн" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Oткрытиe счeтoв" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
                //,OneTimeKeyboard = true
            };
        }
        
        //products buttons
        private static IReplyMarkup GetButtons3()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Кредиты" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Депозиты" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Денежные переводы" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Банковские карты" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Интернет / Мобильный банкинг" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }

        //Bankovskie karti buttons
        private static IReplyMarkup FAQBKButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как я могу получить банковскую карту?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие существуют виды банковских карт?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие операции я могу осуществить посредством банковской карты?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "В каких случаях у меня могут изъять или блокировать банковскую платежную карту?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        
        //Valyutnie operacii buttons
        private static IReplyMarkup FAQVOButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Можно ли оплачивать покупки иностранной валютой?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Где я могу приобрести иностранную валюту?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Почему на товарах нет цен в иностранной валюте?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Имеются ли ограничения на ввоз валюты в Республику Таджикистан?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" } }, 
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQVOButtons2()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какую сумму в иностранной валюте при вывозе мне придётся декларировать?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какую сумму в иностранной валюте я могу вывезти из Республики Таджикистан без предоставления документов, подтверждающих источник происхождения валютных ценностей?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "При какой сумме в иностранной валюте при вывозе мне придётся предоставлять документы, подтверждающие источник происхождения валютных ценностей?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие документы являются основанием для вывоза валютных ценностей из Республики Таджикистан?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }

        //Denejnie perevodi buttons
        private static IReplyMarkup FAQDPButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Могу ли я получить или отправить деньги из РТ?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как мне получить или отправить деньги, если они связаны с коммерческой деятельностью?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Каким образом я могу получить или отправить деньги?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "В какой валюте я могу получить денежный перевод?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQDPButtons2()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Можно ли перевести валюту, которая была ранее ввезена в Республику Таджикистан?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какую сумму можно перевести из Республики Таджикистан?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Под каким назначением можно перевести деньги из Республики Таджикистан?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как мне перевести валюту из Республики Таджикистан, если я не гражданин этой страны?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }

        //Depoziti buttons
        private static IReplyMarkup FAQDButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Возмещаются ли проценты по депозитам, если кредитная организация объявлена банкротом?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Сколько денег мне возместит Государство?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие проценты выплачиваются если снять вклад досрочно?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что с процентами, если я не требую возврата вклада по истечению срока?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQDButtons2()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "На какую сумму я могу рассчитывать, если имею несколько вкладов в национальной валюте при страховом случае?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "На какую сумму я могу рассчитывать, если имею несколько вкладов в иностранной валюте при страховом случае?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "На какую сумму я могу рассчитывать, если имею несколько вкладов в национальной и иностранной валюте при страховом случае?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что мне делать, если у меня несколько вкладов в разных организациях при страховом случае?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQDButtons3()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как возмещаются сбережения, размещённые в национальной валюте при страховом случае?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как возмещаются сбережения, размещённые в иностранной валюте при страховом случае?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }

        //kreditnoe byuro buttons
        private static IReplyMarkup FAQKBButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что такое кредитное бюро?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Могу ли я получить информацию о себе от кредитного бюро и платна ли она?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Отправляют ли информацию обо мне в кредитное бюро без согласия моего согласия?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Сколько лет может храниться информация в кредитном бюро?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQKBButtons2()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Кто несёт ответственность за искажение информации о клиентах в кредитном бюро?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Кто может увидеть мою кредитную историю?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Ведёт ли кредитное бюро «чёрный» список неплательщиков?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Почему кредитное бюро передает информацию обо мне как о «неплательщике»?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }

        //kreditnie operacii buttons
        private static IReplyMarkup FAQKOButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Сколько времени понадобится, чтобы получить одобрение на получение кредита?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Могу ли я получить кредит в долларах США?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие виды залога принимаются?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как начисляются проценты по кредитам?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" }, 
                                               new KeyboardButton { Text = "4️⃣" }, new KeyboardButton { Text = "5️⃣" }, new KeyboardButton { Text = "6️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQKOButtons2()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что такое аннуитетные платежи?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что такое дифференцированные платежи?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что такое эффективная процентная ставка?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Где я могу досрочно погасить свой кредит?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" },
                                               new KeyboardButton { Text = "4️⃣" }, new KeyboardButton { Text = "5️⃣" }, new KeyboardButton { Text = "6️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQKOButtons3()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Могу ли я передать сумму погашения кредитному специалисту или сотруднику Банка?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Может ли третье лицо осуществить выплату кредита за клиента? Что для этого потребуется?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что будет, если я забуду погасить кредит вовремя?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Могу ли я полностью или частично погасить свой кредит? Если да, то буду ли я оштрафован или наказан за это?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" },
                                               new KeyboardButton { Text = "4️⃣" }, new KeyboardButton { Text = "5️⃣" }, new KeyboardButton { Text = "6️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQKOButtons4()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Мне отказали в выдаче кредита без объяснения причин. Почему?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Если мой предыдущий запрос кредита был отклонен, можно ли ожидать положительного решения при подаче заявления на новый кредит?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что происходит с погашением кредита в случае смерти клиента?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Почему, кроме кредитных специалистов, нас проверяют ещё и другие сотрудники кредитной организации?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" },
                                               new KeyboardButton { Text = "4️⃣" }, new KeyboardButton { Text = "5️⃣" }, new KeyboardButton { Text = "6️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQKOButtons5()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Могу ли я выплатить долг в национальной валюте, даже если мой долг получен в долларах США?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Предоставляет ли кредитная организация кредит нерезидентам?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие другие услуги предлагает кредитная организация, кроме кредитов?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Если у меня возникнут проблемы с филиалом или ЦБО, к кому я должен обратиться?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" },
                                               new KeyboardButton { Text = "4️⃣" }, new KeyboardButton { Text = "5️⃣" }, new KeyboardButton { Text = "6️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQKOButtons6()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "В чем заключается ответственность поручителя?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Почему клиент должен открыть депозитный счет, если он заинтересован только в получении кредита?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как получить справку об отсутствии задолженности?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }, new KeyboardButton { Text = "3️⃣" },
                                               new KeyboardButton { Text = "4️⃣" }, new KeyboardButton { Text = "5️⃣" }, new KeyboardButton { Text = "6️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        //obrawenie grajdan buttons
        private static IReplyMarkup FAQOGButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие обращения рассматриваются в отделе обращений граждан?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Кто имеет право на подачу обращения?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Могу ли я обратиться к вам от имени другого человека?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Каким образом можно подать жалобу?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        private static IReplyMarkup FAQOGButtons2()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие обращения не будут рассматриваться отделом обращений граждан?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Существует ли возможность подачи обращения в устной форме?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Что мне следует указать в обращении?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Каков период времени рассмотрения обращения?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "1️⃣" }, new KeyboardButton { Text = "2️⃣" }},
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }
        // otkritie wetov buttons
        private static IReplyMarkup FAQOSButtons()
        {
            return new ReplyKeyboardMarkup
            {
                Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton { Text = "Как можно открыть счет в банке?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Какие  документы необходимы для открытия счета? (физическое лицо)" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "Нужно ли платить комиссию за открытие счета?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "В каких случаях закрывается банковский счет?" } },
                    new List<KeyboardButton> { new KeyboardButton { Text = "◀ Назад" }, new KeyboardButton { Text = "🏠 На главную" } }
                },
                ResizeKeyboard = true
            };
        }

        // inline buttons
        private static IReplyMarkup GetInlineButton(string url)
        {
            return new InlineKeyboardMarkup(new InlineKeyboardButton { Text = "Подробнее", Url = url });
        }
    }

    class Currency
    {
        public float USD { get; set; }
        public float EUR { get; set; }
        public float RUB { get; set; }
    }
}