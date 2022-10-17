using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace YouTubeUploadUtilites.Extensions
{
    internal class Keyboards
    {
        public static ReplyKeyboardMarkup GetStandKeyboard()
        {
            return new ReplyKeyboardMarkup(new[] {
                    new KeyboardButton[] { "✖️ Удалить дубликаты из Тэгов" /*, "🔎 Проверить соответствие тэгов и названия"*/ },
                    new KeyboardButton[] { "🌎 Получить страну по коду"},
                })
            {
                ResizeKeyboard = true
            };
        }

        public static InlineKeyboardMarkup GetCancelInlineKeyboard()
        {
            return new(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Отмена", callbackData: "Cancel"),
                    }
                });
        }
    }
}
