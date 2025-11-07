using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using InventoryApp.Data;

namespace InventoryApp.Forms
{
    public partial class MainForm : Form
    {
        private readonly InventoryManager _manager;

        public MainForm()
        {
            InitializeComponent();
            _manager = new InventoryManager(AppendOutput);
            RefreshProductList();
        }

        private void AppendOutput(string message)
        {
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new Action(() => AppendOutput(message)));
                return;
            }
            txtOutput.AppendText(message + Environment.NewLine);
        }

        private void RefreshProductList()
        {
            var items = _manager.GetProducts()
                .Select((p, i) => new { Index = i, Display = $"{p.Name} (Qty: {p.Quantity})" })
                .ToList();
            lstProducts.DisplayMember = "Display";
            lstProducts.ValueMember = "Index";
            lstProducts.DataSource = items;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)
                || !int.TryParse(txtQty.Text, out int qty)
                || !double.TryParse(txtPrice.Text, out double price))
            {
                AppendOutput("❌ Please enter valid Name, Quantity and Price.");
                return;
            }
            _manager.AddProduct(txtName.Text.Trim(), qty, price);
            RefreshProductList();
            txtName.Clear();
            txtQty.Clear();
            txtPrice.Clear();
        }

        private int? SelectedIndex()
        {
            if (lstProducts.SelectedValue == null) return null;
            return (int)lstProducts.SelectedValue;
        }

        private void btnUpdateStock_Click(object sender, EventArgs e)
        {
            var idx = SelectedIndex();
            if (idx == null)
            {
                AppendOutput("❌ Select a product first.");
                return;
            }
            if (!int.TryParse(txtActionQty.Text, out int sold) || sold < 0)
            {
                AppendOutput("❌ Enter a valid quantity to sell.");
                return;
            }
            _manager.UpdateStock(idx.Value, sold);
            RefreshProductList();
        }

        private void btnRestock_Click(object sender, EventArgs e)
        {
            var idx = SelectedIndex();
            if (idx == null)
            {
                AppendOutput("❌ Select a product first.");
                return;
            }
            if (!int.TryParse(txtActionQty.Text, out int addQty) || addQty <= 0)
            {
                AppendOutput("❌ Enter a valid quantity to add.");
                return;
            }
            _manager.RestockProduct(idx.Value, addQty);
            RefreshProductList();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            _manager.DisplayAllProducts();
        }

        private void btnLowStock_Click(object sender, EventArgs e)
        {
            _manager.ShowLowStock();
        }

        private void btnForecast_Click(object sender, EventArgs e)
        {
            _manager.ForecastAll();
        }

        private void chkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox cb)
            {
                ApplyDarkMode(cb.Checked);
            }
        }

        private void ApplyDarkMode(bool enabled)
        {
            // Define colors for light and dark themes
            Color back = enabled ? Color.FromArgb(34, 34, 34) : SystemColors.Control;
            Color panelBack = enabled ? Color.FromArgb(45, 45, 48) : SystemColors.Window;
            Color text = enabled ? Color.WhiteSmoke : SystemColors.ControlText;
            Color buttonBack = enabled ? Color.FromArgb(63, 63, 70) : SystemColors.ControlLight;
            Color buttonText = enabled ? Color.White : SystemColors.ControlText;

            // Form background
            this.BackColor = back;
            this.ForeColor = text;

            // Labels
            lblTitle.ForeColor = text;
            lblName.ForeColor = text;
            lblQty.ForeColor = text;
            lblPrice.ForeColor = text;
            lblProducts.ForeColor = text;
            lblActionQty.ForeColor = text;

            // TextBoxes
            txtName.BackColor = panelBack;
            txtName.ForeColor = text;
            txtQty.BackColor = panelBack;
            txtQty.ForeColor = text;
            txtPrice.BackColor = panelBack;
            txtPrice.ForeColor = text;
            txtActionQty.BackColor = panelBack;
            txtActionQty.ForeColor = text;
            txtOutput.BackColor = panelBack;
            txtOutput.ForeColor = text;

            // ListBox
            lstProducts.BackColor = panelBack;
            lstProducts.ForeColor = text;

            // Buttons
            var buttons = new[] { btnAdd, btnUpdateStock, btnRestock, btnView, btnLowStock, btnForecast, btnClearOutput, btnResetData };
            foreach (var b in buttons)
            {
                b.BackColor = buttonBack;
                b.ForeColor = buttonText;
                b.FlatStyle = FlatStyle.System;
            }

            // Checkbox
            chkDarkMode.BackColor = Color.Transparent;
            chkDarkMode.ForeColor = text;
        }

        private void btnClearOutput_Click(object sender, EventArgs e)
        {
            // Clear the output textbox
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new Action(() => txtOutput.Clear()));
                return;
            }
            txtOutput.Clear();
            AppendOutput("➖ Output cleared.");
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            // Clear inventory data and UI lists, then clear output
            _manager.ClearAllProducts();
            RefreshProductList();
            if (txtOutput.InvokeRequired)
            {
                txtOutput.Invoke(new Action(() => txtOutput.Clear()));
                return;
            }
            txtOutput.Clear();
            AppendOutput("🔄 Inventory reset completed.");
        }
    }
}
