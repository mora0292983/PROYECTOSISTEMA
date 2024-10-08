﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
	public partial class PG_Expediente : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			// Verifica si es la primera carga de la página
			if (!IsPostBack)
			{
				// Llama al método para cargar los datos en el GridView
				LoadData();
			}
		}
		private void LoadData()
		{
			// Obtiene la cadena de conexión del archivo web.config
			string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["conexion"].ConnectionString;

			// Define la consulta SQL
			string query = "SELECT DepartamentoID, NombreDepartamento, Descripcion FROM sistemaGp8.Departamentos";

			// Crea una instancia de SqlConnection y SqlDataAdapter
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

				// Crea un DataTable para almacenar los datos
				DataTable dataTable = new DataTable();

				// Llena el DataTable con los datos
				dataAdapter.Fill(dataTable);

				// Asigna el DataTable al GridView
				gridDepartamentos.DataSource = dataTable;
				gridDepartamentos.DataBind();
			}
		}



		protected void btnEditar_Click(object sender, EventArgs e)
		{
			// Redireccionar a la página de edición
			Response.Redirect("EditarRegistro.aspx");
		}

		protected void btnEliminar_Click(object sender, EventArgs e)
		{
			// Redireccionar a la página de eliminación
			Response.Redirect("EliminarRegistro.aspx");
		}
	}
}