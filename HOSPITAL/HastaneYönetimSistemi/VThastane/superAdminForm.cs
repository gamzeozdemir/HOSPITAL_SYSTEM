using CsvHelper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Linq;

namespace VThastane
{
    public partial class SuperAdminForm : Form
    {
       

        // Parametreli Yapıcı Metod (Bağlantı nesnesi alınır)
        public SuperAdminForm()
        {
            InitializeComponent();
           // Bağlantıyı yapıcıdan alıyoruz
        }

        // Varsayılan Yapıcı Metod (Veritabanı bağlantısı burada oluşturulur)
        readonly SqlConnection Con = new SqlConnection(@"Server=DESKTOP-VHCFNJ7\SQLEXPRESS;Database=VThastane;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");


        // Adminleri listeleme fonksiyonu
        public void ListeleAdminler()
        {
            try
            {
                // Bağlantı kullanılarak veri çekme işlemi yapılır
                using (SqlConnection conn = new SqlConnection(Con.ConnectionString))
                {
                    string query = "SELECT AdminID, FirstName, LastName, Psword, SuperAdmin FROM Adminn";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);  // Çekilen veriler DataTable'a dolduruluyor
                    dataGridViewAdmin.DataSource = dt;  // DataGridView'e veri bağlanıyor
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri listeleme hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Form yüklendiğinde adminleri listeleme
        private void SuperAdminForm_Load(object sender, EventArgs e)
        {
            ListeleAdminler();  // Form yüklendiğinde adminleri listele
        }

        // Listeleme butonuna tıklandığında adminleri listele
        private void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                ListeleAdminler();  // Listeleme butonuna tıklayınca listeyi güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show("Listeleme sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
     

       

        // TextBox'ları temizleme işlemi
        private void btnClear_Click(object sender, EventArgs e)
        {
            try
            {
                name.Clear();
                surname.Clear();
                psword.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Temizleme sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Çıkış butonuna tıklanarak uygulamayı kapatma
        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Çıkış sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewAdmin_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewAdmin_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Geçerli bir satır seçildiyse.
            {
                DataGridViewRow row = dataGridViewAdmin.Rows[e.RowIndex];
               
                name.Text = row.Cells["FirstName"].Value.ToString();
                surname.Text = row.Cells["LastName"].Value.ToString();
                psword.Text = row.Cells["Psword"].Value.ToString();
            }

        }

       

        private void btnExit_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }

        private void add_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcı girişlerini kontrol et
                if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(surname.Text) || string.IsNullOrWhiteSpace(psword.Text))
                {
                    MessageBox.Show("Tüm alanlar doldurulmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(Con.ConnectionString)) // Con nesnesini kullanarak bağlantı kuruyoruz
                {
                    conn.Open();

                    // Transaction başlatıyoruz
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // 1. Adım: Admin tablosuna ekleme yap
                        string adminInsertQuery = @"
                INSERT INTO Adminn (FirstName, LastName, Psword, SuperAdmin) 
                VALUES (@FirstName, @LastName, @Psword, @SuperAdmin); 
                SELECT SCOPE_IDENTITY();"; // SCOPE_IDENTITY() ile son eklenen AdminID'yi alıyoruz

                        SqlCommand adminCmd = new SqlCommand(adminInsertQuery, conn, transaction);
                        adminCmd.Parameters.AddWithValue("@FirstName", name.Text.Trim());
                        adminCmd.Parameters.AddWithValue("@LastName", surname.Text.Trim());
                        adminCmd.Parameters.AddWithValue("@Psword", psword.Text.Trim());
                        adminCmd.Parameters.AddWithValue("@SuperAdmin", 0); // Normal Admin olarak ekliyoruz

                        // AdminID'yi almak için SCOPE_IDENTITY() kullanıyoruz
                        int newAdminId = Convert.ToInt32(adminCmd.ExecuteScalar());

                        // 2. Adım: LoginPage tablosuna ekleme yap
                        string username = $"{name.Text.Trim()}.{surname.Text.Trim()}"; // Kullanıcı adı olarak ad.soyad
                        string loginInsertQuery = @"
                INSERT INTO LoginPage (Username, Psword, Duty, aID) 
                VALUES (@Username, @Psword, @Duty, @aID)";

                        SqlCommand loginCmd = new SqlCommand(loginInsertQuery, conn, transaction);
                        loginCmd.Parameters.AddWithValue("@Username", username);
                        loginCmd.Parameters.AddWithValue("@Psword", psword.Text.Trim());
                        loginCmd.Parameters.AddWithValue("@Duty", "Adminn");
                        loginCmd.Parameters.AddWithValue("@aID", newAdminId);

                        loginCmd.ExecuteNonQuery();

                        // İşlemi onayla
                        transaction.Commit();
                        MessageBox.Show("Admin ve Login bilgileri başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Adminleri tekrar listele
                        ListeleAdminler();
                    }
                    catch (Exception ex)
                    {
                        // Hata durumunda işlemi geri al
                        transaction.Rollback();
                        MessageBox.Show("Ekleme sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bağlantı sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Update_Click(object sender, EventArgs e)
        {
            try
            {
                // Kullanıcı girişlerini kontrol et
                if (string.IsNullOrWhiteSpace(name.Text) || string.IsNullOrWhiteSpace(surname.Text) || string.IsNullOrWhiteSpace(psword.Text))
                {
                    MessageBox.Show("Tüm alanlar doldurulmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Seçilen AdminID'yi kontrol et
                if (dataGridViewAdmin.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Güncellenecek bir admin seçilmelidir!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int selectedAdminID = Convert.ToInt32(dataGridViewAdmin.SelectedRows[0].Cells["AdminID"].Value);

                // SQL bağlantısı
                using (SqlConnection conn = new SqlConnection(Con.ConnectionString)) // Con nesnesini kullanarak bağlantı kuruyoruz
                {
                    conn.Open();

                    // Admin güncelleme işlemi
                    string queryUpdateAdmin = "UPDATE Adminn SET FirstName = @FirstName, LastName = @LastName, Psword = @Psword WHERE AdminID = @AdminID";
                    SqlCommand cmdUpdateAdmin = new SqlCommand(queryUpdateAdmin, conn);
                    cmdUpdateAdmin.Parameters.AddWithValue("@FirstName", name.Text);
                    cmdUpdateAdmin.Parameters.AddWithValue("@LastName", surname.Text);
                    cmdUpdateAdmin.Parameters.AddWithValue("@Psword", psword.Text);
                    cmdUpdateAdmin.Parameters.AddWithValue("@AdminID", selectedAdminID);
                    cmdUpdateAdmin.ExecuteNonQuery();

                    // LoginPage güncelleme işlemi
                    string queryUpdateLogin = "UPDATE LoginPage SET Username = @Username, Psword = @Psword WHERE aID = @AdminID";
                    SqlCommand cmdUpdateLogin = new SqlCommand(queryUpdateLogin, conn);
                    cmdUpdateLogin.Parameters.AddWithValue("@Username", $"{name.Text}.{surname.Text}".ToLower()); // Kullanıcı adı formatı
                    cmdUpdateLogin.Parameters.AddWithValue("@Psword", psword.Text);
                    cmdUpdateLogin.Parameters.AddWithValue("@AdminID", selectedAdminID);
                    cmdUpdateLogin.ExecuteNonQuery();

                    MessageBox.Show("Admin ve giriş bilgileri başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ListeleAdminler(); // Admin güncelledikten sonra listeyi yenile
            }
            catch (Exception ex)
            {
                MessageBox.Show("Admin güncelleme sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void dataGridViewAdmin_SelectionChanged(object sender, EventArgs e)
        {

            try
            {
                // Seçilen satırda verilerin olup olmadığını kontrol ediyoruz
                if (dataGridViewAdmin.SelectedRows.Count > 0)
                {
                    // Seçilen satırdaki verileri alıyoruz
                    name.Text = dataGridViewAdmin.SelectedRows[0].Cells["FirstName"].Value.ToString();   
                    surname.Text = dataGridViewAdmin.SelectedRows[0].Cells["LastName"].Value.ToString();

                    psword.Text = dataGridViewAdmin.SelectedRows[0].Cells["Psword"].Value.ToString(); 
                
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda mesaj gösteriyoruz
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void btnDelete_Click_1(object sender, EventArgs e)
        {

            try
            {
                if (dataGridViewAdmin.CurrentRow != null)
                {
                    int adminID = Convert.ToInt32(dataGridViewAdmin.CurrentRow.Cells["AdminID"].Value);
                    bool isSuperAdmin = Convert.ToBoolean(dataGridViewAdmin.CurrentRow.Cells["SuperAdmin"].Value);

                    if (isSuperAdmin)
                    {
                        MessageBox.Show("SuperAdmin silinemez!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    using (SqlConnection conn = new SqlConnection(Con.ConnectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM Adminn WHERE AdminID = @AdminID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@AdminID", adminID);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Admin başarıyla silindi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    ListeleAdminler();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Admin silme sırasında hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ListeleAdminler();
        }

        private void btnList_Click_1(object sender, EventArgs e)
        {
            ListeleAdminler();
        }

        private void import_Click(object sender, EventArgs e)
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

                        dataGridViewAdmin.DataSource = dt;  // Bind data to DataGridView
                        MessageBox.Show("Data imported successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error importing CSV: {ex.Message}");
            }
        }

        private void export_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                    {
                        // Writing headers
                        var headers = dataGridViewAdmin.Columns.Cast<DataGridViewColumn>()
                                        .Select(column => column.HeaderText)
                                        .ToArray();
                        writer.WriteLine(string.Join(",", headers));

                        // Writing rows
                        foreach (DataGridViewRow row in dataGridViewAdmin.Rows)
                        {
                            var cells = row.Cells.Cast<DataGridViewCell>()
                                                 .Select(cell => cell.Value.ToString())
                                                 .ToArray();
                            writer.WriteLine(string.Join(",", cells));
                        }
                    }
                    MessageBox.Show("Data exported to CSV successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to CSV: {ex.Message}");
            }
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            name.Clear();
            surname.Clear();
            psword.Clear();
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
                    PdfPTable table = new PdfPTable(dataGridViewAdmin.Columns.Count);

                    // Add headers
                    foreach (DataGridViewColumn column in dataGridViewAdmin.Columns)
                    {
                        table.AddCell(column.HeaderText);
                    }

                    // Add rows
                    foreach (DataGridViewRow row in dataGridViewAdmin.Rows)
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
    }
}
