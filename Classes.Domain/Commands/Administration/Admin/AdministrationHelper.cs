using System.Linq;

namespace Classes.Domain.Commands.Administration.Admin;

public static class AdministrationHelper
{
    public static bool CanExecuteCommand(string username)
    {
        var allowedUsers = new[] { "nazikBro", "taras_zouk", "kovalinas" };
        return allowedUsers.Any(x => x.Equals(username));
    }
}