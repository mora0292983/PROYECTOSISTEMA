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
	public partial class PG_VacacionesColectivas : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar que la fecha de inicio y fin sean válidas
                DateTime fechaInicioDate;
                DateTime fechaFinDate;

                if (!DateTime.TryParse(fechaInicio.Text, out fechaInicioDate) || !DateTime.TryParse(fechaFin.Text, out fechaFinDate))
                {
                    lblMensaje.Text = "Por favor, ingrese fechas válidas.";
                    lblMensaje.CssClass = "mensaje-error";
                    return;
                }

                if (fechaInicioDate <= DateTime.Today)
                {
                    lblMensaje.Text = "La fecha de inicio debe ser mayor que hoy.";
                    lblMensaje.CssClass = "mensaje-error";
                    return;
                }

                if (fechaFinDate <= fechaInicioDate)
                {
                    lblMensaje.Text = "La fecha de fin debe ser mayor que la fecha de inicio.";
                    lblMensaje.CssClass = "mensaje-error";
                    return;
                }

                // Calcular el número de días hábiles excluyendo sábados y domingos
                int diasHabiles = CalcularDiasHabiles(fechaInicioDate, fechaFinDate);

                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sistemaGp8.sp_InsertarVacacionColectiva", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros al comando
                        command.Parameters.AddWithValue("@FechaInicio", fechaInicioDate);
                        command.Parameters.AddWithValue("@FechaFin", fechaFinDate);
                        command.Parameters.AddWithValue("@Descripcion", descripcion.Text);
                        command.Parameters.AddWithValue("@DepartamentoID", 1); // Valor fijo para el DepartamentoID

                        connection.Open();
                        command.ExecuteNonQuery();

                        // Mostrar mensaje de éxito
                        lblMensaje.Text = $"¡Vacaciones colectivas registradas con éxito! Duración en días hábiles: {diasHabiles}.";
                        lblMensaje.CssClass = "mensaje-exito";

                        // Limpiar los campos
                        fechaInicio.Text = "";
                        fechaFin.Text = "";
                        descripcion.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al registrar las vacaciones. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
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
    }
}