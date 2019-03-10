using System;
using System.Runtime.CompilerServices;
using PasswordManager.Service;

namespace PasswordManager.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            // Startup.cs => dependency injection somehow?
            var service = new PasswordAdapterService(new PasswordAdapterFactory());
            service.Login();
        }
        

    }
    

}
