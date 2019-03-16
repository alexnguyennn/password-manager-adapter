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
        private readonly bool _debug;
        
        public PasswordAdapterService(IPasswordAdapterFactory passwordAdapterFactory,
            IDictionary<AdapterType, string> adapterTypeMap)
        {
            _passwordAdapterFactory = passwordAdapterFactory;
            _records = null;
            _adapterTypeMap = adapterTypeMap;
            _debug = false;
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

            return String.Join(Environment.NewLine, _records.Keys);
            
//            //TODO limit url length
//            // TODO Get map of Display format => id only

        }

        public async Task Show(bool sync = false)
        {
            
            var output = await GetShowString();
            Console.WriteLine(output);
        }

        public void Lookup(string formattedOutput, string field, bool copyToClipboard = true)
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
            if (_records is null || !_records.ContainsKey(formattedOutput))
            {
                if (_debug)
                _records.Keys.Select(record =>
                {
                    Console.WriteLine(record);
                    return record;
                }).ToList();
                
                Console.WriteLine($"Requested record not found: {formattedOutput}");
            }
            
            var id = _records[formattedOutput].id;
            adapter.GetFieldById(id, field, copyToClipboard);
        }

        public Task<(bool status, string account)> Status()
        {
            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
            return adapter.GetStatus();
        }

    }
}
