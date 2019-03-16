using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap.Models;
using PasswordManager.Model;

namespace PasswordManager.Service.Contract
{
    public interface IPasswordAdapter
    {
        // Login method
        Task<bool> Login(string user, string pathToAskPassFile = null);
        Task<(bool status, string account)> GetStatus();
        Task<IList<Record>> GetRecords();
        Task<IDictionary<string, Record>> GetRecordsMap();
        void GetFieldById(string id, string fieldName, bool copyToClipboard = false);
    }
}