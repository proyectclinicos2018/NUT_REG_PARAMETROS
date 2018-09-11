using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace Registrar_Parametros
{
    public partial class Frm_Registrar_Parametros : Form
    {

        #region Variables

        #region Variables Staticas

        static int cod_parametros=0;
           static string msg_reg_corr = "";
        static string msg_mod_corr = "";
        static string msg_reg_erro = "";
        static string msg_mod_erro = "";
        static string msg_cam_vaci = "";
        static string msg_con_info = "";
        static string msg_cam_inco = "";
        static string msg_cam_nval = "";
        static string msg_dat_exit = "";

        #endregion

        #region Datatables

        DataTable dt = new DataTable();
        DataTable dt_mensajes = new DataTable();

        #endregion

        #region Datos Conexion

        ConectarFalp CnnFalp;
        Configuration Config;
        string User = string.Empty;
        string[] Conexion = { "", "", "" };
        string PCK = "PCK_NUT001I";
        string PCK1 = "PCK_NUT001M";

        #endregion

        #endregion
        public Frm_Registrar_Parametros()
        {
            InitializeComponent();
        }

        private void Frm_Registrar_Parametros_Load(object sender, EventArgs e)
        {
            conectar();
            Cargar_dtmensajes();
            buscar_mensajes();
            btn_guardar.Enabled = false;
        }


     

        #region Cargar


        #region Cargar Conexion

        protected void conectar()
        {

            if (!(CnnFalp != null))
            {

                ExeConfigurationFileMap FileMap = new ExeConfigurationFileMap();
                FileMap.ExeConfigFilename = Application.StartupPath + @"\..\WF.config";
                Config = ConfigurationManager.OpenMappedExeConfiguration(FileMap, ConfigurationUserLevel.None);

                CnnFalp = new ConectarFalp(Config.AppSettings.Settings["dbServer"].Value,//ConfigurationManager.AppSettings["dbServer"],
                                           Config.AppSettings.Settings["dbUser"].Value,//ConfigurationManager.AppSettings["dbUser"],
                                           Config.AppSettings.Settings["dbPass"].Value,//ConfigurationManager.AppSettings["dbPass"],
                                           ConectarFalp.TipoBase.Oracle);

                if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir(); // abre la conexion

                Conexion[0] = Config.AppSettings.Settings["dbServer"].Value;
                Conexion[1] = Config.AppSettings.Settings["dbUser"].Value;
                Conexion[2] = Config.AppSettings.Settings["dbPass"].Value;
            }



            // this.Text = this.Text + " [Versión: " + Application.ProductVersion + "] [Conectado: " + Conexion[0] + "]";
            //User = ValidaMenu.LeeUsuarioMenu();
            User = "SICI";
            LblUsuario.Text = "Usuario: " + User;
            //LblUsuario.Text = "Usuario: " + User;
        }
        #endregion

        #region Cargar Grilla

        #region Listar Grilla

        protected void Cargar_grilla()
        {

            if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();
            dt.Clear();
            CnnFalp.CrearCommand(CommandType.StoredProcedure, PCK + ".P_CARGAR_RESULTADO_PARAMETROS");
            CnnFalp.ParametroBD("PIN_TIPO", cod_parametros, DbType.Int64, ParameterDirection.Input);
            dt.Load(CnnFalp.ExecuteReader());

            if (dt.Rows.Count > 0)
            {
              /*  txtmsg.Visible = false;
                txtmsg.Text = "";*/
                grilla_parametros.AutoGenerateColumns = false;
                grilla_parametros.DataSource = dt;
                agregarimagen();

            }
            else
            {
                //no trajo datos
            }

            CnnFalp.Cerrar();

        }

        #endregion

        #region Agrupar

        #endregion

        #region Agregar Imagen

        protected void agregarimagen()
        {
            foreach (DataGridViewRow row in grilla_parametros.Rows)
            {

                string ve = Convert.ToString(row.Cells["V"].Value);
                DataGridViewImageCell Imagen = row.Cells["Vigencia"] as DataGridViewImageCell;

                if (ve == "True")
                {
                    Imagen.Value = (System.Drawing.Image)Registrar_Parametros.Properties.Resources.Check;
                }
                else
                {
                    Imagen.Value = (System.Drawing.Image)Registrar_Parametros.Properties.Resources.Delete;

                }

            }



        }

        #endregion

        #region Ocultar Columnas

        #endregion

        #region Ordenar Columnas

        #endregion

        #region Pintar Grilla

        private void grilla_parametros_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                e.PaintBackground(e.ClipBounds, false);
                Font drawFont = new Font("Trebuchet MS", 8, FontStyle.Bold);
                SolidBrush drawBrush = new SolidBrush(Color.White);
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Center;
                StrFormat.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawImage(Properties.Resources.HeaderGV, e.CellBounds);
                e.Graphics.DrawString(grilla_parametros.Columns[e.ColumnIndex].HeaderText, drawFont, drawBrush, e.CellBounds, StrFormat);

                e.Handled = true;
                drawBrush.Dispose();
            }

        }

        #endregion

        #region Pintar Extraer grilla

        private void grilla_parametros_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        #endregion

        #endregion

        #region Cargar DataTables

       protected  void Cargar_dtmensajes()
        {

            if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

            CnnFalp.CrearCommand(CommandType.StoredProcedure, PCK + ".P_CARGAR_MENSAJES");
            CnnFalp.ParametroBD("PIN_TIPO", 19, DbType.String, ParameterDirection.Input);
            dt_mensajes.Load(CnnFalp.ExecuteReader());


            CnnFalp.Cerrar();
        }

        #endregion

        #endregion

        #region Botones

        private void btn_guardar_Click(object sender, EventArgs e)
        {
            if (Validar_Campos())
            {
                DialogResult resp = MessageBox.Show("" + msg_con_info + " Tipo  Parametro " + txtparametros.Text.ToUpper() + "  Descripción " + txtdescripcion.Text.ToUpper().Trim() + " ?", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (resp == DialogResult.Yes)
                {
                    Guardar_parametros();
                    dt.Clear();
                    Cargar_grilla();
                    txtdescripcion.Text = "";

                }

            }
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            limpiar();
        }

        private void btntipo_alimento_Click(object sender, EventArgs e)
        {
            txtparametros.Text = "";
            Cargar_parametros();

            if (cod_parametros == 0)
            {
                txtdescripcion.Enabled = false;
                txtparametros.Focus();
                Cargar_parametros();
            }
            else
            {
                Cargar_parametros();
                Cargar_grilla();
                txtdescripcion.Enabled = true;

                txtdescripcion.Focus();
            }
        
        }


        #endregion

      

        #region Metodos

        #region Limpiar

        protected void limpiar()
        {
            txtparametros.Text = "";
            cod_parametros = 0;
            txtdescripcion.Text = "";
            dt.Clear();
         /*   txtmsg.Text = "";
            txtmsg.Visible = false;*/
        }

        #endregion

        #region Cargar Parametros

        protected void Cargar_parametros()
        {
            Cargar_datos_parametros(ref Ayuda);

            if (!Ayuda.EOF())
            {
                cod_parametros = Convert.ToInt32(Ayuda.Fields(0));
                txtparametros.Text = Ayuda.Fields(1);
            }
            else
            {
                if(cod_parametros==0)
                {
                    txtparametros.Text = "";
                }
            }


        }

        void Cargar_datos_parametros(ref AyudaSpreadNet.AyudaSprNet Ayuda)
        {
            string[] NomCol = { "Código", "Descripción" };
            int[] AnchoCol = { 80, 350 };
            Ayuda.Nombre_BD_Datos = Conexion[0];
            Ayuda.Pass = Conexion[1];
            Ayuda.User = Conexion[2];
            Ayuda.TipoBase = 1;
            Ayuda.NombreColumnas = NomCol;
            Ayuda.AnchoColumnas = AnchoCol;
            Ayuda.TituloConsulta = "Ingreso Parametros Nutrición";
            Ayuda.Package = PCK;
            Ayuda.Procedimiento = "P_CARGAR_TIPO_PARAMETROS";
            Ayuda.Generar_ParametroBD("PIN_DESCRIPCION", txtparametros.Text.ToUpper(), DbType.String, ParameterDirection.Input);
            Ayuda.EjecutarSql();

        }

        #endregion

        #region Buscar Mensajes 

        protected void buscar_mensajes()
        {

            foreach (DataRow row in dt_mensajes.Rows)
            {
                if (row["codigo"].ToString() == "1")
                {
                    msg_reg_corr = row["descripcion"].ToString();
                }
                if (row["codigo"].ToString() == "2")
                {
                    msg_mod_corr = row["descripcion"].ToString();
                }
                if (row["codigo"].ToString() == "3")
                {
                    msg_cam_vaci = row["descripcion"].ToString();
                }
                if (row["codigo"].ToString() == "4")
                {
                    msg_reg_erro = row["descripcion"].ToString();
                }

                if (row["codigo"].ToString() == "5")
                {
                    msg_mod_erro = row["descripcion"].ToString();
                }

                if (row["codigo"].ToString() == "6")
                {
                    msg_con_info = row["descripcion"].ToString();
                }


                if (row["codigo"].ToString() == "7")
                {
                    msg_cam_inco = row["descripcion"].ToString();
                }

                if (row["codigo"].ToString() == "8")
                {
                    msg_cam_nval = row["descripcion"].ToString();
                }
                if (row["codigo"].ToString() == "9")
                {
                    msg_dat_exit = row["descripcion"].ToString();
                }
            }
        }

        #endregion

        #region Guardar

        protected void Guardar_parametros()
        {


            string txtusuario = "SICI";
            string estado = "S";
            if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

            CnnFalp.CrearCommand(CommandType.StoredProcedure, "PCK_NUT001I.P_REGISTRAR_PARAMETROS");

            CnnFalp.ParametroBD("PIN_CODIGO", cod_parametros, DbType.Int64, ParameterDirection.Input);
            CnnFalp.ParametroBD("PIN_DESCRIPCION", txtdescripcion.Text.ToUpper().Trim().Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U"), DbType.String, ParameterDirection.Input);
            CnnFalp.ParametroBD("PIN_USUARIO", txtusuario.ToUpper().Trim(), DbType.String, ParameterDirection.Input);
            int registro = CnnFalp.ExecuteNonQuery();

            CnnFalp.Cerrar();

            if (registro == -1)
            {
                MessageBox.Show("" + msg_reg_corr + "", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("" + msg_reg_erro + "", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        #endregion

        #endregion

        #region Validaciones


        protected Boolean Validar_Campos()
        {
            Boolean var = false;

            if (txtparametros.Text == "" && cod_parametros == 0)
            {
                MessageBox.Show("Estimado usuario, El Campo Tipo Comida se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtparametros.Focus();
            }
            else
            {
                if (txtdescripcion.Text == "" )
                {
                    MessageBox.Show("Estimado usuario, El Campo Descripción se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtdescripcion.Focus();
                }
                else
                {
                    int cont = 0;
                    foreach (DataRow miRow in dt.Select("Descripcion = '" + txtdescripcion.Text.ToUpper() + "'"))
                    {
                        cont++;
                    }

                    if (cont == 0)
                    {
                        var = true;
                    }
                    else
                    {
                        MessageBox.Show("Estimado usuario, La Descripción Ingresado  ya se encuentra Registrado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtdescripcion.Text = "";
                    
                        var = false;
                    }
                }

            }

            return var;

        }


        private void txtparametros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && (e.KeyChar != (char)Keys.Space))
            {
                e.Handled = true;
                return;
            }
            else
            {
                Cargar_parametros();
                if (e.KeyChar == (char)13)
                {

                    if (cod_parametros != 0 && txtparametros.Text != "")
                    {
                        Cargar_grilla();
                        txtdescripcion.Enabled = true;
                        txtdescripcion.Focus();

                    }
                    else
                    {
                        txtdescripcion.Enabled = false;
                        txtparametros.Focus();
                    }



                }
            }
        }

        private void txtdescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && (char.IsNumber(e.KeyChar)) )
            {
                e.Handled = true;
                return;
            }

            else
            {
                if (e.KeyChar == (char)13)
                {

                    if (txtdescripcion.Text == "")
                    {

                        btn_guardar.Enabled = false;
                        txtdescripcion.Focus();
                    }
                    else
                    {
                        btn_guardar.Enabled = true;
                        btn_guardar.Focus();
                    }
                }
            }
     
        }


        private void CambiarBlanco_TextLeave(object sender, EventArgs e)
        {
            TextBox GB = (TextBox)sender;
            GB.BackColor = Color.White;

        }

        private void CambiarColor_TextEnter(object sender, EventArgs e)
        {
            TextBox GB = (TextBox)sender;
            GB.BackColor = Color.FromArgb(255, 224, 192);
        }

        #endregion

      

      


    }
}
