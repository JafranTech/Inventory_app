using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using InventoryApp.Services;
using InventoryApp.Models;

namespace InventoryApp.UserControls
{
    public partial class LowStockControl : UserControl
    {
        private readonly CsvDataService _dataService;
        private readonly InventoryApp.Data.InventoryManager _manager;
        private readonly DataGridView gridLowStock;
        private readonly Button btnRefresh;

        public LowStockControl(InventoryApp.Data.InventoryManager manager)
        {
            _dataService = new CsvDataService();
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            
            // Initialize control
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;
            this.Padding = new Padding(20);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };

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
            btnRefresh.Click += (s, e) => LoadLowStock();
            panel.Controls.Add(btnRefresh);

            // Create grid
            gridLowStock = new DataGridView
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

            gridLowStock.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 51, 76);
            gridLowStock.DefaultCellStyle.SelectionForeColor = Color.White;

            panel.Controls.Add(gridLowStock);
            this.Controls.Add(panel);

            // subscribe to inventory updates so this list refreshes when inventory changes
            _manager.InventoryUpdated += LoadLowStock;
            LoadLowStock();
        }

        private void LoadLowStock()
        {
            var products = _manager.GetProducts()
                .Where(p => p.IsLowStock)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Category,
                    p.Quantity,
                    p.Price,
                    ReorderStatus = p.Quantity == 0 ? "Out of Stock!" : "Low Stock"
                })
                .ToList();

            gridLowStock.DataSource = products;

            if (gridLowStock.Columns["ReorderStatus"] != null)
            {
                foreach (DataGridViewRow row in gridLowStock.Rows)
                {
                    var status = row.Cells["ReorderStatus"].Value.ToString();
                    row.Cells["ReorderStatus"].Style.ForeColor = status == "Out of Stock!" ? Color.Red : Color.Orange;
                }
            }
        }
    }
}