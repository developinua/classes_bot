namespace TarasZoukClasses.Domain.Utils
{
    using System.Collections.Generic;
    using Telegram.Bot.Types.ReplyMarkups;

    public class InlineKeyboardBuilder
    {
        #region Properties: Private

        private List<List<InlineKeyboardButton>> Keyboard { get; set; }

        private List<InlineKeyboardButton> Rows { get; set; }

        #endregion

        #region Constructor

        private InlineKeyboardBuilder()
        {
            Keyboard = new List<List<InlineKeyboardButton>>();
            Rows = new List<InlineKeyboardButton>();
        }

        #endregion

        #region Methods: Public

        public static InlineKeyboardBuilder Create()
        {
            return new InlineKeyboardBuilder();
        }

        public static InlineKeyboardBuilder Create(long chatId)
        {
            var builder = new InlineKeyboardBuilder();
            builder.SetChatId(chatId);

            return builder;
        }

        public InlineKeyboardBuilder SetChatId(long chatId)
        {
            return this;
        }

        public InlineKeyboardBuilder SetText(string text)
        {
            return this;
        }

        public InlineKeyboardBuilder AddButton(string text, string callbackData)
        {
            Rows.Add(new InlineKeyboardButton
            {
                Text = text,
                CallbackData = callbackData
            });

            return this;
        }

        public InlineKeyboardBuilder NewLine()
        {
            Keyboard.Add(Rows);
            Rows = new List<InlineKeyboardButton>();

            return this;
        }

        public InlineKeyboardMarkup Build()
        {
            Keyboard.Add(Rows);
            return new InlineKeyboardMarkup(Keyboard);
        }

        #endregion
    }
}
