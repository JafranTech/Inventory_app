using System;
using System.Collections.Generic;
using System.Linq;
using InventoryApp.Models;

namespace InventoryApp.Data
{
    public class InventoryManager
    {
        private List<Product> products = new List<Product>();
        private const int LOW_STOCK_THRESHOLD = 5;
        private readonly Action<string> _write;

        public InventoryManager() : this(Console.WriteLine) { }

        public InventoryManager(Action<string> write)
        {
            _write = write ?? Console.WriteLine;
        }

        public void AddProduct(string name, int qty, double price)
        {
            products.Add(new Product(name, qty, price));
            _write($"✅ Product '{name}' added successfully!\n");
        }

        // --- Update stock (Sale) ---
        public void UpdateStock()
        {
            if (!products.Any())
            {
                _write("❌ No products available to update.");
                return;
            }

            _write("\n--- 🧾 CURRENT INVENTORY ---");
            for (int i = 0; i < products.Count; i++)
            {
                _write($"{i + 1}. {products[i].Name} (Qty: {products[i].Quantity})");
            }

            _write("UI mode: Select a product in the UI to update stock.");
        }

        // Non-interactive overload for UI usage (index is zero-based)
        public void UpdateStock(int index, int soldQty)
        {
            if (index < 0 || index >= products.Count)
            {
                _write("❌ Invalid product selection!");
                return;
            }
            var product = products[index];
            if (soldQty < 0)
            {
                _write("❌ Invalid quantity!");
                return;
            }
            if (soldQty > product.Quantity)
            {
                _write("⚠️ Not enough stock available!");
                return;
            }
            product.PastSales.Add(soldQty);
            product.Quantity -= soldQty;
            _write($"📦 Updated '{product.Name}' stock. Remaining: {product.Quantity}");
        }

        // --- Restock (Add Quantity) ---
        public void RestockProduct()
        {
            if (!products.Any())
            {
                _write("❌ No products available to restock.");
                return;
            }

            _write("\n--- 🧾 CURRENT INVENTORY ---");
            for (int i = 0; i < products.Count; i++)
            {
                _write($"{i + 1}. {products[i].Name} (Qty: {products[i].Quantity})");
            }

            _write("UI mode: Select a product in the UI to restock.");
        }

        // Non-interactive overload for UI usage (index is zero-based)
        public void RestockProduct(int index, int addQty)
        {
            if (index < 0 || index >= products.Count)
            {
                _write("❌ Invalid product selection!");
                return;
            }
            if (addQty <= 0)
            {
                _write("❌ Invalid quantity!");
                return;
            }
            var product = products[index];
            product.Quantity += addQty;
            _write($"✅ Added {addQty} units to '{product.Name}'. New stock: {product.Quantity}");
        }

        // --- Display All Products ---
        public void DisplayAllProducts()
        {
            _write("\n--- 🧾 INVENTORY STATUS ---");
            if (!products.Any())
            {
                _write("No products available.");
                return;
            }

            foreach (var p in products)
                _write(p.ToString());
        }

        // --- Show Low Stock Alert ---
        public void ShowLowStock()
        {
            var lowStock = products.Where(p => p.Quantity < LOW_STOCK_THRESHOLD).ToList();
            _write("\n⚠️ LOW STOCK ALERT ⚠️");
            if (!lowStock.Any())
                _write("All products have sufficient stock.");
            else
                foreach (var p in lowStock)
                    _write($"🚨 {p.Name} is low on stock ({p.Quantity} left).");
        }

        // --- Demand Forecast ---
        public void ForecastAll()
        {
            _write("\n📈 DEMAND FORECAST REPORT");
            if (!products.Any())
            {
                _write("No products to forecast.");
                return;
            }

            foreach (var p in products)
                _write($"🔹 {p.Name} → Next expected demand: {p.ForecastDemand():0.0}");
        }

        public IReadOnlyList<Product> GetProducts() => products.AsReadOnly();

        // Clears all products from the inventory
        public void ClearAllProducts()
        {
            products.Clear();
            _write("✅ All products cleared.");
        }
    }
}
