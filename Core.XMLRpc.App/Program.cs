using Core.XMLRpc.App;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.XMLRpc.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();

            XMLRpcClient caller = new XMLRpcClient("SERVERURL", "USER", "PASS", "DB");
            var parameters = new XMLRpcParameter<XMLRpcParamList<IXMLRpcParameter>>
            {
                Name = "Params",
                FilterOption = FilterOption.None,
                Value = new XMLRpcParamList<IXMLRpcParameter>
                {
                    new XMLRpcParameter<string>
                    {
                        Name="cuenta",
                        FilterOption = FilterOption.Equals,
                        Value="112093"
                    },
                    new XMLRpcParameter<string>
                    {
                        Name="ano",
                        FilterOption = FilterOption.Equals,
                        Value="2010"
                    }
                }
            };
            //Act
            var returned = Task.Run(async () => await caller.Send<List<int>>("search", "modelname", parameters)).Result;
            stopWatch.Stop();

            foreach (var value in returned)
            {
                Console.WriteLine($"id, {value}");
            }

            Console.WriteLine($"Tiempo transcurrido {stopWatch.ElapsedMilliseconds}");
            Console.ReadLine();
        }
    }
}
