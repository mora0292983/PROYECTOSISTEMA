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
    public partial class historialHorasExtraSupervisor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDatosGridView();
            }
        }

        private void CargarDatosGridView()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT HoraExtraID, EmpleadoID, Fecha, EstadoSolicitud, Estado FROM HorasExtraFinal";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);

                        // Convertir la columna Fecha al formato dd-MM-yyyy, manejando posibles errores de conversión
                        foreach (DataRow row in dt.Rows)
                        {
                            if (DateTime.TryParse(row["Fecha"].ToString(), out DateTime fecha))
                            {
                                row["Fecha"] = fecha.ToString("dd-MM-yyyy");
                            }
                            else
                            {
                                // Manejar la conversión fallida según sea necesario, por ejemplo, asignar un valor por defecto o lanzar una excepción
                                row["Fecha"] = "Fecha inválida";
                            }
                        }

                        gridView.DataSource = dt;
                        gridView.DataBind();
                    }
                }
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Redirigir a la página SolicitudHorasExtraSupervisor.aspx
            Response.Redirect("SolicitudHorasExtraSupervisor.aspx");
        }

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string estado = DataBinder.Eval(e.Row.DataItem, "Estado").ToString();
                string estadoSolicitud = DataBinder.Eval(e.Row.DataItem, "EstadoSolicitud").ToString();
                int horaExtraID = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "HoraExtraID"));

                Button btnGestionar = (Button)e.Row.FindControl("btnGestionar");

                // Habilitar o deshabilitar el botón según las condiciones especificadas
                if (estado == "Sin Gestionar" && estadoSolicitud == "Aceptada")
                {
                    btnGestionar.Enabled = true;
                    btnGestionar.CssClass = "btn btn-submit"; // Aplicar clase CSS cuando está habilitado
                    btnGestionar.PostBackUrl = $"~/gestionHorasExtra.aspx?HoraExtraID={horaExtraID}"; // Redirigir a la página con el ID
                }
                else
                {
                    btnGestionar.Enabled = false;
                    //btnGestionar.CssClass = "btn-submit:disabled"; // Aplicar clase CSS cuando está deshabilitado
                }
            }
        }
    }
}