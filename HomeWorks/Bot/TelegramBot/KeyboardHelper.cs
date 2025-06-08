using Telegram.Bot.Types.ReplyMarkups;

namespace Bot;

public static class KeyboardHelper
{
    public static ReplyKeyboardMarkup GetDefaultKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "/addtask", "/showalltasks", "/showtasks", "/report" }
        })
        {
            ResizeKeyboard = true
        };
    }
}