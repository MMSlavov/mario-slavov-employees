using SirmaTask.Services;

namespace SirmaTask
{
    public partial class MainForm : Form
    {
        private Button btnSelectFile;
        private DataGridView dataGridView;
        private Label lblStatus;
        private EmployeeProjectService service;

        public MainForm()
        {
            InitializeComponent();
            service = new EmployeeProjectService();
        }

        private void InitializeComponent()
        {
            this.Text = "Employee Project Analyzer";
            this.Size = new System.Drawing.Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            btnSelectFile = new Button
            {
                Text = "Select CSV File",
                Location = new System.Drawing.Point(20, 20),
                Size = new System.Drawing.Size(150, 40),
                Font = new System.Drawing.Font("Segoe UI", 10F)
            };
            btnSelectFile.Click += BtnSelectFile_Click;

            lblStatus = new Label
            {
                Text = "Please select a CSV file to analyze...",
                Location = new System.Drawing.Point(190, 20),
                Size = new System.Drawing.Size(580, 40),
                Font = new System.Drawing.Font("Segoe UI", 9F),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            dataGridView = new DataGridView
            {
                Location = new System.Drawing.Point(20, 80),
                Size = new System.Drawing.Size(740, 460),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmpID1",
                HeaderText = "Employee ID #1",
                DataPropertyName = "EmpID1"
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EmpID2",
                HeaderText = "Employee ID #2",
                DataPropertyName = "EmpID2"
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ProjectID",
                HeaderText = "Project ID",
                DataPropertyName = "ProjectID"
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DaysWorked",
                HeaderText = "Days Worked",
                DataPropertyName = "DaysWorked"
            });

            this.Controls.Add(btnSelectFile);
            this.Controls.Add(lblStatus);
            this.Controls.Add(dataGridView);
        }

        private void BtnSelectFile_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Select a CSV File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        lblStatus.Text = $"Processing file: {Path.GetFileName(openFileDialog.FileName)}...";
                        lblStatus.ForeColor = System.Drawing.Color.Black;
                        Application.DoEvents();

                        var results = service.GetTopEmployeePairs(openFileDialog.FileName);

                        dataGridView.DataSource = results;

                        if (results.Any())
                        {
                            lblStatus.Text = $"File processed successfully.";
                            lblStatus.ForeColor = System.Drawing.Color.Green;
                        }
                        else
                        {
                            lblStatus.Text = "No employee pairs found who worked together on projects.";
                            lblStatus.ForeColor = System.Drawing.Color.Orange;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblStatus.Text = $"Error: {ex.Message}";
                        lblStatus.ForeColor = System.Drawing.Color.Red;
                        MessageBox.Show($"Error processing file: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
