using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Models;
using PasswordManager.Model;
using PasswordManager.Service.Contract;

namespace Service.Providers
{
    public class Bitwarden : IPasswordAdapter
    {
        private string executable { get; set; } = "bw";
        private readonly ICli _cli;
        private readonly bool _debug = true;
        private const string abbrev = "BW";
         
        public Task<bool> Login(string user, string pathToAskPassFile = null)
        {
            // bw unlock, set env variable
            // bw unlock --raw
            throw new System.NotImplementedException();
        }

        public Task<(bool status, string account)> GetStatus()
        {
            throw new System.NotImplementedException();
            // bw sync
        }

        public Task<IList<Record>> GetRecords()
        {
            // bw get item <id>
            // or bw list items --search (term)
            throw new System.NotImplementedException();
            // TODO bw has special direct search - implement one just for it
        }

        public Task<IDictionary<string, Record>> GetRecordsMap()
        {
            throw new System.NotImplementedException();
        }

        public void GetFieldById(string id, string fieldName, bool copyToClipboard = false)
        {
            //bw get item <id>
            throw new System.NotImplementedException();
        }
        // TODO tap into password generator
    }
}