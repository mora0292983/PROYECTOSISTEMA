using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace proyectoC2
{
    public partial class agregaDepartamento : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Si necesitas alguna inicialización, colócala aquí
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string nombreDepartamento = NombreDepartamento.Text.Trim();
            string descripcion = Motivo.Text.Trim();

            // Validar que los campos no estén vacíos
            if (string.IsNullOrEmpty(nombreDepartamento) || string.IsNullOrEmpty(descripcion))
            {
                lblMensaje.Text = "Por favor, complete todos los campos.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            // Registrar el nuevo departamento
            if (RegistrarDepartamento(nombreDepartamento, descripcion))
            {
                lblMensaje.Text = "Departamento agregado exitosamente.";
                lblMensaje.CssClass = "mensaje-exito";
                // Limpiar los campos después del registro exitoso
                NombreDepartamento.Text = string.Empty;
                Motivo.Text = string.Empty;
            }
            else
            {
                lblMensaje.Text = "Error al agregar el departamento.";
                lblMensaje.CssClass = "mensaje-error";
            }
        }

        private bool RegistrarDepartamento(string nombreDepartamento, string descripcion)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "INSERT INTO Departamentos (NombreDepartamento, Descripcion) VALUES (@NombreDepartamento, @Descripcion)";

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NombreDepartamento", nombreDepartamento);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción aquí (mostrar mensaje, log, etc.)
                lblMensaje.Text = "Error al agregar el departamento: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
                return false;
            }
        }
    }
}
