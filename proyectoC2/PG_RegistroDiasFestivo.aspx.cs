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
	public partial class PG_RegistroDiasFestivo : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}
        protected void btnSubmit_Click(object sender, EventArgs e)
        
        {
            try
            {
                // Validar que la fecha sea válida
                DateTime fechaFestivaDate;

                if (!DateTime.TryParse(fechaInicio.Text, out fechaFestivaDate))
                {
                    lblMensaje.Text = "Por favor, ingrese una fecha válida.";
                    lblMensaje.CssClass = "mensaje-error";
                    return;
                }

                // Validar que la fecha no sea hoy ni una fecha pasada
                if (fechaFestivaDate <= DateTime.Today)
                {
                    lblMensaje.Text = "La fecha debe ser una fecha futura y tampoco puede ser hoy.";
                    lblMensaje.CssClass = "mensaje-error";
                    return;
                }

                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sistemaGp8.sp_InsertarDiaFestivo", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros al comando
                        command.Parameters.AddWithValue("@FechaOriginal", fechaFestivaDate);
                        command.Parameters.AddWithValue("@PagoObligatorio", pago.SelectedValue == "si" ? (object)true : (object)false);
                        command.Parameters.AddWithValue("@Estado", estado.SelectedValue == "activo" ? (object)true : (object)false);
                        command.Parameters.AddWithValue("@Descripcion", descripcion.Text);

                        connection.Open();
                        command.ExecuteNonQuery();

                        // Mostrar mensaje de éxito
                        lblMensaje.Text = "¡Día festivo registrado con éxito!";
                        lblMensaje.CssClass = "mensaje-exito";

                        // Limpiar los campos
                        fechaInicio.Text = "";
                        pago.ClearSelection();
                        estado.ClearSelection();
                        descripcion.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al registrar el día festivo. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }


    }
}