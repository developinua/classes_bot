namespace TarasZoukClasses.Models.TelegramBotModels.Commands
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using TarasZoukClasses.Models.TelegramBotModels;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class StartCommand : ICommand
    {
        public string CommandName => @"/start";

        public string CallbackQueryPattern => @"(?<query>\w+):(?<data>\w{2}-\w{2})";

        public string ResponseMessage { get; set; } = "Привіт! 😊\n\n" +
            "Якою мовою ти бажаєш спілкуватися?\n" +
            "What language do you want to communicate in?\n" +
            "На каком языке ты хочешь общаться?";

        public bool Contains(Message message)
        {
            return message.Type == MessageType.Text && message.Text.Contains(CommandName);
        }

        public bool Contains(string callbackQueryData)
        {
            return new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;
        }

        public async Task Execute(Message message, TelegramBotClient botClient)
        {
            if (botClient == null)
            {
                throw new ArgumentNullException(nameof(TelegramBotClient));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(Message));
            }

            var chatId = (message.Chat?.Id).GetValueOrDefault();

            await botClient.SendChatActionAsync(chatId, ChatAction.Typing);

            var replyKeyboardMarkup = InlineKeyboardBuilder.Create(chatId)
                .SetText(ResponseMessage)
                .AddButton("Ukrainian", "language:uk-UA")
                .AddButton("Russian", "language:ru-RU")
                .AddButton("English", "language:en-US")
                .Build();

            await botClient.SendTextMessageAsync(chatId,
                ResponseMessage,
                parseMode: ParseMode.Markdown,
                replyMarkup: replyKeyboardMarkup);
        }

        public async Task Execute(CallbackQuery callbackQuery, TelegramBotClient botClient)
        {
            // TODO: Save user information (ChatId, CultureName, UserName)
            var chatId = callbackQuery.From.Id;
            var cultureName = string.Empty;

            if (callbackQuery.From.IsBot
                || callbackQuery.Message.Chat.Type != ChatType.Private
                || callbackQuery.Message.ForwardFromChat != null)
            {
                throw new NotSupportedException();
            }

            await botClient.SendChatActionAsync(chatId, ChatAction.Typing);

            var cultureMatch = Regex.Match(callbackQuery.Data, CallbackQueryPattern);

            if (cultureMatch.Success && cultureMatch.Groups["query"].Value == "language")
                cultureName = cultureMatch.Groups["data"].Value;

            //callbackQuery.Message.Chat.Username                   Save
            //callbackQuery.Message.Date                            Save as User Creation date

            //await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "You hav choosen " + language, true);
            //await botClient.AnswerCallbackQueryAsync(callbackQuery.Id); // отсылаем пустое, чтобы убрать "частики" на кнопке
            //await botClient.SendTextMessageAsync(message.Chat.Id, "Ну, за охоту!", replyToMessageId: message.MessageId); //To reply for message
            throw new NotImplementedException();
        }
    }
}
