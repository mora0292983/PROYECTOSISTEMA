using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class historialInconsistenciasSupervisor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetHistorialInconsistencias();
            }
        }


        private void GetHistorialInconsistencias()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string storedProcedure = "sp_ObtenerHistorialInconsistenciasSupervisor";

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
                string idInconsistencia = DataBinder.Eval(e.Row.DataItem, "IDInconsistencia").ToString();
                string fecha = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "Fecha")).ToString("yyyy-MM-dd");
                string tipoInconsistencia = DataBinder.Eval(e.Row.DataItem, "TipoInconsistencia").ToString();
                string estado = DataBinder.Eval(e.Row.DataItem, "Estado").ToString();
                string empleadoID = DataBinder.Eval(e.Row.DataItem, "IDEmpleado").ToString(); // Obtener el ID del empleado

                // Configurar la URL del botón "Gestionar"
                Button btnGestionar = (Button)e.Row.FindControl("btnGestionar");
                if (btnGestionar != null)
                {
                    btnGestionar.PostBackUrl = $"~/gestionInconsistencias.aspx?IDInconsistencia={idInconsistencia}&EmpleadoID={empleadoID}&Fecha={fecha}&TipoInconsistencia={tipoInconsistencia}";

                    // Habilitar el botón solo si el estado es "Sin Gestionar"
                    btnGestionar.Enabled = estado == "Sin Gestionar";
                }
            }
        }

    }
}