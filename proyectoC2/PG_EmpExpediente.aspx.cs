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
	public partial class PG_EmpExpediente : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                // Cargar datos del primer empleado al cargar la página
                CargarDatosEmpleado();
            }
        }
        private void CargarDatosEmpleado()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "sp_ObtenerEmpleadoConNombres"; // Usa el nuevo procedimiento almacenado
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmpleadoID", 19); // Cambiar el ID según sea necesario

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            txtnombre.Text = reader["Nombre"].ToString();
                            txtapellido.Text = reader["Apellido"].ToString();
                            txtcedula.Text = reader["Cedula"].ToString();
                            txtfechaNacimiento.Text = Convert.ToDateTime(reader["FechaNac"]).ToString("yyyy-MM-dd");
                            txtdireccion.Text = reader["Direccion"].ToString();
                            txtprovincia.Text = reader["provincia"].ToString();
                            txtciudad.Text = reader["ciudad"].ToString();
                            txttelefono.Text = reader["Telefono"].ToString();
                            txtcorreo.Text = reader["correo"].ToString();
                            txtFechaContratacion.Text = Convert.ToDateTime(reader["FechaContratacion"]).ToString("yyyy-MM-dd");

                            // Mostrar nombres en lugar de IDs
                            txtDepartamento.Text = reader["DepartamentoNombre"].ToString();
                            txtPuesto.Text = reader["PuestoNombre"].ToString();
                            txtRol.Text = reader["RolNombre"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al cargar los datos del empleado. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
        }
        protected void btncontinuar_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_ActualizarEmpleado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir parámetros al comando
                        command.Parameters.AddWithValue("@EmpleadoID", 19); // Cambiar el ID según sea necesario
                        command.Parameters.AddWithValue("@Nombre", txtnombre.Text);
                        command.Parameters.AddWithValue("@Apellido", txtapellido.Text);
                        command.Parameters.AddWithValue("@Cedula", txtcedula.Text);
                        command.Parameters.AddWithValue("@FechaNac", DateTime.Parse(txtfechaNacimiento.Text));
                        command.Parameters.AddWithValue("@Direccion", txtdireccion.Text);
                        command.Parameters.AddWithValue("@Provincia", txtprovincia.Text);
                        command.Parameters.AddWithValue("@Ciudad", txtciudad.Text);
                        command.Parameters.AddWithValue("@Telefono", txttelefono.Text);
                        command.Parameters.AddWithValue("@Correo", txtcorreo.Text);
                        command.Parameters.AddWithValue("@FechaContratacion", new DateTime(2020, 1, 15));
                        command.Parameters.AddWithValue("@DepartamentoID", 1);
                        command.Parameters.AddWithValue("@PuestoID", 1);
                        command.Parameters.AddWithValue("@RolID", 1);

                        connection.Open();
                        command.ExecuteNonQuery();

                        // Mostrar mensaje de éxito
                        lblMensaje.Text = "¡Se ha actualizado con éxito!";
                        lblMensaje.CssClass = "mensaje-exito"; // Añadir una clase CSS para estilizar el mensaje
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al actualizar el empleado. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
        }
    }
}