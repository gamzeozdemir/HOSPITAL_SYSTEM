using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CsvHelper;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Globalization;

namespace VThastane
{
    public partial class Diagnosis : Form
    {
        public Diagnosis()
        {
            InitializeComponent();
            DisplayDiagnosis();
        }
        readonly SqlConnection Con = new SqlConnection(@"Server=DESKTOP-VHCFNJ7\SQLEXPRESS;Database=VThastane;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        private void DisplayDiagnosis()
        {
            try
            {
                Con.Open();
                string query = "SELECT * FROM Diagnosis";
                SqlDataAdapter sda = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
        }

       private void DisplayPatientId()
       {

            try
            {
                Con.Open();
                string query = "SELECT PId FROM Patient";
                SqlDataAdapter sda = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                comboBox1.ValueMember = "PId";
                comboBox1.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
       }
       
        private void Diagnosis_Load(object sender, EventArgs e)
        {
            DisplayDiagnosis();
            DisplayPatientId();
            DisplayDoctorId();
          
        }

        string pname;

        private void DisplayDoctorId()
        {
            try
            {
                Con.Open();
                string query = "SELECT DocId FROM Doctor";
                SqlDataAdapter sda = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                comboBox2.ValueMember = "DocId";
                comboBox2.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
        }

        private void DisplayPatientName()
        {
            try
            {
                Con.Open();
                string ss = "SELECT * FROM Patient WHERE PId=" + comboBox1.SelectedValue.ToString();
                SqlCommand cmd = new SqlCommand(ss, Con);
                DataTable dt = new DataTable();
                SqlDataAdapter ada = new SqlDataAdapter(cmd);
                ada.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    textBox1.Text = dr["PName"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Con.Close();
            }
        }
        

       
        private void button5_Click(object sender, EventArgs e)  //HOME
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }

       


        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Text = dataGridView1.SelectedRows[0].Cells["PatientId"].Value.ToString();
                comboBox2.Text = dataGridView1.SelectedRows[0].Cells["DoctorId"].Value.ToString();
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["PatientName"].Value.ToString();
                symptoms.Text = dataGridView1.SelectedRows[0].Cells["Symptoms"].Value.ToString();
                textBox3.Text = dataGridView1.SelectedRows[0].Cells["DiagnosticTest"].Value.ToString();
                textBox4.Text = dataGridView1.SelectedRows[0].Cells["Medicines"].Value.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Con.Close();
            }

        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DisplayPatientName();
        }

       

        

        private void button1_Click(object sender, EventArgs e)
        {
            DisplayDiagnosis();
        }

        // Diagnosis_Log tablosunu görüntüleyen fonksiyon
        private void DisplayDiagnosisLog()
        {
            try
            {
                Con.Open();
                string query = "SELECT * FROM Diagnosis_Log";  // Diagnosis_Log tablosunun tamamını seç
                SqlDataAdapter sda = new SqlDataAdapter(query, Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                dataGridView1.DataSource = dt;  // DataGridView kontrolüne veriyi bağla
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
        }

   
      

       

        private void button3_Click(object sender, EventArgs e)
        {
            string searchSymptoms = search.Text.Trim();

            if (string.IsNullOrWhiteSpace(searchSymptoms))
            {
                MessageBox.Show("Lütfen bir semptom girin.");
                return;
            }

            try
            {
                if (Con.State != ConnectionState.Open)
                {
                    Con.Open();
                }

                // Stored Procedure'ü çağırıyoruz
                SqlCommand cmd = new SqlCommand("SearchDiagnosisBySymptoms", Con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Parametreyi ekliyoruz
                cmd.Parameters.AddWithValue("@Symptoms", searchSymptoms);

                // Verileri almak için DataAdapter kullanıyoruz
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                // Sonuçları DataGridView'a yükle
                dataGridView1.DataSource = dt;

                // Kullanıcıya bilgi ver
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Eşleşen semptom bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void X_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Document doc = new Document();
                    PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    doc.Open();

                    // Adding a title to the PDF
                    doc.Add(new Paragraph("Diagnosis Report"));
                    doc.Add(new Paragraph("Generated on: " + DateTime.Now.ToString()));
                    doc.Add(new Paragraph("\n"));

                    // Create a table with the same number of columns as in DataGridView
                    PdfPTable table = new PdfPTable(dataGridView1.Columns.Count);

                    // Add headers
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        table.AddCell(column.HeaderText);
                    }

                    // Add rows
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            table.AddCell(cell.Value.ToString());
                        }
                    }

                    doc.Add(table);
                    doc.Close();
                    MessageBox.Show("Data exported to PDF successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}");
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DisplayDiagnosis();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DisplayDiagnosisLog();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }

        private void AddBtn_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(comboBox1.Text) || string.IsNullOrWhiteSpace(comboBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(symptoms.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Eksik bilgi! Lütfen tüm alanları doldurun.");
                    return;
                }

                Con.Open();
                string query = "INSERT INTO Diagnosis (PatientId, DoctorId, PatientName, Symptoms, DiagnosticTest, Medicines) " +
                               "VALUES (@PatientId, @DoctorId, @PatientName, @Symptoms, @DiagnosticTest, @Medicines)";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@PatientId", comboBox1.Text);
                cmd.Parameters.AddWithValue("@DoctorId", comboBox2.Text);
                cmd.Parameters.AddWithValue("@PatientName", textBox1.Text);
                cmd.Parameters.AddWithValue("@Symptoms", symptoms.Text);
                cmd.Parameters.AddWithValue("@DiagnosticTest", textBox3.Text);
                cmd.Parameters.AddWithValue("@Medicines", textBox4.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Kayıt başarıyla eklendi.");
                DisplayDiagnosis();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
            DisplayDiagnosis();
        }

        private void update_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen güncellenecek kaydı seçin.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(comboBox1.Text) || string.IsNullOrWhiteSpace(comboBox2.Text) ||
                    string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(symptoms.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) || string.IsNullOrWhiteSpace(textBox4.Text))
                {
                    MessageBox.Show("Eksik bilgi! Lütfen tüm alanları doldurun.");
                    return;
                }

                int selectedDId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DId"].Value);
                Con.Open();
                string query = "UPDATE Diagnosis SET PatientId = @PatientId, DoctorId = @DoctorId, PatientName = @PatientName, " +
                               "Symptoms = @Symptoms, DiagnosticTest = @DiagnosticTest, Medicines = @Medicines WHERE DId = @DId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@PatientId", comboBox1.Text);
                cmd.Parameters.AddWithValue("@DoctorId", comboBox2.Text);
                cmd.Parameters.AddWithValue("@PatientName", textBox1.Text);
                cmd.Parameters.AddWithValue("@Symptoms", symptoms.Text);
                cmd.Parameters.AddWithValue("@DiagnosticTest", textBox3.Text);
                cmd.Parameters.AddWithValue("@Medicines", textBox4.Text);
                cmd.Parameters.AddWithValue("@DId", selectedDId);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Kayıt başarıyla güncellendi.");
                DisplayDiagnosis();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
            DisplayDiagnosis();
        }

        private void DelBtn_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silinecek kaydı seçin.");
                    return;
                }

