<%@ Register tagprefix="rails" namespace="Castle.CastleOnRails.Framework.Views.Aspx" assembly="Castle.CastleOnRails.Framework" %>
<%@ Page language="c#" Codebehind="index.aspx.cs" AutoEventWireup="false" Inherits="TestSite.views.ajax.index" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>index</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body>
		<p>Static Part: <input type="text" value="never changes"></p>
		<div id="formContainer">
		<form id="Form1" method="post" runat="server">
			<rails:invokehelper id="GetJavascriptFunctions" runat="server" Method="GetJavascriptFunctions" Name="AjaxHelper"></rails:invokehelper>
			<h2>implemented</h2>
			<!-- ObserveField --> Observer field example: Please enter the zip code:<br>
			<input id="zip" type="text" name="zip">
			<br>
			<div id="address"></div>
			<rails:invokehelper id=ObserveField_Zip runat="server" Method="ObserveField" Name="AjaxHelper" Args='<%# new object[]{"zip", 2, "inferaddress.rails", "address", "\"aa new content\"" } %>'>
			</rails:invokehelper>
			<hr>
			<!-- ObserveForm -->
			<p>
				Observer form example: Fill the field below to create an account:<br>
				Name:
				<asp:textbox id="name" runat="server"></asp:textbox><br>
				Address:
				<asp:textbox id="addressf" runat="server"></asp:textbox><br>
				<asp:label id="message" runat="server"></asp:label>
			</p>
			<rails:invokehelper id=ObserveForm runat="server" Method="ObserveForm" Name="AjaxHelper" Args='<%# new object[]{ "Form1", 2, "accountformvalidate.rails", "message", "" } %>'>
			</rails:invokehelper>
			<hr>
			<h2>not implemented yet</h2>
			<!-- Remote Form -->
			<h4 id="status">Status</h4>
			<p>
				<asp:datagrid id="DataGrid1" runat="server"></asp:datagrid><br>
				<b>Add New User:</b> 
				
				<rails:invokehelper id="RemoteForm" runat="server"
				Name="AjaxHelper"
				Method="RemoteForm"
				Args='<%# new object[]{ "Form1",
										"AddUserWithAjax.rails", 
										"formContainer", 
										String.Empty, 
										"$(\"status\").innerHTML = \"Saving...\"", 
										String.Empty, 
										String.Empty, 
										"$(\"status\").innerHTML = \"Done!\"" } %>' />
				
				<table>
					<tr>
						<td>Name:</td>
						<td><asp:Textbox id="userNameField" runat="server" /></td>
					</tr>
					<tr>
						<td>EMail:</td>
						<td><asp:Textbox id="email" Runat="server" /></td>
					</tr>
					<tr>
						<td colspan="2" align="center">
							<asp:button id="addUserBtn" runat="server" text="Add User" onclick="OnAddUserClick" />
						</td>
					</tr>
				</table>
			</p>
			<asp:Button id="postBackerBtn" runat="server" Text="Do a simple PostBack"></asp:Button>
		</form>
		</div>
	</body>
</HTML>
