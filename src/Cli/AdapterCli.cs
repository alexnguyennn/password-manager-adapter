using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap.Exceptions;
using CommandDotNet.Attributes;
using PasswordManager.Model.Enums;
using PasswordManager.Service;
using PasswordManager.Service.Contract;

namespace PasswordManager.Cli
{
    public class AdapterCli
    {
        private readonly IPasswordAdapterService _service;
        private readonly bool _debug;
        public AdapterCli()
        {
            _service = new PasswordAdapterService(new PasswordAdapterFactory(), 
                new Dictionary<AdapterType, string>
                {
                    {AdapterType.Bitwarden, "BW"},
                    {AdapterType.LastPass, "LP"},
                });

            _debug = true;
        }
        
        [ApplicationMetadata(Description = "Initiates Login to each provider with specified user credential")]
        public int Login(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                Console.WriteLine("User is empty - run cli.dll Login -h");
                return 1;
            }
            
            var (lpLoggedIn, account) = _service.Status();
            if (!lpLoggedIn || account != user)
            {
                var loginResult = _service.Login(user);
                return loginResult ? 0 : 1;
            }

            return 0;
        }

        
        [ApplicationMetadata(Description = "Lists available records if logged in. Otherwise return exit code 1")]
        public int List()
        {
            if (!(_service.Status()).status)
            {
                Console.WriteLine("Not Logged in.");
                return 1;
            }
            _service.Show();
            return 0;
        }

        [ApplicationMetadata(Description = "Gets record by id")]
        public int Retrieve(string id, string field, bool c)
        {
            
            if (!(_service.Status()).status)
            {
                Console.WriteLine("Not Logged in.");
                return 1;
            } 
            
            _service.Lookup(id, field, c);
            return 0;
        }

        [ApplicationMetadata(Description = "Gets record by id")]
        public async Task<int> DmenuLookupPassword(bool c)
        {
            string records = null;
            try
            {
                
                records = _service.GetShowString();
                var result = await CliWrap.Cli.Wrap("dmenu")
                    .SetStandardInput(records)
                    .ExecuteAsync();
                var choice = result.StandardOutput.Trim();

                return Retrieve(choice, "password", true);

            }
            catch (ExitCodeValidationException e)
            {
                if (_debug) Console.WriteLine($"Class: {nameof(AdapterCli)} / Method: {nameof(DmenuLookupPassword)}\n" +
                                  $"raw cli: {records} | dmenu \n" +
                                  $"{e.ExecutionResult.StandardError}");
                return 1;
            }
        }
    }
}