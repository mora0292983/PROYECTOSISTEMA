using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class modificaDepartamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verificar si el DepartamentoID está presente en la query string
                string departamentoID = Request.QueryString["DepartamentoID"];
                if (!string.IsNullOrEmpty(departamentoID))
                {
                    // Cargar los datos del departamento
                    CargarDatosDepartamento(departamentoID);
                }
            }
        }

        private void CargarDatosDepartamento(string departamentoID)
        {
            // Conectar a la base de datos y obtener los datos del departamento
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT NombreDepartamento, Descripcion FROM Departamentos WHERE DepartamentoID = @DepartamentoID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DepartamentoID", departamentoID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Asignar los valores a los controles
                    DepartamentoID.Text = departamentoID;
                    NombreDepartamento.Text = reader["NombreDepartamento"].ToString();
                    Descripcion.Text = reader["Descripcion"].ToString();
                }
                conn.Close();
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validar que los campos no estén vacíos
            if (string.IsNullOrEmpty(NombreDepartamento.Text) || string.IsNullOrEmpty(Descripcion.Text))
            {
                lblMensaje.Text = "Por favor, complete todos los campos.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            // Actualizar los datos del departamento
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Departamentos SET NombreDepartamento = @NombreDepartamento, Descripcion = @Descripcion WHERE DepartamentoID = @DepartamentoID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DepartamentoID", DepartamentoID.Text);
                cmd.Parameters.AddWithValue("@NombreDepartamento", NombreDepartamento.Text);
                cmd.Parameters.AddWithValue("@Descripcion", Descripcion.Text);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                // Mostrar mensaje de éxito o error
                if (rowsAffected > 0)
                {
                    lblMensaje.Text = "Departamento actualizado exitosamente.";
                    lblMensaje.CssClass = "mensaje-exito";
                }
                else
                {
                    lblMensaje.Text = "Error al actualizar el departamento. Intente de nuevo.";
                    lblMensaje.CssClass = "mensaje-error";
                }
                conn.Close();
            }
        }
    }
}