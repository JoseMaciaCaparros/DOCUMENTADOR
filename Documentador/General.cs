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
using Documentador.Models;
using System.IO;
using System.Globalization;

namespace Documentador
{
    public partial class General : Form
    {
        public General()
        {
            InitializeComponent();
        }

        private void PreloadApp(object sender, EventArgs e)
        {
            this.Text = ConfigurationManager.AppSettings["AppName"];
            this.WindowState = FormWindowState.Maximized;
            LABEL_AppVersion.Text = "v" + ConfigurationManager.AppSettings["AppVersion"];
            LoadProjects();
            DataGrid_ListadoProyectos.ReadOnly = true;
        }

        public void LoadProjects()
        {
            LISTADO_ProyectosDocumentar.Items.Clear();
            LISTA_ProyectosBuscador.Items.Clear();
            LISTA_ProyectosEditar.Items.Clear();
            LISTADO_ProyectosLog.Items.Clear();
            LISTADO_Proyecto_Eliminar.Items.Clear();
            string SqlProyectos = "SELECT IdProyecto, Nombre FROM SYS_PROYECTO WHERE Estado='A' ORDER BY Nombre ASC";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlProyectos, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
                if (RS.FieldCount > 0)
                {
                    while (RS.Read())
                    {
                        ComboBoxItem Item = new ComboBoxItem();
                        Item.Value = RS["IdProyecto"];
                        Item.Text = RS["Nombre"].ToString();
                        LISTADO_ProyectosDocumentar.Items.Add(Item);
                        LISTA_ProyectosBuscador.Items.Add(Item);
                        LISTA_ProyectosEditar.Items.Add(Item);
                        LISTADO_Proyecto_Eliminar.Items.Add(Item);
                        LISTADO_ProyectosLog.Items.Add(Item);
                    }
                }
                else
                {
                    MessageBox.Show("Actualmente no existen proyectos en alta.", "Listado de Proyectos", MessageBoxButtons.OK);
                }
            }
        }

        private void CONFIGURACION_AcercaDe(object sender, EventArgs e)
        {
            AppInfo Info = new AppInfo();
            Info.Show();
        }
        
        private void INICIO_Load(object sender, EventArgs e)
        {
            PANEL_PROYECTOS_Listado.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PROYECTOS_NuevoProyecto.Visible = false;
            PANEL_Documentador.Visible = false;
            PANEL_Buscador.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PANEL_Logs.Visible = false;
        }

        private void listarProyectosToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PROYECTOS_NuevoProyecto.Visible = false;
            PANEL_Documentador.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PANEL_Buscador.Visible = false;
            PANEL_Logs.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = false;

            DataTable DT = new DataTable();
            DT.Columns.Add(new DataColumn("Nombre", typeof(string)));
            DT.Columns.Add(new DataColumn("Descripción", typeof(string)));
            DT.Columns.Add(new DataColumn("Ruta", typeof(string)));
            DT.Columns.Add(new DataColumn("Estado", typeof(string)));
            DT.Columns.Add(new DataColumn("Documentado", typeof(string)));
            DataGrid_ListadoProyectos.DataSource = DT;