                // Silinecek DId'yi alıyoruz           
                int selectedDId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DId"].Value);

                // Silme işlemi
                Con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Diagnosis WHERE DId = @DId", Con);
                cmd.Parameters.AddWithValue("@DId", selectedDId);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Kayıt başarıyla silindi ve log kaydına işlendi.");
                DisplayDiagnosis(); // Güncel listeyi göster       
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }

            DisplayDiagnosis();
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {

            textBox1.Text = " ";
            symptoms.Text = " ";
            comboBox1.Text = " ";
            textBox3.Text = " ";
            textBox4.Text = " ";
        }

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {
            try
            {
                // Seçilen satırda verilerin olup olmadığını kontrol ediyoruz
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Seçilen satırdaki verileri alıyoruz
                    comboBox1.Text = dataGridView1.SelectedRows[0].Cells["PatientId"].Value.ToString();   // PatientId
                    comboBox2.Text = dataGridView1.SelectedRows[0].Cells["DoctorId"].Value.ToString();

                    textBox1.Text = dataGridView1.SelectedRows[0].Cells["PatientName"].Value.ToString(); // PatientName
                    symptoms.Text = dataGridView1.SelectedRows[0].Cells["Symptoms"].Value.ToString();    // Symptoms
                    textBox3.Text = dataGridView1.SelectedRows[0].Cells["DiagnosticTest"].Value.ToString(); // DiagnosticTest
                    textBox4.Text = dataGridView1.SelectedRows[0].Cells["Medicines"].Value.ToString();   // Medicines
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj gösteriyoruz
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void import_Click_1(object sender, EventArgs e)
        {

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                    using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        var records = csv.GetRecords<dynamic>().ToList();
                        DataTable dt = new DataTable();
                        // Assuming the first row contains the header
                        foreach (var header in records.First().Keys)
                        {
                            dt.Columns.Add(header);
                        }

                        foreach (var record in records)
                        {
                            DataRow row = dt.NewRow();
                            foreach (var column in record)
                            {
                                row[column.Key] = column.Value;
                            }
                            dt.Rows.Add(row);
                        }

                        dataGridView1.DataSource = dt;
                    }
                    MessageBox.Show("Data imported from CSV successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing from CSV: {ex.Message}");
            }
        }

        private void export_Click_1(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Document doc = new Document();
                    PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));
                    doc.Open();

                    // Adding a title to the PDF
                    doc.Add(new Paragraph("Diagnosis Report"));
                    doc.Add(new Paragraph("Generated on: " + DateTime.Now.ToString()));
                    doc.Add(new Paragraph("\n"));

                    // Create a table with the same number of columns as in DataGridView
                    PdfPTable table = new PdfPTable(dataGridView1.Columns.Count);

                    // Add headers
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        table.AddCell(column.HeaderText);
                    }

                    // Add rows
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            table.AddCell(cell.Value.ToString());
                        }
                    }

                    doc.Add(table);
                    doc.Close();
                    MessageBox.Show("Data exported to PDF successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(comboBox1.Text))
        //        {
        //            // Veritabanı bağlantısını aç
        //            Con.Open();

        //            // PatientId'ye göre hasta adını al
        //            string query = "SELECT PatientName FROM Patient WHERE PatientID = @PatientID";
        //            SqlCommand cmd = new SqlCommand(query, Con);
        //            cmd.Parameters.AddWithValue("@PatientID", comboBox1.Text);

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            // Hasta adını ilgili TextBox'a yaz
        //            if (reader.Read())
        //            {
        //                textBox1.Text = reader["PatientName"].ToString();
        //            }
        //            else
        //            {
        //                textBox1.Text = ""; // Eğer bir eşleşme yoksa boş bırak
        //            }

        //            reader.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Hata: {ex.Message}");
        //    }
        //    finally
        //    {
        //        // Bağlantıyı kapat
        //        Con.Close();
        //    }
        //}
    }
}
