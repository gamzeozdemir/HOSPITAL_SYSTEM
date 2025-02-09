using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VThastane
{
    public partial class Home : Form
    {
        readonly SqlConnection Con = new SqlConnection(@"Server=DESKTOP-VHCFNJ7\SQLEXPRESS;Database=VThastane;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        public Home()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // Backup butonunu elips şeklinde yapmak için çağırıyoruz
            MakeButtonElliptical(Backup);
            // Restore butonunu da elips şeklinde yapmak için çağırıyoruz
            MakeButtonElliptical(Restore);
        }

        private void MakeButtonElliptical(Button button)
        {
            // Elips şekli için GraphicsPath kullanıyoruz
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, button.Width, button.Height); // Elips şekli ekliyoruz

            // Butonun Region özelliğini elips şeklinde ayarlıyoruz
            button.Region = new Region(path);

            // Düz buton stili ayarlıyoruz
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0; // Kenarlık kaldırıyoruz
            button.BackColor = Color.Transparent; // Arka plan rengini şeffaf yapıyoruz
        }

        // Backup butonu için MouseEnter olayında arka plan rengini beyaz yapıyoruz
        private void Backup_MouseEnter(object sender, EventArgs e)
        {
            this.Backup.BackColor = Color.White;
        }

        // Backup butonu için MouseLeave olayında arka plan rengini tekrar şeffaf yapıyoruz
        private void Backup_MouseLeave(object sender, EventArgs e)
        {
            this.Backup.BackColor = Color.Transparent;
        }

        // Restore butonu için MouseEnter olayında arka plan rengini beyaz yapıyoruz
        private void Restore_MouseEnter(object sender, EventArgs e)
        {
            this.Restore.BackColor = Color.White;
        }

        // Restore butonu için MouseLeave olayında arka plan rengini tekrar şeffaf yapıyoruz
        private void Restore_MouseLeave(object sender, EventArgs e)
        {
            this.Restore.BackColor = Color.Transparent;
        }

        //

       

       

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            // Eğer eklemek istediğiniz başka bir işlev varsa, burayı kullanabilirsiniz.
        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Doctor doctor = new Doctor();
            doctor.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Patient patient = new Patient();
            patient.Show();
            this.Hide();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Diagnosis diagnosis = new Diagnosis();
            diagnosis.Show();
            this.Hide();
        }

        private void Backup_Click_1(object sender, EventArgs e)
        {
            try
            {
                // Yedekleme dosyasının yolu, tarih formatıyla dosya adı oluşturuluyor
                string backupFile = @"C:\Yedekler\VThastane_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak";

                // SQL Yedekleme Sorgusu
                string query = $"BACKUP DATABASE VThastane TO DISK = '{backupFile}'";

                // Bağlantıyı açıyoruz
                Con.Open();

                // Komut oluşturuluyor
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.ExecuteNonQuery();

                // Başarı mesajı
                MessageBox.Show("Veritabanı başarıyla yedeklendi. Yedek dosyası: " + backupFile);
            }
            catch (Exception ex)
            {
                // Hata mesajı
                MessageBox.Show($"Yedekleme sırasında bir hata oluştu: {ex.Message}");
            }
            finally
            {
                // Bağlantıyı kapatıyoruz
                Con.Close();
            }
        }

        private void Restore_Click_1(object sender, EventArgs e)
        {

            // OpenFileDialog ile dosya seçimi
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = @"C:\Yedekler"; // Varsayılan klasör
                openFileDialog.Filter = "BAK Dosyaları (*.bak)|*.bak"; // Sadece .bak dosyalarını göster
                openFileDialog.Title = "Geri Yükleme İçin Yedek Dosyası Seçin";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Seçilen dosyanın tam yolu
                        string backupFile = openFileDialog.FileName;

                        // Geri yükleme sorgusu
                        string query = $"USE master; " +
                                       $"ALTER DATABASE VThastane SET SINGLE_USER WITH ROLLBACK IMMEDIATE; " +
                                       $"RESTORE DATABASE VThastane FROM DISK = '{backupFile}' WITH REPLACE; " +
                                       $"ALTER DATABASE VThastane SET MULTI_USER;";

                        // Bağlantıyı açıyoruz
                        Con.Open();

                        // Komut oluşturuluyor
                        SqlCommand cmd = new SqlCommand(query, Con);
                        cmd.ExecuteNonQuery();

                        // Başarı mesajı
                        MessageBox.Show("Veritabanı başarıyla geri yüklendi.");
                    }
                    catch (Exception ex)
                    {
                        // Hata mesajı
                        MessageBox.Show($"Geri yükleme sırasında bir hata oluştu: {ex.Message}");
                    }
                    finally
                    {
                        // Bağlantıyı kapatıyoruz
                        Con.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Geri yükleme işlemi iptal edildi.");
                }
            }
        }

        private void X_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