            string SqlProyectos = "SELECT Nombre, Descripcion, Ruta, CASE WHEN Estado='A' THEN 'Activo' ELSE 'Inactivo' END as Estado, CASE WHEN Documentado='S' THEN 'Si' ELSE 'No' END as Documentado FROM SYS_PROYECTO ORDER BY Nombre ASC";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlProyectos, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
                if (RS.FieldCount>0)
                {                    
                    while (RS.Read())
                    {
                        DT.Rows.Add(RS["Nombre"].ToString(), RS["Descripcion"].ToString(), RS["Ruta"].ToString(), RS["Estado"].ToString(), RS["Documentado"].ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Actualmente no existen proyectos en alta.", "Listado de Proyectos", MessageBoxButtons.OK);
                }
            }
            PANEL_PROYECTOS_Listado.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable DT = new DataTable();
            DT.Columns.Add(new DataColumn("Nombre", typeof(string)));
            DT.Columns.Add(new DataColumn("Descripción", typeof(string)));
            DT.Columns.Add(new DataColumn("Ruta", typeof(string)));
            DT.Columns.Add(new DataColumn("Estado", typeof(string)));
            DT.Columns.Add(new DataColumn("Documentado", typeof(string)));
            DataGrid_ListadoProyectos.DataSource = DT;

            string SqlProyectos = "SELECT Nombre, Descripcion, Ruta, CASE WHEN Estado='A' THEN 'Activo' ELSE 'Inactivo' END as Estado, CASE WHEN Documentado='S' THEN 'Si' ELSE 'No' END as Documentado FROM SYS_PROYECTO ";
            string where = "";

            if (PROYECTOS_Listado_Nombre.Text != "")
            {
                if (where != "")
                    where += " AND Nombre LIKE '%" + PROYECTOS_Listado_Nombre.Text + "%'";
                else
                    where += " WHERE Nombre LIKE '%" + PROYECTOS_Listado_Nombre.Text + "%'";
            }
            if (PROYECTOS_Listado_Estado.Text != "")
            {
                string Estado = "I";
                if (PROYECTOS_Listado_Estado.Text == "Activo")
                    Estado = "A";

                if (where != "")
                    where += " AND Estado='" + Estado + "'";
                else
                    where += " WHERE Estado='" + Estado + "'";
            }
            if (PROYECTOS_Listado_Documentado.Text != "")
            {
                string Documentado = "N";
                if (PROYECTOS_Listado_Documentado.Text == "Si")
                    Documentado = "S";

                if (where != "")
                    where += " AND Documentado='" + Documentado + "'";
                else
                    where += " WHERE Documentado='" + Documentado + "'";
            }

            SqlProyectos += where + " ORDER BY Nombre ASC";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlProyectos, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
                if (RS.FieldCount > 0)
                {
                    while (RS.Read())
                    {
                        DT.Rows.Add(RS["Nombre"].ToString(), RS["Descripcion"].ToString(), RS["Ruta"].ToString(), RS["Estado"].ToString(), RS["Documentado"].ToString());
                    }
                }
                else
                {
                    MessageBox.Show("No existen proyectos con los filtros aplicados.", "Listado de Proyectos", MessageBoxButtons.OK);
                }
            }
        }

        private void nuevoProyectoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PANEL_PROYECTOS_Listado.Visible = false;
            PANEL_Documentador.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = false;
            PANEL_Logs.Visible = false;
            PROYECTOS_NuevoProyecto.Visible = true;
        }

        public bool EsProyectoRepetido(string Nombre, int IdProyecto=0)
        {
            bool ProyectoRepetido = false;
            string SqlProyectoRepetido = "SELECT Nombre FROM SYS_PROYECTO WHERE Nombre='" + Nombre + "'";
            if (IdProyecto > 0)
                SqlProyectoRepetido += " AND IdProyecto!=" + IdProyecto;

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlProyectoRepetido, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
                if (RS.FieldCount > 0)
                {
                    while (RS.Read())
                    {
                        ProyectoRepetido = true;
                    }
                }
            }

            return ProyectoRepetido;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btn_GuardarNuevoProyecto.Enabled = false;
            string Nombre = NombreNuevoProyecto.Text;
            string Descripcion = DescripcionNuevoProyecto.Text;
            string Ruta = RutaNuevoProyecto.Text;
            string Estado = (EstadoNuevoProyecto.Text == "Activo") ? "A" : "I";
            bool ProyectoRepetido = EsProyectoRepetido(Nombre);

            if (!ProyectoRepetido)
            {
                string SqlNuevoProyecto = "INSERT INTO SYS_PROYECTO(Nombre,Ruta,Estado,Documentado,FechaAlta,Descripcion) VALUES('" + Nombre + "','" + Ruta + "','" + Estado + "','N',GetDate(),'" + Descripcion + "')";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command = new SqlCommand(SqlNuevoProyecto, con);
                    con.Open();
                    if (command.ExecuteNonQuery() > 0)
                    {
                        NombreNuevoProyecto.Text = "";
                        DescripcionNuevoProyecto.Text = "";
                        RutaNuevoProyecto.Text = "";
                        EstadoNuevoProyecto.Text = "Activo";
                        LoadProjects();
                        MessageBox.Show("Proyecto creado correctamente.", "Alta de Proyectos", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("Hubo un error al crear el proyecto.\nContacto con el administrador.", "Alta de Proyectos", MessageBoxButtons.OK);
                    }
                }
                btn_GuardarNuevoProyecto.Enabled = true;
            }else
            {
                MessageBox.Show("Ya existe un proyecto con este nombre.\nPor favor, revise el dato indicado para modificarlo.", "Alta de Proyectos", MessageBoxButtons.OK);
            }
        }

        private void documentadorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PANEL_PROYECTOS_Listado.Visible = false;
            PROYECTOS_NuevoProyecto.Visible = false;
            PANEL_Buscador.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = false;
            PANEL_Logs.Visible = false;
            PANEL_Documentador.Visible = true;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string Project = LISTADO_ProyectosDocumentar.Text;
            int IdProyecto = 0;
            int TotalFicherosProyecto = 0;
            float PorcentajePorIteracion = 0;
            ProgresoDocumentacion.Value = 0;
            LogDocumentacion.Text = "";

            LogDocumentacion.Text += "[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Inicio de la documentación del proyecto " + Project + ".";
            ProgresoDocumentacion.Value = 1;
            ProgresoDocumentacion.Refresh();
            LogDocumentacion.Refresh();
            LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Recuperando path del proyecto.";
            ProgresoDocumentacion.Value += 1;
            ProgresoDocumentacion.Refresh();
            LogDocumentacion.Refresh();
            string Ruta = "";
            string SqlDocumentarProyecto = "SELECT IdProyecto,Ruta FROM SYS_PROYECTO WHERE Nombre='" + Project + "'";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlDocumentarProyecto, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
                if (RS.FieldCount > 0)
                {
                    while (RS.Read())
                    {
                        Ruta = RS["Ruta"].ToString();
                        IdProyecto = System.Convert.ToInt32(RS["IdProyecto"]);
                    }
                }
                else
                {
                    MessageBox.Show("Hubo un error al intentar documentar el proyecto.", "Documentación de Proyectos", MessageBoxButtons.OK);
                }
            }
            string SqlDocumentarProyectoFichero = "DELETE FROM SYS_PROYECTO_FICHERO WHERE IdProyecto=" + IdProyecto;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlDocumentarProyectoFichero, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
            }
            string SqlDocumentarProyectoDeleteDetalle = "DELETE FROM SYS_PROYECTO_DETALLE WHERE IdProyecto=" + IdProyecto;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlDocumentarProyectoDeleteDetalle, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
            }
            LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Path del proyecto recuperado.";
            ProgresoDocumentacion.Value += 2;
            ProgresoDocumentacion.Refresh();
            LogDocumentacion.Refresh();
            LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Procesando documentación.";
            ProgresoDocumentacion.Value += 1;
            ProgresoDocumentacion.Refresh();
            LogDocumentacion.Refresh();
            string[] files = Directory.GetFiles(Ruta, "*", SearchOption.AllDirectories);
            TotalFicherosProyecto = files.Length;
            PorcentajePorIteracion = 90 / TotalFicherosProyecto;
            foreach (var file in files)
            {
                LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Procesando el fichero " + file;
                LogDocumentacion.Refresh();
                LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Obteniendo indentificador por el fichero.";
                LogDocumentacion.Refresh();
                int IdFichero = RegistraFichero(file, IdProyecto);
                LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Identificador (" + IdFichero + ") obtenido para el fichero a procesar.";
                LogDocumentacion.Refresh();
                LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Leyendo fichero de datos.";
                LogDocumentacion.Refresh();

                bool Lectura = GetContentCompleteToFile(file, null, null, IdProyecto, IdFichero);
                string EstadoLectura = "correctamente";
                if (!Lectura)
                    EstadoLectura = "con errores";

                LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Fichero leido " + EstadoLectura + ".";
                LogDocumentacion.Refresh();
                if (ProgresoDocumentacion.Value < 90)
                {
                    ProgresoDocumentacion.Value += System.Convert.ToInt32(PorcentajePorIteracion);
                    ProgresoDocumentacion.Refresh();
                }
            }

            LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Guardando log de sucesos.";
            LogDocumentacion.Refresh();
            SaveLog(IdProyecto);
            LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Log de sucesos guardado.";
            LogDocumentacion.Refresh();

            LogDocumentacion.Text += "\r\n[" + DateTime.Now.ToString("dd-MM-yyyy") + "] [" + DateTime.Now.ToString("HH:mm:ss") + "] - Proceso de documentación finalizado.";
            ProgresoDocumentacion.Value = 100;
            ProgresoDocumentacion.Refresh();
            LogDocumentacion.Refresh();
            FinalizaLogDocumentacion(IdProyecto);

            //btn_DownloadLog.Visible = true;
            MessageBox.Show("Ha finalizado la documentación.", "Documentación de Proyectos", MessageBoxButtons.OK);
        }

        private bool GetContentCompleteToFile(string ProjectFile, string SearchType, string SearchParam, int IdProyecto, int IdFichero)
        {
            string contentFile = "";
            bool Lectura = true;
            if (File.Exists(ProjectFile))
            {
                string linea = "";
                int contadorLineas = 1;
                System.IO.StreamReader FData = new System.IO.StreamReader(ProjectFile);
                while ((linea = FData.ReadLine()) != null)
                {
                    string SqlDocumentarProyecto = "INSERT INTO SYS_PROYECTO_DETALLE(IdProyecto,IdFichero,Linea, NumLinea) VALUES(" + IdProyecto + ", " + IdFichero + ", '" + LimpiaContenido(linea) + "'," + contadorLineas + ")";
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                    {
                        SqlCommand command = new SqlCommand(SqlDocumentarProyecto, con);
                        con.Open();
                        if (command.ExecuteNonQuery() < 1)
                        {
                            Lectura = false;
                        }
                    }
                    contadorLineas++;
                }
                FData.Close();
            }
            return Lectura;

        }

        public int RegistraFichero(string File, int IdProyecto)
        {
            bool Insertado = false;
            int IdFichero = 0;
            string FileName = Path.GetFileName(File);
            string FilePath = File.Replace(FileName, "");
            string SqlFile = "INSERT INTO SYS_PROYECTO_FICHERO(NombreFichero,RutaFichero,IdProyecto) VALUES('" + FileName + "','" + FilePath + "'," + IdProyecto + ")";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlFile, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
                if (RS.FieldCount < 1)
                {
                    Insertado = true;
                }
            }
            if (Insertado)
            {
                string SqlLastFile = "SELECT TOP 1 IdFichero FROM SYS_PROYECTO_FICHERO WHERE IdProyecto=" + IdProyecto + " AND NombreFichero='" + FileName + "' ORDER BY IdFichero DESC";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command = new SqlCommand(SqlLastFile, con);
                    con.Open();
                    var RS = command.ExecuteReader();
                    if (RS.FieldCount > 0)
                    {
                        IdFichero = RS.Read() ? RS.GetInt32(0) : 0;
                    }
                }
            }
            return IdFichero;
        }

        public string LimpiaContenido(string content)
        {
            content = content.Replace("\t", "");
            content = content.Replace("'", "''");
            return content;
        }

        public void SaveLog(int IdProyecto)
        {
            string SqlSaveLog = "INSERT INTO SYS_LOG(TypeLog, DateLog, LogText, IdProyecto) VALUES('D',GetDate(),N'" + LimpiaContenido(LogDocumentacion.Text) + "'";

            if (IdProyecto > 0)
                SqlSaveLog += ", " + IdProyecto + ")";
            else
                SqlSaveLog += ", null)";

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlSaveLog, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
            }
        }

        public void FinalizaLogDocumentacion(int IdProyecto)
        {
            string SqlDocumentarProyecto = "UPDATE SYS_PROYECTO SET UltimoProcesado=GetDate(), Documentado='S' WHERE IdProyecto=" + IdProyecto;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlDocumentarProyecto, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
            }
        }

        private void btn_DownloadLog_Click(object sender, EventArgs e)
        {
           //Do-Nothing
        }

        private void btn_BUSCAR_Click(object sender, EventArgs e)
        {
            string Proyecto = LISTA_ProyectosBuscador.Text;
            string Busqueda = CustomSearch.Text;
            if (Proyecto != "")
            {
                if (Busqueda != "")
                {
                    DataTable DT = new DataTable();
                    string SqlBusqueda = "";
                    if (LISTADO_TipoBusqueda.Text == "Contenido de Fichero")
                    {
                        DT.Columns.Add(new DataColumn("Fichero", typeof(string)));
                        DT.Columns.Add(new DataColumn("Path", typeof(string)));
                        DT.Columns.Add(new DataColumn("Num. Línea", typeof(string)));
                        DT.Columns.Add(new DataColumn("Linea", typeof(string)));

                        Busqueda = LimpiaContenido(Busqueda);
                        SqlBusqueda = "SELECT F.NombreFichero,F.RutaFichero,D.NumLinea,D.Linea FROM SYS_PROYECTO_DETALLE D INNER JOIN SYS_PROYECTO_FICHERO F ON D.IdFichero = F.IdFichero INNER JOIN SYS_PROYECTO P ON P.idProyecto = F.idProyecto WHERE D.Linea LIKE '%" + Busqueda + "%' AND P.Nombre = '" + Proyecto + "' ORDER BY F.NombreFichero, D.NumLinea";
                    }else
                    {
                        if (LISTADO_TipoBusqueda.Text == "Nombre de Fichero" || LISTADO_TipoBusqueda.Text == "Extensión de Fichero")
                        {
                            DT.Columns.Add(new DataColumn("Path", typeof(string)));
                            DT.Columns.Add(new DataColumn("Fichero", typeof(string)));
                            SqlBusqueda = "SELECT F.NombreFichero,F.RutaFichero FROM SYS_PROYECTO_FICHERO F INNER JOIN SYS_PROYECTO P ON P.idProyecto = F.idProyecto WHERE F.NombreFichero LIKE '%" + Busqueda + "%' AND P.Nombre = '" + Proyecto + "' ORDER BY F.NombreFichero";
                        }else
                        {
                            DT.Columns.Add(new DataColumn("Path", typeof(string)));
                            DT.Columns.Add(new DataColumn("Fichero", typeof(string)));
                            SqlBusqueda = "SELECT F.NombreFichero,F.RutaFichero FROM SYS_PROYECTO_FICHERO F INNER JOIN SYS_PROYECTO P ON P.idProyecto = F.idProyecto WHERE F.RutaFichero LIKE '%" + Busqueda + "%' AND P.Nombre = '" + Proyecto + "' ORDER BY F.RutaFichero, F.NombreFichero";
                        }
                    }

                    GridResultadosBuscador.DataSource = DT;
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                    {
                        SqlCommand command = new SqlCommand(SqlBusqueda, con);
                        con.Open();
                        SqlDataReader RS = command.ExecuteReader();
                        if (RS.FieldCount > 0)
                        {
                            while (RS.Read())
                            {
                                if (LISTADO_TipoBusqueda.Text == "Contenido de Fichero")
                                    DT.Rows.Add(RS["NombreFichero"].ToString(), RS["RutaFichero"].ToString(), RS["NumLinea"].ToString(), RS["Linea"].ToString());
                                else
                                    DT.Rows.Add(RS["RutaFichero"].ToString(), RS["NombreFichero"].ToString());
                            }
                        }
                        else
                        {
                            MessageBox.Show("No pudimos encontrar nada con la búsqueda realizada.\nPruebe a cambiar el filtro de búsqueda.", "Búsqueda en Proyectos", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Debe indicar el fragmento de texto que desea buscar.", "Búsqueda en Proyectos", MessageBoxButtons.OK);
                }
            }else
            {
                MessageBox.Show("Debe seleccionar el proyecto sobre el que desea buscar.", "Búsqueda en Proyectos", MessageBoxButtons.OK);
            }
        }

        private void buscadorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PANEL_PROYECTOS_Listado.Visible = false;
            PROYECTOS_NuevoProyecto.Visible = false;
            PANEL_Documentador.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PANEL_Logs.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = false;
            PANEL_Buscador.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Update del proyecto
        }

        private void ActualizaInformacionProyecto(object sender, EventArgs e)
        {
            //GetDatosProyecto
            string NombreProyecto = LISTA_ProyectosEditar.Text;
            if(NombreProyecto == "")
            {
                PANEL_DATOS_Editar_Proyecto.Visible = false;
            }else
            {
                string SqlGetDatosProyectoEditar = "SELECT IdProyecto, Nombre, Ruta, CASE WHEN Estado='A' THEN 'Activo' ELSE 'Inactivo' END as Estado, CASE WHEN Documentado='S' THEN 'Si' ELSE 'No' END as Documentado, CASE WHEN UltimoProcesado IS NULL THEN 'Sin Procesar' ELSE CONVERT(varchar, UltimoProcesado, 103) + '  ' + LEFT(CONVERT(VARCHAR, CONVERT(TIME, UltimoProcesado), 120), 10) END as UltimoProcesado, CONVERT(varchar, FechaAlta, 103) + '  ' + LEFT(CONVERT(VARCHAR, CONVERT(TIME, FechaAlta), 120), 10) as FechaAlta, Descripcion FROM SYS_PROYECTO WHERE Nombre='" + NombreProyecto + "'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command = new SqlCommand(SqlGetDatosProyectoEditar, con);
                    con.Open();
                    SqlDataReader RS = command.ExecuteReader();
                    if (RS.HasRows)
                    {
                        RS.Read();
                        PROYECTO_EDITAR_Id.Text = RS.GetInt32(0).ToString();
                        PROYECTO_EDITAR_Nombre.Text = RS.GetString(1);
                        PROYECTO_EDITAR_Ruta.Text = RS.GetString(2);
                        PROYECTO_EDITAR_Descripcion.Text = RS.GetString(7);
                        PROYECTO_EDITAR_Estado.Text = RS.GetString(3);
                        PROYECTO_EDITAR_FechaAlta.Text = RS.GetString(6);
                        PROYECTO_EDITAR_Documentado.Text = RS.GetString(4);
                        PROYECTO_EDITAR_UltimoProcesado.Text = RS.GetString(5);
                        PANEL_DATOS_Editar_Proyecto.Visible = true;
                    }
                    else
                    {
                        MessageBox.Show("Hubo un error al intentar recuperar la información del proyecto " + NombreProyecto + ".", "Edición de Proyectos", MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void editarProyectoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PROYECTO_EDITAR_Id.Text = "";
            PROYECTO_EDITAR_Nombre.Text = "";
            PROYECTO_EDITAR_Ruta.Text = "";
            PROYECTO_EDITAR_Descripcion.Text = "";
            PROYECTO_EDITAR_Estado.Text = "";
            PROYECTO_EDITAR_Documentado.Text = "";
            PROYECTO_EDITAR_UltimoProcesado.Text = "";
            PROYECTO_EDITAR_FechaAlta.Text = "";
            LISTA_ProyectosEditar.Text = "";
            PANEL_DATOS_Editar_Proyecto.Visible = false;

            PROYECTOS_NuevoProyecto.Visible = false;
            PANEL_Documentador.Visible = false;
            PANEL_Buscador.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = false;
            PANEL_Logs.Visible = false;
            PANEL_EditarProyecto.Visible = true;
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (!EsProyectoRepetido(PROYECTO_EDITAR_Nombre.Text, System.Convert.ToInt32(PROYECTO_EDITAR_Id.Text)))
            {
                string Estado = "A";
                string SqlUpdateProyecto = "UPDATE SYS_PROYECTO SET ";
                if (PROYECTO_EDITAR_Nombre.Text != "")
                    SqlUpdateProyecto += "Nombre='" + PROYECTO_EDITAR_Nombre.Text + "'";
                if (PROYECTO_EDITAR_Ruta.Text != "")
                    SqlUpdateProyecto += ", Ruta='" + PROYECTO_EDITAR_Ruta.Text + "'";
                if (PROYECTO_EDITAR_Descripcion.Text != "")
                    SqlUpdateProyecto += ", Descripcion='" + PROYECTO_EDITAR_Descripcion.Text + "'";
                if (PROYECTO_EDITAR_Estado.Text != "")
                {
                    if (PROYECTO_EDITAR_Estado.Text == "Inactivo")
                        Estado = "I";

                    SqlUpdateProyecto += ", Estado='" + Estado + "'";
                }

                bool ProcesarActualizacion = true;
                if (Estado == "I")
                {
                    var confirmResultTask = MessageBox.Show("Se va a desactivar el proyecto " + PROYECTO_EDITAR_Nombre.Text + ".\n\n¿Desea continuar?", "Cancelar Ejecución", MessageBoxButtons.YesNo);
                    if (confirmResultTask == DialogResult.No)
                    {
                        ProcesarActualizacion = false;
                    }
                }

                if (ProcesarActualizacion)
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                    {
                        SqlCommand command = new SqlCommand(SqlUpdateProyecto, con);
                        con.Open();
                        if (command.ExecuteNonQuery() > 0)
                            MessageBox.Show("Proyecto actualizado correctamente.", "Edición de Proyectos", MessageBoxButtons.OK);
                        else
                            MessageBox.Show("Hubo un error al intentar actualizar el proyecto.", "Edición de Proyectos", MessageBoxButtons.OK);
                    }
                }

            }else{ 
                MessageBox.Show("Ya existe un proyecto con este nombre.\nPor favor, revise el dato indicado para ser modificado.", "Edición de Proyectos", MessageBoxButtons.OK);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string NombreProyecto = LISTADO_Proyecto_Eliminar.Text;
            int IdProyecto = 0;

            var confirmResultTask = MessageBox.Show("Se va a eliminar el proyecto " + NombreProyecto + ".\nRecuerde que una vez eliminado no podrá recuperarse.\n\n¿Desea continuar?", "Cancelar Ejecución", MessageBoxButtons.YesNo);
            if (confirmResultTask == DialogResult.Yes)
            {
                //Obtenemos el ID del proyecto
                string SqlGetIdProyecto = "SELECT IdProyecto FROM SYS_PROYECTO WHERE Nombre='" + NombreProyecto + "'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command = new SqlCommand(SqlGetIdProyecto, con);
                    con.Open();
                    SqlDataReader RS = command.ExecuteReader();
                    if (RS.HasRows)
                    {
                        RS.Read();
                        IdProyecto = RS.GetInt32(0);
                    }
                }

                if(IdProyecto > 0)
                {
                    //Borramos el proyecto
                    string SqlBorrarProyecto = "DELETE FROM SYS_PROYECTO WHERE IdProyecto=" + IdProyecto;
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                    {
                        SqlCommand command = new SqlCommand(SqlBorrarProyecto, con);
                        con.Open();
                        if (command.ExecuteNonQuery() > 0)
                        {
                            //Borramos los documentos relacionados con el proyecto
                            string SqlBorrarDocumentosProyecto = "DELETE FROM SYS_PROYECTO_FICHERO WHERE IdProyecto=" + IdProyecto;
                            using (SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                            {
                                SqlCommand command2 = new SqlCommand(SqlBorrarDocumentosProyecto, con2);
                                con2.Open();
                                if (command2.ExecuteNonQuery() > 0)
                                {
                                    //Borramos los datos referentes a los documentos del proyecto
                                    string SqlBorrarDatosFicherosProyecto = "DELETE FROM SYS_PROYECTO_DETALLE WHERE IdProyecto=" + IdProyecto;
                                    using (SqlConnection con3 = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                                    {
                                        SqlCommand command3 = new SqlCommand(SqlBorrarDatosFicherosProyecto, con3);
                                        con3.Open();
                                        if (command3.ExecuteNonQuery() > 0)
                                        {
                                            //Borramos los datos referentes a los logs del proyecto
                                            string SqlBorrarLogsProyecto = "DELETE FROM SYS_LOG WHERE IdProyecto=" + IdProyecto;
                                            using (SqlConnection con4 = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                                            {
                                                SqlCommand command4 = new SqlCommand(SqlBorrarLogsProyecto, con3);
                                                con3.Open();
                                                if (command3.ExecuteNonQuery() > 0)
                                                {
                                                    LISTADO_Proyecto_Eliminar.Text = "";
                                                    LoadProjects();
                                                    MessageBox.Show("Proyecto eliminado con éxito.", "Eliminar Proyectos", MessageBoxButtons.OK);
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Hubo un error al intentar borrar los logs del proyecto.\nContacto con el administrador.", "Eliminar Proyectos", MessageBoxButtons.OK);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Hubo un error al intentar borrar los datos de los ficheros del proyecto.\nContacto con el administrador.", "Eliminar Proyectos", MessageBoxButtons.OK);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Hubo un error al intentar borrar los ficheros del proyecto.\nContacto con el administrador.", "Eliminar Proyectos", MessageBoxButtons.OK);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Hubo un error al intentar borrar el proyecto.\nContacto con el administrador.", "Eliminar Proyectos", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Hubo un error al intentar obtener el proyecto a eliminar.\nContacto con el administrador.", "Eliminar Proyectos", MessageBoxButtons.OK);
                }
            }
        }

        private void eliminarProyectoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PANEL_PROYECTOS_Listado.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PROYECTOS_NuevoProyecto.Visible = false;
            PANEL_Documentador.Visible = false;
            PANEL_Buscador.Visible = false;
            PANEL_Logs.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = true;
        }

        private void logsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PANEL_PROYECTOS_Listado.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PROYECTOS_NuevoProyecto.Visible = false;
            PANEL_Documentador.Visible = false;
            PANEL_Buscador.Visible = false;
            PANEL_Eliminar_Proyecto.Visible = false;
            PANEL_EditarProyecto.Visible = false;
            PANEL_Logs.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LOGS_Container.Visible = false;
            LOGS_Container.Text = "";

            string Proyecto = LISTADO_ProyectosLog.Text;
            string Fecha = FECHAS_ProyectosLog.Text;
            int IdProyecto = 0;

            if(Proyecto=="" || Fecha == "")
            {
                MessageBox.Show("Debe indicar el proyecto y fecha de log.", "Logs de Proyectos", MessageBoxButtons.OK);
            }else
            {
                string SqlGetIdProyecto = "SELECT IdProyecto FROM SYS_PROYECTO WHERE Nombre='" + Proyecto + "'";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command = new SqlCommand(SqlGetIdProyecto, con);
                    con.Open();
                    SqlDataReader RS = command.ExecuteReader();
                    if (RS.HasRows)
                    {
                        RS.Read();
                        IdProyecto = RS.GetInt32(0);
                    }
                }

                string SqlGetLog = "SELECT LogText FROM SYS_LOG WHERE IdProyecto=" + IdProyecto + " AND CONVERT(varchar(MAX),DateLog)='" + Fecha + "'";
                using (SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command2 = new SqlCommand(SqlGetLog, con2);
                    con2.Open();
                    SqlDataReader RS = command2.ExecuteReader();
                    if (RS.HasRows)
                    {
                        RS.Read();
                        LOGS_Container.Text = RS.GetString(0);
                        LOGS_Container.Visible = true;
                    }
                }
            }
    }

        private void CargaFechasLog(object sender, EventArgs e)
        {
            FECHAS_ProyectosLog.Items.Clear();
            int IdProyecto = 0;
            string Proyecto = LISTADO_ProyectosLog.Text;
            string SqlGetIdProyecto = "SELECT IdProyecto FROM SYS_PROYECTO WHERE Nombre='" + Proyecto + "'";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
            {
                SqlCommand command = new SqlCommand(SqlGetIdProyecto, con);
                con.Open();
                SqlDataReader RS = command.ExecuteReader();
                if (RS.HasRows)
                {
                    RS.Read();
                    IdProyecto = RS.GetInt32(0);
                }
            }

            if (IdProyecto > 0)
            {
                string SqlGetLogsProyecto = "SELECT CONVERT(varchar(MAX),DateLog) as Fecha FROM SYS_LOG WHERE IdProyecto=" + IdProyecto;
                using (SqlConnection con2 = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ToString()))
                {
                    SqlCommand command2 = new SqlCommand(SqlGetLogsProyecto, con2);
                    con2.Open();
                    SqlDataReader RS = command2.ExecuteReader();
                    if (RS.HasRows)
                    {
                        while (RS.Read())
                        {
                            ComboBoxItem Item = new ComboBoxItem();
                            Item.Text = RS["Fecha"].ToString();
                            Item.Value = RS["Fecha"].ToString();
                            FECHAS_ProyectosLog.Items.Add(Item);
                        }
                    }
                }
            }
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var confirmResultTask = MessageBox.Show("Se va a cerrar la aplicación.\n\n¿Desea continuar?", "Cerrar Aplicación", MessageBoxButtons.YesNo);
            if (confirmResultTask == DialogResult.Yes)
                this.Dispose();
        }

        private void ConfirmarSalida(object sender, FormClosingEventArgs e)
        {
            var confirmResultTask = MessageBox.Show("Se va a cerrar la aplicación.\n\n¿Desea continuar?", "Cerrar Aplicación", MessageBoxButtons.YesNo);
            e.Cancel = (confirmResultTask == DialogResult.No);
        }
    }
    }
