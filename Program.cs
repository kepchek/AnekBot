using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{

    private static ITelegramBotClient _botClient;

    private static ReceiverOptions _receiverOptions;

    static async Task Main()
    {

        _botClient = new TelegramBotClient("7275747390:AAE7Rbgle7itlNhyhrqW04eTc4z2aGdFGAI"); 
        _receiverOptions = new ReceiverOptions 
        {
            AllowedUpdates = new[] 
            {
                UpdateType.Message, 
            },
            
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();

        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); 

        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1); 
    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        var message = update.Message;
                        var user = message.From;

                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        var chat = message.Chat;
                        switch(message.Type)
                        {
                            case MessageType.Text:
                                {
                                    if (message.Text == "/start")
                                    {
                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Здарова, чтоб получить анек, напиши - Анек");
                                        var replyKeyboard = new ReplyKeyboardMarkup(
                                        new List<KeyboardButton[]>()
                                        {
                                            new KeyboardButton[]
                                            {
                                                new KeyboardButton("Анек"),
                                            },

                                        })
                                        {
                                            ResizeKeyboard = true,
                                        };

                                        await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            "Кайфуйте",
                                            replyMarkup: replyKeyboard); 

                                        return;
                                        }
                                        if(message.Text == "СВО")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                $"Гражданину {user.Username}" +
                                                $" Серия, номер:{user.Id}" +
                                                $" В соответствии с Федеральным законом \"О воинской обязанности и военной службе\"" +
                                                $" Вы подлежите первоначальной постановке на воинский учет и обязаны явиться в военный комиссариат в день получения повестки!" +
                                                $" При себе иметь свидетельство о рождении, паспорт (иной документ, удостоверяющий личность), а также" +
                                                $" справку с места жительства и о семейном положении, справку с места работы или учебы, фотографии" +
                                                $" размером 3 x 4 - 6 шт., документ об образовании, медицинские документы о состоянии здоровья и помытую попу.\n" +
                                                $"Военный комиссар Абдулбек Шахазарович Дрын угли");
                                        }
                                        if(message.Text == "Сом"||message.Text == "сом")
                                        {
                                            await botClient.SendTextMessageAsync(
                                                chat.Id,
                                                "Сом сосом");
                                        }
                                        if(message.Text == "Анек")
                                        {
                                        Random rand = new Random();
                                        int randomId = rand.Next(0, 1101);  

                                        string url = $"https://baneks.ru/{randomId}";

                                        var config = Configuration.Default.WithDefaultLoader();
                                        var context = BrowsingContext.New(config);
                                        try
                                        {
                                            
                                            var document = await context.OpenAsync(url);

                                            var articleElement = document.QuerySelector("article p");
                                            string jokeText = articleElement?.TextContent ?? "Анекдот не найден";
                                            Console.WriteLine($"Анекдот #{randomId}");
                                            await botClient.SendTextMessageAsync(
                                            chat.Id,
                                            jokeText);
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"Ошибка при парсинге: {ex.Message}");
                                        }
                                        }
                                        

                                    return;
                                }

                        }
                        return;
                    }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
