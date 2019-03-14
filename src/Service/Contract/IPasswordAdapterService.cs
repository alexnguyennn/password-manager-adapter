using System.Threading.Tasks;

namespace PasswordManager.Service.Contract
{
    public interface IPasswordAdapterService
    {
        // Keeps all cached entries 
        // Copies to clipboard on lookup
        Task<bool> Login(string user);
        Task<string> GetShowString(bool sync = false);
        Task Show(bool sync = false);
        Task<string> Lookup(string id, string field, bool copyToClipboard = true);

        Task<(bool status, string account)> Status();
        //TODO lookup on id
        // Display 
    }
}