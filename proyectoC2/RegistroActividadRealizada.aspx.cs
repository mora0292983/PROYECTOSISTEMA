using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class RegistroActividadRealizada : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                // Cargar los tipos de actividad desde la base de datos
                CargarTiposDeActividad();

                // Cargar las horas en los DropDownLists de horaInicio y horaFin
                CargarHoras(ddlHoraInicio);
                CargarHoras(ddlHoraFin);
            }

        }

        private void CargarHoras(DropDownList ddl)
        {
            ddl.Items.Clear();
            for (int hour = 0; hour < 24; hour++)
            {
                for (int minute = 0; minute <= 30; minute += 30)
                {
                    string time = $"{hour:00}:{minute:00}";
                    ddl.Items.Add(new ListItem(time, time));
                }
            }
            ddl.Items.Insert(0, new ListItem("-- Selecciona una Hora --", "0"));
        }

        private void CargarTiposDeActividad()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT TipoActividadID, NombreTipo FROM TipoActividadFinal";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    ddlTipoActividad.DataSource = reader;
                    ddlTipoActividad.DataTextField = "NombreTipo"; // El nombre que se mostrará en el DropDownList
                    ddlTipoActividad.DataValueField = "TipoActividadID"; // El valor que se almacenará para cada opción
                    ddlTipoActividad.DataBind();
                }
            }

            ddlTipoActividad.Items.Insert(0, new ListItem("-- Seleccione un tipo de actividad --", "0"));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Verificar que todos los campos obligatorios estén completos
            if (string.IsNullOrEmpty(fecha.Text) || ddlTipoActividad.SelectedIndex == -1 ||
                ddlHoraInicio.SelectedIndex == -1 || ddlHoraFin.SelectedIndex == -1 || !pdfUpload.HasFile)
            {
                lblMensaje.Text = "Debe completar todos los campos y adjuntar un archivo.";
                lblMensaje.CssClass = "mensaje";
                return;
            }

            try
            {
                // Obtener los datos del formulario
                DateTime fechaActividad = DateTime.Parse(fecha.Text);
                int tipoActividadID = int.Parse(ddlTipoActividad.SelectedValue);
                string horaInicio = ddlHoraInicio.SelectedValue;
                string horaFin = ddlHoraFin.SelectedValue;

                // Leer el archivo PDF
                byte[] fileBytes;
                using (Stream fileStream = pdfUpload.PostedFile.InputStream)
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    fileBytes = binaryReader.ReadBytes((int)fileStream.Length);
                }

                // Conexión a la base de datos
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO ActividadesFinal (EmpleadoID, TipoActividadID, Fecha, horaInicio, horaFin, Estado, EstadoSolicitud, Rendimiento, DocumentoPDF) " +
                                   "VALUES (@EmpleadoID, @TipoActividadID, @Fecha, @HoraInicio, @HoraFin, @Estado, @EstadoSolicitud, @Rendimiento, @DocumentoPDF)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EmpleadoID", 19);
                        command.Parameters.AddWithValue("@TipoActividadID", tipoActividadID);
                        command.Parameters.AddWithValue("@Fecha", fechaActividad);
                        command.Parameters.AddWithValue("@HoraInicio", horaInicio);
                        command.Parameters.AddWithValue("@HoraFin", horaFin);
                        command.Parameters.AddWithValue("@Estado", "Pendiente");
                        command.Parameters.AddWithValue("@EstadoSolicitud", "Sin Gestionar");
                        command.Parameters.AddWithValue("@Rendimiento", "Pendiente");
                        command.Parameters.AddWithValue("@DocumentoPDF", fileBytes);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lblMensaje.Text = "Actividad registrada con éxito.";
                            lblMensaje.CssClass = "mensaje-exito";
                        }
                        else
                        {
                            lblMensaje.Text = "Error al registrar la actividad.";
                            lblMensaje.CssClass = "mensaje";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al registrar la actividad: " + ex.Message;
                lblMensaje.CssClass = "mensaje";
            }
        }


    }
}