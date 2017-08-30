<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EventStatus.aspx.cs" Inherits="Maestro.app_monitoring.EventStatus" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
   
        <form id="form1" runat="server">
            
            <p style="margin-left: 30px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/assets/images/80288-200.png" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Image ID="Image2" runat="server" ImageUrl="~/assets/images/80288-200.png" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Image ID="Image3" runat="server" ImageUrl="~/assets/images/80288-200.png" />

                <asp:PlaceHolder ID="PlaceHolder1" runat="server"></asp:PlaceHolder>
            </p> 
            
         
          

            <asp:PlaceHolder ID="PlaceHolder2" runat="server"></asp:PlaceHolder>
           
        </form>

   </body>
</html>


<script type="text/javascript">

</script>
