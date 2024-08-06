using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace proyectoC2
{
	public partial class HistoricoEmpPuestos : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                LoadData();
                LoadPuestos(); // Método para cargar los puestos en el dropdown
            }

        }
        private void LoadData(string searchTerm = "", DateTime? assignmentDate = null, int? puestoID = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = @"
                SELECT hp.EmpleadoPuestoID, e.Nombre AS NombreEmpleado, e.Apellido AS ApellidoEmpleado, 
                       p.NombrePuesto, hp.FechaAsignacion, hp.Departamento
                FROM sistemaGp8.EmpleadosPuestos hp
                JOIN sistemaGp8.Empleados e ON hp.EmpleadoID = e.EmpleadoID
                JOIN sistemaGp8.Puestos p ON hp.PuestoID = p.PuestoID
                WHERE (e.Nombre LIKE '%' + @searchTerm + '%' 
                       OR e.Apellido LIKE '%' + @searchTerm + '%')
                  AND (@assignmentDate IS NULL OR hp.FechaAsignacion = @assignmentDate)
                  AND (@puestoID IS NULL OR hp.PuestoID = @puestoID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchTerm", searchTerm ?? string.Empty);
                command.Parameters.AddWithValue("@assignmentDate", (object)assignmentDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@puestoID", (object)puestoID ?? DBNull.Value);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                gridView.DataSource = dataTable;
                gridView.DataBind();
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            string searchTerm = search.Text.Trim();
            DateTime? assignmentDate = null;
            int? puestoID = null;

            if (DateTime.TryParse(assignmentDateTextBox.Text, out DateTime parsedAssignmentDate))
            {
                assignmentDate = parsedAssignmentDate;
            }

            if (puestoDropDown.SelectedValue != "")
            {
                puestoID = int.Parse(puestoDropDown.SelectedValue);
            }

            LoadData(searchTerm, assignmentDate, puestoID);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            DataTable dataTable = (DataTable)gridView.DataSource;
            if (dataTable != null)
            {
                StringBuilder sb = new StringBuilder();
                // Agregar encabezados
                IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>().
                                                Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                // Agregar filas
                foreach (DataRow row in dataTable.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join(",", fields));
                }

                // Exportar como archivo CSV
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=Export.csv");
                Response.Charset = "";
                Response.ContentType = "application/text";
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
        }

        private void LoadPuestos()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = "SELECT PuestoID, NombrePuesto FROM sistemaGp8.Puestos";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                puestoDropDown.DataSource = dataTable;
                puestoDropDown.DataTextField = "NombrePuesto";
                puestoDropDown.DataValueField = "PuestoID";
                puestoDropDown.DataBind();
                puestoDropDown.Items.Insert(0, new ListItem("Seleccione un puesto", ""));
            }
        }
    }
}