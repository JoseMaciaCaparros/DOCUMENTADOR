using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace Documentador
{
    public partial class Login : Form
    {
        public string Password = "";

        public Login()
        {
            InitializeComponent();
            UserPassword.PasswordChar = '*';
        }

        private void Login_Load(object sender, EventArgs e)
        {
            this.Text = ConfigurationManager.AppSettings["AppName"];
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            LABEL_AppName.Text = ConfigurationManager.AppSettings["AppName"];
        }

        private void EncriptaPassword(object sender, KeyEventArgs e)
        {
            //Do-Nothing
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string NombreUsuario = UserName.Text;
            string PasswordUsuario = UserPassword.Text;
            
            if(NombreUsuario == "" || PasswordUsuario == "")
            {
                MessageBox.Show("Debe indicar el nombre de usuario y contraseña.", "Log In", MessageBoxButtons.OK);
            }
            else
            {
                string SqlGetUsuario = "SELECT IdUsuario FROM SYS_USUARIO WHERE NombreUsuario='" + NombreUsuario + "' AND PasswordUsuario='" + PasswordUsuario + "'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command = new SqlCommand(SqlGetUsuario, con);
                    con.Open();
                    SqlDataReader RS = command.ExecuteReader();
                    if (RS.HasRows)
                    {
                        RS.Read();
                        if (RS.GetInt32(0) > 0)
                        {
                            UserName.Text = "";
                            UserPassword.Text = "";
                            General Prj = new General();
                            Prj.Show();
                            this.Hide();
                        }else
                        {
                            MessageBox.Show("Datos erroneos.", "Log In", MessageBoxButtons.OK);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Datos erroneos.", "Log In", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void CierraLogin(object sender, FormClosingEventArgs e)
        {
            /*e.Cancel = true;
            this.Hide();*/
        }
    }
}
