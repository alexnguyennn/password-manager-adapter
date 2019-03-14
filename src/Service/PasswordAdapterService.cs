using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordManager.Model;
using PasswordManager.Model.Enums;
using PasswordManager.Service.Contract;
using Service.Contract;

namespace PasswordManager.Service
{
    public class PasswordAdapterService : IPasswordAdapterService
    {
        private readonly IPasswordAdapterFactory _passwordAdapterFactory;
        private Dictionary<string, Record> _records;
        
        public PasswordAdapterService(IPasswordAdapterFactory passwordAdapterFactory)
        {
            _passwordAdapterFactory = passwordAdapterFactory;
            _records = null;
        }
        
        public void Login(string user)
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
            adapter.Login(user);
        }

        public void Show(bool sync = false)
        {
            if (_records is null || sync)
            {
                var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
                _records = adapter.GetRecords();
            }
        }

        public async Task<string> Lookup(string id, string field, bool copyToClipboard = true)
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
            var result = await adapter.GetField(id, field, copyToClipboard);
            return result.StandardOutput;
        }
    }
}
