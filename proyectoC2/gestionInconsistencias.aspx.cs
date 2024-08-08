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
    public partial class gestionInconsistencias : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Recuperar valores de la URL
                string idInconsistenciaValor = Request.QueryString["IDInconsistencia"];
                string empleadoIDValor = Request.QueryString["EmpleadoID"];
                string fechaValor = Request.QueryString["Fecha"];
                string tipoInconsistenciaValor = Request.QueryString["TipoInconsistencia"];

                // Asignar valores a los TextBox si no están vacíos
                if (!string.IsNullOrEmpty(idInconsistenciaValor))
                {
                    idInconsistencia.Text = idInconsistenciaValor;
                }
                if (!string.IsNullOrEmpty(empleadoIDValor))
                {
                    Empleado.Text = empleadoIDValor;
                }
                if (!string.IsNullOrEmpty(fechaValor))
                {
                    fecha.Text = fechaValor;
                }
                if (!string.IsNullOrEmpty(tipoInconsistenciaValor))
                {
                    tipoInconsistencia.Text = tipoInconsistenciaValor;
                }
                // Configurar el enlace para ver el archivo
                if (int.TryParse(idInconsistencia.Text, out int inconsistenciaID))
                {
                    lnkVerArchivo.NavigateUrl = $"VerPDF.aspx?ID={inconsistenciaID}";
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
                int inconsistenciaID = int.Parse(idInconsistencia.Text);
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                string query = "UPDATE InconsistenciasFinal SET Estado = @Estado, EstadoSolicitud = @EstadoSolicitud WHERE InconsistenciaID = @InconsistenciaID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@EstadoSolicitud", nuevoEstadoSolicitud);
                    command.Parameters.AddWithValue("@InconsistenciaID", inconsistenciaID);

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