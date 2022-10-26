using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebApplicationNW.Models
{
    public partial class OrderDetail
    {
        public static List<PropertyInfo> Columns = CargarColumnas();
        private static List<PropertyInfo> CargarColumnas()
        {
            List<PropertyInfo> c = new List<PropertyInfo>();
            foreach (PropertyInfo column in typeof(OrderDetail).GetProperties())
            {
                string[] columnasAOcultar = new string[] { "" };
                string tipo = column.PropertyType.Name;
                if (!columnasAOcultar.Contains(column.Name.ToUpper()) && tipo != "ICollection`1")
                    c.Add(column);
            }
            return c;
        }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        //public string ProductName { get { return Product!=null? Product.ProductName:""; } }
    }
}
