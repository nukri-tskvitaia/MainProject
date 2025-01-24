using MvcProject.Configuration;

namespace MvcProject.Helper
{
    public static class RequestValidator
    {
        public static bool ValidateDeposit(List<ClientSettings> clients, string clientId)
        {
            bool isClientValid = clients.Any(x => x.ClientId == clientId);

            return isClientValid;
        }
    }
}
