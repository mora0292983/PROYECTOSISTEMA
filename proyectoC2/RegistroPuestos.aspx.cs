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
	public partial class RegistroPuestos : System.Web.UI.Page
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
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT PuestoID, NombrePuesto, Departamento, Descripcion FROM sistemaGp8.Puestos";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                gridPuestos.DataSource = dataTable;
                gridPuestos.DataBind();
            }
        }
        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_InsertarPuesto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@NombrePuesto", txtNombrePuesto.Text);
                        command.Parameters.AddWithValue("@Departamento", txtDepartamento.Text);
                        command.Parameters.AddWithValue("@Descripcion", txtDescripcion.Text);

                        connection.Open();
                        command.ExecuteNonQuery();

                        lblMensaje.Text = "¡Puesto registrado con éxito!";
                        lblMensaje.CssClass = "mensaje-exito";
                        LoadData();
                        txtNombrePuesto.Text = "";
                        txtDepartamento.Text = "";
                        txtDescripcion.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al crear el puesto. Por favor, intenta de nuevo. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            string nombrePuesto = txtNombrePuestoEliminar.Text.Trim();

            if (string.IsNullOrEmpty(nombrePuesto))
            {
                lblMensaje.Text = "Por favor, ingrese el nombre del puesto.";
                lblMensaje.CssClass = "mensaje-error";
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand("sp_EliminarPuesto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@NombrePuesto", nombrePuesto);

                        connection.Open();
                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            lblMensaje.Text = "Puesto eliminado con éxito.";
                            lblMensaje.CssClass = "mensaje-exito";
                            LoadData();
                        }
                        else
                        {
                            lblMensaje.Text = "No se encontró ningún puesto con el nombre especificado.";
                            lblMensaje.CssClass = "mensaje-error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al eliminar el puesto. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }

        protected void btnActualizar_Click(object sender, EventArgs e)
        {
            string nombrePuestoActual = txtNombrePuestoActual.Text.Trim();
            string nuevoNombrePuesto = txtNuevoNombrePuesto.Text.Trim();
            string nuevaDescripcion = txtNuevaDescripcion.Text.Trim();

            if (string.IsNullOrEmpty(nombrePuestoActual) || string.IsNullOrEmpty(nuevoNombrePuesto) || string.IsNullOrEmpty(nuevaDescripcion))
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
                    using (SqlCommand command = new SqlCommand("sp_ActualizarPuesto", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@NombrePuestoActual", nombrePuestoActual);
                        command.Parameters.AddWithValue("@NuevoNombrePuesto", nuevoNombrePuesto);
                        command.Parameters.AddWithValue("@NuevaDescripcion", nuevaDescripcion);

                        connection.Open();
                        int filasAfectadas = command.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            lblMensaje.Text = "Puesto actualizado con éxito.";
                            lblMensaje.CssClass = "mensaje-exito";
                            LoadData();
                        }
                        else
                        {
                            lblMensaje.Text = "No se encontró ningún puesto con el nombre especificado.";
                            lblMensaje.CssClass = "mensaje-error";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al actualizar el puesto. Detalles: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
    }
}