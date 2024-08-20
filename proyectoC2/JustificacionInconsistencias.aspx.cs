using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Net.Mail;
using System.Web.UI;

namespace proyectoC2
{
    public partial class JustificacionInconsistencias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Recuperar valores de la URL
                string idInconsistenciaValor = Request.QueryString["IDInconsistencia"];
                string empleadoIDValor = Request.QueryString["EmpleadoID"];
                string fechaValor = Request.QueryString["Fecha"];
                string tipoInconsistenciaValor = Request.QueryString["TipoInconsistencia"];

                // Asignar valores a los TextBox
                if (!string.IsNullOrEmpty(idInconsistenciaValor))
                {
                    idInconsistencia.Text = idInconsistenciaValor;
                }
                if (!string.IsNullOrEmpty(empleadoIDValor))
                {
                    Empleado.Text = empleadoIDValor;
                }
                if (!string.IsNullOrEmpty(fechaValor))
                {
                    fecha.Text = fechaValor;
                }
                if (!string.IsNullOrEmpty(tipoInconsistenciaValor))
                {
                    tipoInconsistencia.Text = tipoInconsistenciaValor;
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (pdfUpload.HasFile && pdfUpload.PostedFile.ContentLength > 0)
            {
                try
                {
                    // Leer el archivo seleccionado en un array de bytes
                    byte[] fileBytes;
                    using (Stream fileStream = pdfUpload.PostedFile.InputStream)
                    using (BinaryReader binaryReader = new BinaryReader(fileStream))
                    {
                        fileBytes = binaryReader.ReadBytes((int)fileStream.Length);
                    }

                    // Verificar que el ID de inconsistencia es un valor válido
                    int inconsistenciaID;
                    if (int.TryParse(idInconsistencia.Text, out inconsistenciaID))
                    {
                        // Obtener el TipoInconsistenciaID a partir del NombreTipo
                        int tipoInconsistenciaID = ObtenerTipoInconsistenciaID(tipoInconsistencia.Text);

                        if (tipoInconsistenciaID == -1)
                        {
                            lblMensaje.Text = "Tipo de inconsistencia no válido";
                            lblMensaje.CssClass = "mensaje-error";
                            return;
                        }

                        string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                        string query = "UPDATE InconsistenciasFinal SET DocumentoPDF = @DocumentoPDF, TipoInconsistenciaID = @TipoInconsistenciaID, Estado = 'Pendiente', EstadoSolicitud = 'Sin Gestionar' WHERE InconsistenciaID = @InconsistenciaID";

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@DocumentoPDF", fileBytes);
                            command.Parameters.AddWithValue("@TipoInconsistenciaID", tipoInconsistenciaID);
                            command.Parameters.AddWithValue("@InconsistenciaID", inconsistenciaID);

                            connection.Open();
                            int rowsAffected = command.ExecuteNonQuery();
                            connection.Close();

                            if (rowsAffected > 0)
                            {
                                lblMensaje.Text = "Justificación enviada con éxito";
                                lblMensaje.CssClass = "mensaje-exito";

                                // Enviar notificación por correo
                                EnviarCorreoInconsistencia(inconsistenciaID);
                            }
                            else
                            {
                                lblMensaje.Text = "Error al enviar la justificación";
                                lblMensaje.CssClass = "mensaje-error";
                            }
                        }
                    }
                    else
                    {
                        lblMensaje.Text = "ID de inconsistencia no válido";
                        lblMensaje.CssClass = "mensaje-error";
                    }
                }
                catch (Exception ex)
                {
                    lblMensaje.Text = "Error al enviar la justificación: " + ex.Message;
                    lblMensaje.CssClass = "mensaje-error";
                }
            }
            else
            {
                lblMensaje.Text = "Debe seleccionar un archivo para enviar la justificación";
                lblMensaje.CssClass = "mensaje-error";
            }
        }

        private int ObtenerTipoInconsistenciaID(string nombreTipo)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT TipoInconsistenciaID FROM TiposInconsistencias WHERE NombreTipo = @NombreTipo";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@NombreTipo", nombreTipo);

                connection.Open();
                object result = command.ExecuteScalar();
                connection.Close();

                if (result != null && int.TryParse(result.ToString(), out int tipoInconsistenciaID))
                {
                    return tipoInconsistenciaID;
                }
                else
                {
                    return -1; // Valor no encontrado
                }
            }
        }

        private void EnviarCorreoInconsistencia(int inconsistenciaID)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("gestionprositiosweb@gmail.com");
                correo.To.Add("cesar.gomez.calderon@cuc.cr");
                correo.Subject = "Notificación de Inconsistencia en el Sistema de Control Empresarial";
                correo.Body = $@"Hola,

La inconsistencia con ID: '{inconsistenciaID}' ha sido justificada por el empleado.
Apruebe o deniegue la justificación lo antes posible para evitar inconvenientes.

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

