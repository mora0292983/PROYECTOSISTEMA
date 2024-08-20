using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
                // Establecer la fecha actual en el TextBox de fecha en el formato dd-MM-yyyy
                fecha.Text = DateTime.Now.ToString("dd-MM-yyyy");

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
            if (string.IsNullOrEmpty(fecha.Text) || ddlTipoActividad.SelectedIndex == 0 ||
                ddlHoraInicio.SelectedIndex == 0 || ddlHoraFin.SelectedIndex == 0 || !pdfUpload.HasFile)
            {
                lblMensaje.Text = "Debe completar todos los campos y adjuntar un archivo.";
                lblMensaje.CssClass = "mensaje";
                return;
            }

            // Validar que la fecha esté en el formato correcto
            DateTime fechaActividad;
            if (!DateTime.TryParseExact(fecha.Text, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out fechaActividad))
            {
                lblMensaje.Text = "Formato de fecha inválido. Utilice dd-MM-yyyy.";
                lblMensaje.CssClass = "mensaje";
                return;
            }

            // Validar que la hora de inicio y fin estén dentro del rango permitido (14:00 - 22:00)
            DateTime horaInicio = DateTime.Parse(ddlHoraInicio.SelectedValue);
            DateTime horaFin = DateTime.Parse(ddlHoraFin.SelectedValue);

            if (horaInicio.Hour < 14 || horaInicio.Hour > 22 || (horaInicio.Hour == 22 && horaInicio.Minute > 0) ||
                horaFin.Hour < 14 || horaFin.Hour > 22 || (horaFin.Hour == 22 && horaFin.Minute > 0))
            {
                lblMensaje.Text = "Las horas de inicio y fin deben estar dentro del horario establecido.";
                lblMensaje.CssClass = "mensaje";
                return;
            }

            // Validar que la hora de inicio no sea posterior a la hora de fin y que no sean iguales
            if (horaInicio >= horaFin)
            {
                lblMensaje.Text = "La hora de inicio debe ser anterior a la hora de fin, y no pueden ser iguales.";
                lblMensaje.CssClass = "mensaje";
                return;
            }

            try
            {
                // Obtener los datos del formulario
                int tipoActividadID = int.Parse(ddlTipoActividad.SelectedValue);

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
                        command.Parameters.AddWithValue("@HoraInicio", horaInicio.ToString("HH:mm"));
                        command.Parameters.AddWithValue("@HoraFin", horaFin.ToString("HH:mm"));
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

                            // Enviar correo de notificación
                            EnviarCorreoActividad();
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


        private void EnviarCorreoActividad()
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("gestionprositiosweb@gmail.com");
                correo.To.Add("cesar.gomez.calderon@cuc.cr");
                correo.Subject = "Notificación de Actividad en el Sistema de Control Empresarial";
                correo.Body = $@"Hola,

Se ha registrado una nueva actividad realizada por el empleado con ID: '19'.

Saludos cordiales,

Atentamente,
Gestión Pro";

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("gestionprositiosweb@gmail.com", "zpmkjyrxkdrgprfm");
                smtp.EnableSsl = true;
                smtp.Send(correo);
            }
            catch (Exception ex)
            {
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Text = "Error al enviar la notificación por correo: " + ex.Message;
            }
        }
    }
}