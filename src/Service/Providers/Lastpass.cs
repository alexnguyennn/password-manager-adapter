using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Models;
using Newtonsoft.Json;
using PasswordManager.Model;
using PasswordManager.Model.Enums;
using PasswordManager.Service.Contract;

namespace PasswordManager.Service.Providers {
    public class Lastpass : IPasswordAdapter
    {
        private string executable { get; set; } = "lpass";
        private readonly ICli _cli;
        
        public Lastpass(ICli cli)
        {
            _cli = cli;
        }
        
        public async Task<bool> Login(string user, string pathToAskPassFile = null)
        {
            var args = $"login {user} --trust";
            string callback = null;
            var result = _cli.SetArguments(args);
            if (!string.IsNullOrEmpty(pathToAskPassFile)) result.SetEnvironmentVariable("LPASS_ASKPASS", pathToAskPassFile);
            
            await result.ExecuteAsync();
            return (await GetStatus()).status;
        }

        public async Task<(bool status, string account)> GetStatus()
        {
            var result = await _cli.SetArguments("status")
                .ExecuteAsync();
            var output = result.StandardOutput;

            if (output.Contains("Not logged in")) return (false, null);
            var fragments = output.Split(' ');
            return (true, fragments[fragments.Length - 1].TrimEnd('.'));
        }

        public async Task<IList<Record>> GetRecords()
        {
            var status = (await GetStatus()).status;
            if (!status) return null;
            var groups = new []
            {
                "(none)", "Banking", "Business", "Education",
                "Email", "Entertainment", "Finance", "Games",
                "General", "Home", "Mobile", "News/Reference",
                "Productivity Tools", "Secure Notes"
            };
            
            var result = await _cli.SetArguments($"ls --format=\"%ai\"")
                .ExecuteAsync();
            if (string.IsNullOrEmpty(result.StandardOutput)) return null;
            var ids = result.StandardOutput
                .Split(new [] {Environment.NewLine}, StringSplitOptions.None)
                .Where(id => !(groups.Contains(id)));
            var jsonRecords = await _cli.SetArguments($"show --json {String.Join(" ", ids)}")
                .ExecuteAsync();
            if (string.IsNullOrEmpty(jsonRecords.StandardOutput)) return null;
            
            return JsonConvert.DeserializeObject<IList<Record>>(jsonRecords.StandardOutput)
                .Select(
                record =>
                {
                    record.source = AdapterType.LastPass;
                    return record;
                }).ToList(); 
            
            // TODO lpass also has special direct search - implement one just for it
            // lpass show -site- => gets both if term gives up duplicates though; handle this
            // use -F
        }

        public Task<ExecutionResult> GetField(string id, string fieldName, bool copyToClipboard = false)
        {
            string copyArgument = copyToClipboard ? "-c" : "";
            return _cli.SetArguments($"show {copyArgument} --{fieldName} {id}")
                .ExecuteAsync();
        }
    }
}