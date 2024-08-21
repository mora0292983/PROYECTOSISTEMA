using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace proyectoC2
{
	public partial class WebForm1666 : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Llamar al método para cargar los días disponibles al cargar la página
                CargarDiasDisponibles();
            }
        }

        private void CargarDiasDisponibles()
        {
            // Tu código para cargar días disponibles
        }

       
    }
}