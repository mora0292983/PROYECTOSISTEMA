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
         
        }
    }
}