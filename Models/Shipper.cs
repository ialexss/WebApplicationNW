using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebApplicationNW.Models
{
    public partial class Shipper
    {
        public static List<PropertyInfo> Columns = CargarColumnas();
        private static List<PropertyInfo> CargarColumnas()
        {
            List<PropertyInfo> c = new List<PropertyInfo>();
            foreach (PropertyInfo column in typeof(Shipper).GetProperties())
            {
                string[] columnasAOcultar = new string[] { "SHIPPERID" };
                string tipo = column.PropertyType.Name;
                if (!columnasAOcultar.Contains(column.Name.ToUpper()) && tipo != "ICollection`1")
                    c.Add(column);
            }
            return c;
        }
        public Shipper()
        {
            Orders = new HashSet<Order>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Phone { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
