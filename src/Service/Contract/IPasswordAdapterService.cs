using System.Threading.Tasks;

namespace PasswordManager.Service.Contract
{
    public interface IPasswordAdapterService
    {
        // Keeps all cached entries 
        // Copies to clipboard on lookup
        bool Login(string user);
        string GetShowString(bool sync = false);
        void Show(bool sync = false);
        void Lookup(string id, string field, bool copyToClipboard = true);

        (bool status, string account) Status();
        //TODO lookup on id
        // Display 
    }
}