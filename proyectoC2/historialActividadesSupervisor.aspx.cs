using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class historialActividadesSupervisor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetHistorialActividades();
            }
        }

        private void GetHistorialActividades()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string storedProcedure = "sp_ObtenerDatosActividad";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Verifica el contenido del DataTable para depuración
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        Debug.WriteLine("Column: " + column.ColumnName);
                    }

                    foreach (DataRow row in dataTable.Rows)
                    {
                        Debug.WriteLine(string.Join(", ", row.ItemArray));
                    }

                    gridView.DataSource = dataTable;
                    gridView.DataBind();
                }
            }
        }

        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Obtener los valores desde el DataRow
                string idActividad = DataBinder.Eval(e.Row.DataItem, "ActividadID").ToString();
                string empleadoID = DataBinder.Eval(e.Row.DataItem, "EmpleadoID").ToString();
                string fecha = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "Fecha")).ToString("yyyy-MM-dd");
                string nombre = DataBinder.Eval(e.Row.DataItem, "NombreCompletoEmpleado").ToString();
                string rendimiento = DataBinder.Eval(e.Row.DataItem, "Rendimiento").ToString();
                string estado = DataBinder.Eval(e.Row.DataItem, "EstadoSolicitud").ToString();

                // Buscar el botón "Gestionar" en la fila actual
                Button btnGestionar = (Button)e.Row.FindControl("btnGestionar");

                if (btnGestionar != null)
                {
                    // Deshabilitar el botón si el estado es "Gestionada"
                    bool isEnabled = estado != "Gestionada";
                    btnGestionar.Enabled = isEnabled;

                    // Aplicar el estilo si el botón está deshabilitado
                    if (!isEnabled)
                    {
                        btnGestionar.CssClass += " btn-disabled";
                    }
                }
            }
        }


        protected void btnGestionar_Click(object sender, EventArgs e)
        {
            Button btnGestionar = (Button)sender;
            string actividadID = btnGestionar.CommandArgument;

            // Redirigir a gestionActividades.aspx pasando el ActividadID
            Response.Redirect($"gestionActividades.aspx?ActividadID={actividadID}");
        }

    }
}