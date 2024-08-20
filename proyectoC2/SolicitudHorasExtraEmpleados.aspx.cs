using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class SolicitudHorasExtraEmpleados : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verifica si existe el parámetro HoraExtraID en la URL
                string horaExtraIDParam = Request.QueryString["HoraExtraID"];
                if (!string.IsNullOrEmpty(horaExtraIDParam))
                {
                    int horaExtraID;
                    if (int.TryParse(horaExtraIDParam, out horaExtraID))
                    {
                        // Llama a un método para cargar los datos de la base de datos
                        CargarDatosSolicitud(horaExtraID);
                    }
                }
            }
        }

        private void CargarDatosSolicitud(int horaExtraID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Fecha, HoraInicio, HoraFinal, CantidadHoras FROM HorasExtraFinal WHERE HoraExtraID = @HoraExtraID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@HoraExtraID", horaExtraID);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        idSolicitud.Text = horaExtraID.ToString();
                        fecha.Text = Convert.ToDateTime(reader["Fecha"]).ToString("dd-MM-yyyy");

                        // Manejo de TimeSpan para HoraInicio y HoraFinal
                        TimeSpan horaInicioTS = (TimeSpan)reader["HoraInicio"];
                        TimeSpan horaFinTS = (TimeSpan)reader["HoraFinal"];
                        horaInicio.Text = horaInicioTS.ToString(@"hh\:mm");
                        horaFin.Text = horaFinTS.ToString(@"hh\:mm");

                        cantidadHoras.Text = reader["CantidadHoras"].ToString();
                    }
                }
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ActualizarEstadoSolicitud("Aceptada");
        }

        protected void btnSubmit2_Click(object sender, EventArgs e)
        {
            ActualizarEstadoSolicitud("Denegada");
        }

        private void ActualizarEstadoSolicitud(string estadoSolicitud)
        {
            try
            {
                int horaExtraID = int.Parse(idSolicitud.Text); // Asumiendo que el ID está en el TextBox idSolicitud

                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE HorasExtraFinal SET EstadoSolicitud = @EstadoSolicitud WHERE HoraExtraID = @HoraExtraID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@EstadoSolicitud", estadoSolicitud);
                        command.Parameters.AddWithValue("@HoraExtraID", horaExtraID);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            lblMensaje.CssClass = "mensaje-exito";
                            if (estadoSolicitud == "Aceptada")
                            {
                                lblMensaje.Text = "Solicitud aceptada con éxito.";
                            }
                            else if (estadoSolicitud == "Denegada")
                            {
                                lblMensaje.Text = "Solicitud denegada correctamente.";
                            }

                            // Enviar correo de notificación
                            EnviarCorreoHorasExtra(horaExtraID, estadoSolicitud);
                        }
                        else
                        {
                            lblMensaje.CssClass = "mensaje-error";
                            lblMensaje.Text = "Error al actualizar el estado de la solicitud.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Text = "Ocurrió un error: " + ex.Message;
            }
        }

        private void EnviarCorreoHorasExtra(int horaExtraID, string estadoSolicitud)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("gestionprositiosweb@gmail.com");
                correo.To.Add("cesar.gomez.calderon@cuc.cr");
                correo.Subject = "Notificación de Horas Extra en el Sistema de Control Empresarial";
                correo.Body = $@"Hola,

La solicitud de horas extra con el ID '{horaExtraID}' ha sido '{estadoSolicitud}'.

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
                // Registrar o manejar el error si ocurre al enviar el correo
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Text = "Error al enviar la notificación por correo.";
            }
        }
    }
}