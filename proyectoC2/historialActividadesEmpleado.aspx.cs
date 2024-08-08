using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
    public partial class historialActividadesEmpleado : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Establecer el valor inicial del TextBox
                empleadoIDtxt.Text = "19"; // Asigna el valor adecuado aquí

                // Llamar al método para cargar los datos del GridView usando el valor del TextBox
                if (int.TryParse(empleadoIDtxt.Text, out int empleadoID))
                {
                    Debug.WriteLine("Llamando a GetHistorialActividades...");
                    GetHistorialActividades(empleadoID);
                }
                else
                {
                    // Manejar el caso donde el ID no es un número válido
                    // Por ejemplo, mostrar un mensaje de error o registrar un log
                }
            }
        }

        private void GetHistorialActividades(int empleadoID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string storedProcedure = "sp_ObtenerActividadesPorEmpleado";

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
                string idActividad = DataBinder.Eval(e.Row.DataItem, "ActividadID").ToString();
                string fecha = Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "Fecha")).ToString("yyyy-MM-dd");
                string tipoActividad = DataBinder.Eval(e.Row.DataItem, "TipoActividad").ToString();
                string estado = DataBinder.Eval(e.Row.DataItem, "Estado").ToString();

                // Configurar la URL de PostBack para el botón Agregar Actividad
                //Button btnAgregarActividad = (Button)e.Row.FindControl("btnAgregarActividad");
                //btnAgregarActividad.PostBackUrl = $"~/RegistroActividadRealizada.aspx?ActividadID={idActividad}&EmpleadoID={empleadoID}&Fecha={fecha}&TipoActividad={tipoActividad}";

                // Habilitar o deshabilitar el botón basado en el estado
                //btnAgregarActividad.Enabled = estado == "Pendiente";
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            // Redirige al usuario a la página "RegistroActividadRealizada.aspx"
            Response.Redirect("RegistroActividadRealizada.aspx");
        }

    }
}