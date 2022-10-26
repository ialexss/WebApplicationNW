using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebApplicationNW.Models
{
    public partial class Category
    {
        public static List<PropertyInfo> Columns = CargarColumnas();
        private static List<PropertyInfo> CargarColumnas()
        {
            List<PropertyInfo> c = new List<PropertyInfo>();
            foreach (PropertyInfo column in typeof(Category).GetProperties())
            {
                //string nombre = column.Name.ToUpper();
                //string tipo = column.PropertyType.Name;
                //if (nombre != "CATEGORY" + "ID" && tipo!= "ICollection`1")
                //    c.Add(column);
                string[] columnasAOcultar = new string[] { "PICTURE","CATEGORYID" };
                string tipo = column.PropertyType.Name;
                if (!columnasAOcultar.Contains(column.Name.ToUpper()) && tipo != "ICollection`1")
                    c.Add(column);
            }
            return c;
        }
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
