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

            XMLRpcClient caller = new XMLRpcClient("http://webservices.enerca.com.co:8069", "admin", "admin", "enerca");
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
            var returned = Task.Run(async () => await caller.Send<List<Factura>>("search_read", "siec.factura", parameters)).Result;
            stopWatch.Stop();

            Console.WriteLine($"Cuenta, Año, Mes, Valor");
            foreach (var factura in returned)
            {
                Console.WriteLine($"{factura.Cuenta}, {factura.Ano}, {factura.Mes}, {factura.Valor}");
            }

            Console.WriteLine($"Tiempo transcurrido {stopWatch.ElapsedMilliseconds}");
            Console.ReadLine();
        }
    }
}
