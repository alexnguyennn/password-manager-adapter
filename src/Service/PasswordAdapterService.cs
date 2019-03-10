using PasswordManager.Model.Enums;
using PasswordManager.Service.Contract;
using Service.Contract;

namespace PasswordManager.Service
{
    public class PasswordAdapterService : IPasswordAdapterService
    {
        private readonly IPasswordAdapterFactory _passwordAdapterFactory;
        
        public PasswordAdapterService(IPasswordAdapterFactory passwordAdapterFactory)
        {
            _passwordAdapterFactory = passwordAdapterFactory;

        }
        
        public void Login()
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
        }
        
    }
}
