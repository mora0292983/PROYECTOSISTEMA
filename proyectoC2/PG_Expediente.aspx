<%@ Page Title="" Language="C#" MasterPageFile="~/PaginaMaestra.Master" AutoEventWireup="true" CodeBehind="PG_Expediente.aspx.cs" Inherits="proyectoC2.PG_Expediente" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
     .main-content3 {
     display: flex;
     justify-content: center;
     align-items: center;
     height: calc(100vh - 60px);
     padding: 125px;
 }

 .content-box {
     background-color: #fff;
     border-radius: 8px;
     box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
     width: 80%;
     max-width: 1100px;
     display: flex;
 }

     .content-box img {
         border-top-left-radius: 8px;
         border-bottom-left-radius: 8px;
         width: 50%;
         object-fit: cover;
     }

</style>
        <div class="main-content3">
        <div class="content-box">
            <img src="img/imgM.png" alt="Imagen de Ejemplo">
            <div class="login-form">
             
                <form>
                   
                 
                </form>
            </div>
        </div>
    </div>
</asp:Content>
