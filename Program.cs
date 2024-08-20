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

    // Это клиент для работы с Telegram Bot API, который позволяет отправлять сообщения, управлять ботом, подписываться на обновления и многое другое.
    private static ITelegramBotClient _botClient;

    // Это объект с настройками работы бота. Здесь мы будем указывать, какие типы Update мы будем получать, Timeout бота и так далее.
    private static ReceiverOptions _receiverOptions;

    static async Task Main()
    {

        _botClient = new TelegramBotClient("7275747390:AAE7Rbgle7itlNhyhrqW04eTc4z2aGdFGAI"); // Присваиваем нашей переменной значение, в параметре передаем Token, полученный от BotFather
        _receiverOptions = new ReceiverOptions // Также присваем значение настройкам бота
        {
            AllowedUpdates = new[] // Тут указываем типы получаемых Update`ов, о них подробнее расказано тут https://core.telegram.org/bots/api#update
            {
                UpdateType.Message, // Сообщения (текст, фото/видео, голосовые/видео сообщения и т.д.)
            },
            // Параметр, отвечающий за обработку сообщений, пришедших за то время, когда ваш бот был оффлайн
            // True - не обрабатывать, False (стоит по умолчанию) - обрабаывать
            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();

        // UpdateHander - обработчик приходящих Update`ов
        // ErrorHandler - обработчик ошибок, связанных с Bot API
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token); // Запускаем бота

        var me = await _botClient.GetMeAsync(); // Создаем переменную, в которую помещаем информацию о нашем боте.
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1); // Устанавливаем бесконечную задержку, чтобы наш бот работал постоянно
    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Обязательно ставим блок try-catch, чтобы наш бот не "падал" в случае каких-либо ошибок
        try
        {
            // Сразу же ставим конструкцию switch, чтобы обрабатывать приходящие Update
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        // эта переменная будет содержать в себе все связанное с сообщениями
                        var message = update.Message;

                        // From - это от кого пришло сообщение (или любой другой Update)
                        var user = message.From;

                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        // Chat - содержит всю информацию о чате
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
                                            replyMarkup: replyKeyboard); // опять передаем клавиатуру в параметр replyMarkup

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
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
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