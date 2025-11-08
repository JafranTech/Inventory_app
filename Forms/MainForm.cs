using System;
using System.Windows.Forms;
using System.Drawing;
using InventoryApp.Data;
using InventoryApp.Services;

namespace InventoryApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly InventoryManager _inventoryManager;
        private readonly CsvDataService _csvDataService;
        private bool _isSidebarExpanded = true;
        private const int CollapsedSidebarWidth = 60;
        private const int ExpandedSidebarWidth = 250;
        private readonly Color ActiveButtonColor = Color.FromArgb(60, 60, 63);
        private Button? _currentActiveButton;

        public MainForm()
        {
            InitializeComponent();
            _csvDataService = new CsvDataService();
            _csvDataService.OnCsvUpdated += UpdateTimestamp;
            _inventoryManager = new InventoryManager();
            ConfigureForm();
            SetupEventHandlers();
            UpdateTimestamp();
            
            // Hook up menu click events
            this.btnDashboard.Click += OnDashboardClick;
            this.btnAddProduct.Click += OnAddProductClick;
            this.btnViewProducts.Click += OnViewProductsClick;
            this.btnLowStock.Click += OnLowStockClick;
            this.btnSoldProducts.Click += OnSoldProductsClick;
            this.btnClearRestart.Click += OnClearRestartClick;

            // Start with Dashboard selected
            SelectMenuButton(this.btnDashboard);
            LoadDashboardContent();
        }

        private void ConfigureForm()
        {
            // Store initial text as Tag for collapsible sidebar
            foreach (Control control in this.sidebarButtonsPanel.Controls)
            {
                if (control is Button button)
                {
                    button.Tag = button.Text;
                }
            }
        }

        private void SetupEventHandlers()
        {
            this.btnToggleSidebar.Click += this.btnToggleSidebar_Click;
            this.Resize += this.MainForm_Resize;
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            // Update layout when form is resized
            this.contentPanel.Invalidate();
        }

        private void btnToggleSidebar_Click(object? sender, EventArgs e)
        {
            _isSidebarExpanded = !_isSidebarExpanded;
            this.sidebarPanel.Width = _isSidebarExpanded ? ExpandedSidebarWidth : CollapsedSidebarWidth;

            // Update button visibility based on sidebar state
            foreach (Control control in this.sidebarButtonsPanel.Controls)
            {
                if (control is Button button)
                {
                    button.Text = _isSidebarExpanded ? button.Tag?.ToString() : "";
                    button.TextAlign = _isSidebarExpanded ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
                }
            }
        }

        private void SelectMenuButton(Button button)
        {
            if (_currentActiveButton != null)
            {
                _currentActiveButton.BackColor = Color.FromArgb(45, 45, 48);
            }

            _currentActiveButton = button;
            button.BackColor = ActiveButtonColor;
        }

        #region Menu Click Handlers
        private void OnDashboardClick(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                SelectMenuButton(button);
                LoadDashboardContent();
            }
        }

        private void OnAddProductClick(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                SelectMenuButton(button);
                LoadAddProductContent();
            }
        }

        private void OnViewProductsClick(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                SelectMenuButton(button);
                LoadViewProductsContent();
            }
        }

        private void OnLowStockClick(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                SelectMenuButton(button);
                LoadLowStockContent();
            }
        }

        private void OnSoldProductsClick(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                SelectMenuButton(button);
                LoadSoldProductsContent();
            }
        }

        private void OnClearRestartClick(object? sender, EventArgs e)
        {
            var confirm = MessageBox.Show("This will clear all inventory and sales data. Continue?", "Confirm Clear", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                var svc = new InventoryApp.Services.CsvDataService();
                svc.ClearAllData();
                // reload inventory manager state
                _inventoryManager.ClearAllProducts();
                LoadDashboardContent();
                MessageBox.Show("Data cleared.", "Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #region Content Loading
        private void LoadDashboardContent()
        {
            var dashboard = new UserControls.DashboardControl(_inventoryManager);
            dashboard.Dock = DockStyle.Fill;
            ClearContentPanel();
            this.contentPanel.Controls.Add(dashboard);
            dashboard.RefreshDashboard();
        }

        private void LoadInventoryContent()
        {
            var inventory = new UserControls.InventoryControl(_inventoryManager);
            inventory.Dock = DockStyle.Fill;
            ClearContentPanel();
            this.contentPanel.Controls.Add(inventory);
            inventory.RefreshData();
        }

        private void LoadAddProductContent()
        {
            var add = new UserControls.AddProductControl();
            add.Dock = DockStyle.Fill;
            ClearContentPanel();
            this.contentPanel.Controls.Add(add);
        }

        private void LoadViewProductsContent()
        {
            var view = new UserControls.ViewProductsControl();
            view.Dock = DockStyle.Fill;
            ClearContentPanel();
            this.contentPanel.Controls.Add(view);
        }

        private void LoadLowStockContent()
        {
            var low = new UserControls.LowStockControl();
            low.Dock = DockStyle.Fill;
            ClearContentPanel();
            this.contentPanel.Controls.Add(low);
        }

        private void LoadSoldProductsContent()
        {
            var sold = new UserControls.SoldProductsControl();
            sold.Dock = DockStyle.Fill;
            ClearContentPanel();
            this.contentPanel.Controls.Add(sold);
        }

        private void LoadTransactionsContent()
        {
            // TODO: Implement transactions view
            MessageBox.Show("Transactions view coming soon!");
        }

        private void LoadReportsContent()
        {
            // TODO: Implement reports view
            MessageBox.Show("Reports view coming soon!");
        }

        private void LoadSettingsContent()
        {
            // TODO: Implement settings view
            MessageBox.Show("Settings view coming soon!");
        }

        private void ClearContentPanel()
        {
            this.contentPanel.Controls.Clear();
        }
        #endregion

        // Called by child controls to request dashboard refresh when data changes
        public void RefreshDashboardIfPresent()
        {
            foreach (Control c in this.contentPanel.Controls)
            {
                if (c is UserControls.DashboardControl dc)
                {
                    dc.RefreshDashboard();
                    break;
                }
            }
        }

        // Allows child controls to add product via the main InventoryManager so in-memory state stays in sync
        public void AddProductViaManager(string name, int qty, decimal price, string category)
        {
            _inventoryManager.AddProduct(name, qty, price, category);
            RefreshDashboardIfPresent();
        }

        // Helpers for CSV data operations
        public void UpdateTimestamp()
        {
            if (lblUpdated.InvokeRequired)
            {
                lblUpdated.Invoke(new Action(UpdateTimestamp));
                return;
            }
            lblUpdated.Text = "Updated: " + DateTime.Now.ToString("g");
        }

        private void btnOpenFolder_Click(object? sender, EventArgs e)
        {
            _csvDataService.OpenDataFolder();
        }

        // End of MainForm class

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Add any custom painting here if needed
        }
    }
}
