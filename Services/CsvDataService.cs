using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var lines = File.ReadAllLines(_productsFile).Skip(1); // Skip header
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    products.Add(Product.FromCsvRow(line));
                }
            }
            return products;
        }

        public void SaveProducts(List<Product> products)
        {
            var lines = new List<string> { Product.CsvHeader };
            lines.AddRange(products.Select(p => p.ToCsvRow()));
            File.WriteAllLines(_productsFile, lines);

            // Update low stock file
            var lowStockProducts = products.Where(p => p.IsLowStock).ToList();
            lines = new List<string> { Product.CsvHeader };
            lines.AddRange(lowStockProducts.Select(p => p.ToCsvRow()));
            File.WriteAllLines(_lowStockFile, lines);
            
            OnCsvUpdated?.Invoke();
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
            var sales = LoadSales();
            sales.Add(sale);
            var lines = new List<string> { Sale.CsvHeader };
            lines.AddRange(sales.Select(s => s.ToCsvRow()));
            File.WriteAllLines(_salesFile, lines);
            
            OnCsvUpdated?.Invoke();
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