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
    public partial class gestionHorasExtra : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Obtener el HoraExtraID de la query string
                string horaExtraIDParam = Request.QueryString["HoraExtraID"];

                // Verificar si el parámetro HoraExtraID es válido
                if (int.TryParse(horaExtraIDParam, out int horaExtraID))
                {
                    // Cargar los datos de la hora extra
                    CargarDatosHoraExtra(horaExtraID);

                    // Configurar el enlace para ver el archivo PDF
                    lnkVerArchivo.NavigateUrl = $"~/VerPDF3.aspx?HoraExtraID={horaExtraID}";
                }
                else
                {
                    lblMensaje.Text = "No se recibió un ID de solicitud válido.";
                }
            }
        }


        private void CargarDatosHoraExtra(int horaExtraID)
        {
            // Cadena de conexión desde tu archivo web.config
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT HoraExtraID, Fecha, HoraInicio, HoraFinal FROM HorasExtraFinal WHERE HoraExtraID = @HoraExtraID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@HoraExtraID", horaExtraID);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Cargar datos en los controles
                        idSolicitud.Text = reader["HoraExtraID"].ToString();
                        fecha.Text = Convert.ToDateTime(reader["Fecha"]).ToString("dd-MM-yyyy");
                        horaInicio.Text = TimeSpan.Parse(reader["HoraInicio"].ToString()).ToString(@"hh\:mm");
                        horaFin.Text = TimeSpan.Parse(reader["HoraFinal"].ToString()).ToString(@"hh\:mm");
                    }
                    con.Close();
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ActualizarEstado("Aprobada", "Horas extra aprobadas correctamente");
        }

        protected void btnSubmit2_Click(object sender, EventArgs e)
        {
            ActualizarEstado("Rechazada", "Horas extra rechazadas correctamente");
        }

        private void ActualizarEstado(string nuevoEstado, string mensajeExito)
        {
            int horaExtraID;
            if (int.TryParse(idSolicitud.Text, out horaExtraID))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                string query = "UPDATE HorasExtraFinal SET Estado = @Estado WHERE HoraExtraID = @HoraExtraID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@HoraExtraID", horaExtraID);

                    connection.Open();
                    int filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        lblMensaje.CssClass = "mensaje-exito";
                        lblMensaje.Text = mensajeExito;
                        EnviarCorreoHorasExtra(horaExtraID, nuevoEstado);
                    }
                    else
                    {
                        lblMensaje.CssClass = "mensaje-error";
                        lblMensaje.Text = "Error al actualizar el estado de las horas extra.";
                    }
                }
            }
            else
            {
                lblMensaje.CssClass = "mensaje-error";
                lblMensaje.Text = "ID de solicitud no válido.";
            }
        }

        private void EnviarCorreoHorasExtra(int horaExtraID, string estado)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("gestionprositiosweb@gmail.com");
                correo.To.Add("cesar.gomez.calderon@cuc.cr");
                correo.Subject = "Notificación de Horas Extra en el Sistema de Control Empresarial";
                correo.Body = $@"Hola,

La evidencia de las horas extra laboradas con el ID de solicitud '{horaExtraID}' ha sido '{estado}'.

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
                lblMensaje.Text = "Error al enviar la notificación por correo.";
            }
        }
    }
}