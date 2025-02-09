using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using iTextSharp;
using iTextSharp.text.pdf;
using System.IO;
using CsvHelper;

namespace VThastane
{
    public partial class Patient : Form
    {
        private int selectedPId;
        public Patient()
        {
            InitializeComponent();
            DisplayPatient();
        }
        readonly SqlConnection Con = new SqlConnection(@"Server=DESKTOP-VHCFNJ7\SQLEXPRESS;Database=VThastane;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        private void DisplayPatient()
        {
            try
            {
                Con.Open();
                string query = "SELECT * FROM Patient";
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
        private void Patient_Load(object sender, EventArgs e)
        {
            DisplayPatient();
            LoadDoctors();
        }



        private void LoadDoctors()
        {
            try
            {
                Con.Open();
                string query = "SELECT DocID FROM Doctor"; // Doctor tablonuzdaki ID'ler
                SqlCommand cmd = new SqlCommand(query, Con);
                SqlDataReader reader = cmd.ExecuteReader();

                // ComboBox'a her DoctorID'yi ekliyoruz
                while (reader.Read())
                {
                    comboBoxDoctorID.Items.Add(reader["DocID"].ToString());
                }
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



      
        private void button6_Click(object sender, EventArgs e) //çıkış
        {
            this.Close();
        }



      
        private void button5_Click(object sender, EventArgs e)
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }



        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                // Seçilen satırın boş olmadığını kontrol ediyoruz
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Verileri alıp ilgili TextBox ve ComboBox'a yerleştiriyoruz
                    Name.Text = dataGridView1.SelectedRows[0].Cells["PName"].Value.ToString();  // Hasta Adı
                    textBoxAddress.Text = dataGridView1.SelectedRows[0].Cells["PAddress"].Value.ToString();  // Adres
                    BirthDate.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["PBirthDate"].Value);  // Doğum Tarihi
                    textBoxPhone.Text = dataGridView1.SelectedRows[0].Cells["PPhone"].Value.ToString();  // Telefon
                    comboBoxGender.Text = dataGridView1.SelectedRows[0].Cells["PGen"].Value.ToString();  // Cinsiyet
                    comboBoxBloodGroup.Text = dataGridView1.SelectedRows[0].Cells["BloodGroup"].Value.ToString();  // Kan Grubu
                    textBoxDisease.Text = dataGridView1.SelectedRows[0].Cells["MajorDisease"].Value.ToString();  // Hastalık
                    textBoxEmail.Text = dataGridView1.SelectedRows[0].Cells["Email"].Value.ToString();  // Email
                    textBoxNationalID.Text = dataGridView1.SelectedRows[0].Cells["NationalID"].Value.ToString();  // TCKimlikNo
                    comboBoxDoctorID.Text = dataGridView1.SelectedRows[0].Cells["AssignedDoctorID"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();  // Bağlantıyı kapatıyoruz
            }
        }

      


       

        

        private void button6_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

       

     
        public void ExportToCsv(DataGridView dataGridView, string filePath)
        {
            try
            {
                StringBuilder csvContent = new StringBuilder();

                // Sütun başlıklarını yaz
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    csvContent.Append(dataGridView.Columns[i].HeaderText);
                    if (i < dataGridView.Columns.Count - 1)
                        csvContent.Append(",");
                }
                csvContent.AppendLine();

                // Satır verilerini yaz
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        for (int i = 0; i < dataGridView.Columns.Count; i++)
                        {
                            csvContent.Append(row.Cells[i].Value?.ToString().Replace(",", " "));
                            if (i < dataGridView.Columns.Count - 1)
                                csvContent.Append(",");
                        }
                        csvContent.AppendLine();
                    }
                }

                // CSV'yi dosyaya yaz
                File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
                MessageBox.Show("Veriler başarıyla dışa aktarıldı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void X_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }

      

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Eksik bilgi kontrolü
                if (string.IsNullOrWhiteSpace(Name.Text) ||
                    string.IsNullOrWhiteSpace(textBoxAddress.Text) ||
                    string.IsNullOrWhiteSpace(textBoxPhone.Text) ||

                    string.IsNullOrWhiteSpace(comboBoxGender.Text) ||
                    string.IsNullOrWhiteSpace(comboBoxBloodGroup.Text) ||
                    string.IsNullOrWhiteSpace(textBoxDisease.Text) ||
                    string.IsNullOrWhiteSpace(textBoxEmail.Text) ||
                    string.IsNullOrWhiteSpace(textBoxNationalID.Text))
                {
                    MessageBox.Show("Lütfen tüm alanları doldurun.");
                    return;
                }

                Con.Open();
                string query = @"
            INSERT INTO Patient 
            (PName, PAddress, PBirthDate, PPhone, PGen, BloodGroup, MajorDisease, Email, NationalID, AssignedDoctorID)
            VALUES 
            (@PName, @PAddress, @PBirthDate, @PPhone, @PGen, @BloodGroup, @MajorDisease, @Email, @NationalID, @AssignedDoctorID)";

                SqlCommand cmd = new SqlCommand(query, Con);

                // Parametreler ekleniyor
                cmd.Parameters.AddWithValue("@PName", Name.Text);  // Hasta adı
                cmd.Parameters.AddWithValue("@PAddress", textBoxAddress.Text);  // Adres
                cmd.Parameters.AddWithValue("@PBirthDate", BirthDate.Value.Date);  // Doğum tarihi


                cmd.Parameters.AddWithValue("@PPhone", textBoxPhone.Text);  // Telefon
                cmd.Parameters.AddWithValue("@PGen", comboBoxGender.Text);  // Cinsiyet
                cmd.Parameters.AddWithValue("@BloodGroup", comboBoxBloodGroup.Text);  // Kan grubu
                cmd.Parameters.AddWithValue("@MajorDisease", textBoxDisease.Text);  // Hastalık
                cmd.Parameters.AddWithValue("@Email", textBoxEmail.Text);  // Email
                cmd.Parameters.AddWithValue("@NationalID", textBoxNationalID.Text);  // TCKimlikNo

                // Doktor atanmışsa null kontrolü
                if (string.IsNullOrWhiteSpace(comboBoxDoctorID.Text))
                    cmd.Parameters.AddWithValue("@AssignedDoctorID", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@AssignedDoctorID", Convert.ToInt32(comboBoxDoctorID.Text));

                cmd.ExecuteNonQuery();
                MessageBox.Show("Hasta başarıyla eklendi.");
                DisplayPatient();  // Listeyi güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
            DisplayPatient();  // Listeyi güncelle
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen güncellemek istediğiniz hastayı seçin.");
                    return;
                }

                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PId"].Value);

                Con.Open();
                string query = @"
            UPDATE Patient 
            SET 
                PName = @PName, 
                PAddress = @PAddress, 
                PBirthDate = @PBirthDate, 
                PPhone = @PPhone, 
                PGen = @PGen, 
                BloodGroup = @BloodGroup, 
                MajorDisease = @MajorDisease, 
                Email = @Email, 
                NationalID = @NationalID, 
                AssignedDoctorID = @AssignedDoctorID
            WHERE PId = @PId";

                SqlCommand cmd = new SqlCommand(query, Con);

                // Parametreler ekleniyor
                cmd.Parameters.AddWithValue("@PName", Name.Text);
                cmd.Parameters.AddWithValue("@PAddress", textBoxAddress.Text);
                cmd.Parameters.AddWithValue("@PBirthDate", BirthDate.Value.Date);
                cmd.Parameters.AddWithValue("@PPhone", textBoxPhone.Text);
                cmd.Parameters.AddWithValue("@PGen", comboBoxGender.Text);
                cmd.Parameters.AddWithValue("@BloodGroup", comboBoxBloodGroup.Text);
                cmd.Parameters.AddWithValue("@MajorDisease", textBoxDisease.Text);
                cmd.Parameters.AddWithValue("@Email", textBoxEmail.Text);
                cmd.Parameters.AddWithValue("@NationalID", textBoxNationalID.Text);

                if (string.IsNullOrWhiteSpace(comboBoxDoctorID.Text))
                    cmd.Parameters.AddWithValue("@AssignedDoctorID", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@AssignedDoctorID", Convert.ToInt32(comboBoxDoctorID.Text));

                cmd.Parameters.AddWithValue("@PId", selectedId);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Hasta bilgileri başarıyla güncellendi.");
                DisplayPatient();  // Listeyi güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
            DisplayPatient();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silinecek kaydı seçin.");
                    return;
                }

                // Seçilen satırdan PId değerini alıyoruz
                int selectedPId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PId"].Value);

                Con.Open();

                // Bu sorgu çalıştırıldığında Trigger tetiklenecek
                string query = "DELETE FROM Patient WHERE PId = @PId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@PId", selectedPId);

                cmd.ExecuteNonQuery();  // Trigger'ı tetikler

                MessageBox.Show("Kayıt başarıyla silindi.");
                DisplayPatient();  // Listeyi güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
            DisplayPatient();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Name.Text = " ";
            textBoxAddress.Text = " ";
            comboBoxGender.Text = " ";
            textBoxAddress.Clear();
            textBoxPhone.Text = " ";
            textBoxDisease.Text = " ";
            comboBoxBloodGroup.Text = " ";
            textBoxEmail.Clear();
            textBoxNationalID.Clear();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Kaydetme yeri belirleme
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files|*.pdf",
                    Title = "Hasta Verilerini PDF Olarak Kaydet"
                };

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    // PDF dokümanı oluşturma
                    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        iTextSharp.text.Document document = new iTextSharp.text.Document();
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        document.Open();

