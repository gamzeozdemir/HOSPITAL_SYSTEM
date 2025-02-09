using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using VThastane;  // Eğer SuperAdminForm VThastane namespace'inde ise.

namespace VThastane
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly SqlConnection Con = new SqlConnection(@"Server=DESKTOP-VHCFNJ7\SQLEXPRESS;Database=VThastane;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

       

        

        private void clearButton_Click(object sender, EventArgs e)
        {
            // Tüm TextBox alanlarını temizle
            userNameTextBox.Clear();
            passwordTextBox.Clear();
        }


        private void clearButton_Click_1(object sender, EventArgs e)
        {
            // Tüm TextBox alanlarını temizle
            userNameTextBox.Clear();  // Kullanıcı adı text kutusunu temizler
            passwordTextBox.Clear();  // Şifre text kutusunu temizler

          
        }

     
       
      

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

       
        private void adminLoginButton_Click_2(object sender, EventArgs e)
        {
            string username = userNameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(Con.ConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(1) FROM LoginPage WHERE Username = @Username AND Psword = @Password";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count == 1)
                        {
                            MessageBox.Show("Giriş Başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form home = new Home();
                            home.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void superAdminLoginButton_Click(object sender, EventArgs e)
        {
            string username = userNameTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Kullanıcı adı ve şifre boş olamaz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SqlConnection conn = new SqlConnection(Con.ConnectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(1) FROM Adminn WHERE FirstName = @Username AND Psword = @Password AND SuperAdmin = 1";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count == 1)
                        {
                            MessageBox.Show("Süper Admin Girişi Başarılı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Form superAdminForm = new SuperAdminForm();
                            superAdminForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı adı veya şifre hatalı ya da Süper Admin yetkiniz yok.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}