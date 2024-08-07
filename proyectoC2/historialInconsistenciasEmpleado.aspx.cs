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

namespace proyectoC2
{
    public partial class historialInconsistenciasEmpleado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Establecer el valor inicial del TextBox
                empleadoIDtxt.Text = "19";

                // Llamar al método para cargar los datos del GridView usando el valor del TextBox
                if (int.TryParse(empleadoIDtxt.Text, out int empleadoID))
                {
                    Debug.WriteLine("Llamando a GetHistorialInconsistencias...");
                    GetHistorialInconsistencias(empleadoID);
                }
                else
                {
                    // Manejar el caso donde el ID no es un número válido
                    // Por ejemplo, mostrar un mensaje de error o registrar un log
                }
            }
        }


        private void GetHistorialInconsistencias(int empleadoID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string storedProcedure = "sp_ObtenerHistorialInconsistencias";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@EmpleadoID", empleadoID);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Verifica el contenido del DataTable
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
                // Obtener el ID del empleado desde el TextBox fuera del GridView
                string empleadoID = empleadoIDtxt.Text; // Usar el TextBox que ya está en la página
                string idInconsistencia = DataBinder.Eval(e.Row.DataItem, "IDInconsistencia").ToString();
                string fecha = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "Fecha")).ToString("yyyy-MM-dd");
                string tipoInconsistencia = DataBinder.Eval(e.Row.DataItem, "TipoInconsistencia").ToString();
                string estado = DataBinder.Eval(e.Row.DataItem, "Estado").ToString();

                // Asignar el valor al HiddenField dentro del GridView
                HiddenField hiddenEmpleadoID = (HiddenField)e.Row.FindControl("HiddenEmpleadoID");
                hiddenEmpleadoID.Value = empleadoID;

                // Configurar la URL de PostBack
                Button btnJustificar = (Button)e.Row.FindControl("btnJustificar");
                btnJustificar.PostBackUrl = $"~/JustificacionInconsistencias.aspx?IDInconsistencia={idInconsistencia}&EmpleadoID={empleadoID}&Fecha={fecha}&TipoInconsistencia={tipoInconsistencia}";

                // Habilitar o deshabilitar el botón basado en el estado
                btnJustificar.Enabled = estado == "Pendiente";
            }
        }

    }
}