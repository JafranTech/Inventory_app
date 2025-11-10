using System;
using System.Windows.Forms;
using System.Drawing;
using InventoryApp.Services;
using InventoryApp.Models;
using System.Globalization;
using Microsoft.VisualBasic.FileIO;

namespace InventoryApp.UserControls
{
    public partial class AddProductControl : UserControl
    {
        private readonly CsvDataService _dataService;
        private readonly TextBox txtName;
        private readonly TextBox txtCategory;
        private readonly NumericUpDown numQuantity;
        private readonly NumericUpDown numPrice;
        private readonly Button btnAdd;
        
        public AddProductControl()
        {
            _dataService = new CsvDataService();
            
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
            mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // Create form layout
            var formPanel = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 2,
                RowCount = 6,
                Width = 400,
                Anchor = AnchorStyles.Top | AnchorStyles.Left,
                Margin = new Padding(0, 20, 0, 0)
            };

            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
            formPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280F));

            // Add import button at the top
            var btnImport = new Button
            {
                Text = "📄 Import from CSV",
                Width = 150,
                Height = 35,
                BackColor = Color.FromArgb(0, 120, 212),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 0, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnImport.FlatAppearance.BorderSize = 0;
            btnImport.Click += BtnImport_Click;

            // Add form controls
            var labelStyle = new { Margin = new Padding(0, 8, 5, 8), TextAlign = ContentAlignment.MiddleLeft };
            var controlStyle = new { Margin = new Padding(0, 5, 0, 5), Height = 30 };

            // Product Name
            formPanel.Controls.Add(new Label { 
                Text = "Product Name:", 
                Margin = labelStyle.Margin, 
                TextAlign = labelStyle.TextAlign 
            }, 0, 0);
            txtName = new TextBox { 
                Margin = controlStyle.Margin,
                Height = controlStyle.Height
            };
            formPanel.Controls.Add(txtName, 1, 0);

            // Category
            formPanel.Controls.Add(new Label { 
                Text = "Category:", 
                Margin = labelStyle.Margin, 
                TextAlign = labelStyle.TextAlign 
            }, 0, 1);
            txtCategory = new TextBox { 
                Margin = controlStyle.Margin,
                Height = controlStyle.Height
            };
            formPanel.Controls.Add(txtCategory, 1, 1);

            // Quantity
            formPanel.Controls.Add(new Label { 
                Text = "Quantity:", 
                Margin = labelStyle.Margin, 
                TextAlign = labelStyle.TextAlign 
            }, 0, 2);
            numQuantity = new NumericUpDown { 
                Minimum = 0, 
                Maximum = 1000,
                Margin = controlStyle.Margin,
                Height = controlStyle.Height
            };
            formPanel.Controls.Add(numQuantity, 1, 2);

            // Price
            formPanel.Controls.Add(new Label { 
                Text = "Price:", 
                Margin = labelStyle.Margin, 
                TextAlign = labelStyle.TextAlign 
            }, 0, 3);
            numPrice = new NumericUpDown { 
                Minimum = 0, 
                Maximum = 10000, 
                DecimalPlaces = 2,
                Margin = controlStyle.Margin,
                Height = controlStyle.Height
            };
            formPanel.Controls.Add(numPrice, 1, 3);

            // Add button
            btnAdd = new Button
            {
                Text = "Add Product",
                Width = 120,
                Height = 35,
                BackColor = Color.FromArgb(51, 51, 76),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 15, 0, 0)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            var buttonPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Right
            };
            buttonPanel.Controls.Add(btnAdd);
            formPanel.Controls.Add(buttonPanel, 1, 4);

            // Add panels to main layout
            mainPanel.Controls.Add(btnImport);
            mainPanel.Controls.Add(formPanel);
            this.Controls.Add(mainPanel);
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Prefer adding through MainForm's InventoryManager so in-memory state stays synced
            var main = this.FindForm() as InventoryApp.Forms.MainForm;
            if (main != null)
            {
                main.AddProductViaManager(txtName.Text.Trim(), (int)numQuantity.Value, numPrice.Value, txtCategory.Text.Trim());
                MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var products = _dataService.LoadProducts();
                var newId = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;

                var product = new Product(
                    newId,
                    txtName.Text.Trim(),
                    txtCategory.Text.Trim(),
                    (int)numQuantity.Value,
                    numPrice.Value
                );

                products.Add(product);
                _dataService.SaveProducts(products);

                MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
            // Clear inputs
            txtName.Clear();
            txtCategory.Clear();
            numQuantity.Value = 0;
            numPrice.Value = 0;
            txtName.Focus();
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv",
                Title = "Select Products CSV File"
            };

            if (dialog.ShowDialog() != DialogResult.OK) return;

            var file = dialog.FileName;
            var main = this.FindForm() as InventoryApp.Forms.MainForm;
            int success = 0, errors = 0;
            var errorSamples = new List<string>();

            try
            {
                using (var parser = new TextFieldParser(file))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;

                    if (parser.EndOfData)
                    {
                        MessageBox.Show("The CSV file is empty.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string[]? headers = parser.ReadFields();
                    if (headers == null)
                    {
                        MessageBox.Show("Invalid CSV header.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Map header indices (case-insensitive)
                    int idxName = -1, idxCategory = -1, idxQty = -1, idxPrice = -1;
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var h = headers[i].Trim().ToLowerInvariant();
                        if (h.Contains("name") || h.Contains("product") || h.Contains("title")) idxName = i;
                        else if (h.Contains("category") || h.Contains("cat")) idxCategory = i;
                        else if (h.Contains("quantity") || h.Contains("qty") || h.Contains("amount")) idxQty = i;
                        else if (h.Contains("price") || h.Contains("cost")) idxPrice = i;
                    }

                    if (idxName == -1 || idxQty == -1 || idxPrice == -1)
                    {
                        MessageBox.Show("CSV must contain at least Name, Quantity and Price columns (header names can vary).",
                            "Invalid CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    while (!parser.EndOfData)
                    {
                        try
                        {
                            var fields = parser.ReadFields();
                            if (fields == null) continue;

                            var name = idxName < fields.Length ? fields[idxName].Trim() : string.Empty;
                            var category = (idxCategory >= 0 && idxCategory < fields.Length) ? fields[idxCategory].Trim() : string.Empty;
                            var qtyStr = idxQty < fields.Length ? fields[idxQty].Trim() : string.Empty;
                            var priceStr = idxPrice < fields.Length ? fields[idxPrice].Trim() : string.Empty;

                            if (string.IsNullOrWhiteSpace(name))
                            {
                                errors++; if (errorSamples.Count < 5) errorSamples.Add("Empty name");
                                continue;
                            }

                            if (!int.TryParse(qtyStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int qty))
                            {
                                // Try current culture
                                int.TryParse(qtyStr, NumberStyles.Integer, CultureInfo.CurrentCulture, out qty);
                            }

                            if (!decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
                            {
                                decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.CurrentCulture, out price);
                            }

                            if (qty < 0 || price < 0)
                            {
                                errors++; if (errorSamples.Count < 5) errorSamples.Add($"Invalid qty/price for '{name}'");
                                continue;
                            }

                            // Add via main form manager if present, otherwise write directly
                            if (main != null)
                            {
                                main.AddProductViaManager(name, qty, price, category);
                            }
                            else
                            {
                                var products = _dataService.LoadProducts();
                                var newId = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
                                var product = new Product(newId, name, category, qty, price);
                                products.Add(product);
                                _dataService.SaveProducts(products);
                            }

                            success++;
                        }
                        catch (MalformedLineException)
                        {
                            errors++; if (errorSamples.Count < 5) errorSamples.Add("Malformed line");
                        }
                    }
                }

                var msg = $"Import complete:\n{success} products imported successfully";
                if (errors > 0)
                {
                    msg += $"\n{errors} products failed.";
                    if (errorSamples.Count > 0)
                    {
                        msg += "\nExamples: \n" + string.Join("; ", errorSamples);
                    }
                    MessageBox.Show(msg, "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(msg, "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing CSV: {ex.Message}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}