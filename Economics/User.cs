using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace LARP.Science.Economics
{
    public class User
    {
        public int Id;
        public string UserName;
        public string Cash;

        public User() { }
    }
}
