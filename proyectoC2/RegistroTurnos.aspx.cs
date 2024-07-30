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
	public partial class RegistroTurnos : System.Web.UI.Page
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
            string query = "SELECT TurnoID, NombreTurno, Descripcion FROM sistemaGp8.Turnos";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                gridTurnos.DataSource = dataTable;
                gridTurnos.DataBind();
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_InsertarTurno", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@NombreTurno", txtNombreTurno.Text);
                        command.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);

                        connection.Open();
                        command.ExecuteNonQuery();

                        lblMensaje.Text = "¡Turno registrado con éxito!";
                        lblMensaje.CssClass = "mensaje-exito";
                        LoadData();
                        txtNombreTurno.Text = "";
                        txtDescripcion.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al crear el turno. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            string nombreTurno = txtNombreTurnoEliminar.Text.Trim();

            if (string.IsNullOrEmpty(nombreTurno))
            {
                lblMensaje.Text = "Por favor, ingrese el nombre del turno.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_EliminarTurno", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NombreTurno", nombreTurno);

                        connection.Open();
                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            lblMensaje.Text = "Turno eliminado con éxito.";
                            lblMensaje.CssClass = "mensaje-exito";
                            LoadData();
                        }
                        else
                        {
                            lblMensaje.Text = "No se encontró ningún turno con el nombre especificado.";
                            lblMensaje.CssClass = "mensaje-error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al eliminar el turno. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            string nombreTurnoActual = txtNombreTurnoActual.Text.Trim();
            string nuevoNombreTurno = txtNuevoNombreTurno.Text.Trim();
            string nuevaDescripcion = txtNuevaDescripcion.Text.Trim();

            if (string.IsNullOrEmpty(nombreTurnoActual) || string.IsNullOrEmpty(nuevoNombreTurno) || string.IsNullOrEmpty(nuevaDescripcion))
            {
                lblMensaje.Text = "Por favor, complete todos los campos.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_ActualizarTurno", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Añadir los parámetros al comando
                        command.Parameters.AddWithValue("@NombreTurnoActual", nombreTurnoActual);
                        command.Parameters.AddWithValue("@NuevoNombreTurno", nuevoNombreTurno);
                        command.Parameters.AddWithValue("@NuevaDescripcion", nuevaDescripcion);

                        // Abrir la conexión y ejecutar el comando
                        connection.Open();
                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            lblMensaje.Text = "Turno actualizado con éxito.";
                            lblMensaje.CssClass = "mensaje-exito";
                            LoadData(); // Recargar los datos para reflejar los cambios
                        }
                        else
                        {
                            lblMensaje.Text = "No se encontró ningún turno con el nombre especificado.";
                            lblMensaje.CssClass = "mensaje-error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al actualizar el turno. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
    }
}