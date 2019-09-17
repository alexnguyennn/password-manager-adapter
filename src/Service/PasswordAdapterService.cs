using System;
using System.Collections;
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
        private readonly IList<IPasswordAdapter> _adapters;
        private readonly bool _debug;

        public PasswordAdapterService(IPasswordAdapterFactory passwordAdapterFactory,
            IDictionary<AdapterType, string> adapterTypeMap)
        {
            _passwordAdapterFactory = passwordAdapterFactory;
            _adapters = new List<IPasswordAdapter>
            {
                //TODO toggle bw when ready
                _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass),
//                _passwordAdapterFactory.GetPasswordAdapter(AdapterType.Bitwarden),
            };
            _records = null;
            _adapterTypeMap = adapterTypeMap;
            _debug = false;
        }

        public bool Login(string user)
        {
            var loginResults = _adapters.Select(async adapter => await adapter.Login(user));
            return !loginResults.Contains(Task.FromResult(false));
        }

        public string GetShowString(bool sync = false)
        {
            if (_records is null || sync)
            {
                _records = _adapters.Select(async adapter => await adapter.GetRecordsMap())
                    .SelectMany(dict => dict.Result)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);
            }

            return String.Join(Environment.NewLine, _records.Keys);

//            //TODO limit url length
//            // TODO Get map of Display format => id only
        }
        public void Show(bool sync = false)
        {
            var output = GetShowString();
            Console.WriteLine(output);
        }

        public void Lookup(string formattedOutput, string field, bool copyToClipboard = true)
        {
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
            _passwordAdapterFactory.GetPasswordAdapter(_records[formattedOutput].source)
                .GetFieldById(id, field, copyToClipboard);
        }

        public (bool status, string account) Status()
        {
            var statusResults = _adapters.Select(async adapter => await adapter.GetStatus()).ToList();
            if (statusResults.Where(result => !result.Result.status).ToList().Count != 0) return (false, null);

            return (true, String.Join(Environment.NewLine, statusResults.Select(result => result.Result.account)));

//            var adapter = _passwordAdapterFactory.GetPasswordAdapter(AdapterType.LastPass);
//            return adapter.GetStatus();
        }
    }
}