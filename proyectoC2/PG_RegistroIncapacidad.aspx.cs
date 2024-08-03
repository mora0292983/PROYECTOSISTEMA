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
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_InsertarIncapacidad3", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros al comando
                        command.Parameters.AddWithValue("@EmpleadoID", Convert.ToInt32(nombreEmpleado.SelectedValue));
                        command.Parameters.AddWithValue("@TipoIncapacidadID", Convert.ToInt32(DropDownList1.SelectedValue));
                        command.Parameters.AddWithValue("@FechaInicio", DateTime.Parse(fechaInicio.Text));
                        command.Parameters.AddWithValue("@FechaFin", DateTime.Parse(fechaFin.Text));
                        command.Parameters.AddWithValue("@Descripcion", descripcion.Text);

                        connection.Open();
                        command.ExecuteNonQuery();

                        // Mostrar mensaje de éxito
                        lblMensaje.Text = "¡Incapacidad registrada con éxito!";
                        lblMensaje.CssClass = "mensaje-exito"; // Añadir una clase CSS para estilizar el mensaje

                        // Limpiar los campos
                        nombreEmpleado.ClearSelection();
                        DropDownList1.ClearSelection();
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
