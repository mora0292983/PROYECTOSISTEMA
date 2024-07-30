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

		}

        protected void btncontinuar_Click(object sender, EventArgs e)
        {
            
                try
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand command = new SqlCommand("sp_InsertarEmpleado", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Añadir parámetros al comando
                            command.Parameters.AddWithValue("@Nombre", txtnombre.Text);
                            command.Parameters.AddWithValue("@Apellido", txtapellido.Text);
                            command.Parameters.AddWithValue("@Cedula", txtcedula.Text);
                            command.Parameters.AddWithValue("@FechaNac", DateTime.Parse(txtfechaNacimiento.Text));
                            command.Parameters.AddWithValue("@Direccion", txtdireccion.Text);
                            command.Parameters.AddWithValue("@Provincia", txtprovincia.Text);
                            command.Parameters.AddWithValue("@Ciudad", txtciudad.Text);
                            command.Parameters.AddWithValue("@Telefono", txttelefono.Text);
                            command.Parameters.AddWithValue("@Correo", txtcorreo.Text);
                            command.Parameters.AddWithValue("@FechaContratacion", new DateTime(2024, 2, 3)); // Ejemplo de fecha
                            command.Parameters.AddWithValue("@DepartamentoID", 1); // Ejemplo de ID de departamento
                            command.Parameters.AddWithValue("@PuestoID", 1); // Ejemplo de ID de puesto
                            command.Parameters.AddWithValue("@RolID", 1); // Ejemplo de ID de rol
                            command.Parameters.AddWithValue("@JefeID", 1); // Ejemplo de ID de jefe

                            connection.Open();
                            command.ExecuteNonQuery();

                        // Mostrar mensaje de éxito
                        lblMensaje.Text = "¡Se ha registrado con éxito!";
                        lblMensaje.CssClass = "mensaje-exito"; // Añadir una clase CSS para estilizar el mensaje

                        // Limpiar los campos
                        txtnombre.Text = "";
                        txtapellido.Text = "";
                        txtcedula.Text = "";
                        txtdireccion.Text = "";
                        txtprovincia.Text = "";
                        txtciudad.Text = "";
                        txttelefono.Text = "";
                        txtcorreo.Text = "";
                    }
                    }
                }
                catch (Exception ex)
            {
                lblMensaje.Text = "Error al crear el empleado. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error"; // Añadir una clase CSS para estilizar el mensaje
            }
            
        }
    }
}