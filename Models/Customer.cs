using System;
using System.Collections.Generic;
using System.Reflection;

namespace WebApplicationNW.Models
{
    public partial class Customer
    {
        public static List<PropertyInfo> Columns = CargarColumnas();
        private static List<PropertyInfo> CargarColumnas()
        {
            List<PropertyInfo> c = new List<PropertyInfo>();
            foreach (PropertyInfo column in typeof(Customer).GetProperties())
            {
                string nombre = column.Name.ToUpper();
                string tipo = column.PropertyType.Name;
                if (nombre != "CUSTOMER" + "ID" && tipo != "ICollection`1")
                    c.Add(column);
            }
            return c;
        }
        public Customer()
        {
            Orders = new HashSet<Order>();
            CustomerTypes = new HashSet<CustomerDemographic>();
        }

        public string CustomerId { get; set; } = null!;
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


        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<CustomerDemographic> CustomerTypes { get; set; }
    }
}
