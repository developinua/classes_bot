﻿namespace TarasZoukClasses.Domain.Utils
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

        public InlineKeyboardBuilder AddButton(string text, string callbackData)
        {
            Rows.Add(new InlineKeyboardButton
            {
                Text = text,
                CallbackData = callbackData
            });

            return this;
        }

        public InlineKeyboardBuilder AddUrlButton(string text, string callbackData, string url)
        {
            Rows.Add(new InlineKeyboardButton
            {
                Text = text,
                CallbackData = callbackData,
                Pay = true,
                Url = url
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
