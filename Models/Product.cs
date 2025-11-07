using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApp.Models
{
    public class Product
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public List<int> PastSales { get; set; } = new List<int>();

        public Product(string name, int qty, double price)
        {
            Name = name;
            Quantity = qty;
            Price = price;
        }

        public double ForecastDemand()
        {
            if (PastSales.Count < 3)
                return PastSales.Count > 0 ? PastSales.Average() : 0;

            // Average of last 3 sales
            return PastSales.Skip(Math.Max(0, PastSales.Count - 3)).Average();
        }

        public override string ToString()
        {
            return $"{Name,-15} | Qty: {Quantity,-5} | Price: ₹{Price,-8} | Forecast: {ForecastDemand():0.0}";
        }
    }
}
