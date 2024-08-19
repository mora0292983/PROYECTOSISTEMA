using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
	public partial class PG_RegistroIncapacidad : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                LoadDropDownLists();
            }
        }
        private void LoadDropDownLists()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            // Cargar empleados
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetEmpleados2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        nombreEmpleado.DataSource = reader;
                        nombreEmpleado.DataTextField = "Cedula";
                        nombreEmpleado.DataValueField = "EmpleadoID";
                        nombreEmpleado.DataBind();
                    }
                }
            }

            // Cargar tipos de incapacidad
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("sp_GetTiposIncapacidad3", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DropDownList1.DataSource = reader;
                        DropDownList1.DataTextField = "NombreTipo";
                        DropDownList1.DataValueField = "TipoIncapacidadID";
                        DropDownList1.DataBind();
                    }
                }
            }
        }
        // Método para calcular días hábiles excluyendo sábados y domingos
        private int CalcularDiasHabiles(DateTime fechaInicio, DateTime fechaFin)
        {
            int diasHabiles = 0;
            DateTime fechaActual = fechaInicio;

            while (fechaActual <= fechaFin)
            {
                if (fechaActual.DayOfWeek != DayOfWeek.Saturday && fechaActual.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasHabiles++;
                }
                fechaActual = fechaActual.AddDays(1);
            }

            return diasHabiles;
        }
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                // Convertir fechas
                DateTime fechaInicioDate = DateTime.Parse(fechaInicio.Text);
                DateTime fechaFinDate = DateTime.Parse(fechaFin.Text);

                // Calcular el número de días hábiles (excluyendo sábados y domingos)
                int diasIncapacidad = CalcularDiasHabiles(fechaInicioDate, fechaFinDate);

                // Ajustar el mensaje si los días exceden tres
                string mensaje;
                if (diasIncapacidad > 3)
                {
                    int diasNoPagados = diasIncapacidad - 3;
                    mensaje = $"¡Incapacidad registrada con éxito! Sus días de incapacidad son {diasIncapacidad}. De estos, solo se pagarán 3 días. Los {diasNoPagados} días adicionales son no pagados.";
                }
                else
                {
                    mensaje = $"¡Incapacidad registrada con éxito! Sus días de incapacidad son {diasIncapacidad}.";
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_InsertarIncapacidad4", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros al comando
                        command.Parameters.AddWithValue("@EmpleadoID", Convert.ToInt32(nombreEmpleado.SelectedValue));
                        command.Parameters.AddWithValue("@TipoIncapacidadID", Convert.ToInt32(DropDownList1.SelectedValue));
                        command.Parameters.AddWithValue("@FechaInicio", fechaInicioDate);
                        command.Parameters.AddWithValue("@FechaFin", fechaFinDate);
                        command.Parameters.AddWithValue("@Descripcion", descripcion.Text);
                        command.Parameters.AddWithValue("@DiasIncapacidad", diasIncapacidad); // Si necesitas almacenar los días en la base de datos
                        command.Parameters.AddWithValue("@EstadoSolicitud", "Sin Gestionar");
                        connection.Open();
                        command.ExecuteNonQuery();

                        // Mostrar mensaje de éxito con los días de incapacidad
                        lblMensaje.Text = mensaje;
                        lblMensaje.CssClass = "mensaje-exito"; // Añadir una clase CSS para estilizar el mensaje

                        // Limpiar los campos

                        fechaInicio.Text = "";
                        fechaFin.Text = "";
                        descripcion.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al registrar la incapacidad. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
        }
    }
}

