using System.Security.Claims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Exceptions;
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
        private readonly bool _debug = false;
        private const string abbrev = "LP";
        private const int batchSize = 30;

        private bool _isWin;

        
        public Lastpass(ICli cli, bool isWin)
        {
            _cli = cli;
            _isWin = isWin;
        }
        
        public async Task<bool> Login(string user, string pathToAskPassFile = null)
        {
            string args = null;
            try
            {
                args = $"login {user} --trust";
                var result = _cli.SetArguments(WrapArgs(_isWin, args));
                if (!string.IsNullOrEmpty(pathToAskPassFile)) result.SetEnvironmentVariable("LPASS_ASKPASS", 
                    pathToAskPassFile);
                
                await result.ExecuteAsync();
                return (await GetStatus()).status;
            }
            catch (ExitCodeValidationException e)
            {
                if (_debug) Console.WriteLine($"Adapter: {nameof(Lastpass)} / Method: {nameof(Login)} \n" +
                                  $"raw cli: {executable} {args} \n" +
                                  $"{e.ExecutionResult.StandardError}");
                return false;
            }
        }

        public async Task<(bool status, string account)> GetStatus()
        {
            try
            {
                var result = await _cli.SetArguments(WrapArgs(_isWin, "status"))
                    .ExecuteAsync();
                var output = result.StandardOutput;

                if (output.Contains("Not logged in")) return (false, null);
                var fragments = output.Split(' ');
                return (true, fragments[fragments.Length - 1].TrimEnd('.'));
            }
            catch (ExitCodeValidationException e)
            {
                // TODO convert this into logging output instead
                if (_debug) Console.WriteLine($"Adapter: {nameof(Lastpass)} / Method: {nameof(GetStatus)}\n" +
                                  $"raw cli: {executable} status\n" +
                                  $"{e.ExecutionResult.StandardError}");
                return (false, null);
            }

        }

        public async Task<IList<Record>> GetRecords()
        {
            var status = (await GetStatus()).status;
            if (!status) return null;
            var groups = new[]
            {
                "(none)", "Banking", "Business", "Education",
                "Email", "Entertainment", "Finance", "Games",
                "General", "Home", "Mobile", "News/Reference",
                "Productivity Tools", "Secure Notes"
            };

            var result = await _cli.SetArguments(
                WrapArgs(_isWin, $"ls --format=\"%ai\"")
                ).ExecuteAsync();

            if (string.IsNullOrEmpty(result.StandardOutput)) return null;

            var ids = result.StandardOutput
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Where(id => !(groups.Contains(id)))
                .ToList();

            var batchNum = ids.Count() / batchSize;

            return Enumerable.Range(0, batchNum)
                .Select(b => ParseRecordsFromIds(ids
                    .Skip(b * batchSize).Take(batchSize)))
                .Select(t => t.Result)
                .SelectMany(rb => rb, (recordBatch, record) => record)
                .ToList();

            // TODO lpass also has special direct search - implement one just for it
            // lpass show -site- => gets both if term gives up duplicates though; handle this
            // use -F
        }

        private async Task<IList<Record>> ParseRecordsFromIds(IEnumerable<string> ids)
        {
            var jsonRecords = await _cli.SetArguments(
                    WrapArgs(_isWin, $"show --json {String.Join(" ", ids)}")
                ).ExecuteAsync();

            if (string.IsNullOrEmpty(jsonRecords.StandardOutput)) return null;

            Console.WriteLine(jsonRecords.StandardOutput);

            return JsonConvert.DeserializeObject<IList<Record>>(jsonRecords.StandardOutput)
                .Select(
                    record =>
                    {
                        record.source = AdapterType.LastPass;
                        return record;
                    }).ToList();
        }

        public async Task<IDictionary<string, Record>> GetRecordsMap()
        {
            var recordsList = await GetRecords();
            return recordsList.ToDictionary(record =>
            {
                // TODO do something with url + formatting option
//                string formattedUrl;
//                try
//                {
//                    formattedUrl = new Uri(record.url).Host;
//                }
//                catch (UriFormatException e)
//                {
//                    if (_debug) Console.WriteLine($"Bad uri format exception: {e} \nrecord: {record.url}");
//                    formattedUrl = record.url;
//                }
                
//                return $"{record.name}|| {record.username} | {formattedUrl} | {record.id} || {abbrev}";
                return $"{record.name}|| {record.username} {record.id} || {abbrev}";
            });
        }

        public void GetFieldById(string id, string fieldName, bool copyToClipboard = false)
        {
            string copyArgument = copyToClipboard && !_isWin ? "-c" : "";
            var pipeToClip = copyToClipboard && _isWin ? $"| clip.exe" : "";
            var args = $"show {copyArgument} --{fieldName} {id} | tr -d '\n' {pipeToClip}";

            if (_debug) Console.WriteLine($"About to call lpass with args: {args}");
            var result = _cli.SetArguments(WrapArgs(_isWin, args))
                .EnableExitCodeValidation()
                .EnableStandardErrorValidation()
                .Execute();

            if (copyToClipboard) return;

            Console.WriteLine(result.StandardOutput);
        }

        private string WrapArgs(bool isWin, string args) {
            return isWin ? $"{executable} {args}" : args;
        }
        

    }
}