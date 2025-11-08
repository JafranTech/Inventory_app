using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using InventoryApp.Services;
using InventoryApp.Models;

namespace InventoryApp.UserControls
{
    public partial class SoldProductsControl : UserControl
    {
        private readonly CsvDataService _dataService;
        private readonly DataGridView gridSales;
        private readonly Button btnRefresh;
        private readonly ComboBox cmbDateFilter;
        
        public SoldProductsControl()
        {
            _dataService = new CsvDataService();
            
            // Initialize control
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;
            this.Padding = new Padding(20);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2
            };

            // Add date filter
            cmbDateFilter = new ComboBox
            {
                Dock = DockStyle.Left,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDateFilter.Items.AddRange(new object[] {
                "All Time",
                "Today",
                "Last 7 Days",
                "Last 30 Days"
            });
            cmbDateFilter.SelectedIndex = 0;
            cmbDateFilter.SelectedIndexChanged += (s, e) => LoadSales();
            panel.Controls.Add(cmbDateFilter);

            // Add refresh button
            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Dock = DockStyle.Right,
                Width = 100,
                BackColor = Color.FromArgb(51, 51, 76),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRefresh.Click += (s, e) => LoadSales();
            panel.Controls.Add(btnRefresh);

            // Create grid
            gridSales = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            gridSales.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 51, 76);
            gridSales.DefaultCellStyle.SelectionForeColor = Color.White;

            panel.Controls.Add(gridSales);
            this.Controls.Add(panel);

            LoadSales();
        }

        private void LoadSales()
        {
            var sales = _dataService.LoadSales();

            // Apply date filter
            if (cmbDateFilter.SelectedItem != null)
            {
                switch (cmbDateFilter.SelectedItem.ToString())
                {
                    case "Today":
                        sales = sales.Where(s => s.SaleDate.Date == DateTime.Today).ToList();
                        break;
                    case "Last 7 Days":
                        sales = sales.Where(s => s.SaleDate >= DateTime.Today.AddDays(-7)).ToList();
                        break;
                    case "Last 30 Days":
                        sales = sales.Where(s => s.SaleDate >= DateTime.Today.AddDays(-30)).ToList();
                        break;
                }
            }

            gridSales.DataSource = sales.Select(s => new
            {
                s.Id,
                s.ProductName,
                s.QuantitySold,
                s.TotalPrice,
                SaleDate = s.SaleDate.ToString("g")
            }).ToList();
        }
    }
}