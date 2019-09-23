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
    public partial class Principal : Form
    {

        #region codigo para efecto shadow

        private bool Drag;
        private int MouseX;
        private int MouseY;

        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        private bool m_aeroEnabled;

        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]

        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );

        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();
                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW; return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0; DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 0,
                            rightWidth = 0,
                            topHeight = 0
                        }; DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                    }
                    break;
                default: break;
            }
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT) m.Result = (IntPtr)HTCAPTION;
        }
        private void PanelMove_MouseDown(object sender, MouseEventArgs e)
        {
            Drag = true;
            MouseX = Cursor.Position.X - this.Left;
            MouseY = Cursor.Position.Y - this.Top;
        }
        private void PanelMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (Drag)
            {
                this.Top = Cursor.Position.Y - MouseY;
                this.Left = Cursor.Position.X - MouseX;
            }
        }
        private void PanelMove_MouseUp(object sender, MouseEventArgs e) { Drag = false; }

        #endregion

        lts_sindicatoDataContext db_sindicato = new lts_sindicatoDataContext();

        // connectionString="Data Source=DIEGO-LENOVO;Initial Catalog=SEC_PJ;Integrated Security=True"
        public Principal()
        {
            InitializeComponent();
            dgv_mostrar_socios.AutoGenerateColumns = false;
        }

        private void btn_salir_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void btn_aceptar_Click(object sender, EventArgs e)
        {
            buscar_socios();
        }
        private void buscar_socios()
        {
            var socios = (from a in db_sindicato.maesoc
                          join socemp in db_sindicato.socemp on a.MAESOC_CUIL equals socemp.SOCEMP_CUIL
                          join emp in db_sindicato.maeemp on socemp.SOCEMP_CUITE equals emp.MAEEMP_CUIT
                          join pj in db_sindicato.Padron_Capital on Convert.ToInt32(a.MAESOC_NRODOC) equals pj.numdoc into nopj // aqui hago el left join
                          from pj in nopj.DefaultIfEmpty()
                          join cap in db_sindicato.Capital on Convert.ToInt32(a.MAESOC_NRODOC) equals cap.DOC into padron_electoral_capital // aqui hago el left join
                          from cap in padron_electoral_capital.DefaultIfEmpty()
                          where
                          (a.MAESOC_APELLIDO.Contains(txt_buscar_afiliado.Text.Trim()) ||
                          a.MAESOC_NOMBRE.Contains(txt_buscar_afiliado.Text.Trim()) ||
                          a.MAESOC_NRODOC.Contains(txt_buscar_afiliado.Text.Trim()) ||
                          emp.MAEEMP_RAZSOC.Contains(txt_buscar_afiliado.Text.Trim()))
                          && socemp.SOCEMP_ULT_EMPRESA == 'S'
                          select new
                          {
                              numero_socio = a.MAESOC_NROAFIL,
                              dni_socio = a.MAESOC_NRODOC,
                              apeynom = a.MAESOC_APELLIDO.Trim() + " " + a.MAESOC_NOMBRE.Trim(),
                              empresa = emp.MAEEMP_RAZSOC.Trim(),
                              domicilio = cap.DOMICILIO.Trim(),//a.MAESOC_CALLE,
                              numero = a.MAESOC_NROCALLE,
                              apellido = a.MAESOC_APELLIDO.Trim(),
                              nombre = a.MAESOC_NOMBRE.Trim(),
                              circuito = cap.CIRCUITO,//pj.circuito,
                              afiliado_pj = (pj.numdoc == null || pj.numdoc == 0) ? "NO" : "SI",
                              profesion = pj.prof,
                              estado_civil = a.MAESOC_ESTCIV,
                              fecha_nac = a.MAESOC_FECHANAC,
                              sexo = a.MAESOC_SEXO,
                              clase = a.MAESOC_FECHANAC.Year
                             
                         }).ToList().OrderBy(x=>x.apeynom);

                         dgv_mostrar_socios.DataSource = socios.ToList();

            lbl_cantidad_registros.Text = socios.Count().ToString();
            lbl_cantidad_afiliados.Text = socios.Count(x => x.circuito != null ).ToString();
            lbl_cantidad_NO_afil.Text = socios.Count(x => x.circuito == null).ToString();

        }

        private void mostrar_datos_socios()//20959496   
        {
            txt_apellido.Text = dgv_mostrar_socios.CurrentRow.Cells["apellido"].Value.ToString();
            txt_nombre.Text = dgv_mostrar_socios.CurrentRow.Cells["nombre"].Value.ToString();
            txt_dni.Text = dgv_mostrar_socios.CurrentRow.Cells["dni_socio"].Value.ToString();
            txt_clase.Text = dgv_mostrar_socios.CurrentRow.Cells["clase"].Value == null ? "": dgv_mostrar_socios.CurrentRow.Cells["clase"].Value.ToString();
            txt_fnac.Text = Convert.ToDateTime(dgv_mostrar_socios.CurrentRow.Cells["fecha_nac"].Value).ToString("dd/MM/yyyy");
            txt_sexo.Text = dgv_mostrar_socios.CurrentRow.Cells["sexo"].Value == null ? "": dgv_mostrar_socios.CurrentRow.Cells["sexo"].Value.ToString();
            txt_profesion.Text = dgv_mostrar_socios.CurrentRow.Cells["profesion"].Value == null ? "" : dgv_mostrar_socios.CurrentRow.Cells["profesion"].Value.ToString();
            txt_distrito.Text = dgv_mostrar_socios.CurrentRow.Cells["circuito"].Value == null ? "" : dgv_mostrar_socios.CurrentRow.Cells["circuito"].Value.ToString();
            txt_calle_.Text = dgv_mostrar_socios.CurrentRow.Cells["domicilio"].Value == null ? "" : dgv_mostrar_socios.CurrentRow.Cells["domicilio"].Value.ToString();
            txt_numero_.Text = dgv_mostrar_socios.CurrentRow.Cells["numero"].Value.ToString();
            if (dgv_mostrar_socios.CurrentRow.Cells["afiliado_pj"].Value.ToString() == "SI")
            {

                btn_imprimir.Enabled = false;
            }
            else
            {
                btn_imprimir.Enabled = true;
            }
        }

        private void dgv_mostrar_socios_SelectionChanged(object sender, EventArgs e)
        {
            mostrar_datos_socios();
            
        }

        private void txt_buscar_afiliado_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buscar_socios();
            }
        }

        private void dgv_mostrar_socios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btn_imprimir_Click(object sender, EventArgs e)
        {


           // db_sindicato.ExecuteCommand("truncate table impresion"); // borro la tabla

            // db_sindicato.SubmitChanges();
            //limpio la tabla de impresion
            var im = from a in db_sindicato.impresion select a;
            foreach (var item in im)
            {
                db_sindicato.impresion.DeleteOnSubmit(item);
                db_sindicato.SubmitChanges();
            }

            impresion imp = new impresion();

            imp.apellido = txt_apellido.Text; //
            imp.nombre = txt_nombre.Text; //
            imp.numdoc = Convert.ToInt32(txt_dni.Text); //
            imp.DM = txt_DM.Text; //
            imp.reg = txt_reg.Text; //
            imp.clase = Convert.ToInt32(txt_clase.Text); //
            imp.sexo = txt_sexo.Text; //
            imp.dia = txt_fnac.Text.Substring(0, 2); //
            imp.mes = txt_fnac.Text.Substring(3,2); //
            imp.año = txt_fnac.Text.Substring(6,4); //
            imp.lugar = txt_lugar.Text; //
            imp.prof = txt_profesion.Text ; //
            imp.estadocivil = txt_estado_civil.Text; //
            imp.circuito = txt_distrito.Text; //
            imp.cuartel = txt_ciudad_.Text; //
            imp.ciudad = txt_pueblo_.Text; //
            imp.domicilio = txt_calle_.Text; //
            imp.nro_calle = Convert.ToInt32(txt_numero_.Text); //
            imp.piso = txt_piso_.Text; //
            imp.dpto = txt_dpto_.Text; //
            
            db_sindicato.impresion.InsertOnSubmit(imp);
            db_sindicato.SubmitChanges();

            frm_impresion reportes = new frm_impresion();
            reportes.Show();
            

            //txt_apellido.Text = dgv_mostrar_socios.CurrentRow.Cells["apellido"].Value.ToString();
            //txt_nombre.Text = dgv_mostrar_socios.CurrentRow.Cells["nombre"].Value.ToString();
            //txt_dni.Text = dgv_mostrar_socios.CurrentRow.Cells["dni_socio"].Value.ToString();
            //txt_clase.Text = dgv_mostrar_socios.CurrentRow.Cells["clase"].Value == null ? "" : dgv_mostrar_socios.CurrentRow.Cells["clase"].Value.ToString();
            //txt_fnac.Text = Convert.ToDateTime(dgv_mostrar_socios.CurrentRow.Cells["fecha_nac"].Value).ToString("dd/MM/yyyy");
            //txt_sexo.Text = dgv_mostrar_socios.CurrentRow.Cells["sexo"].Value == null ? "" : dgv_mostrar_socios.CurrentRow.Cells["sexo"].Value.ToString();
            //txt_profesion.Text = dgv_mostrar_socios.CurrentRow.Cells["profesion"].Value == null ? "" : dgv_mostrar_socios.CurrentRow.Cells["profesion"].Value.ToString();
            //txt_calle_.Text = dgv_mostrar_socios.CurrentRow.Cells["domicilio"].Value.ToString();
            //txt_numero_.Text = dgv_mostrar_socios.CurrentRow.Cells["numero"].Value.ToString();


        }

        private void Principal_Load(object sender, EventArgs e)
        {
            var horas = (from a in db_sindicato.pettazi select a).Sum(x => x.HorasExtras.Value.Hour);
            var minutos = (from a in db_sindicato.pettazi select a).Sum(x => x.HorasExtras.Value.Minute);

            label1.Text = horas.ToString() + " -- " + minutos.ToString();
        }
    }
}
