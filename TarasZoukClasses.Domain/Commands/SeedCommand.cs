namespace TarasZoukClasses.Domain.Commands
{
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Contract;
    using Data.Models;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class SeedCommand : ICommand
    {
        public string Name => "/seed";

        public string CallbackQueryPattern => "throw new System.NotImplementedException()";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) => new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public async Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            await services.Subscriptions.DeleteManyAsync(x => x.IsActive);

            var subscriptions = new List<Subscription>
            {
                new Subscription
                {
                    Name = "One Day Class",
                    Description = "One day class",
                    Price = 200,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.Day,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Week Classes",
                    Description = "Two day classes",
                    Price = 400,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Two Week Classes",
                    Description = "Four day classes",
                    Price = 800,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "One Month Classes",
                    Description = "Eight day classes",
                    Price = 1600,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Three Months Classes",
                    Description = "Sixteen day classes",
                    Price = 3200,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                }
            };

            await services.Subscriptions.InsertManyAsync(subscriptions);
        }

        public Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            return Task.CompletedTask;
        }
    }
}
