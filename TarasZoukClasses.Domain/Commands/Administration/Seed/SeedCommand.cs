namespace TarasZoukClasses.Domain.Commands.Administration.Seed
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Admin;
    using Contract;
    using Data.Models;
    using Data.Models.Subscription;
    using Service.BaseService;
    using Telegram.Bot;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class SeedCommand : ICommand
    {
        public string Name => "/seed";

        public string CallbackQueryPattern => "Not Implemented";

        public bool Contains(Message message) => message.Type == MessageType.Text && message.Text.Contains(Name);

        public bool Contains(string callbackQueryData) =>
            new Regex(CallbackQueryPattern).Match(callbackQueryData).Success;

        public async Task Execute(Message message, TelegramBotClient client, IUnitOfWork services)
        {
            var chatId = message.From.Id;

            if (!AdministrationHelper.CanExecuteCommand(message.From.Username))
            {
                await client.SendTextMessageAsync(chatId, "Access denied. You can't execute this command.");
                return;
            }

            await ProcessSubscriptions(services);
            await ProcessZoukUserSubscriptions(services);

            await client.SendTextMessageAsync(chatId, "*Successfully seeded*", ParseMode.Markdown);
        }

        public Task Execute(CallbackQuery callbackQuery, TelegramBotClient client, IUnitOfWork services)
        {
            return Task.CompletedTask;
        }

        private static async Task ProcessSubscriptions(IUnitOfWork services)
        {
            await services.Subscriptions.DeleteManyAsync(x => x.IsActive);

            var subscriptions = new List<Subscription>
            {
                #region SubscriptionType.None

                new Subscription
                {
                    Name = "Whoops",
                    Description = "Nothing to do here",
                    Price = 0,
                    DiscountPercent = 0,
                    ClassesCount = 0,
                    Type = SubscriptionType.None,
                    Period = SubscriptionPeriod.None,
                    IsActive = true
                },

                #endregion

                #region SubscriptionType.Novice

                new Subscription
                {
                    Name = "One class",
                    Description = "One class",
                    Price = 200,
                    DiscountPercent = 0,
                    ClassesCount = 1,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.Day,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Week Classes",
                    Description = "Two classes",
                    Price = 400,
                    DiscountPercent = 0,
                    ClassesCount = 2,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Two Week Classes",
                    Description = "Four classes",
                    Price = 800,
                    DiscountPercent = 0,
                    ClassesCount = 4,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "One Month Classes",
                    Description = "Eight classes",
                    Price = 1600,
                    DiscountPercent = 0,
                    ClassesCount = 8,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Three Months Classes",
                    Description = "Sixteen classes",
                    Price = 3200,
                    DiscountPercent = 15,
                    ClassesCount = 16,
                    Type = SubscriptionType.Novice,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                },

                #endregion

                #region SubscriptionType.Medium

                new Subscription
                {
                    Name = "One class",
                    Description = "One class",
                    Price = 200,
                    DiscountPercent = 0,
                    ClassesCount = 1,
                    Type = SubscriptionType.Medium,
                    Period = SubscriptionPeriod.Day,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Week Classes",
                    Description = "Two classes",
                    Price = 400,
                    DiscountPercent = 0,
                    ClassesCount = 2,
                    Type = SubscriptionType.Medium,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Two Week Classes",
                    Description = "Four classes",
                    Price = 800,
                    DiscountPercent = 0,
                    ClassesCount = 4,
                    Type = SubscriptionType.Medium,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "One Month Classes",
                    Description = "Eight classes",
                    Price = 1600,
                    DiscountPercent = 0,
                    ClassesCount = 8,
                    Type = SubscriptionType.Medium,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Three Months Classes",
                    Description = "Sixteen classes",
                    Price = 3200,
                    DiscountPercent = 15,
                    ClassesCount = 16,
                    Type = SubscriptionType.Medium,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                },

                #endregion

                #region SubscriptionType.Lady

                new Subscription
                {
                    Name = "One class",
                    Description = "One class",
                    Price = 200,
                    DiscountPercent = 0,
                    ClassesCount = 1,
                    Type = SubscriptionType.Lady,
                    Period = SubscriptionPeriod.Day,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Week Classes",
                    Description = "Two classes",
                    Price = 400,
                    DiscountPercent = 0,
                    ClassesCount = 2,
                    Type = SubscriptionType.Lady,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Two Week Classes",
                    Description = "Four classes",
                    Price = 800,
                    DiscountPercent = 0,
                    ClassesCount = 4,
                    Type = SubscriptionType.Lady,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "One Month Classes",
                    Description = "Eight classes",
                    Price = 1600,
                    DiscountPercent = 0,
                    ClassesCount = 8,
                    Type = SubscriptionType.Lady,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Three Months Classes",
                    Description = "Sixteen classes",
                    Price = 3200,
                    DiscountPercent = 15,
                    ClassesCount = 16,
                    Type = SubscriptionType.Lady,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                },

                #endregion

                #region SubscriptionType.NoviceMedium

                new Subscription
                {
                    Name = "One day classes",
                    Description = "Two classes",
                    Price = 400,
                    DiscountPercent = 0,
                    ClassesCount = 2,
                    Type = SubscriptionType.NoviceMedium,
                    Period = SubscriptionPeriod.Day,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Week Classes",
                    Description = "Four classes",
                    Price = 800,
                    DiscountPercent = 0,
                    ClassesCount = 4,
                    Type = SubscriptionType.NoviceMedium,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Two Week Classes",
                    Description = "Eight classes",
                    Price = 1600,
                    DiscountPercent = 0,
                    ClassesCount = 8,
                    Type = SubscriptionType.NoviceMedium,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "One Month Classes",
                    Description = "Sixteen classes",
                    Price = 3200,
                    DiscountPercent = 10,
                    ClassesCount = 16,
                    Type = SubscriptionType.NoviceMedium,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Three Months Classes",
                    Description = "Thirty two classes",
                    Price = 6400,
                    DiscountPercent = 15,
                    ClassesCount = 32,
                    Type = SubscriptionType.NoviceMedium,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                },

                #endregion

                #region SubscriptionType.NoviceLady

                new Subscription
                {
                    Name = "One day classes",
                    Description = "Two classes",
                    Price = 400,
                    DiscountPercent = 0,
                    ClassesCount = 2,
                    Type = SubscriptionType.NoviceLady,
                    Period = SubscriptionPeriod.Day,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Week Classes",
                    Description = "Four classes",
                    Price = 800,
                    DiscountPercent = 0,
                    ClassesCount = 4,
                    Type = SubscriptionType.NoviceLady,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Two Week Classes",
                    Description = "Eight classes",
                    Price = 1600,
                    DiscountPercent = 0,
                    ClassesCount = 8,
                    Type = SubscriptionType.NoviceLady,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "One Month Classes",
                    Description = "Sixteen classes",
                    Price = 3200,
                    DiscountPercent = 10,
                    ClassesCount = 16,
                    Type = SubscriptionType.NoviceLady,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Three Months Classes",
                    Description = "Thirty two classes",
                    Price = 6400,
                    DiscountPercent = 15,
                    ClassesCount = 32,
                    Type = SubscriptionType.NoviceLady,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                },

                #endregion

                #region SubscriptionType.MediumLady

                new Subscription
                {
                    Name = "One day classes",
                    Description = "Two classes",
                    Price = 400,
                    DiscountPercent = 0,
                    ClassesCount = 2,
                    Type = SubscriptionType.MediumLady,
                    Period = SubscriptionPeriod.Day,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Week Classes",
                    Description = "Four classes",
                    Price = 800,
                    DiscountPercent = 0,
                    ClassesCount = 4,
                    Type = SubscriptionType.MediumLady,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Two Week Classes",
                    Description = "Eight classes",
                    Price = 1600,
                    DiscountPercent = 0,
                    ClassesCount = 8,
                    Type = SubscriptionType.MediumLady,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "One Month Classes",
                    Description = "Sixteen classes",
                    Price = 3200,
                    DiscountPercent = 10,
                    ClassesCount = 16,
                    Type = SubscriptionType.MediumLady,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "Three Months Classes",
                    Description = "Thirty two classes",
                    Price = 6400,
                    DiscountPercent = 15,
                    ClassesCount = 32,
                    Type = SubscriptionType.MediumLady,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                },

                #endregion

                #region SubscriptionType.Premium

                new Subscription
                {
                    Name = "All classes",
                    Description = "Novice, medium, lady style classes for week",
                    Price = 1200,
                    DiscountPercent = 5,
                    ClassesCount = 12,
                    Type = SubscriptionType.Premium,
                    Period = SubscriptionPeriod.Week,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "All classes",
                    Description = "Novice, medium, lady style classes for two weeks",
                    Price = 2400,
                    DiscountPercent = 5,
                    ClassesCount = 24,
                    Type = SubscriptionType.Premium,
                    Period = SubscriptionPeriod.TwoWeeks,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "All classes",
                    Description = "Novice, medium, lady style classes for month",
                    Price = 4800,
                    DiscountPercent = 10,
                    ClassesCount = 48,
                    Type = SubscriptionType.Premium,
                    Period = SubscriptionPeriod.Month,
                    IsActive = true
                },
                new Subscription
                {
                    Name = "All classes",
                    Description = "Novice, medium, lady style classes for three months",
                    Price = 9600,
                    DiscountPercent = 15,
                    ClassesCount = 144,
                    Type = SubscriptionType.Premium,
                    Period = SubscriptionPeriod.ThreeMonths,
                    IsActive = true
                }

                #endregion
            };

            await services.Subscriptions.InsertManyAsync(subscriptions);
        }

        private static async Task ProcessZoukUserSubscriptions(IUnitOfWork services)
        {
            var zoukUserNazar = await services.ZoukUsers.FindOneAsync(x => x.NickName.Equals("nazikBro"));

            var subscriptionPremiumMonth = await services.Subscriptions.FindOneAsync(x =>
                x.Type == SubscriptionType.Premium && x.Period == SubscriptionPeriod.Month);
            var userSubscriptionPremiumMonth = new ZoukUserSubscription
            {
                ZoukUser = zoukUserNazar,
                Subscription = subscriptionPremiumMonth,
                RemainingClassesCount = subscriptionPremiumMonth.ClassesCount
            };

            var premiumSubscriptionInDb = await services.ZoukUsersSubscriptions.FilterBy(x =>
                x.ZoukUser.NickName == userSubscriptionPremiumMonth.ZoukUser.NickName
                && x.Subscription.Type == userSubscriptionPremiumMonth.Subscription.Type);

            if (!premiumSubscriptionInDb.Any())
                await services.ZoukUsersSubscriptions.InsertAsync(userSubscriptionPremiumMonth);
        }
    }
}
