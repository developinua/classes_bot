namespace TarasZoukClasses.Domain.Commands
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Data.Models;
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
            if (client == null)
                throw new ArgumentNullException(nameof(TelegramBotClient));

            if (message == null)
                throw new ArgumentNullException(nameof(Message));

            var chatId = message.Chat.Id;
            const string responseMessage = "*😊 Hi!\n\n*What language do you want to communicate in?";

            await client.SendChatActionAsync(chatId, ChatAction.Typing);

            var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
                .AddButton("English", "language:en-US")
                .Build();

            await client.SendTextMessageAsync(chatId, responseMessage, ParseMode.Markdown, replyMarkup: replyKeyboardMarkup);
        }

        public async Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            if (callbackQuery.Validate())
                throw new NotSupportedException();

            var chatId = callbackQuery.From.Id;
            const string responseCallbackQueryMessage = "*😊Successfully!😊*\nPress /subscription to manage your class subscription.";

            await client.SendChatActionAsync(chatId, ChatAction.Typing);
            await StartCommandHelper.SaveUser(services, callbackQuery, CallbackQueryPattern);
            await client.SendTextMessageAsync(chatId, responseCallbackQueryMessage, ParseMode.Markdown);
        }

        private static class StartCommandHelper
        {
            public static async Task SaveUser(IUnitOfWork services, CallbackQuery callbackQuery, string callbackQueryPattern)
            {
                var cultureName = GetCultureNameFromCallbackQuery(callbackQuery, callbackQueryPattern);
                var culture = await services.Cultures.GetCultureByCodeAsync(cultureName);
                var dbUser = await services.ZoukUsers.FindOneAsync(x =>
                    x.NickName == callbackQuery.Message.Chat.Username);
                var zoukUserAdditionalInfo = new ZoukUserAdditionalInformation
                {
                    Culture = culture,
                    ChatId = callbackQuery.From.Id,
                    FirstName = callbackQuery.Message.Chat.FirstName,
                    SecondName = callbackQuery.Message.Chat.LastName
                };

                if (dbUser != null)
                    await UpdateUser(services, dbUser, zoukUserAdditionalInfo);
                else
                    await CreateUser(services, callbackQuery.Message.Chat.Username, zoukUserAdditionalInfo);
            }

            private static async Task UpdateUser(IUnitOfWork services, ZoukUser zoukUser, ZoukUserAdditionalInformation zoukUserAdditionalInfo)
            {
                var zoukUserInfoAlreadyStoredInDb = await services.ZoukUsersAdditionalInformation.FindOneAsync(x =>
                    x.FirstName == zoukUserAdditionalInfo.FirstName
                    && x.ChatId == zoukUserAdditionalInfo.ChatId);

                if (zoukUserInfoAlreadyStoredInDb == null)
                    await services.ZoukUsersAdditionalInformation.InsertAsync(zoukUserAdditionalInfo);
                else
                    await services.ZoukUsersAdditionalInformation.ReplaceAsync(zoukUserAdditionalInfo);

                zoukUser.ZoukUserAdditionalInformation = zoukUserAdditionalInfo;
                await services.ZoukUsers.ReplaceAsync(zoukUser);
            }

            private static async Task CreateUser(IUnitOfWork services, string username, ZoukUserAdditionalInformation zoukUserAdditionalInformation)
            {
                await services.ZoukUsersAdditionalInformation.InsertAsync(zoukUserAdditionalInformation);
                await services.ZoukUsers.InsertAsync(new ZoukUser
                {
                    NickName = username,
                    ZoukUserAdditionalInformation = zoukUserAdditionalInformation
                });
            }

            private static string GetCultureNameFromCallbackQuery(CallbackQuery callbackQuery, string callbackQueryPattern)
            {
                var cultureName = string.Empty;
                var cultureMatch = Regex.Match(callbackQuery.Data, callbackQueryPattern);

                if (cultureMatch.Success && cultureMatch.Groups["query"].Value.Equals("language"))
                    cultureName = cultureMatch.Groups["data"].Value;

                if (string.IsNullOrEmpty(cultureName))
                    throw new ArgumentException("Culture name can't be parsed.");

                return cultureName;
            }
        }
    }
}
