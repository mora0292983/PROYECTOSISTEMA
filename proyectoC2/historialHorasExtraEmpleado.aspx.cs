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
    public partial class historialHorasExtraEmpleado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Asignar el valor predeterminado de 19 al TextBox
                int empleadoID = 19;
                empleadoIDtxt.Text = empleadoID.ToString();

                // Cargar el historial de horas extra usando el valor predeterminado
                CargarHistorialHorasExtra(empleadoID);
            }
        }


        private void CargarHistorialHorasExtra(int empleadoID)
        {
            // Obtener la cadena de conexión del archivo web.config
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

            // Establecer la conexión a la base de datos
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Configurar el comando SQL para ejecutar el procedimiento almacenado
                    using (SqlCommand cmd = new SqlCommand("sp_ObtenerHorasExtraPorEmpleado", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                        // Crear un adaptador para llenar un DataTable con los resultados
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // Enlazar el DataTable al GridView
                            gridView.DataSource = dt;
                            gridView.DataBind();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier error que pueda ocurrir mediante un mensaje emergente
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Error al cargar el historial de horas extra: " + ex.Message.Replace("'", "\\'") + "');", true);
                }
            }
        }

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string estado = DataBinder.Eval(e.Row.DataItem, "Estado").ToString();
                string estadoJefatura = DataBinder.Eval(e.Row.DataItem, "EstadoJefatura").ToString();
                int horaExtraID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "HoraExtraID"));

                Button btnGestionar = (Button)e.Row.FindControl("btnGestionar");
                Button btnEvidencia = (Button)e.Row.FindControl("btnEvidencia");

                // Configurar el botón "Gestionar"
                btnGestionar.Enabled = estado == "Pendiente";
                btnGestionar.CssClass = btnGestionar.Enabled ? "btn btn-submit" : "btn btn-submit";
                btnGestionar.PostBackUrl = btnGestionar.Enabled ? "~/SolicitudHorasExtraEmpleados.aspx" : "#";

                // Configurar el botón "Enviar Evidencia"
                btnEvidencia.Enabled = estado == "Aprobada" && estadoJefatura == "Sin Gestionar";
                btnEvidencia.CssClass = btnEvidencia.Enabled ? "btn btn-submit" : "btn btn-submit";
                if (btnEvidencia.Enabled)
                {
                    btnEvidencia.PostBackUrl = "~/evidenciaHorasExtra.aspx?HoraExtraID=" + horaExtraID;
                }
                else
                {
                    btnEvidencia.PostBackUrl = "#";
                }
            }
        }

    }
}