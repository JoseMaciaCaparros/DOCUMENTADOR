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

namespace Documentador
{
    public partial class AppInfo : Form
    {
        public AppInfo()
        {
            InitializeComponent();
        }

        private void PreloadApp(object sender, EventArgs e)
        {
            this.Text = ConfigurationManager.AppSettings["AppName"];
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            LABEL_AppName.Text = ConfigurationManager.AppSettings["AppName"];
            LABEL_AppVersion.Text = "v" + ConfigurationManager.AppSettings["AppVersion"];
            LABEL_AppAuthor.Text = ConfigurationManager.AppSettings["AppAuthor"];
            LABEL_AppAuthorEmail.Text = ConfigurationManager.AppSettings["AppAuthorEmail"];
            LABEL_AppLastUpdate.Text = ConfigurationManager.AppSettings["AppLastUpdate"];
        }
    }
}
