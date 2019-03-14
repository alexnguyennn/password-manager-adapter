using System;
using System.Collections.Generic;
using System.Linq;
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
        private IDictionary<string, Record> _records;
        private readonly IDictionary<AdapterType, string> _adapterTypeMap;
        
        public PasswordAdapterService(IPasswordAdapterFactory passwordAdapterFactory,
            IDictionary<AdapterType, string> adapterTypeMap)
        {
            _passwordAdapterFactory = passwordAdapterFactory;
            _records = null;
            _adapterTypeMap = adapterTypeMap;
        }
        
        public Task<bool> Login(string user)
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
            return adapter.Login(user);
        }

        public async Task<string> GetShowString(bool sync = false)
        {
            if (_records is null || sync)
            {
                var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
                _records = await adapter.GetRecordsMap();
            }

            return String.Join("\n", _records.Select(pair =>
                $"{_adapterTypeMap[pair.Value.source]}||" +
                $"{pair.Value.name}|{pair.Value.username}|{pair.Value.url}|{pair.Value.id}"));
            
        }

        public async Task Show(bool sync = false)
        {
            var output = await GetShowString();
            Console.WriteLine(output);
        }

        public async Task<string> Lookup(string id, string field, bool copyToClipboard = true)
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
            var result = await adapter.GetField(id, field, copyToClipboard);
            return result.StandardOutput;
        }

        public Task<(bool status, string account)> Status()
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
            return adapter.GetStatus();
        }
    }
}
