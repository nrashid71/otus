using Telegram.Bot.Types.ReplyMarkups;

namespace Bot;

public static class KeyboardHelper
{
    public static ReplyKeyboardMarkup GetDefaultKeyboard()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "/addtask", "/show", "/report" }
        })
        {
            ResizeKeyboard = true
        };
    }
    
    public static InlineKeyboardMarkup YesNoKeyboard()
    {
        return new InlineKeyboardMarkup(
            new[]{
                new[]{
                    InlineKeyboardButton.WithCallbackData("✅Да", "yes"),
                    InlineKeyboardButton.WithCallbackData("❌Нет", "no")
                }
            });
    }
    
}