using System;

namespace InventoryApp.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantitySold { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime SaleDate { get; set; }

        public Sale()
        {
            ProductName = string.Empty;
            SaleDate = DateTime.Now;
        }

        public Sale(int id, int productId, string productName, int quantitySold, decimal totalPrice)
        {
            Id = id;
            ProductId = productId;
            ProductName = productName;
            QuantitySold = quantitySold;
            TotalPrice = totalPrice;
            SaleDate = DateTime.Now;
        }

        // Convert to CSV row
        public string ToCsvRow()
        {
            return $"{Id},{ProductId},{ProductName},{QuantitySold},{TotalPrice},{SaleDate:yyyy-MM-dd HH:mm:ss}";
        }

        // Parse from CSV row
        public static Sale FromCsvRow(string row)
        {
            var values = row.Split(',');
            var sale = new Sale(
                int.Parse(values[0]),
                int.Parse(values[1]),
                values[2],
                int.Parse(values[3]),
                decimal.Parse(values[4])
            );
            sale.SaleDate = DateTime.Parse(values[5]);
            return sale;
        }

        public static string CsvHeader => "Id,ProductId,ProductName,QuantitySold,TotalPrice,SaleDate";
    }
}