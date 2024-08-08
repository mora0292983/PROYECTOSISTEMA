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
	public partial class gestionActividades : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Cargar las opciones en el DropDownList
                CargarOpcionesRendimiento();

                // Obtener el ActividadID de la URL
                string actividadID = Request.QueryString["ActividadID"];

                if (!string.IsNullOrEmpty(actividadID))
                {
                    CargarDatosActividad(actividadID);
                }
                // Configurar el enlace para ver el archivo
                if (int.TryParse(idActividad.Text, out int activityID))
                {
                    lnkVerArchivo.NavigateUrl = $"VerPDF2.aspx?ID={activityID}";
                }
            }
        }

        private void CargarOpcionesRendimiento()
        {
            ddlrendimiento.Items.Insert(0, new ListItem("-- Seleccione una opción --", "0"));
            ddlrendimiento.Items.Add(new ListItem("Bajo", "Bajo"));
            ddlrendimiento.Items.Add(new ListItem("Medio", "Medio"));
            ddlrendimiento.Items.Add(new ListItem("Alto", "Alto"));
        }

        private void CargarDatosActividad(string actividadID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT EmpleadoID, horaInicio, horaFin FROM ActividadesFinal WHERE ActividadID = @ActividadID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ActividadID", actividadID);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        idActividad.Text = actividadID;
                        idEmpleado.Text = reader["EmpleadoID"].ToString();
                        horaInicio.Text = reader["horaInicio"].ToString();
                        horaFinal.Text = reader["horaFin"].ToString();
                    }
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Verificar si se seleccionó un valor en el DropDownList antes de proceder
            if (ddlrendimiento.SelectedIndex > 0)
            {
                string rendimiento = ddlrendimiento.SelectedItem.Text;
                ActualizarEstado("Aprobada", "Gestionada", rendimiento);
            }
            else
            {
                lblMensaje.Text = "Por favor, seleccione un rendimiento antes de aprobar.";
                lblMensaje.CssClass = "mensaje-error";
            }
        }

        protected void btnSubmit2_Click(object sender, EventArgs e)
        {
            ActualizarEstado("Denegada", "Gestionada", null);
        }

        private void ActualizarEstado(string nuevoEstado, string nuevoEstadoSolicitud, string rendimiento)
        {
            try
            {
                int actividadID = int.Parse(idActividad.Text);
                string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
                string query = "UPDATE ActividadesFinal SET Estado = @Estado, EstadoSolicitud = @EstadoSolicitud";

                // Si se aprueba, también actualizar el rendimiento
                if (rendimiento != null)
                {
                    query += ", Rendimiento = @Rendimiento";
                }

                query += " WHERE ActividadID = @ActividadID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Estado", nuevoEstado);
                    command.Parameters.AddWithValue("@EstadoSolicitud", nuevoEstadoSolicitud);
                    command.Parameters.AddWithValue("@ActividadID", actividadID);

                    if (rendimiento != null)
                    {
                        command.Parameters.AddWithValue("@Rendimiento", rendimiento);
                    }

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        lblMensaje.Text = "Actividad gestionada con éxito.";
                        lblMensaje.CssClass = "mensaje-exito";
                    }
                    else
                    {
                        lblMensaje.Text = "Error al gestionar la actividad.";
                        lblMensaje.CssClass = "mensaje-error";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error al gestionar la actividad: " + ex.Message;
                lblMensaje.CssClass = "mensaje-error";
            }
        }






    }
}