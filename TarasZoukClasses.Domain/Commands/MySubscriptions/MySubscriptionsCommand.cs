namespace TarasZoukClasses.Domain.Commands.MySubscriptions
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

    public class MySubscriptionsCommand : ICommand
    {
        public string Name => @"/mysubscriptions";

        public string CallbackQueryPattern => @"(?i)(?<query>subsgroup|subsperiod)";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public async Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            await client.SendChatActionAsync(message.From.Id, ChatAction.Typing);
            await MySubscriptionsCommandHelper.GetZoukUserSubscriptionInformation(message, client, services);
        }

        public async Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            if (callbackQuery.Validate())
                throw new NotSupportedException();

            var chatId = callbackQuery.From.Id;
            await client.SendChatActionAsync(chatId, ChatAction.Typing);
            await MySubscriptionsCommandHelper.ParseSubscription(chatId, callbackQuery, client, services);
        }
    }
}
