﻿namespace TarasZoukClasses.Domain.Commands
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Data.Models;
    using MongoDB.Driver;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;
    using Utils;

    public class StartCommand : ICommand
    {
        public string Name => @"/start";

        public string CallbackQueryPattern => @"(?i)(?<query>language):(?<data>\w{2}-\w{2})";

        private const string ResponseMessage = "Привіт! 😊\n\n" +
                                               "Якою мовою ти бажаєш спілкуватися?\n" +
                                               "What language do you want to communicate in?\n" +
                                               "На каком языке ты хочешь общаться?";

        private const string ResponseCallbackQueryMessage = "What do you want to do next?";

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

            if (callbackQuery.From.IsBot
                || callbackQuery.Message.Chat.Type != ChatType.Private
                || callbackQuery.Message.ForwardFromChat != null)
            {
                throw new NotSupportedException();
            }

            await botClient.SendChatActionAsync(chatId, ChatAction.Typing);
            await SaveUser(callbackQuery, services);

            var replyKeyboardMarkup = InlineKeyboardBuilder.Create(chatId)
                .SetText(ResponseCallbackQueryMessage)
                .AddButton("Class subscription", "createSubscription")
                .AddButton("Class check in", "checkIn")
                .Build();

            await botClient.SendTextMessageAsync(chatId,
                ResponseCallbackQueryMessage,
                parseMode: ParseMode.Markdown,
                replyMarkup: replyKeyboardMarkup);
        }

        private async Task SaveUser(CallbackQuery callbackQuery, IUnitOfWork services)
        {
            var cultureName = GetCultureNameFromCallbackQuery(callbackQuery);
            var culture = await services.Cultures.GetCultureByCodeAsync(cultureName);
            var userAdditionalInfo = new UserAdditionalInformation
            {
                Culture = culture,
                FirstName = callbackQuery.Message.Chat.FirstName,
                SecondName = callbackQuery.Message.Chat.LastName
            };

            var dbUser = await services.Users.FindOneAsync(x =>
                x.NickName == callbackQuery.Message.Chat.Username);

            if (dbUser == null)
            {
                await services.Users.InsertAsync(new ZoukUser
                {
                    NickName = callbackQuery.Message.Chat.Username,
                    UserAdditionalInformation = userAdditionalInfo
                });
            }
            else
            {
                dbUser.UserAdditionalInformation = userAdditionalInfo;
                await services.Users.ReplaceAsync(dbUser);
            }
        }

        private string GetCultureNameFromCallbackQuery(CallbackQuery callbackQuery)
        {
            var cultureName = string.Empty;
            var cultureMatch = Regex.Match(callbackQuery.Data, CallbackQueryPattern);

            if (cultureMatch.Success && cultureMatch.Groups["query"].Value == "language")
                cultureName = cultureMatch.Groups["data"].Value;

            if (string.IsNullOrEmpty(cultureName))
                throw new ArgumentException("Culture name can't be parsed.");

            return cultureName;
        }
    }
}
