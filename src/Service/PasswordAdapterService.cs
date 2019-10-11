using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
                SyncRecords();
            }

            return String.Join(Environment.NewLine, _records.Keys);

//            //TODO limit url length
//            // TODO Get map of Display format => id only
        }

        private void SyncRecords()
        {
            _records = _adapters.Select(async adapter => await adapter.GetRecordsMap())
                .SelectMany(dict => dict.Result)
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }


        public void Show(bool sync = false)
        {
            var output = GetShowString();
            Console.WriteLine(output);
        }

        public void ShowJson(bool sync = false) 
        {
            if (_records is null || sync)
            {
                SyncRecords();
            }
            Console.WriteLine(String.Join(
                Environment.NewLine, 
                JsonConvert.SerializeObject(_records.Values)
                ));
        }


        public void Lookup(string id, string field, AdapterType source, bool copyToClipboard = true)
        {
            try {
                if (_debug) Console.WriteLine($"About to get field with {nameof(id)}: {id}, {nameof(field)}: {field}, {nameof(source)}: {source}, {nameof(copyToClipboard)}: {copyToClipboard}");
                _passwordAdapterFactory.GetPasswordAdapter(source)
                    .GetFieldById(id, field, copyToClipboard);
            } catch (Exception e) {
                Console.WriteLine($"Something went wrong on Lookup: {JsonConvert.SerializeObject(e)}");
            }
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