using System;
using System.Collections.Generic;
using System.Text;

namespace Core.XMLRpc.Test.Models
{
    public class Factura
    {
        public bool Active { get; set; }
        public int Ano { get; set; }
        public int Ciclo { get; set; }
        public string ClaseServicio { get; set; }
        public string CobroRecargos { get; set; }
        public int Cuenta { get; set; }
        public int Departamento { get; set; }
        public int Deuda { get; set; }
        public int DeudaOriginal { get; set; }
        public int DigitosChequeo { get; set; }
        public string DisplayName { get; set; }
        public int Empleado { get; set; }
        public int Estrato { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaMaximaPago { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public int Id { get; set; }
        public DateTime LastUpdate { get; set; }
        public string MarcaHistorico { get; set; }
        public int Mes { get; set; }
        public string ModalidadCobro { get; set; }
        public int Municipio { get; set; }
        public int Nit { get; set; }
        public int NivelTension { get; set; }
        public string NombrePlanta { get; set; }
        public int NumeroFactura { get; set; }
        public int PagoParcial { get; set; }
        public int PeriodosVencidos { get; set; }
        public int PlanTarifario { get; set; }
        public int PrdLiquidacion { get; set; }
        public int Sector { get; set; }
        public string TipoServicio { get; set; }
        public string TipoUsuario { get; set; }
        public string Ubicacion { get; set; }
        public string Valor { get; set; }
        public string ValorOriginal { get; set; }
        public Factura()
        {

        }

    }
}
