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
        Task<ExecutionResult> Login(string user);
        Task<(bool status, string account)> GetStatus();
        Task<IList<Record>> GetRecords();
        Task<ExecutionResult> GetField(string id, string fieldName);
    }
}