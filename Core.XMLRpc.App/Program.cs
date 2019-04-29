using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.XMLRpc.App
{
    class Program
    {
        static void Main(string[] args)
        {
            XMLRpcClient caller = new XMLRpcClient("http://webservices.enerca.com.co:8069", "admin", "admin", "enerca");
            var returned = Task.Run(() => caller.Login());
            Console.ReadLine();
        }
    }
}
