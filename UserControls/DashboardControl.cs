using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using InventoryApp.Data;

namespace InventoryApp.UserControls
{
    public partial class DashboardControl : UserControl
    {
        private readonly InventoryManager _inventoryManager;
    private readonly Label lblTotalProducts;
    private readonly Label lblLowStock;
    private readonly Label lblSoldItems;
    private readonly Label lblLastUpdated;
    private readonly FlowLayoutPanel categoryPanel;

        public DashboardControl(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
            
            // Initialize control
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;
            this.Padding = new Padding(20);

            // Create main layout
            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(10)
            };
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Top: summary cards
            var summaryPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(10)
            };
            summaryPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            summaryPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // Create stat panels
            lblTotalProducts = CreateStatsPanel("Total Products", "0");
            lblLowStock = CreateStatsPanel("Low Stock Items", "0");
            lblSoldItems = CreateStatsPanel("Items Sold", "0");

            summaryPanel.Controls.Add(lblTotalProducts, 0, 0);
            summaryPanel.Controls.Add(lblLowStock, 1, 0);
            summaryPanel.Controls.Add(lblSoldItems, 2, 0);

            // Bottom: category breakdown
            categoryPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                FlowDirection = FlowDirection.LeftToRight
            };

            // Last updated label
            lblLastUpdated = new Label
            {
                Text = $"Last Updated: {DateTime.Now:g}",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleRight,
                Height = 30
            };

            mainPanel.Controls.Add(summaryPanel, 0, 0);
            mainPanel.Controls.Add(categoryPanel, 0, 1);

            // Add to control
            this.Controls.Add(mainPanel);
            this.Controls.Add(lblLastUpdated);

            // Load initial data
            // Subscribe to inventory updates so dashboard refreshes automatically
            _inventoryManager.InventoryUpdated += RefreshDashboard;
            RefreshDashboard();
        }

        private Label CreateStatsPanel(string title, string value)
        {
            var label = new Label
            {
                Text = $"{title}\n{value}",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(this.Font.FontFamily, 12, FontStyle.Bold),
                BackColor = Color.White
            };

            return label;
        }

        public void RefreshDashboard()
        {
            var products = _inventoryManager.GetProducts();
            int lowStockCount = products.Count(p => p.Quantity < InventoryManager.LOW_STOCK_THRESHOLD);
            int totalTransactions = products.Sum(p => p.PastSales.Count);

            lblTotalProducts.Text = $"Total Products\n{products.Count}";
            lblLowStock.Text = $"Low Stock Items\n{lowStockCount}";
            lblSoldItems.Text = $"Total Transactions\n{totalTransactions}";
            lblLastUpdated.Text = $"Last Updated: {DateTime.Now:g}";

            // Update category breakdown
            categoryPanel.Controls.Clear();
            var categories = _inventoryManager.GetCategories();
            foreach (var c in categories)
            {
                var count = products.Count(p => string.Equals(p.Category, c, StringComparison.OrdinalIgnoreCase));
                var card = new Panel
                {
                    Width = 200,
                    Height = 80,
                    Margin = new Padding(6),
                    BackColor = Color.White
                };
                var title = new Label { Text = c, Dock = DockStyle.Top, Height = 20, TextAlign = ContentAlignment.MiddleCenter, Font = new Font(this.Font, FontStyle.Bold) };
                var val = new Label { Text = $"Products: {count}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
                card.Controls.Add(val);
                card.Controls.Add(title);
                categoryPanel.Controls.Add(card);
            }
        }
    }
}