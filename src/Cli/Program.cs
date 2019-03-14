using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommandDotNet;
using PasswordManager.Model.Enums;
using PasswordManager.Service;

namespace PasswordManager.Cli
{
    class Program
    {
        static int Main(string[] args)
        {
            // TODO configuration handling
            AppRunner<AdapterCli> appRunner = new AppRunner<AdapterCli>();
            return appRunner.Run(args);
        }
        

    }
    

}
