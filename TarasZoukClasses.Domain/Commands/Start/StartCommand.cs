namespace TarasZoukClasses.Domain.Commands.Start
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

        public string CallbackQueryPattern => @"(?i)(?<query>language):(?<data>\w{2}-\w{2})";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public async Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            await client.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            if (string.IsNullOrEmpty(message.From.Username))
            {
                await client.SendTextMessageAsync(message.Chat.Id, "Fill in username and press /start again", ParseMode.Markdown);
                return;
            }

            const string responseMessage = "*😊 Hi!\n\n*What language do you want to communicate in?";
            var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
                .AddButton("English", "language:en-US")
                .Build();

            await client.SendTextMessageAsync(message.Chat.Id, responseMessage, ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
        }

        public async Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            if (callbackQuery.Validate())
                throw new NotSupportedException();

            var chatId = callbackQuery.From.Id;
            const string responseCallbackQueryMessage = "*😊Successfully!😊*\nPress /mysubscriptions to manage your class subscription.";

            await client.SendChatActionAsync(chatId, ChatAction.Typing);
            await StartCommandHelper.SaveUser(services, callbackQuery, CallbackQueryPattern);
            await client.SendTextMessageAsync(chatId, responseCallbackQueryMessage, ParseMode.Markdown);
        }
    }
}
