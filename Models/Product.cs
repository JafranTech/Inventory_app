using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<int> PastSales { get; set; } = new List<int>();
        public bool IsLowStock => Quantity < Data.InventoryManager.LOW_STOCK_THRESHOLD;

        public Product()
        {
            Name = string.Empty;
            Category = string.Empty;
        }

        public Product(int id, string name, string category, int quantity, decimal price)
        {
            Id = id;
            Name = name;
            Category = category;
            Quantity = quantity;
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

        // Convert to CSV row
        public string ToCsvRow()
        {
            return $"{Id},{Name},{Category},{Quantity},{Price}";
        }

        // Parse from CSV row
        public static Product FromCsvRow(string row)
        {
            var values = row.Split(',');
            return new Product(
                int.Parse(values[0]),
                values[1],
                values[2],
                int.Parse(values[3]),
                decimal.Parse(values[4])
            );
        }

        public static string CsvHeader => "Id,Name,Category,Quantity,Price";
    }
}
