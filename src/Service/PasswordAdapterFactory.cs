using System;
using CliWrap;
using PasswordManager.Model.Enums;
using PasswordManager.Service.Contract;
using PasswordManager.Service.Providers;
using Service.Contract;

namespace PasswordManager.Service
{
    public class PasswordAdapterFactory : IPasswordAdapterFactory
    {
        // make a passwordmanager implementing the adapter interface
        public PasswordAdapterFactory()
        {
        }

        public IPasswordAdapter GetPasswordAdapter(AdapterType type)
        {
            switch (type)
            {
                    case AdapterType.Bitwarden:
                        throw new NotImplementedException();
                    case AdapterType.LastPass:
                        return new Lastpass(new Cli("lpass"));
                    default:
                        return null;
            }

        }
    }
}
