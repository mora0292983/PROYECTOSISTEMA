<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="PG_Expediente.aspx.cs" Inherits="proyectoC2.PG_Expediente" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <style>
        .main-content3 {
            display: flex;
            justify-content: center;
            align-items: center;
            height: auto;
            padding: 20px;
            gap: 20px;
        }

        .content-box {
            background-color: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            width: 45%;
            max-width: 650px; /* Increased width for grid display */
            padding: 20px;
            box-sizing: border-box;
        }

        .content-box h2 {
            color: #7154FC; /* Morado */
            font-weight: bold;
            margin-top: 0;
            text-align: center;
        }

        .form-group {
            margin-bottom: 15px;
        }

        .form-group label {
            display: block;
            font-weight: bold;
            margin-bottom: 5px;
        }

        .asp-control {
            width: 100%;
            padding: 10px;
            border-radius: 5px;
            border: 1px solid #ccc;
            box-sizing: border-box;
        }

        .search-container {
            position: relative;
        }

        .search-container input[type="text"] {
            width: 100%;
            padding-right: 40px; /* Espacio para el icono */
        }

        .search-container .icon {
            position: absolute;
            right: 10px;
            top: 50%;
            transform: translateY(-50%);
            color: #ccc;
            pointer-events: none;
        }

        .btn {
            background-color: #7154FC;
            color: white;
            padding: 10px 15px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            width: 48%;
            font-size: 16px;
            text-align: center;
        }

        .btn:hover {
            background-color: #5a005a;
        }

        .btn-group {
            display: flex;
            justify-content: space-between;
            margin-top: 20px;
        }

        .image-box {
            width: 45%;
            max-width: 650px; /* Increased width for grid display */
            margin-left: 20px; /* Ajusta este valor para mover la imagen hacia la derecha */
        }

        .image-box img {
            width: 100%;
            height: auto;
            border-radius: 8px;
            object-fit: cover;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }

        .gridview th, .gridview td {
            border: 1px solid #ddd;
            padding: 8px;
        }

        .gridview th {
            padding-top: 12px;
            padding-bottom: 12px;
            text-align: left;
            background-color: #7154FC;
            color: white;
        }
    </style>
    <!-- Incluye la biblioteca de iconos FontAwesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.3/css/all.min.css">

        <div class="main-content3">
            <div class="image-box">
                <img src="img/imgM.png" alt="Imagen de Ejemplo">
            </div>
            <div class="content-box">
                <h2>Registro de Días Festivos</h2>
                <div class="form-group search-container">
                    <asp:TextBox ID="search" runat="server" CssClass="asp-control" TextMode="Search" />
                    <i class="fas fa-search icon"></i>
                </div>
                <asp:GridView ID="gridView" runat="server" CssClass="gridview">
                   
                </asp:GridView>
                <div class="btn-group">
                    <asp:Button ID="btnExport" runat="server" CssClass="btn" Text="Exportar" />
                    <asp:Button ID="btnFilter" runat="server" CssClass="btn" Text="Filtrar" />
                </div>
            </div>
        </div>
 
 

</asp:Content>
