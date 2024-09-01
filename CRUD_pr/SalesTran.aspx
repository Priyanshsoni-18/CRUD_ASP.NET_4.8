<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesTran.aspx.cs" Inherits="CRUD_pr.SalesTran" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sales Transactions CRUD</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            margin: 0;
            padding: 20px;
        }

        #form1 {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
        }

        h2 {
            text-align: center;
            color: #333;
        }

        .form-control {
            margin-bottom: 15px;
            width: 100%;
        }

            .form-control input, .form-control select {
                padding: 10px;
                border: 1px solid #ccc;
                border-radius: 4px;
                width: calc(100% - 22px); /* Adjusting width to account for padding */
            }

        .btn {
            background-color: #5cb85c;
            color: white;
            border: none;
            padding: 10px 15px;
            border-radius: 4px;
            cursor: pointer;
            margin-right: 5px;
        }

            .btn:hover {
                background-color: #4cae4c;
            }

        .grid-container {
            margin-top: 20px;
            overflow-x: auto;
        }

            .grid-container table {
                width: 100%;
                border-collapse: collapse;
            }

            .grid-container th, .grid-container td {
                border: 1px solid #ddd;
                padding: 8px;
                text-align: left;
            }

            .grid-container th {
                background-color: #f2f2f2;
            }

        .page_contain {
            display: flex;
            padding: 40px;
            justify-content: center;
            flex-direction: column;
            align-items: center;
        }

        .Section_DataAndProduct {
            display: flex;
            justify-content: center;
        }

        .Main_box {
            width: 70%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="page_contain">
            <h2>Add Sales Transactions</h2>
            <div class="Main_box">
                <div class="form-control">
                    <asp:TextBox ID="txtId" runat="server" Placeholder="Transaction ID"></asp:TextBox>
                </div>
                <div class="form-control Section_DataAndProduct">
                    <asp:TextBox ID="txtSalesDate" runat="server" TextMode="Date" Placeholder="Sales Date"></asp:TextBox>
                    <asp:DropDownList ID="ddlProduct" runat="server"></asp:DropDownList>
                </div>
                <div class="form-control">
                    <asp:TextBox ID="txtSalesQty" runat="server" Placeholder="Sales Quantity"></asp:TextBox>
                </div>
                <div class="form-control">
                    <asp:TextBox ID="txtSalesPrice" runat="server" Placeholder="Sales Price"></asp:TextBox>
                </div>

                <div>
                    <asp:Button ID="btnInsert" runat="server" Text="Insert" OnClick="btnInsert_Click" CssClass="btn" />
                    <%-- <asp:Button ID="btnUpdate" runat="server" Text="Update" OnClick="btnUpdate_Click" CssClass="btn" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn" />
                    <asp:Button ID="btnGet" runat="server" Text="Get By ID" OnClick="btnGet_Click" CssClass="btn" />
                    --%>
                </div>
                <hr /><br />
                <h2>Sales Transactions List</h2>
                <div class="grid-container">
                    <asp:GridView ID="GridView1" runat="server" CssClass="grid-table" AutoGenerateColumns="False" DataKeyNames="id"
                        OnRowEditing="GridView1_RowEditing" OnRowCancelingEdit="GridView1_RowCancelingEdit"
                        OnRowUpdating="GridView1_RowUpdating" OnRowDeleting="GridView1_RowDeleting"
                        OnRowDataBound="GridView1_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="id" HeaderText="Transaction ID" ReadOnly="True" SortExpression="id" />
                            <asp:TemplateField HeaderText="Sales Date">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSalesDate" runat="server" Text='<%# Bind("sales_date") %>' TextMode="Date"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <%# Eval("sales_date") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Product">
                                <EditItemTemplate>
                                    <asp:DropDownList ID="ddlProduct" runat="server"></asp:DropDownList>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <%# Eval("product_name") %>
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Sales Quantity">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSalesQty" runat="server" Text='<%# Bind("sales_qty") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <%# Eval("sales_qty") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sales Price">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtSalesPrice" runat="server" Text='<%# Bind("sales_price") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <%# Eval("sales_price") %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowEditButton="True" />
                            <asp:CommandField ShowDeleteButton="True" />
                        </Columns>
                    </asp:GridView>
                    <div class="form-control">
                        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                    </div>


                </div>
            </div>
        </div>
    </form>
</body>
</html>
