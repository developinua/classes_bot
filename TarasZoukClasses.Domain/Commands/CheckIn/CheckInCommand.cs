namespace TarasZoukClasses.Domain.Commands.CheckIn
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Telegram.Bot.Types.ReplyMarkups;
    using Utils;

    public class CheckInCommand : ICommand
    {
        public string Name => @"/checkin";

        public string CallbackQueryPattern => @"(?i)(?<query>checkinZoukUserSubscriptionId):(?<data>.*)";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public async Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            var chatId = message.Chat.Id;
            await client.SendChatActionAsync(chatId, ChatAction.Typing);

            var isLocationMessage = message.ValidateMessageLocationData();
            if (isLocationMessage)
            {
                // TODO: Check location where classes can be executed
                await CheckInCommandHelper.ShowZoukUserSubscriptionsInformation(message, client, services);
                return;
            }

            const string replyMessage = "Please send me your location, so I can check if you are on classes now.";
            var replyMarkup = new ReplyKeyboardMarkup(KeyboardButton.WithRequestLocation("Send location"))
            {
                OneTimeKeyboard = true,
                ResizeKeyboard = true
            };

            await client.SendTextMessageAsync(chatId, replyMessage, ParseMode.Markdown, replyMarkup: replyMarkup);
        }

        public async Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            if (callbackQuery.Validate())
                throw new NotSupportedException();

            var chatId = callbackQuery.From.Id;
            await client.SendChatActionAsync(chatId, ChatAction.Typing);

            var zoukUserSubscriptionId = CheckInCommandHelper.GetZoukUserSubscriptionIdFromCallbackQuery(callbackQuery.Data, CallbackQueryPattern);
            var zoukUserSubscriptions = (await services.ZoukUsersSubscriptions.GetAllAsync()).ToList();
            var zoukUserSubscription = await services.ZoukUsersSubscriptions.FindOneAsync(x => x.Id.Equals(zoukUserSubscriptionId));

            if (zoukUserSubscription == null)
            {
                await client.SendTextMessageAsync(chatId, "Can't get user subscription from db. Please contact @nazikBro for details.", ParseMode.Markdown);
                return;
            }

            zoukUserSubscription.RemainingClassesCount--;
            await services.ZoukUsersSubscriptions.ReplaceAsync(zoukUserSubscription);

            await client.SendTextMessageAsync(chatId, "*💚*", ParseMode.Markdown);
        }
    }
}
