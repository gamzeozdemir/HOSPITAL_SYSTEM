using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using iTextSharp;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using CsvHelper;


namespace VThastane
{
    public partial class Doctor : Form
    {
        private int selectedId;
        public Doctor()
        {
            InitializeComponent();
        }

        readonly SqlConnection Con = new SqlConnection(@"Server=DESKTOP-VHCFNJ7\SQLEXPRESS;Database=VThastane;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
        private void DisplayDoctor()
        {
            try
            {
                Con.Open();
                string Query = "SELECT * FROM Doctor";
                SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0]; // DataGridView'i verilerle dolduruyor
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
        private void Doctor_Load(object sender, EventArgs e)
        {
            DisplayDoctor();
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
                // Seçilen satırdaki veriler ilgili alanlara dolduruluyor
                search2.Text = dataGridView1.SelectedRows[0].Cells["DocName"].Value.ToString();      // Doktor adı
                comboBox1.Text = dataGridView1.SelectedRows[0].Cells["DocGen"].Value.ToString();      // Cinsiyet
                search.Text = dataGridView1.SelectedRows[0].Cells["Experience"].Value.ToString();   // Deneyim yılı
                textBox1.Text = dataGridView1.SelectedRows[0].Cells["License"].Value.ToString();      // Lisans numarası
                speciality.Text = dataGridView1.SelectedRows[0].Cells["Specialty"].Value.ToString();  // Branş
                phonee.Text = dataGridView1.SelectedRows[0].Cells["Phone"].Value?.ToString();        // Telefon (null kontrolü)
                email.Text = dataGridView1.SelectedRows[0].Cells["Email"].Value?.ToString();          // Email (null kontrolü)
                nationalID.Text = dataGridView1.SelectedRows[0].Cells["NationalID"].Value.ToString(); // TCKimlikNo
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }





        private void SearchBySpecialty(string specialty)
        {
            try
            {
                if (string.IsNullOrEmpty(specialty))
                {
                    MessageBox.Show("Lütfen bir uzmanlık alanı girin.");
                    return;
                }

                if (Con.State != ConnectionState.Open)
                {
                    Con.Open();
                }

                string query = "SearchDoctorsBySpecialty"; // Stored Procedure adı
                using (SqlCommand cmd = new SqlCommand(query, Con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Specialty", specialty);

                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    if (ds.Tables.Count > 0)
                    {
                        dataGridView1.DataSource = ds.Tables[0];
                    }
                    else
                    {
                        MessageBox.Show("Uzmanlık alanına ait kayıt bulunamadı.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }




        private void SearchByName(string name)
        {
            try
            {
                Con.Open();
                string query = "SearchDoctorByName"; // Stored Procedure adı
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@DocName", name);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0]; // Sonuçları DataGridView'e yansıt
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

        

        private void SaveToPdf(string filePath)
        {
            try
            {
                // PDF dökümanı oluştur
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                    PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    // DataGridView'deki verileri tabloya yaz
                    PdfPTable pdfTable = new PdfPTable(dataGridView1.Columns.Count);
                    pdfTable.WidthPercentage = 100;

                    // Başlıklar
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pdfTable.AddCell(cell);
                    }

                    // Satırlar
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            pdfTable.AddCell(cell.Value?.ToString());
                        }
                    }

                    pdfDoc.Add(pdfTable);
                    pdfDoc.Close();
                    stream.Close();

                    MessageBox.Show("PDF başarıyla kaydedildi!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
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
       

        

        private void button12_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveToPdf(saveFileDialog.FileName);
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            string specialty = search.Text.Trim(); // TextBox'tan uzmanlık alanı al
            SearchBySpecialty(specialty);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string name = search2.Text.Trim();
            SearchByName(name);
        }

       

        private void button5_Click_1(object sender, EventArgs e)
        {
            DisplayDoctor();
        }

        private void X_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void X_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        

        private void button4_Click_1(object sender, EventArgs e)
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }

        private void add_Click(object sender, EventArgs e)
        {
            try
            {
                // Eksik bilgi kontrolü
                if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(comboBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) ||
                    string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(speciality.Text) ||
                    string.IsNullOrWhiteSpace(phonee.Text) ||
                    string.IsNullOrWhiteSpace(email.Text) ||
                    string.IsNullOrWhiteSpace(nationalID.Text))  // Branş, Telefon, Email ve TC kimlik için ek kontrol
                {
                    MessageBox.Show("Eksik bilgi! Lütfen tüm alanları doldurun.");
                    return;
                }

                // Veritabanına bağlantı açılıyor
                Con.Open();

                // SQL Insert Sorgusu (Yeni tabloya uygun)
                string query = "INSERT INTO Doctor (DocName, DocGen, Experience, License, Specialty, Phone, Email, NationalID) " +
                               "VALUES (@DocName, @DocGen, @Experience, @License, @Specialty, @Phone, @Email, @NationalID)";
                SqlCommand cmd = new SqlCommand(query, Con);

                // Parametreler ekleniyor
                cmd.Parameters.AddWithValue("@DocName", textBox1.Text);      // Doktor adı
                cmd.Parameters.AddWithValue("@DocGen", comboBox1.Text);      // Cinsiyet
                cmd.Parameters.AddWithValue("@Experience", textBox2.Text);   // Deneyim yılı
                cmd.Parameters.AddWithValue("@License", textBox3.Text);      // Lisans numarası
                cmd.Parameters.AddWithValue("@Specialty", speciality.Text); // Branş
                cmd.Parameters.AddWithValue("@Phone", phonee.Text);         // Telefon
                cmd.Parameters.AddWithValue("@Email", email.Text);           // Email
                cmd.Parameters.AddWithValue("@NationalID", nationalID.Text); // TCKimlikNo
                cmd.Parameters.AddWithValue("@DocId", selectedId);           // Doktor ID


                // Sorgu çalıştırılıyor
                cmd.ExecuteNonQuery();

                MessageBox.Show("Doktor başarıyla eklendi.");
                DisplayDoctor();  // Listeyi yenileyin (bu metodu kendi kodunuza göre özelleştirebilirsiniz)
            }
            catch (Exception ex)
            {
                // Hata mesajı gösteriliyor
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                Con.Close();
            }
            DisplayDoctor();
        }

        private void Update_Click(object sender, EventArgs e)
        {
            try
            {
                // Seçilen satır kontrolü
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen güncellemek istediğiniz doktoru seçin.");
                    return;
                }

                // Boş alan kontrolü
                if (string.IsNullOrWhiteSpace(textBox2.Text) ||
                    string.IsNullOrWhiteSpace(comboBox1.Text) ||
                    string.IsNullOrWhiteSpace(textBox3.Text) ||
                    string.IsNullOrWhiteSpace(textBox1.Text) ||
                    string.IsNullOrWhiteSpace(speciality.Text) ||
                    string.IsNullOrWhiteSpace(phonee.Text) ||
                    string.IsNullOrWhiteSpace(email.Text) ||
                    string.IsNullOrWhiteSpace(nationalID.Text))  // Branş, Telefon, Email ve TC kimlik için ek kontrol
                {
                    MessageBox.Show("Eksik bilgi! Lütfen gerekli alanları doldurun.");
                    return;
                }

                // Seçilen doktorun ID'sini alıyoruz
                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DocId"].Value);

                // Veritabanı bağlantısını açıyoruz
                Con.Open();

                // Güncelleme sorgusu
                string query = @"UPDATE Doctor 
                         SET 
                             DocName = @DocName, 
                             DocGen = @DocGen, 
                             Experience = @Experience, 
                             License = @License,
                             Specialty = @Specialty,
                             Phone = @Phone,
                             Email = @Email,
                             NationalID = @NationalID
                         WHERE 
                             DocId = @DocId";

                // Komut oluşturuluyor
                SqlCommand cmd = new SqlCommand(query, Con);

                // Parametreler ekleniyor
                cmd.Parameters.AddWithValue("@DocName", textBox1.Text);      // Doktor adı
                cmd.Parameters.AddWithValue("@DocGen", comboBox1.Text);      // Cinsiyet
                cmd.Parameters.AddWithValue("@Experience", textBox2.Text);   // Deneyim yılı
                cmd.Parameters.AddWithValue("@License", textBox3.Text);      // Lisans numarası
                cmd.Parameters.AddWithValue("@Specialty", speciality.Text); // Branş
                cmd.Parameters.AddWithValue("@Phone", phonee.Text);         // Telefon
                cmd.Parameters.AddWithValue("@Email", email.Text);           // Email
                cmd.Parameters.AddWithValue("@NationalID", nationalID.Text); // TCKimlikNo
                cmd.Parameters.AddWithValue("@DocId", selectedId);           // Doktor ID

                // Sorgu çalıştırılıyor
                cmd.ExecuteNonQuery();

                // Başarılı mesajı
                MessageBox.Show("Doktor bilgileri güncellendi.");

                // Güncel listeyi getiriyoruz
                DisplayDoctor();
            }
            catch (Exception ex)
            {
                // Hata mesajı
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                Con.Close();
            }
            DisplayDoctor();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silmek istediğiniz doktoru seçin.");
                    return;
                }

                int selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value); // DocId alınıyor
                Con.Open();

                // Bu sorgu çalıştırıldığında trigger tetiklenecek
                string query = "DELETE FROM Doctor WHERE DocId = @DocId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@DocId", selectedId);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Doktor başarıyla silindi.");
                DisplayDoctor(); // Listeyi güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
            finally
            {
                Con.Close();
            }
            DisplayDoctor();
        }

        private void reset_Click(object sender, EventArgs e)
        {
            search2.Clear(); // Doktor adı
            comboBox1.SelectedIndex = -1;
            search.Clear();
            textBox1.Clear();
            search2.Clear();
            speciality.Clear();
            phonee.Clear();
            email.Clear();
            nationalID.Clear();
            textBox3.Clear();  
            textBox2.Clear();  
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                // Seçilen satır olup olmadığını kontrol ediyoruz
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    // Seçilen satırdaki veriler ilgili alanlara dolduruluyor
                    selectedId = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["DocId"].Value);  // Doktor ID
                    textBox1.Text = dataGridView1.SelectedRows[0].Cells["DocName"].Value.ToString();  // Doktor adı
                    comboBox1.Text = dataGridView1.SelectedRows[0].Cells["DocGen"].Value.ToString();  // Cinsiyet
                    textBox2.Text = dataGridView1.SelectedRows[0].Cells["Experience"].Value.ToString(); // Deneyim yılı
                    textBox3.Text = dataGridView1.SelectedRows[0].Cells["License"].Value.ToString();    // Lisans numarası
                    speciality.Text = dataGridView1.SelectedRows[0].Cells["Specialty"].Value.ToString(); // Branş
                    phonee.Text = dataGridView1.SelectedRows[0].Cells["Phone"].Value?.ToString();      // Telefon (null kontrolü)
                    email.Text = dataGridView1.SelectedRows[0].Cells["Email"].Value?.ToString();        // Email (null kontrolü)
                    nationalID.Text = dataGridView1.SelectedRows[0].Cells["NationalID"].Value.ToString(); // TCKimlikNo
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void button5_Click_2(object sender, EventArgs e)
        {
            DisplayDoctor();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Açık olan bağlantıyı kullanın
            try
            {
                // Prosedürü çağıran sorgu
                string query = "EXEC GetDoctorLogs";

                // Komut nesnesini oluştur
                using (SqlCommand cmd = new SqlCommand(query, Con)) // Con zaten açık olan SqlConnection nesnesi
                {
                    // Veri adaptörü ile sorgunun sonucunu al
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    // Verileri DataTable nesnesine doldur
                    da.Fill(dt);

                    // DataGridView'e bağla
                    dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj göster
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                        // DataTable'daki verileri veritabanına eklemek
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
                }
            }
            DisplayDoctor();
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
    }
}