using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using InventoryApp.Data;
using InventoryApp.Models;

namespace InventoryApp.UserControls
{
    public class InventoryControl : UserControl
    {
        private readonly InventoryManager _inventoryManager;
    private DataGridView? dgvProducts;
        private Button? btnAdd;
        private Button? btnUpdate;
        private Button? btnRestock;
    private ComboBox? cmbFilterCategory;

        public InventoryControl(InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
            InitializeComponent();
            LoadProducts();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Create DataGridView
            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };

            // Create buttons panel
            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(5)
            };

            // Category filter
            cmbFilterCategory = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 140,
                Height = 28
            };
            cmbFilterCategory.SelectedIndexChanged += (s, e) => LoadProducts();
            // Populate
            cmbFilterCategory.Items.Add("All");
            foreach (var c in _inventoryManager.GetCategories()) cmbFilterCategory.Items.Add(c);
            cmbFilterCategory.SelectedIndex = 0;

            btnAdd = new Button
            {
                Text = "Add Product",
                Width = 100,
                Height = 30
            };
            btnAdd.Click += OnAddProduct;

            btnUpdate = new Button
            {
                Text = "Update Stock",
                Width = 100,
                Height = 30
            };
            btnUpdate.Click += OnUpdateStock;

            btnRestock = new Button
            {
                Text = "Restock",
                Width = 100,
                Height = 30
            };
            btnRestock.Click += OnRestock;

            buttonsPanel.Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnRestock, cmbFilterCategory });

            // Add controls
            this.Controls.Add(dgvProducts);
            this.Controls.Add(buttonsPanel);

            this.ResumeLayout(false);
        }

        private void LoadProducts()
        {
            if (dgvProducts == null) return;

            IEnumerable<Product> products = _inventoryManager.GetProducts();
            // Apply category filter if present
            if (cmbFilterCategory != null && cmbFilterCategory.SelectedItem != null && cmbFilterCategory.SelectedItem.ToString() != "All")
            {
                var sel = cmbFilterCategory.SelectedItem.ToString();
                products = products.Where(p => string.Equals(p.Category, sel, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var productData = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Category,
                p.Quantity,
                p.Price,
                TotalSales = p.PastSales.Count,
                Status = p.Quantity < 5 ? "Low Stock" : "OK"
            }).ToList();

            dgvProducts.DataSource = productData;
        }

    private void OnAddProduct(object? sender, EventArgs e)
        {
            using var form = new Form
            {
                Text = "Add Product",
                Size = new Size(300, 250),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblName = new Label { Text = "Name:", Left = 20, Top = 20 };
            var txtName = new TextBox { Left = 120, Top = 20, Width = 150 };

            var lblQty = new Label { Text = "Quantity:", Left = 20, Top = 50 };
            var numQty = new NumericUpDown { Left = 120, Top = 50, Width = 150, Minimum = 0, Maximum = 1000 };

            var lblPrice = new Label { Text = "Price:", Left = 20, Top = 80 };
            var numPrice = new NumericUpDown { Left = 120, Top = 80, Width = 150, Minimum = 0, Maximum = 10000, DecimalPlaces = 2 };

            var lblCategory = new Label { Text = "Category:", Left = 20, Top = 110 };
            var cmbCategory = new ComboBox { Left = 120, Top = 110, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            // Populate categories
            foreach (var c in _inventoryManager.GetCategories())
                cmbCategory.Items.Add(c);
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;

            var lnkNewCategory = new LinkLabel { Text = "Add category", Left = 20, Top = 140, AutoSize = true };
            lnkNewCategory.Click += (s, ev) =>
            {
                using var addCat = new Form
                {
                    Text = "Add Category",
                    Size = new Size(300, 140),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false
                };
                var txtCat = new TextBox { Left = 20, Top = 20, Width = 240 };
                var btnOk = new Button { Text = "Add", Left = 20, Top = 50, DialogResult = DialogResult.OK };
                var btnCancelCat = new Button { Text = "Cancel", Left = 100, Top = 50, DialogResult = DialogResult.Cancel };
                addCat.Controls.AddRange(new Control[] { txtCat, btnOk, btnCancelCat });
                addCat.AcceptButton = btnOk; addCat.CancelButton = btnCancelCat;
                if (addCat.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtCat.Text))
                {
                    _inventoryManager.AddCategory(txtCat.Text.Trim());
                    cmbCategory.Items.Clear();
                    foreach (var c2 in _inventoryManager.GetCategories()) cmbCategory.Items.Add(c2);
                    cmbCategory.SelectedItem = txtCat.Text.Trim();
                }
            };

            var btnSave = new Button { Text = "Save", Left = 120, Top = 150, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 200, Top = 150, DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { lblName, txtName, lblQty, numQty, lblPrice, numPrice, lblCategory, cmbCategory, lnkNewCategory, btnSave, btnCancel });
            form.AcceptButton = btnSave;
            form.CancelButton = btnCancel;

            if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
            {
                var selectedCat = cmbCategory.SelectedItem?.ToString() ?? "General";
                _inventoryManager.AddProduct(txtName.Text.Trim(), (int)numQty.Value, numPrice.Value, selectedCat);
                LoadProducts();
                var main = this.FindForm() as InventoryApp.Forms.MainForm;
                main?.RefreshDashboardIfPresent();
            }
        }

    private void OnUpdateStock(object? sender, EventArgs e)
        {
            if (dgvProducts?.SelectedRows == null || dgvProducts.SelectedRows.Count == 0) return;

            var selectedRow = dgvProducts.SelectedRows[0];
            var idObj = selectedRow.Cells["Id"].Value;
            if (idObj == null) return;
            int id = Convert.ToInt32(idObj);
            var productsList = _inventoryManager.GetProducts();
            int productIndex = productsList.ToList().FindIndex(p => p.Id == id);

            using var form = new Form
            {
                Text = "Update Stock",
                Size = new Size(250, 150),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblQty = new Label { Text = "Sold Quantity:", Left = 20, Top = 20 };
            var numQty = new NumericUpDown { Left = 120, Top = 20, Width = 100, Minimum = 1, Maximum = 1000 };

            var btnSave = new Button { Text = "Save", Left = 70, Top = 70, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 150, Top = 70, DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { lblQty, numQty, btnSave, btnCancel });
            form.AcceptButton = btnSave;
            form.CancelButton = btnCancel;

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _inventoryManager.UpdateStock(productIndex, (int)numQty.Value);
                LoadProducts();
                var main = this.FindForm() as InventoryApp.Forms.MainForm;
                main?.RefreshDashboardIfPresent();
            }
        }

    private void OnRestock(object? sender, EventArgs e)
        {
            if (dgvProducts?.SelectedRows == null || dgvProducts.SelectedRows.Count == 0) return;

            var selectedRow = dgvProducts.SelectedRows[0];
            var idObj = selectedRow.Cells["Id"].Value;
            if (idObj == null) return;
            int id = Convert.ToInt32(idObj);
            var productsList = _inventoryManager.GetProducts();
            int productIndex = productsList.ToList().FindIndex(p => p.Id == id);

            using var form = new Form
            {
                Text = "Restock Product",
                Size = new Size(250, 150),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var lblQty = new Label { Text = "Add Quantity:", Left = 20, Top = 20 };
            var numQty = new NumericUpDown { Left = 120, Top = 20, Width = 100, Minimum = 1, Maximum = 1000 };

            var btnSave = new Button { Text = "Save", Left = 70, Top = 70, DialogResult = DialogResult.OK };
            var btnCancel = new Button { Text = "Cancel", Left = 150, Top = 70, DialogResult = DialogResult.Cancel };

            form.Controls.AddRange(new Control[] { lblQty, numQty, btnSave, btnCancel });
            form.AcceptButton = btnSave;
            form.CancelButton = btnCancel;

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _inventoryManager.RestockProduct(productIndex, (int)numQty.Value);
                LoadProducts();
                var main = this.FindForm() as InventoryApp.Forms.MainForm;
                main?.RefreshDashboardIfPresent();
            }
        }

        public void RefreshData()
        {
            LoadProducts();
        }
    }
}