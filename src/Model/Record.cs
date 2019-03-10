using System;
using PasswordManager.Model.Enums;

namespace PasswordManager.Model
{
    public class Record
    {
        public AdapterType source { get; set; } 
        public string id { get; set; } 
        public string name { get; set; } 
        public string group { get; set; } 
        public string username { get; set; } 
        public string url { get; set; } 
        public string note { get; set; } 
    }
}
