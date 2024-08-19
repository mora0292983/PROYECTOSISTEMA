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
    public partial class SolicitudHorasExtraSupervisor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarEmpleados();

                // Cargar las horas en los DropDownLists de horaInicio y horaFin
                CargarHoras(ddlHoraInicio);
                CargarHoras(ddlHoraFin);
            }

            // Registra el evento SelectedIndexChanged para el DropDownList
            ddlempleado.SelectedIndexChanged += new EventHandler(ddlempleado_SelectedIndexChanged);
            ddlempleado.AutoPostBack = true; // Asegúrate de que el DropDownList haga un PostBack al cambiar de selección
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

        private void CargarEmpleados()
        {
            // Define la consulta SQL para obtener los nombres y apellidos de los empleados
            string query = "SELECT EmpleadoID, CONCAT(Nombre, ' ', Apellido) AS NombreCompleto FROM Empleados";

            // Establece la conexión a la base de datos
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Asigna los datos al DropDownList
                        ddlempleado.DataSource = reader;
                        ddlempleado.DataTextField = "NombreCompleto";
                        ddlempleado.DataValueField = "EmpleadoID";
                        ddlempleado.DataBind();
                    }
                }
            }

            // Añade una opción predeterminada al inicio del DropDownList
            ddlempleado.Items.Insert(0, new ListItem("-- Seleccione un empleado --", "0"));
        }

        protected void ddlempleado_SelectedIndexChanged(object sender, EventArgs e)
        {
            int empleadoID;
            if (int.TryParse(ddlempleado.SelectedValue, out empleadoID) && empleadoID > 0)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT HoraEntrada, HoraSalida FROM HorariosFinal WHERE EmpleadoID = @EmpleadoID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        TimeSpan horaEntrada = (TimeSpan)reader["HoraEntrada"];
                        TimeSpan horaSalida = (TimeSpan)reader["HoraSalida"];

                        string horario = $"{horaEntrada:hh\\:mm}-{horaSalida:hh\\:mm}";
                        idHorario.Text = horario;
                    }
                    else
                    {
                        // Manejo cuando no se encuentra el horario
                        idHorario.Text = "No disponible";
                    }
                    reader.Close();
                }
            }
            else
            {
                idHorario.Text = string.Empty; // Limpiar el TextBox si no hay selección válida
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validar el formulario
            if (ValidarFormulario())
            {
                // Obtener los valores del formulario
                int empleadoID = int.Parse(ddlempleado.SelectedValue);
                DateTime fechaSeleccionada = DateTime.Parse(fecha.Text);

                // Validar si ya existe un registro para el empleado y la fecha seleccionados
                if (ExisteRegistroHorasExtra(empleadoID, fechaSeleccionada))
                {
                    lblMensaje.CssClass = "mensaje-error";
                    lblMensaje.Text = "Ya existe una solicitud para el empleado en la fecha indicada.";
                    return; // Detener el proceso si ya existe un registro
                }

                // Obtener otros valores del formulario
                int horarioID = ObtenerHorarioID(empleadoID); // Método para obtener el HorarioID
                TimeSpan horaInicio = TimeSpan.Parse(ddlHoraInicio.SelectedValue);
                TimeSpan horaFin = TimeSpan.Parse(ddlHoraFin.SelectedValue);
                string motivo = Motivo.Text;

                // Validar que la hora de inicio no sea igual a la hora de fin
                if (horaInicio == horaFin)
                {
                    lblMensaje.CssClass = "mensaje-error";
                    lblMensaje.Text = "La hora de inicio y de fin no pueden ser iguales";
                    return; // Detener el proceso si las horas son iguales
                }

                // Calcular la diferencia en horas entre HoraInicio y HoraFinal
                double cantidadHoras = (horaFin - horaInicio).TotalHours;

                // Asegurarse de que la diferencia no sea negativa
                if (cantidadHoras < 0)
                {
                    lblMensaje.CssClass = "mensaje-error";
                    lblMensaje.Text = "La hora de fin no puede ser anterior a la hora de inicio.";
                    return; // Detener el proceso si la diferencia es negativa
                }

                // Insertar el nuevo registro en la tabla HorasExtraFinal y obtener el HoraExtraID
                string query = @"
INSERT INTO HorasExtraFinal 
(EmpleadoID, HorarioID, Fecha, HoraInicio, HoraFinal, Motivo, CantidadHoras, Estado, EstadoSolicitud, DocumentoPDF)
VALUES 
(@EmpleadoID, @HorarioID, @Fecha, @HoraInicio, @HoraFinal, @Motivo, @CantidadHoras, 'Sin Gestionar', 'Pendiente', NULL);
SELECT SCOPE_IDENTITY();"; // Obtener el ID generado

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoID);
                        cmd.Parameters.AddWithValue("@HorarioID", horarioID);
                        cmd.Parameters.AddWithValue("@Fecha", fechaSeleccionada);
                        cmd.Parameters.AddWithValue("@HoraInicio", horaInicio);
                        cmd.Parameters.AddWithValue("@HoraFinal", horaFin);
                        cmd.Parameters.AddWithValue("@Motivo", motivo);
                        cmd.Parameters.AddWithValue("@CantidadHoras", cantidadHoras);

                        conn.Open();
                        int horaExtraID = Convert.ToInt32(cmd.ExecuteScalar()); // Obtener el ID del registro insertado

                        // Enviar el correo electrónico
                        EnviarCorreoHorasExtra(horaExtraID);

                        // Mostrar el mensaje de éxito
                        lblMensaje.CssClass = "mensaje-exito";
                        lblMensaje.Text = "La solicitud de horas extra se ha enviado correctamente.";
                    }
                }
            }
        }

        // Método para validar si ya existe un registro para el empleado y la fecha seleccionados
        private bool ExisteRegistroHorasExtra(int empleadoID, DateTime fechaSeleccionada)
        {
            string query = "SELECT COUNT(*) FROM HorasExtraFinal WHERE EmpleadoID = @EmpleadoID AND Fecha = @Fecha";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmpleadoID", empleadoID);
                    cmd.Parameters.AddWithValue("@Fecha", fechaSeleccionada);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // Si existe al menos un registro, devolver true
                }
            }
        }

        private int ObtenerHorarioID(int empleadoID)
        {
            int horarioID = 0;
            string query = "SELECT HorarioID FROM HorariosFinal WHERE EmpleadoID = @EmpleadoID";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conexion"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmpleadoID", empleadoID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        horarioID = Convert.ToInt32(result);
                    }
                }
            }
            return horarioID;
        }

        private bool ValidarFormulario()
        {
            lblMensaje.CssClass = "mensaje-error";

            if (ddlempleado.SelectedValue == "0")
            {
                lblMensaje.Text = "Debe seleccionar un empleado.";
                return false;
            }

            if (string.IsNullOrEmpty(fecha.Text) || DateTime.Parse(fecha.Text) < DateTime.Today)
            {
                lblMensaje.Text = "Debe seleccionar una fecha válida (la actual o futura).";
                return false;
            }

            if (ddlHoraInicio.SelectedValue == "0")
            {
                lblMensaje.Text = "Debe seleccionar una hora de inicio.";
                return false;
            }

            if (ddlHoraFin.SelectedValue == "0")
            {
                lblMensaje.Text = "Debe seleccionar una hora de fin.";
                return false;
            }

            if (string.IsNullOrEmpty(Motivo.Text))
            {
                lblMensaje.Text = "Debe ingresar un motivo.";
                return false;
            }

            TimeSpan horaInicio = TimeSpan.Parse(ddlHoraInicio.SelectedValue);
            TimeSpan horaFin = TimeSpan.Parse(ddlHoraFin.SelectedValue);
            string[] horarioEmpleado = idHorario.Text.Split('-');
            TimeSpan horaEntrada = TimeSpan.Parse(horarioEmpleado[0]);
            TimeSpan horaSalida = TimeSpan.Parse(horarioEmpleado[1]);

            if (horaInicio >= horaEntrada && horaInicio <= horaSalida || horaFin >= horaEntrada && horaFin <= horaSalida)
            {
                lblMensaje.Text = "Las hora extra no pueden estar dentro del horario laboral.";
                return false;
            }

            if ((horaFin - horaInicio).TotalHours > 4)
            {
                lblMensaje.Text = "El rango de horas extra no puede exceder las 4 horas.";
                return false;
            }

            return true;
        }

        // Método para enviar el correo electrónico
        private void EnviarCorreoHorasExtra(int horaExtraID)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("gestionprositiosweb@gmail.com");
                correo.To.Add("cesar.gomez.calderon@cuc.cr");
                correo.Subject = "Notificación de Horas Extra en el Sistema de Control Empresarial";
                correo.Body = $@"Hola,

Se ha recibido una nueva solicitud de horas extra con el ID {horaExtraID} en tu perfil en nuestro sistema de control empresarial. Recuerda aceptar o rechazar dicha solicitud lo antes posible para evitar inconvenientes.

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
            }
        }

    }
}