using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using InventoryApp.Services;
using InventoryApp.Models;

namespace InventoryApp.UserControls
{
    public partial class ViewProductsControl : UserControl
    {
        private readonly CsvDataService _dataService;
        private readonly DataGridView gridProducts;
        private readonly Button btnRefresh;
        private readonly Button btnSell;
        private readonly Button btnRestock;
        private Product? selectedProduct;
        
        public ViewProductsControl()
        {
            _dataService = new CsvDataService();
            
            // Initialize control
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;
            this.Padding = new Padding(20);

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,  // Increased to add button row
                ColumnCount = 1
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var topButtonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Add refresh button
            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Width = 100,
                Height = 35,
                BackColor = Color.FromArgb(51, 51, 76),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5, 0, 0, 0)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadProducts();

            // Add download button
            var btnDownload = new Button
            {
                Text = "📥 Download CSV",
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Click += btnDownload_Click;

            topButtonPanel.Controls.Add(btnRefresh);
            topButtonPanel.Controls.Add(btnDownload);
            panel.Controls.Add(topButtonPanel);

            // Create grid
            gridProducts = new DataGridView
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

            gridProducts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(51, 51, 76);
            gridProducts.DefaultCellStyle.SelectionForeColor = Color.White;

            panel.Controls.Add(gridProducts);

            // Add action buttons panel at the bottom
            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                Margin = new Padding(0, 10, 0, 0)
            };

            // Add sell button
            btnSell = new Button
            {
                Text = "📉 Sell Product",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(220, 53, 69), // Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnSell.FlatAppearance.BorderSize = 0;
            btnSell.Click += BtnSell_Click;

            // Add restock button
            btnRestock = new Button
            {
                Text = "📦 Restock Product",
                Width = 150,
                Height = 40,
                BackColor = Color.FromArgb(40, 167, 69), // Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnRestock.FlatAppearance.BorderSize = 0;
            btnRestock.Click += BtnRestock_Click;

            bottomPanel.Controls.Add(btnSell);
            bottomPanel.Controls.Add(btnRestock);
            panel.Controls.Add(bottomPanel);

            this.Controls.Add(panel);

            // Wire up grid selection changed event
            gridProducts.SelectionChanged += GridProducts_SelectionChanged;

            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = _dataService.LoadProducts();
            gridProducts.DataSource = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Category,
                p.Quantity,
                p.Price,
                Status = p.IsLowStock ? "Low Stock" : "OK"
            }).ToList();

            if (gridProducts.Columns["Status"] != null)
            {
                gridProducts.Columns["Status"].DefaultCellStyle.ForeColor = Color.Red;
            }
        }

        private void btnDownload_Click(object? sender, EventArgs e)
        {
            var productsFile = _dataService.GetProductsFilePath();
            if (!File.Exists(productsFile))
            {
                MessageBox.Show("No CSV data found yet.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                FileName = "products.csv",
                Title = "Save Products CSV"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(productsFile, dialog.FileName, true);
                    MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GridProducts_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridProducts.SelectedRows.Count > 0)
            {
                var row = gridProducts.SelectedRows[0];
                var productId = (int)row.Cells["Id"].Value;
                selectedProduct = _dataService.LoadProducts().FirstOrDefault(p => p.Id == productId);
                btnSell.Enabled = btnRestock.Enabled = (selectedProduct != null);

                // Show low stock warning if needed
                if (selectedProduct?.Quantity < 10)
                {
                    MessageBox.Show($"Warning: {selectedProduct.Name} has low stock!\nOnly {selectedProduct.Quantity} units remaining.", 
                        "Low Stock Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                selectedProduct = null;
                btnSell.Enabled = btnRestock.Enabled = false;
            }
        }

        private void BtnSell_Click(object? sender, EventArgs e)
        {
            if (selectedProduct == null) return;

            using var form = new Form
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Sell Product",
                StartPosition = FormStartPosition.CenterParent
            };

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(15),
                Height = 120
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblQuantity = new Label
            {
                Text = $"Quantity to sell (max {selectedProduct.Quantity}):",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };

            var numQuantity = new NumericUpDown
            {
                Width = 260,
                Minimum = 1,
                Maximum = selectedProduct.Quantity,
                Value = 1,
                Height = 25
            };

            var buttonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Margin = new Padding(0, 15, 0, 0)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Width = 80,
                Height = 30,
                Margin = new Padding(0, 0, 10, 0)
            };

            var btnOk = new Button
            {
                Text = "Sell",
                DialogResult = DialogResult.OK,
                Width = 80,
                Height = 30,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnOk);

            panel.Controls.Add(lblQuantity);
            panel.Controls.Add(numQuantity);
            panel.Controls.Add(buttonPanel);
            form.Controls.Add(panel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                var qty = (int)numQuantity.Value;
                var products = _dataService.LoadProducts();
                var product = products.First(p => p.Id == selectedProduct.Id);
                product.Quantity -= qty;
                _dataService.SaveProducts(products);
                LoadProducts();

                MessageBox.Show($"Sold {qty} units of {product.Name}.", "Sale Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                if (product.Quantity < 10)
                {
                    MessageBox.Show($"Warning: {product.Name} is now low on stock!\nOnly {product.Quantity} units remaining.", 
                        "Low Stock Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnRestock_Click(object? sender, EventArgs e)
        {
            if (selectedProduct == null) return;

            using var form = new Form
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Restock Product",
                StartPosition = FormStartPosition.CenterParent
            };

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(15),
                Height = 120
            };
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var lblQuantity = new Label
            {
                Text = "Quantity to add:",
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 5)
            };

            var numQuantity = new NumericUpDown
            {
                Width = 260,
                Minimum = 1,
                Maximum = 1000,
                Value = 10,
                Height = 25
            };

            var buttonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Margin = new Padding(0, 15, 0, 0)
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Width = 80,
                Height = 30,
                Margin = new Padding(0, 0, 10, 0)
            };

            var btnOk = new Button
            {
                Text = "Restock",
                DialogResult = DialogResult.OK,
                Width = 80,
                Height = 30,
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOk.FlatAppearance.BorderSize = 0;

            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(btnOk);

            panel.Controls.Add(lblQuantity);
            panel.Controls.Add(numQuantity);
            panel.Controls.Add(buttonPanel);
            form.Controls.Add(panel);
            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                var qty = (int)numQuantity.Value;
                var products = _dataService.LoadProducts();
                var product = products.First(p => p.Id == selectedProduct.Id);
                product.Quantity += qty;
                _dataService.SaveProducts(products);
                LoadProducts();

                MessageBox.Show($"Added {qty} units to {product.Name}.", "Restock Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}