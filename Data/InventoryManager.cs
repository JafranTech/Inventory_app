using System;
using System.Collections.Generic;
using System.Linq;
using InventoryApp.Models;
using InventoryApp.Services;

namespace InventoryApp.Data
{
    public class InventoryManager
    {
        private List<Product> products = new List<Product>();
        private List<string> categories = new List<string>() { "General" };
        private const int LOW_STOCK_THRESHOLD = 5;
        private readonly Action<string> _write;
        private readonly CsvDataService _csvService;

        public InventoryManager() : this(Console.WriteLine) { }

        public InventoryManager(Action<string> write)
        {
            _write = write ?? Console.WriteLine;
            _csvService = new CsvDataService();
            // Load existing data from CSVs
            var loaded = _csvService.LoadProducts();
            if (loaded != null && loaded.Count > 0)
            {
                products = loaded.ToList();
                // collect categories
                foreach (var p in products)
                {
                    if (!string.IsNullOrWhiteSpace(p.Category) && !categories.Contains(p.Category))
                        categories.Add(p.Category);
                }
            }
            // Load sales and populate PastSales counts
            var sales = _csvService.LoadSales();
            foreach (var s in sales)
            {
                var prod = products.FirstOrDefault(p => p.Id == s.ProductId);
                if (prod != null)
                {
                    prod.PastSales.Add(s.QuantitySold);
                }
            }
        }

        public void AddProduct(string name, int qty, decimal price, string? category = null)
        {
            var id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
            var cat = string.IsNullOrWhiteSpace(category) ? "General" : category;
            if (!categories.Contains(cat))
                categories.Add(cat);
            var prod = new Product(id, name, cat, qty, price);
            products.Add(prod);
            // persist
            _csvService.SaveProducts(products);
            _write($"✅ Product '{name}' added successfully!\n");
        }

        // Category helpers
        public IReadOnlyList<string> GetCategories() => categories.AsReadOnly();

        public void AddCategory(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            if (!categories.Contains(name))
            {
                categories.Add(name);
                _write($"✅ Category '{name}' added.");
            }
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
            // log sale
            var saleId = _csvService.LoadSales().Count + 1;
            var sale = new Models.Sale(saleId, product.Id, product.Name, soldQty, soldQty * product.Price);
            _csvService.SaveSale(sale);
            // persist products and lowstock
            _csvService.SaveProducts(products);
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
            // persist
            _csvService.SaveProducts(products);
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
