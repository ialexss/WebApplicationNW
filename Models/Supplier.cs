using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebApplicationNW.Models
{
    public partial class Supplier
    {
        public static List<PropertyInfo> Columns = CargarColumnas();
        private static List<PropertyInfo> CargarColumnas()
        {
            List<PropertyInfo> c = new List<PropertyInfo>();
            foreach (PropertyInfo column in typeof(Supplier).GetProperties())
            {
                string[] columnasAOcultar = new string[] { "SUPPLIERID" };
                string tipo = column.PropertyType.Name;
                if (!columnasAOcultar.Contains(column.Name.ToUpper()) && tipo != "ICollection`1")
                    c.Add(column);
            }
            return c;
        }
        public Supplier()
        {
            Products = new HashSet<Product>();
        }

        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public string? HomePage { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
