using System;
using System.Drawing;
using System.Windows.Forms;

namespace InventoryApp.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // Initialize controls
            this.topPanel = new Panel();
            this.sidebarPanel = new Panel();
            this.mainPanel = new Panel();
            this.contentPanel = new Panel();
            this.sidebarButtonsPanel = new FlowLayoutPanel();
            this.btnToggleSidebar = new Button();
            this.lblTitle = new Label();
            this.btnDashboard = new Button();
            this.btnAddProduct = new Button();
            this.btnViewProducts = new Button();
            this.btnLowStock = new Button();
            this.btnSoldProducts = new Button();
            this.btnClearRestart = new Button();
            this.lblUpdated = new Label();

            this.SuspendLayout();

            // Configure main form
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Inventory Management System";
            this.Name = "MainForm";

            // Configure top panel
            this.topPanel.Dock = DockStyle.Top;
            this.topPanel.Height = 60;
            this.topPanel.BackColor = Color.FromArgb(30, 30, 30);

            // Configure sidebar panel
            this.sidebarPanel.Dock = DockStyle.Left;
            this.sidebarPanel.Width = 250;
            this.sidebarPanel.BackColor = Color.FromArgb(45, 45, 48);

            // Configure main panel
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.BackColor = Color.FromArgb(37, 37, 38);

            // Configure content panel
            this.contentPanel.Dock = DockStyle.Fill;
            this.contentPanel.Padding = new Padding(20);

            // Configure sidebar buttons panel
            this.sidebarButtonsPanel.Dock = DockStyle.Top;
            this.sidebarButtonsPanel.AutoSize = true;
            this.sidebarButtonsPanel.FlowDirection = FlowDirection.TopDown;
            this.sidebarButtonsPanel.WrapContents = false;
            this.sidebarButtonsPanel.Padding = new Padding(10);

            // Configure toggle button
            this.btnToggleSidebar.Size = new Size(32, 32);
            this.btnToggleSidebar.Location = new Point(10, 14);
            this.btnToggleSidebar.FlatStyle = FlatStyle.Flat;
            this.btnToggleSidebar.FlatAppearance.BorderSize = 0;
            this.btnToggleSidebar.Text = "☰";
            this.btnToggleSidebar.ForeColor = Color.White;
            this.btnToggleSidebar.BackColor = Color.Transparent;
            this.btnToggleSidebar.Cursor = Cursors.Hand;

            // Configure title
            this.lblTitle.Text = "Inventory Management System";
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.White;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new Point(50, 15);

            // Configure menu buttons
            this.btnDashboard.Text = "Dashboard";
            this.btnAddProduct.Text = "Add Product";
            this.btnViewProducts.Text = "View Products";
            this.btnLowStock.Text = "Low Stock";
            this.btnSoldProducts.Text = "Sold Products";
            this.btnClearRestart.Text = "Clear / Restart";

            foreach (Control control in new Control[] {
                this.btnDashboard,
                this.btnAddProduct,
                this.btnViewProducts,
                this.btnLowStock,
                this.btnSoldProducts,
                this.btnClearRestart
            })
            {
                if (control is Button button)
                {
                    button.Width = 200;
                    button.Height = 40;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.ForeColor = Color.White;
                    button.BackColor = Color.FromArgb(45, 45, 48);
                    button.TextAlign = ContentAlignment.MiddleLeft;
                    button.Margin = new Padding(10, 5, 10, 5);
                    button.Cursor = Cursors.Hand;
                }
            }

            // Configure updated label
            this.lblUpdated.Text = "Updated: N/A";
            this.lblUpdated.ForeColor = Color.LightGray;
            this.lblUpdated.AutoSize = true;
            this.lblUpdated.Location = new Point(topPanel.Width - 300, 20);
            this.lblUpdated.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Configure open folder button
            this.btnOpenFolder = new Button();
            this.btnOpenFolder.Size = new Size(32, 32);
            this.btnOpenFolder.Location = new Point(topPanel.Width - 40, 14);
            this.btnOpenFolder.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnOpenFolder.FlatStyle = FlatStyle.Flat;
            this.btnOpenFolder.FlatAppearance.BorderSize = 0;
            this.btnOpenFolder.Text = "📂";
            this.btnOpenFolder.ForeColor = Color.LightGray;
            this.btnOpenFolder.BackColor = Color.Transparent;
            this.btnOpenFolder.Cursor = Cursors.Hand;
            this.btnOpenFolder.Click += btnOpenFolder_Click;

            // Add controls to panels
            this.topPanel.Controls.Add(this.btnToggleSidebar);
            this.topPanel.Controls.Add(this.lblTitle);
            this.topPanel.Controls.Add(this.lblUpdated);
            this.topPanel.Controls.Add(this.btnOpenFolder);

            this.sidebarButtonsPanel.Controls.AddRange(new Control[] {
                this.btnDashboard,
                this.btnAddProduct,
                this.btnViewProducts,
                this.btnLowStock,
                this.btnSoldProducts,
                this.btnClearRestart
            });

            this.sidebarPanel.Controls.Add(this.sidebarButtonsPanel);
            this.mainPanel.Controls.Add(this.contentPanel);

            // Add panels to form
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.sidebarPanel);
            this.Controls.Add(this.topPanel);

            this.ResumeLayout(false);
        }

        #endregion

        #region Designer Fields
    private Panel topPanel;
    private Panel sidebarPanel;
    private Panel mainPanel;
    private Panel contentPanel;
    private FlowLayoutPanel sidebarButtonsPanel;
    private Button btnToggleSidebar;
    private Label lblTitle;
    private Button btnDashboard;
    private Button btnAddProduct;
    private Button btnViewProducts;
    private Button btnLowStock;
    private Button btnSoldProducts;
    private Button btnClearRestart;
    private Label lblUpdated;
    private Button btnOpenFolder;
        #endregion
    }
}