namespace PasswordManager.Service.Contract
{
    public interface IPasswordAdapterService
    {
        // Keeps all cached entries 
        // Copies to clipboard on lookup
        void Login();
    }
}