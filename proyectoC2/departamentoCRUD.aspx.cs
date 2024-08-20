using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class departamentoCRUD1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosDepartamentos();
            }
        }

        private void CargarDatosDepartamentos()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT DepartamentoID, NombreDepartamento, Descripcion FROM Departamentos";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();

                try
                {
                    connection.Open();
                    dataAdapter.Fill(dataTable);
                    gridView.DataSource = dataTable;
                    gridView.DataBind();
                }
                catch (Exception ex)
                {
                    // Manejar cualquier excepción aquí (mostrar mensaje, log, etc.)
                    lblMensaje.Text = "Error al cargar los datos: " + ex.Message;
                    lblMensaje.CssClass = "mensaje-error";
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Redirigir a la página "agregaDepartamento.aspx"
            Response.Redirect("agregaDepartamento.aspx");
        }
    }
}
