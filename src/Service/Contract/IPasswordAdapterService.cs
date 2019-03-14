using System.Threading.Tasks;

namespace PasswordManager.Service.Contract
{
    public interface IPasswordAdapterService
    {
        // Keeps all cached entries 
        // Copies to clipboard on lookup
        void Login(string user);

        void Show(bool sync = false);

        Task<string> Lookup(string id, string field, bool copyToClipboard = true);
        //TODO lookup on id
        // Display 
    }
}