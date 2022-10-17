using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using YouTubeUploadUtilites.Extensions;

namespace YouTubeUploadUtilites
{
    internal class Handlers
    {
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            //Обработка ForceReply
            if (message.ReplyToMessage != null)
            {
                await CallbackHandler(botClient, message);
            }
            switch (message.Text)
            {
                case "/start":
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Welcome",
                            replyMarkup: Keyboards.GetStandKeyboard());
                    return;
                case "/repair":
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Чинимся 🔨",
                            replyMarkup: Keyboards.GetStandKeyboard());
                    return;
                case "✖️ Удалить дубликаты из Тэгов":
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Отправьте файл в формате .txt для удаления повторяющихся тэгов",
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                    return;
                case "🔎 Проверить соответствие тэгов и названия":
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Пока что не реализованно",
                            replyMarkup: Keyboards.GetStandKeyboard());
                    return;
                case "🌎 Получить страну по коду":
                    await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Введите код страны",
                            replyMarkup: new ForceReplyMarkup { Selective = true });
                    return;
                default:
                    return;
            }

        }

        /// <summary>
        /// Обработка ForceReply (быстрых ответов) Не клавиатура
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task CallbackHandler(ITelegramBotClient botClient, Message message)
        {
            switch (message.ReplyToMessage?.Text)
            {
                case String a when a.Contains("Отправьте файл в формате .txt для удаления повторяющихся тэгов"):
                    if (message.Document != null && message.Document?.MimeType == "text/plain")
                        await Commands.RemoveDuplicated(botClient, message);
                    else
                        await botClient.SendTextMessageAsync(
                            chatId: message.Chat.Id,
                            text: "Отменено. Вы отправили не txt файл",
                            replyMarkup: Keyboards.GetStandKeyboard());
                    return;
                case String a when a.Contains("Введите код страны"):
                    await Commands.GetCountry(botClient, message);
                    return;
            }
        }

        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            var msg = callbackQuery.Message;
            var chatID = msg.Chat.Id;
            switch (callbackQuery.Data)
            {
               
            }
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

    }
}