                        // Başlık ekleme
                        document.Add(new iTextSharp.text.Paragraph("Hasta Listesi"));
                        document.Add(new iTextSharp.text.Paragraph("\n"));

                        // Tablo oluşturma
                        PdfPTable pdfTable = new PdfPTable(dataGridView1.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        // Tablo başlıklarını ekleme
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            pdfTable.AddCell(new iTextSharp.text.Phrase(column.HeaderText));
                        }

                        // Tablo satırlarını ekleme
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    pdfTable.AddCell(cell.Value?.ToString() ?? string.Empty);
                                }
                            }
                        }

                        document.Add(pdfTable);
                        document.Close();
                    }

                    MessageBox.Show("PDF başarıyla kaydedildi.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void import_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV Files (.csv)|*.csv|All Files (.)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    DataTable dt = new DataTable();

                    // CSV dosyasını okuma
                    try
                    {
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            string[] headers = sr.ReadLine().Split(',');
                            foreach (string header in headers)
                            {
                                dt.Columns.Add(header); // DataTable'a sütunları ekle
                            }

                            while (!sr.EndOfStream)
                            {
                                string[] rows = sr.ReadLine().Split(',');
                                dt.Rows.Add(rows); // Her satırı DataTable'a ekle
                            }
                        }

                        // Veritabanına ekleme
                        Con.Open();
                        foreach (DataRow row in dt.Rows)
                        {
                            string query = "INSERT INTO Doctor (DocName, DocGen, Experience, License, Specialty, Phone, Email, NationalID) " +
                                           "VALUES (@DocName, @DocGen, @Experience, @License, @Specialty, @Phone, @Email, @NationalID)";
                            SqlCommand cmd = new SqlCommand(query, Con);

                            cmd.Parameters.AddWithValue("@DocName", row["DocName"]);
                            cmd.Parameters.AddWithValue("@DocGen", row["DocGen"]);
                            cmd.Parameters.AddWithValue("@Experience", row["Experience"]);
                            cmd.Parameters.AddWithValue("@License", row["License"]);
                            cmd.Parameters.AddWithValue("@Specialty", row["Specialty"]);
                            cmd.Parameters.AddWithValue("@Phone", row["Phone"]);
                            cmd.Parameters.AddWithValue("@Email", row["Email"]);
                            cmd.Parameters.AddWithValue("@NationalID", row["NationalID"]);

                            cmd.ExecuteNonQuery(); // Sorguyu çalıştır
                        }
                        MessageBox.Show("CSV başarıyla içe aktarıldı ve veritabanına eklendi.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Hata: {ex.Message}");
                    }
                    finally
                    {
                        Con.Close();
                    }

                    // DataGridView'e veriyi yükle
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void export_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV Dosyaları (*.csv)|*.csv",
                Title = "CSV Dosyasını Kaydet",
                FileName = "Doktorlar.csv"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExportToCsv(dataGridView1, saveFileDialog.FileName);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcıdan alınan hasta adı
                string patientName = search.Text.Trim();

                if (string.IsNullOrEmpty(patientName))
                {
                    MessageBox.Show("Lütfen aramak istediğiniz hasta adını giriniz.");
                    return;
                }

                // Veritabanı bağlantısını açıyoruz
                if (Con.State != ConnectionState.Open)
                {
                    Con.Open();
                }

                // Stored Procedure çağrısı
                SqlCommand cmd = new SqlCommand("SearchPatientByName", Con)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Prosedüre parametre ekleme
                cmd.Parameters.AddWithValue("@PName", patientName);

                // Sonuçları almak için DataAdapter kullanımı
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // DataGridView'e sonuçları doldurma
                dataGridView1.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Hastaya ait veri bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        
        }

        private void dataGridView1_SelectionChanged_1(object sender, EventArgs e)
        {

            try
            {
                // Seçilen satırda verilerin olup olmadığını kontrol ediyoruz
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Seçilen satırdaki verileri alıyoruz
                    selectedPId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["PId"].Value);  // PId
                    Name.Text = dataGridView1.SelectedRows[0].Cells["PName"].Value.ToString();  // Hasta Adı
                    textBoxAddress.Text = dataGridView1.SelectedRows[0].Cells["PAddress"].Value.ToString();  // Adres
                    BirthDate.Value = Convert.ToDateTime(dataGridView1.SelectedRows[0].Cells["PBirthDate"].Value);  // Doğum Tarihi
                    textBoxPhone.Text = dataGridView1.SelectedRows[0].Cells["PPhone"].Value.ToString();  // Telefon
                    comboBoxGender.Text = dataGridView1.SelectedRows[0].Cells["PGen"].Value.ToString();  // Cinsiyet
                    comboBoxBloodGroup.Text = dataGridView1.SelectedRows[0].Cells["BloodGroup"].Value.ToString();  // Kan Grubu
                    textBoxDisease.Text = dataGridView1.SelectedRows[0].Cells["MajorDisease"].Value.ToString();  // Hastalık
                    textBoxEmail.Text = dataGridView1.SelectedRows[0].Cells["Email"].Value.ToString();  // Email
                    textBoxNationalID.Text = dataGridView1.SelectedRows[0].Cells["NationalID"].Value.ToString();  // TCKimlikNo
                    comboBoxDoctorID.Text = dataGridView1.SelectedRows[0].Cells["AssignedDoctorID"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj gösteriyoruz
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            DisplayPatient();
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            try
            {
                Con.Open();
                // Stored Procedure çağırma
                SqlCommand cmd = new SqlCommand("GetPatientLogs", Con);
                cmd.CommandType = CommandType.StoredProcedure;

                // Sonuçları al
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);

                // DataGridView'e verileri bağla
                dataGridView1.DataSource = ds.Tables[0];
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

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }


}




