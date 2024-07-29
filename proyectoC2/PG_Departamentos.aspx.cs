using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace proyectoC2
{
	public partial class PG_Departamentos : System.Web.UI.Page
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
			string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;
			string query = "SELECT DepartamentoID, NombreDepartamento, Descripcion FROM sistemaGp8.Departamentos";

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
				DataTable dataTable = new DataTable();
				dataAdapter.Fill(dataTable);
				gridDepartamentos.DataSource = dataTable;
				gridDepartamentos.DataBind();
			}
		}

		protected void btnNuevo_Click(object sender, EventArgs e)
		{
			Response.Redirect("PG_Login.aspx");
		}

		protected void btnEditar_Click(object sender, EventArgs e)
		{
			Response.Redirect("EditarRegistro.aspx");
		}

		protected void btnEliminar_Click(object sender, EventArgs e)
		{
			Response.Redirect("EliminarRegistro.aspx");
		}
		protected void btncontinuar_Click(object sender, EventArgs e)
		{
			Response.Redirect("PG_Login.aspx");
		}

	}
}