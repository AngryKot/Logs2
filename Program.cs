﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Microsoft.VisualBasic;
using TgBot00;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Data.Sqlite;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using NLog;

namespace TelegramBotExperiments
{



     class Program
    {

        static string file = System.IO.File.ReadAllText(@"Token.txt");
        static ITelegramBotClient bot = new TelegramBotClient(file.ToString());

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            logger.Debug("log {0}", "/start /bb /help"); 
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                System.IO.File.WriteAllText("text.txt", $"Message:{message.Text}, message_id:{message.MessageId}");//сделать цикл
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Приветствую 🎶 ");
                    await botClient.SendTextMessageAsync(message.Chat, "Какой плейлист желаете послушать сегодня? 🔉 ");
                    return;
                    
                }
                if (message.Text.ToLower() == "/bb")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "До свидания");
                    return;
                }
                if (message.Text.ToLower() == "/help")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Данный бот явлется продуктом учебной практики студента \n" +
                        "В нём храниться несколько плейлистов, выводимых по совокумности нажатых клавиш \n" +
                        "Достумные команды:\n" +
                        "/start\n" +
                        "/help \n" +
                        "/bb "
                        );
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, "Не могу уловить ваши ноты");




            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            logger.Debug("log {0}", "Event handler");

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
            using (var connection = new SqliteConnection("Data Source=usersdata.db"))
            {
                connection.Open();
            }
            Console.Read();
        }
    }
}