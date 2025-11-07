using System.Windows.Forms;
using System.Drawing;

namespace InventoryApp.Forms
{
    partial class MainForm
    {
        private Label lblTitle;
        private TextBox txtName;
        private TextBox txtQty;
        private TextBox txtPrice;
        private Button btnAdd;
        private ListBox lstProducts;
        private TextBox txtActionQty;
        private Button btnUpdateStock;
        private Button btnRestock;
        private Button btnView;
        private Button btnLowStock;
        private Button btnForecast;
        private TextBox txtOutput;
    private Button btnClearOutput;
    private Button btnResetData;
    private CheckBox chkDarkMode;
        private Label lblName;
        private Label lblQty;
        private Label lblPrice;
        private Label lblProducts;
        private Label lblActionQty;

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.txtName = new TextBox();
            this.txtQty = new TextBox();
            this.txtPrice = new TextBox();
            this.btnAdd = new Button();
            this.lstProducts = new ListBox();
            this.txtActionQty = new TextBox();
            this.btnUpdateStock = new Button();
            this.btnRestock = new Button();
            this.btnView = new Button();
            this.btnLowStock = new Button();
            this.btnForecast = new Button();
            this.txtOutput = new TextBox();
            this.lblName = new Label();
            this.lblQty = new Label();
            this.lblPrice = new Label();
            this.lblProducts = new Label();
            this.lblActionQty = new Label();
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new Size(960, 600);
            this.Text = "Inventory Management";
            this.StartPosition = FormStartPosition.CenterScreen;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            this.lblTitle.Location = new Point(20, 15);
            this.lblTitle.Text = "Inventory Management";

            // 
            // chkDarkMode
            // 
            this.chkDarkMode = new CheckBox();
            this.chkDarkMode.Location = new Point(820, 20);
            this.chkDarkMode.Size = new Size(110, 24);
            this.chkDarkMode.Text = "Dark Mode";
            this.chkDarkMode.CheckedChanged += new System.EventHandler(this.chkDarkMode_CheckedChanged);

            // 
            // lblName
            // 
            this.lblName.Location = new Point(20, 60);
            this.lblName.AutoSize = true;
            this.lblName.Text = "Name";

            // 
            // txtName
            // 
            this.txtName.Location = new Point(90, 58);
            this.txtName.Size = new Size(180, 23);

            // 
            // lblQty
            // 
            this.lblQty.Location = new Point(290, 60);
            this.lblQty.AutoSize = true;
            this.lblQty.Text = "Qty";

            // 
            // txtQty
            // 
            this.txtQty.Location = new Point(330, 58);
            this.txtQty.Size = new Size(80, 23);

            // 
            // lblPrice
            // 
            this.lblPrice.Location = new Point(430, 60);
            this.lblPrice.AutoSize = true;
            this.lblPrice.Text = "Price";

            // 
            // txtPrice
            // 
            this.txtPrice.Location = new Point(480, 58);
            this.txtPrice.Size = new Size(100, 23);

            // 
            // btnAdd
            // 
            this.btnAdd.Location = new Point(600, 57);
            this.btnAdd.Size = new Size(100, 27);
            this.btnAdd.Text = "Add Product";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);

            // 
            // lblProducts
            // 
            this.lblProducts.Location = new Point(20, 100);
            this.lblProducts.AutoSize = true;
            this.lblProducts.Text = "Products";

            // 
            // lstProducts
            // 
            this.lstProducts.Location = new Point(20, 120);
            this.lstProducts.Size = new Size(380, 300);

            // 
            // lblActionQty
            // 
            this.lblActionQty.Location = new Point(20, 430);
            this.lblActionQty.AutoSize = true;
            this.lblActionQty.Text = "Quantity";

            // 
            // txtActionQty
            // 
            this.txtActionQty.Location = new Point(90, 428);
            this.txtActionQty.Size = new Size(100, 23);

            // 
            // btnUpdateStock
            // 
            this.btnUpdateStock.Location = new Point(210, 427);
            this.btnUpdateStock.Size = new Size(90, 27);
            this.btnUpdateStock.Text = "Sell";
            this.btnUpdateStock.Click += new System.EventHandler(this.btnUpdateStock_Click);

            // 
            // btnRestock
            // 
            this.btnRestock.Location = new Point(310, 427);
            this.btnRestock.Size = new Size(90, 27);
            this.btnRestock.Text = "Restock";
            this.btnRestock.Click += new System.EventHandler(this.btnRestock_Click);

            // 
            // btnView
            // 
            this.btnView.Location = new Point(420, 120);
            this.btnView.Size = new Size(120, 30);
            this.btnView.Text = "View Inventory";
            this.btnView.Click += new System.EventHandler(this.btnView_Click);

            // 
            // btnLowStock
            // 
            this.btnLowStock.Location = new Point(420, 160);
            this.btnLowStock.Size = new Size(120, 30);
            this.btnLowStock.Text = "Low Stock";
            this.btnLowStock.Click += new System.EventHandler(this.btnLowStock_Click);

            // 
            // btnForecast
            // 
            this.btnForecast.Location = new Point(420, 200);
            this.btnForecast.Size = new Size(120, 30);
            this.btnForecast.Text = "Forecast";
            this.btnForecast.Click += new System.EventHandler(this.btnForecast_Click);

            // 
            // txtOutput
            // 
            this.txtOutput.Location = new Point(560, 120);
            this.txtOutput.Multiline = true;
            this.txtOutput.ScrollBars = ScrollBars.Vertical;
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new Size(370, 390);

            // 
            // btnClearOutput
            // 
            this.btnClearOutput = new Button();
            this.btnClearOutput.Location = new Point(560, 520);
            this.btnClearOutput.Size = new Size(120, 27);
            this.btnClearOutput.Text = "Clear Output";
            this.btnClearOutput.Click += new System.EventHandler(this.btnClearOutput_Click);

            // 
            // btnResetData
            // 
            this.btnResetData = new Button();
            this.btnResetData.Location = new Point(700, 520);
            this.btnResetData.Size = new Size(120, 27);
            this.btnResetData.Text = "Reset Data";
            this.btnResetData.Click += new System.EventHandler(this.btnResetData_Click);

            // add controls
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblProducts);
            this.Controls.Add(this.lstProducts);
            this.Controls.Add(this.lblActionQty);
            this.Controls.Add(this.txtActionQty);
            this.Controls.Add(this.btnUpdateStock);
            this.Controls.Add(this.btnRestock);
            this.Controls.Add(this.btnView);
            this.Controls.Add(this.btnLowStock);
            this.Controls.Add(this.btnForecast);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnClearOutput);
            this.Controls.Add(this.btnResetData);
            this.Controls.Add(this.chkDarkMode);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
