using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Classes.Application.Services;
using Classes.Domain.Requests;
using MediatR;
using ResultNet;

namespace Classes.Application.Handlers.Administration;

public class AdminHandler(IBotService botService) : IRequestHandler<AdminRequest, Result>
{
    public async Task<Result> Handle(AdminRequest request, CancellationToken cancellationToken)
    {
        if (!CanExecuteCommand(request.Username))
            await botService.SendTextMessageAsync(
                request.ChatId,
                "Access denied. You can't execute this command.",
                cancellationToken);

        var responseMessage = $"/seed /paymentlink /manage-subscriptions";
        
        return Result.Success();
    }

    // todo: extract to permissions
    private static bool CanExecuteCommand(string username)
    {
        var allowedUsers = new[] { "nazikBro", "taras_zouk", "kovalinas" };
        return allowedUsers.Any(x => x.Equals(username));
    }
}