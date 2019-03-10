using PasswordManager.Model.Enums;
using PasswordManager.Service.Contract;

namespace Service.Contract
{
    public interface IPasswordAdapterFactory
    {
        IPasswordAdapter GetPasswordAdapter(AdapterType type);

    }
}