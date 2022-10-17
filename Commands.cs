
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using YouTubeUploadUtilites.Extensions;

namespace YouTubeUploadUtilites
{
    internal class Commands
    {
        public static async Task RemoveDuplicated(ITelegramBotClient botClient, Message message)
        {
            string path = $@"./Downloads/{Guid.NewGuid()}_{message.Document.FileName}";
            List<string> text;
            using (var fileStream = System.IO.File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                await botClient.GetInfoAndDownloadFileAsync(message.Document.FileId, fileStream);
                fileStream.Close();
                text = System.IO.File.ReadAllText(path).Split(',').ToList();
                System.IO.File.Delete(path);
            }
            var tags = text.Distinct();
            await System.IO.File.WriteAllTextAsync(path, String.Join(',', tags));
            using (var fileStream = System.IO.File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                await botClient.SendDocumentAsync(
                        chatId: message.Chat.Id,
                        document: new InputOnlineFile(content: fileStream, fileName: "tags.txt"),
                        caption: "Дубликаты удалены",
                        replyMarkup: Keyboards.GetStandKeyboard());
            }
            //await using Stream stream = System.IO.File.OpenRead(path);

        }

        public static async Task GetCountry(ITelegramBotClient botClient, Message message)
        {
            try
            {
                RegionInfo info = new RegionInfo(message.Text);
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: $"Скорее всего это {info.DisplayName}",
                    replyMarkup: Keyboards.GetStandKeyboard());
            }
            catch (ArgumentException argEx)
            {
                await botClient.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Не удалось найти страну по данному коду",
                    replyMarkup: Keyboards.GetStandKeyboard());
            }
        }
    }
}
