﻿using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PriceCheckerBot
{
    class Program
    {
        private static TelegramBotClient botClient;
        private static readonly string botToken = "6179606733:AAGKkMhHWhmllGHec92Hj_sO9EjgzebsUYU";
        private static readonly string apiUrl = "https://localhost:7092/swagger/index.html";

        static void Main()
        {
            botClient = new TelegramBotClient(botToken);
            botClient.OnMessage += Bot_OnMessage;
            botClient.StartReceiving();

            Console.WriteLine("Bot started. Press any key to exit.");
            Console.ReadKey();

            botClient.StopReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Type == MessageType.Text)
            {
                var chatId = e.Message.Chat.Id;
                var messageText = e.Message.Text;

                if (messageText == "/start")
                {
                    await SendWelcomeMessageAsync(chatId);
                }
                else if (messageText == "/search")
                {
                    await SendSearchMessageAsync(chatId);
                }
                else
                {
                    // Here you can implement the logic to connect to your web API and retrieve the price information
                    var price = await GetProductPriceAsync(messageText);

                    if (price != null)
                    {
                        await botClient.SendTextMessageAsync(chatId, $"Товар: {messageText}\nЦіна: {price}");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId, "Товар не знайдено.");
                    }
                }
            }
        }

        private static async Task SendWelcomeMessageAsync(long chatId)
        {
            var welcomeMessage = "Ласкаво просимо до Price Checker Bot!\n\nЯ допоможу вам знайти ціни на товари. Ви можете скористатися командою /search, щоб знайти ціну певного товару.";
            await botClient.SendTextMessageAsync(chatId, welcomeMessage);
        }

        private static async Task SendSearchMessageAsync(long chatId)
        {
            var searchMessage = "Будь ласка, введіть назву товару:";
            await botClient.SendTextMessageAsync(chatId, searchMessage);
        }

        private static async Task<string> GetProductPriceAsync(string productName)
        {
            // Connect to your web API and retrieve the price information
            // You can use HttpClient or any other HTTP client library to make the request
            // Example:
            // using (var client = new HttpClient())
            // {
            //     var response = await client.GetAsync($"{apiUrl}/products?name={productName}");
            //     if (response.IsSuccessStatusCode)
            //     {
            //         var content = await response.Content.ReadAsStringAsync();
            //         var products = JsonConvert.DeserializeObject<List<Product>>(content);
            //         if (products.Count > 0)
            //         {
            //             return products[0].Price.ToString();
            //         }
            //     }
            // }

            // For demonstration purposes, return a sample price
            return "$X.XX";
        }
    }
}
