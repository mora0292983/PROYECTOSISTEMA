using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Text;

namespace proyectoC2
{
	public partial class HistoricoPuestos : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData(string searchTerm = "", DateTime? startDate = null, DateTime? endDate = null)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = @"SELECT hp.HistoricoID, e.Nombre AS NombreEmpleado, e.Apellido AS ApellidoEmpleado, 
                        p.NombrePuesto, hp.FechaInicio, hp.FechaFin
                     FROM sistemaGp8.HistoricoPuestos hp
                     JOIN sistemaGp8.Empleados e ON hp.EmpleadoID = e.EmpleadoID
                     JOIN sistemaGp8.Puestos p ON hp.PuestoID = p.PuestoID
                     WHERE (e.Nombre LIKE '%' + @searchTerm + '%' 
                            OR e.Apellido LIKE '%' + @searchTerm + '%')
                       AND (@startDate IS NULL OR hp.FechaInicio >= @startDate)
                       AND (@endDate IS NULL OR hp.FechaFin <= @endDate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchTerm", searchTerm ?? string.Empty);
                command.Parameters.AddWithValue("@startDate", (object)startDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@endDate", (object)endDate ?? DBNull.Value);

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
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (DateTime.TryParse(startDateTextBox.Text, out DateTime parsedStartDate))
            {
                startDate = parsedStartDate;
            }
            if (DateTime.TryParse(endDateTextBox.Text, out DateTime parsedEndDate))
            {
                endDate = parsedEndDate;
            }

            LoadData(searchTerm, startDate, endDate);
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
    }
}