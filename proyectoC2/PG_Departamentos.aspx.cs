using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace proyectoC2
{
	public partial class PG_Departamentos : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				LoadData();
			}
		}
		private void LoadData()
		{
			string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
			string query = "SELECT DepartamentoID, NombreDepartamento, Descripcion FROM sistemaGp8.Departamentos";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
				DataTable dataTable = new DataTable();
				dataAdapter.Fill(dataTable);
				gridDepartamentos.DataSource = dataTable;
				gridDepartamentos.DataBind();
			}
		}

		protected void btnNuevo_Click(object sender, EventArgs e)
		{
			Response.Redirect("PG_Login.aspx");
		}

		protected void btnEditar_Click(object sender, EventArgs e)
		{
			Response.Redirect("EditarRegistro.aspx");
		}

		protected void btnEliminar_Click(object sender, EventArgs e)
		{
			Response.Redirect("EliminarRegistro.aspx");
		}
		
        protected void btncontinuar_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Crear el comando para insertar el nuevo departamento
                    using (SqlCommand command = new SqlCommand("sp_InsertarDepartamento", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros al comando
                        command.Parameters.AddWithValue("@NombreDepartamento", txtnombreDepartamento.Text);
                        command.Parameters.AddWithValue("@Descripcion", txtdescripcion.Text);

                        // Abrir la conexión y ejecutar el comando
                        connection.Open();
                        command.ExecuteNonQuery();

                        // Mostrar mensaje de éxito
                        lblMensaje.Text = "¡Departamento registrado con éxito!";
                        lblMensaje.CssClass = "mensaje-exito"; // Añadir una clase CSS para estilizar el mensaje
                        LoadData();
                        // Limpiar los campos
                        txtnombreDepartamento.Text = "";
                        txtdescripcion.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al crear el departamento. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
        }
        protected void btncontinuar1_Click(object sender, EventArgs e)
        {
            // Obtener el nombre del departamento desde el TextBox
            string nombreDepartamento = txtnombreDepartamentoEliminar.Text.Trim();

            if (string.IsNullOrEmpty(nombreDepartamento))
            {
                lblMensaje.Text = "Por favor, ingrese el nombre del departamento.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            try
            {
                // Cadena de conexión a la base de datos
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_EliminarDepartamento", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetro al comando
                        command.Parameters.AddWithValue("@NombreDepartamento", nombreDepartamento);

                        // Abrir la conexión y ejecutar el comando
                        connection.Open();
                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            lblMensaje.Text = "Departamento eliminado con éxito.";
                            lblMensaje.CssClass = "mensaje-exito";
                            LoadData();
                        }
                        else
                        {
                            lblMensaje.Text = "No se encontró ningún departamento con el nombre especificado.";
                            lblMensaje.CssClass = "mensaje-error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al eliminar el departamento. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            // Obtener los valores de los controles de la página
            string nombreActual = txtNombreActual.Text.Trim();
            string nuevoNombre = txtNuevoNombre.Text.Trim();
            string nuevaDescripcion = txtNuevaDescripcion.Text.Trim();

            if (string.IsNullOrEmpty(nombreActual) || string.IsNullOrEmpty(nuevoNombre) || string.IsNullOrEmpty(nuevaDescripcion))
            {
                lblMensaje.Text = "Por favor, complete todos los campos.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            try
            {
                // Cadena de conexión a la base de datos
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_ActualizarDepartamento", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros al comando
                        command.Parameters.AddWithValue("@NombreDepartamentoActual", nombreActual);
                        command.Parameters.AddWithValue("@NuevoNombreDepartamento", nuevoNombre);
                        command.Parameters.AddWithValue("@NuevaDescripcion", nuevaDescripcion);

                        // Abrir la conexión y ejecutar el comando
                        connection.Open();
                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            lblMensaje.Text = "Departamento actualizado con éxito.";
                            lblMensaje.CssClass = "mensaje-exito";
                            LoadData();
                        }
                        else
                        {
                            lblMensaje.Text = "No se encontró ningún departamento con el nombre especificado.";
                            lblMensaje.CssClass = "mensaje-error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al actualizar el departamento. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
    }
}