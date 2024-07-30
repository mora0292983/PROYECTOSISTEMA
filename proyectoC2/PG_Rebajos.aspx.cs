using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Diagnostics;
using System.Web.UI;

namespace proyectoC2
{
    public partial class PG_Rebajos : System.Web.UI.Page
    {
        private object idInconsistencia;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnAprobar_Click(object sender, EventArgs e)
        {
            SaveFormData("Aprobado");

        }

        protected void btnRechazar_Click(object sender, EventArgs e)
        {
            SaveFormData("Rechazado");

        }



        private void SaveFormData(string estadoTramite)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["RebajosDBConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO RebajosSalariales (IdInconsistencia, IdEmpleado, FechaRebajo, FechaReporte, IdJefe, EstadoTramite, DetalleInconsistencia, TipoRebajo) " +
                               "VALUES (@IdInconsistencia, @IdEmpleado, @FechaRebajo, @FechaReporte, @IdJefe, @EstadoTramite, @DetalleInconsistencia, @TipoRebajo)";

               // using (SqlCommand command = new SqlCommand(query, connection))
                {
                   // command.Parameters.AddWithValue("@IdInconsistencia", idInconsistencia.Text);
                   // command.Parameters.AddWithValue("@IdEmpleado", idEmpleado.Text);
                   // command.Parameters.AddWithValue("@FechaRebajo", fechaRebajo.Text);
                   // command.Parameters.AddWithValue("@FechaReporte", fechaReporte.Text);
                   // command.Parameters.AddWithValue("@IdJefe", idJefe.Text);
                   // command.Parameters.AddWithValue("@EstadoTramite", estadoTramite);
                   // command.Parameters.AddWithValue("@DetalleInconsistencia", detalleInconsistencia.Text);
                    //command.Parameters.AddWithValue("@TipoRebajo", tipoRebajo.Text);

                   // connection.Open();
                   // command.ExecuteNonQuery();
                }//
            }
        }
    }
}