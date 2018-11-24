using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SEC_PJ
{
    public partial class frm_impresion : Form
    {
        public frm_impresion()
        {
            InitializeComponent();
        }

        private void frm_impresion_Load(object sender, EventArgs e)
        {
            rpt_afi.DataSourceConnections[0].SetConnection("181.199.155.11", "SEC_PJ", false);
            rpt_afi.DataSourceConnections[0].SetLogon("sec", "nosenose101");


                    //reportDocument.DataSourceConnections[i].SetConnection(Server.Name, Database.Name, Server.User.Name,Server.User.Password);
                    //reportDocument.DataSourceConnections[i].SetLogon(Server.User.Name, Server.User.Password);
            CRV.RefreshReport();
        }
    }
}
