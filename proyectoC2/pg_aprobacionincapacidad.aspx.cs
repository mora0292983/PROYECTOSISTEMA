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
	public partial class pg_cin : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Recuperar los valores desde la URL
                string nombreEmpleado = Request.QueryString["NombreEmpleado"];
                string apellidoEmpleado = Request.QueryString["ApellidoEmpleado"];
                string cedulaEmpleado = Request.QueryString["CedulaEmpleado"];
                string fechaInicio = Request.QueryString["FechaInicio"];
                string fechaFin = Request.QueryString["FechaFin"];
                string tipoIncapacidad = Request.QueryString["TipoIncapacidad"];
                string descripcion = Request.QueryString["Descripcion"];

                // Verificar si los parámetros son válidos
                if (!string.IsNullOrEmpty(nombreEmpleado) &&
                    !string.IsNullOrEmpty(apellidoEmpleado) &&
                    !string.IsNullOrEmpty(cedulaEmpleado) &&
                    !string.IsNullOrEmpty(fechaInicio) &&
                    !string.IsNullOrEmpty(fechaFin) &&
                    !string.IsNullOrEmpty(tipoIncapacidad) &&
                    !string.IsNullOrEmpty(descripcion))
                {
                    // Cargar los datos en los controles
                    lblNombreCompleto.Text = $"{nombreEmpleado}";
                    lblApellido.Text = apellidoEmpleado;
                    lblCedula.Text = cedulaEmpleado;
                    lblFechaInicio.Text = fechaInicio;
                    lblFechaFin.Text = fechaFin;
                    lblTipoIncapacidad.Text = tipoIncapacidad;
                    lblDescripcion.Text = descripcion;
                }
                else
                {
                    // Mostrar un mensaje de error si algún parámetro está vacío
                    lblMensaje.Text = "Error: Datos faltantes.";
                    lblMensaje.CssClass = "mensaje-error";
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ActualizarEstado("Aprobada", "Gestionada");
        }

        protected void btnSubmit2_Click(object sender, EventArgs e)
        {
            ActualizarEstado("Denegada", "Gestionada");
        }

        private void ActualizarEstado(string nuevoEstado, string nuevoEstadoSolicitud)
        {
            try
            {
                string descripcion = lblDescripcion.Text;
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                string query = "UPDATE sistemaGp8.Incapacidades SET EstadoSolicitud = @EstadoSolicitud WHERE Descripcion = @Descripcion";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EstadoSolicitud", nuevoEstadoSolicitud);
                    command.Parameters.AddWithValue("@Descripcion", descripcion);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        lblMensaje.Text = "Justificación gestionada con éxito";
                        lblMensaje.CssClass = "mensaje-exito";
                    }
                    else
                    {
                        lblMensaje.Text = "Error al gestionar la justificación";
                        lblMensaje.CssClass = "mensaje-error";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al gestionar la justificación: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }
    }
}