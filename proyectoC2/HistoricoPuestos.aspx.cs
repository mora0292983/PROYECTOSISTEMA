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

        private void LoadData()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
            string query = @"SELECT hp.HistoricoID, e.Nombre AS NombreEmpleado, e.Apellido AS ApellidoEmpleado, 
                            p.NombrePuesto, hp.FechaInicio, hp.FechaFin
                     FROM sistemaGp8.HistoricoPuestos hp
                     JOIN sistemaGp8.Empleados e ON hp.EmpleadoID = e.EmpleadoID
                     JOIN sistemaGp8.Puestos p ON hp.PuestoID = p.PuestoID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                gridView.DataSource = dataTable;
                gridView.DataBind();
            }
        }
    }
}