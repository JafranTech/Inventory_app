using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
    using System.Threading;
using InventoryApp.Models;

namespace InventoryApp.Services
{
    public class CsvDataService
    {
        private readonly string _dataFolder;
        private readonly string _productsFile;
        private readonly string _salesFile;
        private readonly string _lowStockFile;
        
        // Event to notify about CSV updates
        public event Action? OnCsvUpdated;

        public CsvDataService()
        {
            _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InventoryData");
            _productsFile = Path.Combine(_dataFolder, "products.csv");
            _salesFile = Path.Combine(_dataFolder, "sales.csv");
            _lowStockFile = Path.Combine(_dataFolder, "lowstock.csv");

            // Ensure directory exists
            Directory.CreateDirectory(_dataFolder);
            InitializeFiles();
        }

        private void InitializeFiles()
        {
            // Create files with headers if they don't exist
            if (!File.Exists(_productsFile))
            {
                File.WriteAllText(_productsFile, Product.CsvHeader + Environment.NewLine);
            }
            if (!File.Exists(_salesFile))
            {
                File.WriteAllText(_salesFile, Sale.CsvHeader + Environment.NewLine);
            }
            if (!File.Exists(_lowStockFile))
            {
                File.WriteAllText(_lowStockFile, Product.CsvHeader + Environment.NewLine);
            }
        }

        // Products
        public List<Product> LoadProducts()
        {
            var products = new List<Product>();
            // Attempt to read the products file with a small retry loop and shared read access
            const int maxAttempts = 5;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using (var fs = new FileStream(_productsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var sr = new StreamReader(fs))
                    {
                        // Read header line first
                        var header = sr.ReadLine();
                        while (!sr.EndOfStream)
                        {
                            var line = sr.ReadLine();
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                products.Add(Product.FromCsvRow(line));
                            }
                        }
                    }
                    break; // success
                }
                catch (IOException)
                {
                    // If last attempt, rethrow so caller sees the problem; otherwise wait and retry
                    if (attempt == maxAttempts)
                        throw;
                    Thread.Sleep(200);
                }
            }

            return products;
        }

        public void SaveProducts(List<Product> products)
        {
            const int maxAttempts = 5;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var lines = new List<string> { Product.CsvHeader };
                    lines.AddRange(products.Select(p => p.ToCsvRow()));
                    
                    // Write with FileShare.ReadWrite to allow other processes to read
                    using (var fs = new FileStream(_productsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (var sw = new StreamWriter(fs))
                    {
                        foreach (var line in lines)
                        {
                            sw.WriteLine(line);
                        }
                    }

                    // Update low stock file
                    var lowStockProducts = products.Where(p => p.IsLowStock).ToList();
                    lines = new List<string> { Product.CsvHeader };
                    lines.AddRange(lowStockProducts.Select(p => p.ToCsvRow()));
                    
                    using (var fs = new FileStream(_lowStockFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (var sw = new StreamWriter(fs))
                    {
                        foreach (var line in lines)
                        {
                            sw.WriteLine(line);
                        }
                    }
                    
                    OnCsvUpdated?.Invoke();
                    break;
                }
                catch (IOException) when (attempt < maxAttempts)
                {
                    Thread.Sleep(200 * attempt); // Exponential backoff
                }
            }
        }

        // Sales
        public List<Sale> LoadSales()
        {
            var sales = new List<Sale>();
            var lines = File.ReadAllLines(_salesFile).Skip(1); // Skip header
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    sales.Add(Sale.FromCsvRow(line));
                }
            }
            return sales;
        }

        public void SaveSale(Sale sale)
        {
            const int maxAttempts = 5;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var sales = LoadSales();
                    sales.Add(sale);
                    var lines = new List<string> { Sale.CsvHeader };
                    lines.AddRange(sales.Select(s => s.ToCsvRow()));

                    using (var fs = new FileStream(_salesFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    using (var sw = new StreamWriter(fs))
                    {
                        foreach (var line in lines)
                        {
                            sw.WriteLine(line);
                        }
                    }
                    
                    OnCsvUpdated?.Invoke();
                    break;
                }
                catch (IOException) when (attempt < maxAttempts)
                {
                    Thread.Sleep(200 * attempt); // Exponential backoff
                }
            }
        }

        // Clear all data
        public void ClearAllData()
        {
            File.WriteAllText(_productsFile, Product.CsvHeader + Environment.NewLine);
            File.WriteAllText(_salesFile, Sale.CsvHeader + Environment.NewLine);
            File.WriteAllText(_lowStockFile, Product.CsvHeader + Environment.NewLine);
            
            OnCsvUpdated?.Invoke();
        }

        // Export data folder
        public void OpenDataFolder()
        {
            System.Diagnostics.Process.Start("explorer.exe", _dataFolder);
        }

        // Get products file path for export
        public string GetProductsFilePath()
        {
            return _productsFile;
        }
    }
}