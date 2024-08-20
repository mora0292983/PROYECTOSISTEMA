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

        protected void btnEditar_Click(object sender, EventArgs e)
        {
            // Obtener el DepartamentoID del CommandArgument
            string departamentoID = (sender as Button).CommandArgument;

            // Redirigir a la página modificaDepartamento.aspx pasando el DepartamentoID en la query string
            Response.Redirect("modificaDepartamento.aspx?DepartamentoID=" + departamentoID);
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            Button btnEliminar = (Button)sender;
            string departamentoID = btnEliminar.CommandArgument;

            // Conectar a la base de datos y eliminar el departamento
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Departamentos WHERE DepartamentoID = @DepartamentoID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DepartamentoID", departamentoID);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                // Mostrar mensaje de éxito o error
                if (rowsAffected > 0)
                {
                    lblMensaje.Text = "Departamento eliminado exitosamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }
                else
                {
                    lblMensaje.Text = "Error al eliminar el departamento. Intente de nuevo.";
                    lblMensaje.CssClass = "mensaje-error";
                }

                // Recargar los departamentos en el GridView
                CargarDatosDepartamentos();
            }
        }
    }
}
