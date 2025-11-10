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
        private readonly InventoryApp.Data.InventoryManager _manager;
        private readonly DataGridView gridSales;
        private readonly Button btnRefresh;
        private readonly ComboBox cmbDateFilter;
        private readonly ComboBox cmbCategory;
        private readonly TextBox txtProductSearch;

        public SoldProductsControl(InventoryApp.Data.InventoryManager manager)
        {
            _dataService = new CsvDataService();
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            
            // Initialize control
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;
            this.Padding = new Padding(20);

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                Padding = new Padding(10)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Create filters panel
            var filtersPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Add date filter
            var lblDate = new Label { Text = "Date:", AutoSize = true, Margin = new Padding(0, 3, 5, 0) };
            cmbDateFilter = new ComboBox
            {
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 20, 0)
            };
            cmbDateFilter.Items.AddRange(new object[] {
                "All Time",
                "Today",
                "Last 7 Days",
                "Last 30 Days"
            });

            // Add category filter
            var lblCategory = new Label { Text = "Category:", AutoSize = true, Margin = new Padding(0, 3, 5, 0) };
            cmbCategory = new ComboBox
            {
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 20, 0)
            };
            cmbCategory.Items.Add("All Categories");
            cmbCategory.Items.AddRange(_manager.GetCategories().ToArray());
            cmbCategory.SelectedIndex = 0;

            // Add product search
            var lblProduct = new Label { Text = "Product:", AutoSize = true, Margin = new Padding(0, 3, 5, 0) };
            txtProductSearch = new TextBox
            {
                Width = 150,
                Margin = new Padding(0, 0, 20, 0)
            };
            try { txtProductSearch.PlaceholderText = "Search by name/ID"; } catch { }

            // Add refresh button
            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(51, 51, 76),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;

            // Add all filters to panel
            filtersPanel.Controls.AddRange(new Control[] {
                lblDate, cmbDateFilter,
                lblCategory, cmbCategory,
                lblProduct, txtProductSearch,
                btnRefresh
            });

            // Create sales grid
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                DefaultCellStyle = new DataGridViewCellStyle 
                {
                    Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                    Padding = new Padding(5)
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    BackColor = Color.FromArgb(51, 51, 76),
                    ForeColor = Color.White,
                    Padding = new Padding(5)
                },
                RowTemplate = { Height = 35 },
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(245, 245, 250)
                }
            };

            gridSales.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 51, 76);
            gridSales.DefaultCellStyle.SelectionForeColor = Color.White;

            // Add controls to main panel
            mainPanel.Controls.Add(filtersPanel, 0, 0);
            mainPanel.Controls.Add(gridSales, 0, 1);
            this.Controls.Add(mainPanel);

            // Wire up events
            btnRefresh.Click += (s, e) => LoadSales();
            cmbDateFilter.SelectedIndexChanged += (s, e) => LoadSales();
            cmbCategory.SelectedIndexChanged += (s, e) => LoadSales();
            txtProductSearch.TextChanged += (s, e) => LoadSales();

            // Select initial values
            cmbDateFilter.SelectedIndex = 0;

            // Subscribe to inventory updates
            _manager.InventoryUpdated += LoadSales;
            LoadSales();
        }

        private void LoadSales()
        {
            var sales = _manager.GetSales().ToList();

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

            // Apply category filter
            if (cmbCategory.SelectedItem?.ToString() != "All Categories")
            {
                var category = cmbCategory.SelectedItem?.ToString() ?? "";
                var productsInCategory = _manager.GetProducts()
                    .Where(p => p.Category == category)
                    .Select(p => p.Id)
                    .ToList();
                sales = sales.Where(s => productsInCategory.Contains(s.ProductId)).ToList();
            }

            // Apply product search
            var searchText = txtProductSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                sales = sales.Where(s => 
                    s.ProductName.ToLower().Contains(searchText) || 
                    s.ProductId.ToString().Contains(searchText)
                ).ToList();
            }

            // Calculate totals for filtered sales
            var totalQuantity = sales.Sum(s => s.QuantitySold);
            var totalRevenue = sales.Sum(s => s.TotalPrice);

            // Create view model that includes both sales and totals
            var rows = sales
                .OrderByDescending(s => s.SaleDate)
                .Select(s => new
                {
                    IsTotal = false,
                    Transaction = $"#{s.Id}",
                    Date = s.SaleDate.ToString("g"),
                    Product = s.ProductName,
                    ProductId = s.ProductId,
                    Category = _manager.GetProducts().FirstOrDefault(p => p.Id == s.ProductId)?.Category ?? "",
                    Quantity = s.QuantitySold,
                    UnitPrice = s.TotalPrice / s.QuantitySold,
                    Total = s.TotalPrice
                })
                .ToList();

            // Add total row
            rows.Add(new
            {
                IsTotal = true,
                Transaction = "TOTAL",
                Date = "",
                Product = $"{rows.Count} sales",
                ProductId = 0,
                Category = "",
                Quantity = totalQuantity,
                UnitPrice = 0M,
                Total = totalRevenue
            });

            gridSales.DataSource = rows;

            if (gridSales.Columns.Count > 0)
            {
                // Format currency columns
                gridSales.Columns["UnitPrice"].DefaultCellStyle.Format = "C2";
                gridSales.Columns["Total"].DefaultCellStyle.Format = "C2";
                
                // Right-align numeric columns
                gridSales.Columns["ProductId"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridSales.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridSales.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridSales.Columns["Total"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Hide IsTotal and ProductId columns
                gridSales.Columns["IsTotal"].Visible = false;
                gridSales.Columns["ProductId"].Visible = false;

                // Format total row
                foreach (DataGridViewRow row in gridSales.Rows)
                {
                    if ((bool)row.Cells["IsTotal"].Value)
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(51, 51, 76);
                        row.DefaultCellStyle.ForeColor = Color.White;
                        row.DefaultCellStyle.Font = new Font(gridSales.DefaultCellStyle.Font, FontStyle.Bold);
                    }
                }
            }
        }
    }
}