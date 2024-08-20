using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;
using System.Configuration; // Para ConfigurationManager

namespace proyectoC2
{
	public partial class PG_InicioJe : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Llamar al método para cargar los días disponibles al cargar la página
                CargarDiasDisponibles();
            }
        }

        private void CargarDiasDisponibles()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT TOP 1 DiasDisponibles FROM sistemaGp8.DiasVacacionesDisfrutar ORDER BY EmpleadoID"; // Ajusta la consulta según sea necesario
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != null && result != DBNull.Value)
                        {
                            int diasDisponibles = Convert.ToInt32(result);
                            lblDiasDisponibles.Text = $"{diasDisponibles}";
                        }
                        else
                        {
                            lblDiasDisponibles.Text = "No hay datos disponibles";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar los días disponibles. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            // Obtener los valores del formulario
            DateTime fechaInicioDa;
            DateTime fechaFinDa;

            // Validar que las fechas se puedan analizar correctamente
            if (!DateTime.TryParse(fechaInicio.Text, out fechaInicioDa))
            {
                lblMensaje.Text = "La fecha de inicio no es válida.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            if (!DateTime.TryParse(fechaFin.Text, out fechaFinDa))
            {
                lblMensaje.Text = "La fecha de fin no es válida.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            // Validar que la fecha de inicio sea mayor a la fecha actual
            if (fechaInicioDa <= DateTime.Now.Date)
            {
                lblMensaje.Text = "La fecha de inicio debe ser mayor al día de hoy.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            // Validar que la fecha de fin no sea menor a la fecha de inicio
            if (fechaFinDa < fechaInicioDa)
            {
                lblMensaje.Text = "La fecha de fin no puede ser menor a la fecha de inicio.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            // Validar que las fechas no sean sábados o domingos
            if (fechaInicioDa.DayOfWeek == DayOfWeek.Saturday || fechaInicioDa.DayOfWeek == DayOfWeek.Sunday)
            {
                lblMensaje.Text = "La fecha de inicio no puede ser un sábado o domingo.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            if (fechaFinDa.DayOfWeek == DayOfWeek.Saturday || fechaFinDa.DayOfWeek == DayOfWeek.Sunday)
            {
                lblMensaje.Text = "La fecha de fin no puede ser un sábado o domingo.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            // Calcular el número de días laborables solicitados
            int diasLaborablesSolicitados = CalcularDiasLaborables(fechaInicioDa, fechaFinDa);

            // Obtener días disponibles del empleado
            int empleadoID = 19; // El ID del empleado debe ser proporcionado de alguna manera

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Consultar los días disponibles
                    string queryDiasDisponibles = "SELECT DiasDisponibles FROM sistemaGp8.DiasVacacionesDisfrutar WHERE EmpleadoID = @EmpleadoID";
                    using (SqlCommand cmd = new SqlCommand(queryDiasDisponibles, connection))
                    {
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                        object result = cmd.ExecuteScalar();
                        if (result == null)
                        {
                            lblMensaje.Text = "No se encontraron datos de días disponibles para el empleado.";
                            lblMensaje.CssClass = "mensaje-error";
                            return;
                        }

                        int diasDisponibles = Convert.ToInt32(result);

                        // Verificar si hay suficientes días disponibles
                        if (diasLaborablesSolicitados > diasDisponibles)
                        {
                            lblMensaje.Text = "No tienes suficientes días de vacaciones disponibles.";
                            lblMensaje.CssClass = "mensaje-error";
                            return;
                        }

                        // Insertar la solicitud de vacaciones
                        using (SqlCommand insertCommand = new SqlCommand("sp_Insertarvacaciones3", connection))
                        {
                            insertCommand.CommandType = CommandType.StoredProcedure;

                            // Agregar los parámetros al comando
                            insertCommand.Parameters.AddWithValue("@EmpleadoID", empleadoID);
                            insertCommand.Parameters.AddWithValue("@FechaSolicitud", DateTime.Now);
                            insertCommand.Parameters.AddWithValue("@FechaInicio", fechaInicioDa);
                            insertCommand.Parameters.AddWithValue("@FechaFin", fechaFinDa);
                            insertCommand.Parameters.AddWithValue("@DiasSolicitados", diasLaborablesSolicitados);
                            insertCommand.Parameters.AddWithValue("@EstadoSolicitud", "Sin Gestionar");

                            insertCommand.ExecuteNonQuery();
                        }

                        // Actualizar los días disponibles
                        string updateDiasDisponibles = "UPDATE sistemaGp8.DiasVacacionesDisfrutar SET DiasDisponibles = DiasDisponibles - @DiasSolicitados WHERE EmpleadoID = @EmpleadoID";
                        using (SqlCommand updateCommand = new SqlCommand(updateDiasDisponibles, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@DiasSolicitados", diasLaborablesSolicitados);
                            updateCommand.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                            updateCommand.ExecuteNonQuery();
                        }

                        lblMensaje.Text = "¡Vacaciones registradas con éxito y días actualizados!";
                        lblMensaje.CssClass = "mensaje-exito";
                        // Enviar correo electrónico
                        EnviarCorreo("karina.mora.cortes@cuc.cr", "Nueva Solicitud de Vacaciones", "Solicitud de vacaciones De kevin\tPérez\tCedula 307640983");

                        // Redirigir a la misma página para actualizar
                        Response.Redirect(Request.RawUrl);
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al crear la solicitud de vacaciones. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }

        // Método para calcular los días laborables
        private int CalcularDiasLaborables(DateTime fechaInicio, DateTime fechaFin)
        {
            int diasLaborables = 0;

            for (DateTime fecha = fechaInicio; fecha <= fechaFin; fecha = fecha.AddDays(1))
            {
                if (fecha.DayOfWeek != DayOfWeek.Saturday && fecha.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasLaborables++;
                }
            }

            return diasLaborables;
        }
        // Método para enviar correo electrónico
        private void EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                // Configurar el cliente SMTP para Gmail
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Puerto para TLS
                    Credentials = new NetworkCredential("morakarina708@gmail.com", "rldrddlgrblmhbee"), // Reemplaza con tu correo y contraseña de Gmail
                    EnableSsl = true, // Habilitar SSL
                };

                // Crear el mensaje de correo
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("morakarina708@gmail.com"), // Reemplaza con tu correo
                    Subject = asunto,
                    Body = cuerpo,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(destinatario);

                // Enviar el correo
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Manejar el error de envío de correo
                lblMensaje.Text = "Error al enviar el correo: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
        }

    }
}
