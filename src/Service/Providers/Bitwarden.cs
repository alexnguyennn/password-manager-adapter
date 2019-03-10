using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap.Models;
using PasswordManager.Model;
using PasswordManager.Service.Contract;

namespace Service.Providers
{
    public class Bitwarden : IPasswordAdapter
    {
        public Task<ExecutionResult> Login(string user)
        {
            // TODO figure out how to script this
            throw new System.NotImplementedException();
        }

        public Task<(bool status, string account)> GetStatus()
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<Record>> GetRecords()
        {
            // bw get item <id>
            // or bw list items --search (term)
            throw new System.NotImplementedException();
            // TODO bw has special direct search - implement one just for it
        }

        public Task<ExecutionResult> GetField(string id, string fieldName)
        {
            // bw get password <id>
            // bw get user <id>
            throw new System.NotImplementedException();
        }
    }
}