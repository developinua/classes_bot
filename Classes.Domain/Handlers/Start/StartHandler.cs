using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Services;
using Classes.Domain.Utils;
using MediatR;
using ResultNet;

namespace Classes.Domain.Handlers.Start;

public class StartHandler : IRequestHandler<StartRequest, Result>
{
    private readonly IBotService _botService;
    
    public StartHandler(IBotService botService) => _botService = botService;

    public async Task<Result> Handle(StartRequest request, CancellationToken cancellationToken)
    {
        await _botService.SendChatActionAsync(request.ChatId, cancellationToken);

        if (string.IsNullOrEmpty(request.Username))
        {
            await _botService.SendTextMessageAsync(
                request.ChatId,
                "Fill in username in your account settings and press /start again",
                cancellationToken);
            return Result.Failure()
                .WithMessage("Username is null or empty.");
        }

        var replyKeyboardMarkup = InlineKeyboardBuilder.Create()
            .AddButton("English", "language:en-US")
            .AddButton("Ukrainian", "language:uk-UA")
            .Build();

        await _botService.SendTextMessageWithReplyAsync(
            request.ChatId,
            "*😊 Hi!\n\n*What language do you want to communicate in?",
            replyKeyboardMarkup,
            cancellationToken);
        
        return Result.Success();
    }
}