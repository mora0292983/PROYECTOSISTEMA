using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Web.Configuration;

namespace proyectoC2
{
	public partial class PG_ControlIncapacidades : System.Web.UI.Page
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
            string connectionString = WebConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = @"
        SELECT 
            e.Nombre AS NombreEmpleado,
            e.Apellido AS ApellidoEmpleado,
            e.Cedula AS CedulaEmpleado,
            t.NombreTipo AS TipoIncapacidad,
            i.FechaInicio,
            i.FechaFin,
            i.Descripcion,
            i.EstadoSolicitud
        FROM 
            sistemaGp8.Incapacidades i
        JOIN 
            sistemaGp8.Empleados e ON i.EmpleadoID = e.EmpleadoID
        JOIN 
            sistemaGp8.TiposIncapacidad t ON i.TipoIncapacidadID = t.TipoIncapacidadID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = CommandType.Text;

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
                string nombreEmpleado = DataBinder.Eval(e.Row.DataItem, "NombreEmpleado")?.ToString();
                string apellidoEmpleado = DataBinder.Eval(e.Row.DataItem, "ApellidoEmpleado")?.ToString();
                string cedulaEmpleado = DataBinder.Eval(e.Row.DataItem, "CedulaEmpleado")?.ToString();
                string tipoIncapacidad = DataBinder.Eval(e.Row.DataItem, "TipoIncapacidad")?.ToString();
                string fechaInicio = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "FechaInicio")).ToString("yyyy-MM-dd");
                string fechaFin = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "FechaFin")).ToString("yyyy-MM-dd");
                string descripcion = DataBinder.Eval(e.Row.DataItem, "Descripcion")?.ToString();
                string estadoSolicitud = DataBinder.Eval(e.Row.DataItem, "EstadoSolicitud")?.ToString();

                // Nota: El ID de incapacidad no se usa aquí

                // Configurar la URL del botón "Gestionar"
                Button btnGestionar = (Button)e.Row.FindControl("btnGestionar");
                if (btnGestionar != null)
                {
                    btnGestionar.PostBackUrl = $"~/pg_aprobacionincapacidad.aspx?NombreEmpleado={nombreEmpleado}&ApellidoEmpleado={apellidoEmpleado}&CedulaEmpleado={cedulaEmpleado}&FechaInicio={fechaInicio}&FechaFin={fechaFin}&TipoIncapacidad={tipoIncapacidad}&Descripcion={descripcion}";

                    // Habilitar el botón solo si el estado es "Sin Gestionar"
                    bool isEnabled = estadoSolicitud == "Sin Gestionar";
                    btnGestionar.Enabled = isEnabled;

                    // Aplicar el estilo si el botón está deshabilitado
                    if (!isEnabled)
                    {
                        btnGestionar.CssClass += " btn-disabled";
                    }
                }
            }
        }
    }
}