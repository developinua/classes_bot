namespace TarasZoukClasses.Domain.Commands
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Utils;

    public class StartCommand : ICommand
    {
        public string Name => @"/start";

        public string CallbackQueryPattern => @"(?<query>\w+):(?<data>\w{2}-\w{2})";

        public string ResponseMessage { get; set; } = "Привіт! 😊\n\n" +
            "Якою мовою ти бажаєш спілкуватися?\n" +
            "What language do you want to communicate in?\n" +
            "На каком языке ты хочешь общаться?";

        public bool Contains(Message message)
        {
            return message.Type == MessageType.Text && message.Text.Contains(Name);
        }

        public bool Contains(string callbackQueryData)
        {
            return new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;
        }

        public async Task Execute(Message message, TelegramBotClient botClient, IUnitOfWork services)
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

        public async Task Execute(CallbackQuery callbackQuery, TelegramBotClient botClient, IUnitOfWork services)
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

            // TODO: Save data
            //callbackQuery.Message.Chat.Username                   Save
            //callbackQuery.Message.Date                            Save as User Creation date

            // TODO: Reply due to chosen language.
            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, $"You have choosen: {cultureName}", true);

            throw new NotImplementedException();
        }
    }
}
