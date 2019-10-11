using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        private readonly Dictionary<AdapterType, IPasswordAdapter> _adapters;

        public PasswordAdapterFactory()
        {
            _adapters = new Dictionary<AdapterType, IPasswordAdapter>();
        }

        public IPasswordAdapter GetPasswordAdapter(AdapterType type)
        {
            if (_adapters.ContainsKey(type)) return _adapters[type];
            IPasswordAdapter newAdapter = null;

            switch (type)
            {
                    case AdapterType.Bitwarden:
                        throw new NotImplementedException();
                    case AdapterType.LastPass:
                        // TODO select executable based on running OS
                        var command = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                        ? "wsl"
                        : "lpass";

                        newAdapter = new Lastpass(new Cli(command), true);
                        break;
                    default:
                        return null;
            }

            _adapters[type] = newAdapter;
            return newAdapter;
        }
    }
}
