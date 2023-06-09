using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    class Program
    {
        private static TelegramBotClient? _botClient;
        private static readonly string botToken = "6179606733:AAGKkMhHWhmllGHec92Hj_sO9EjgzebsUYU";
        private static readonly string apiUrl = "https://localhost:7092";

        static void Main()
        {
            _botClient = new TelegramBotClient(botToken);
            _botClient.OnMessage += Bot_OnMessage;
            _botClient.StartReceiving();

            Console.WriteLine("Bot started. Press any key to exit.");
            Console.ReadKey();

            _botClient.StopReceiving();
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
                    var price = await GetProductPriceAsync(messageText);

                    if (price != null)
                    {
                        await _botClient.SendTextMessageAsync(chatId, $"Товар: {messageText}\nЦіна: {price}");
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(chatId, "Товар не знайдено.");
                    }
                }
            }
        }

        private static async Task SendWelcomeMessageAsync(long chatId)
        {
            var welcomeMessage = "Ласкаво просимо до Price Checker Bot!\n\nЯ допоможу вам знайти ціни на товари. Ви можете скористатися командою /search, щоб знайти ціну певного товару.";
            await _botClient.SendTextMessageAsync(chatId, welcomeMessage);
        }

        private static async Task SendSearchMessageAsync(long chatId)
        {
            var searchMessage = "Будь ласка, введіть назву товару:";
            await _botClient.SendTextMessageAsync(chatId, searchMessage);
        }

        private static async Task<string> GetProductPriceAsync(string productName)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"{apiUrl}/api/products/{productName}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var product = JsonConvert.DeserializeObject<Product>(content);
                        return product.Price.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error connecting to API: " + ex.Message);
                }
            }

            return null;
        }

    }

    class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
