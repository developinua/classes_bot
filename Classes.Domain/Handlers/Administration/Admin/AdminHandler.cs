using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Domain.Services;
using MediatR;
using ResultNet;

namespace Classes.Domain.Handlers.Administration.Admin;

public class AdminHandler : IRequestHandler<AdminRequest, Result>
{
    private readonly IBotService _botService;

    public AdminHandler(IBotService botService) => _botService = botService;

    public async Task<Result> Handle(AdminRequest request, CancellationToken cancellationToken)
    {
        if (!CanExecuteCommand(request.Username))
            await _botService.SendTextMessageAsync(
                request.ChatId,
                "Access denied. You can't execute this command.",
                cancellationToken);

        var responseMessage = $"/seed /paymentlink /manage-subscriptions";
        
        return Result.Success();
    }

    // todo: extract to separate class
    private static bool CanExecuteCommand(string username)
    {
        var allowedUsers = new[] { "nazikBro", "taras_zouk", "kovalinas" };
        return allowedUsers.Any(x => x.Equals(username));
    }
}