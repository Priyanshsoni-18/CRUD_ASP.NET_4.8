using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Web.UI;


namespace CRUD_pr
{
    public partial class SalesTran : System.Web.UI.Page
    {
        private string connectionString = WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindProductDropdown();
                BindGridView();
            }

        }
       

        
        private void BindProductDropdown()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id, product_name FROM product", con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddlProduct.DataSource = reader;
                    ddlProduct.DataTextField = "product_name";
                    ddlProduct.DataValueField = "id";
                    ddlProduct.DataBind();
                    con.Close();
                }
            }

            // Optionally, you can add a default item at the beginning of the dropdown list.
            ddlProduct.Items.Insert(0, new ListItem("-- Select Product --", "0"));
        }
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertSalesTran", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@sales_date", Convert.ToDateTime(txtSalesDate.Text));
                        cmd.Parameters.AddWithValue("@product_id", Convert.ToInt32(ddlProduct.SelectedValue));
                        cmd.Parameters.AddWithValue("@sales_qty", Convert.ToInt32(txtSalesQty.Text));
                        cmd.Parameters.AddWithValue("@sales_price", Convert.ToDecimal(txtSalesPrice.Text));

                        SqlParameter returnParameter = cmd.Parameters.Add("RetVal", SqlDbType.Int);
                        returnParameter.Direction = ParameterDirection.ReturnValue;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        int result = (int)returnParameter.Value;

                        if (result == -1)
                        {
                            lblError.Text = "Quantity exceeds available stock!";
                            lblError.ForeColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            lblError.Text = "Transaction successful!";
                            lblError.ForeColor = System.Drawing.Color.Green;
                            BindGridView(); // Refresh data grid after insert
                        }
                        con.Close();
                    }
                }
            }
        }

        private bool ValidateInput()
        {
            if (ddlProduct.SelectedValue == "0")
            {
                lblError.Text = "Please select a product.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSalesDate.Text))
            {
                lblError.Text = "Please enter a valid sales date.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return false;
            }

            int salesQty;
            if (!int.TryParse(txtSalesQty.Text, out salesQty) || salesQty <= 0)
            {
                lblError.Text = "Please enter a valid sales quantity.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return false;
            }

            decimal salesPrice;
            if (!decimal.TryParse(txtSalesPrice.Text, out salesPrice) || salesPrice <= 0)
            {
                lblError.Text = "Please enter a valid sales price.";
                lblError.ForeColor = System.Drawing.Color.Red;
                return false;
            }

            return true;
        }


        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateSalesTran", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtId.Text));
                    cmd.Parameters.AddWithValue("@sales_date", Convert.ToDateTime(txtSalesDate.Text));
                    cmd.Parameters.AddWithValue("@product_id", Convert.ToInt32(ddlProduct.SelectedValue));
                    cmd.Parameters.AddWithValue("@sales_qty", Convert.ToInt32(txtSalesQty.Text));
                    cmd.Parameters.AddWithValue("@sales_price", Convert.ToDecimal(txtSalesPrice.Text));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    BindGridView(); // Refresh data grid after update
                }
            }
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteSalesTran", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtId.Text));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    BindGridView(); // Refresh data grid after delete
                }
            }
        }


        protected void btnGet_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetSalesTranById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtId.Text));
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtSalesDate.Text = Convert.ToDateTime(reader["sales_date"]).ToString("yyyy-MM-dd");
                        ddlProduct.SelectedValue = reader["product_id"].ToString();
                        txtSalesQty.Text = reader["sales_qty"].ToString();
                        txtSalesPrice.Text = reader["sales_price"].ToString();
                    }
                    con.Close();
                }
            }
        }
        

        private void BindGridView()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllSalesTran", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        GridView1.DataSource = dt;
                        GridView1.DataBind();
                    }
                }
            }
        }

        // Row Editing
        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex = e.NewEditIndex;
            BindProductDropdown();
            BindGridView();
        }

        // Row Updating
        // Row Updating
        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            // Access the row that is being updated
            GridViewRow row = GridView1.Rows[e.RowIndex];

            // Retrieve the values from the controls
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
            DateTime salesDate = Convert.ToDateTime((row.FindControl("txtSalesDate") as TextBox).Text);
            int salesQty = Convert.ToInt32((row.FindControl("txtSalesQty") as TextBox).Text);
            decimal salesPrice = Convert.ToDecimal((row.FindControl("txtSalesPrice") as TextBox).Text);

            // Fetch the dropdown value correctly
            DropDownList ddlProduct = (DropDownList)row.FindControl("ddlProduct");
            int productId = Convert.ToInt32(ddlProduct.SelectedValue);

            // Update the database
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UpdateSalesTran", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@sales_date", salesDate);
                    cmd.Parameters.AddWithValue("@product_id", productId);
                    cmd.Parameters.AddWithValue("@sales_qty", salesQty);
                    cmd.Parameters.AddWithValue("@sales_price", salesPrice);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            // Reset the EditIndex and rebind the GridView
            GridView1.EditIndex = -1;
            BindGridView();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Check if the row is in edit mode
                if (e.Row.RowState.HasFlag(DataControlRowState.Edit))
                {
                    // Find the dropdown in the editing template
                    DropDownList ddlProduct = (DropDownList)e.Row.FindControl("ddlProduct");

                    // Check if the dropdown is not null
                    if (ddlProduct != null)
                    {
                        // Populate the dropdown
                        BindProductDropdown(ddlProduct);

                        // Set the selected value
                        int productId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "product_id"));
                        ddlProduct.SelectedValue = productId.ToString();
                    }
                }
            }
        }
        private void BindProductDropdown(DropDownList ddl)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id, product_name FROM product", con))
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    ddl.DataSource = reader;
                    ddl.DataTextField = "product_name";
                    ddl.DataValueField = "id";
                    ddl.DataBind();
                    con.Close();
                }
            }

            // Optionally, you can add a default item at the beginning of the dropdown list.
            ddl.Items.Insert(0, new ListItem("-- Select Product --", "0"));
        }




        // Row Canceling Edit
        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView1.EditIndex = -1;
            BindGridView();
        }

        // Row Deleting
        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteSalesTran", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            BindGridView();
        }



    }
}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.Web.Configuration;
//using System.Web.UI.WebControls;
//using System.Web.UI;


//namespace CRUD_pr
//{
//    public partial class SalesTran : System.Web.UI.Page
//    {
//        private string connectionString = WebConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;

//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!IsPostBack)
//            {
//                BindProductDropdown();
//                BindGridView();
//            }

//        }
//        private void BindProductDropdown()
//        {
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("SELECT id, product_name FROM product", con))
//                {
//                    con.Open();
//                    SqlDataReader reader = cmd.ExecuteReader();
//                    ddlProduct.DataSource = reader;
//                    ddlProduct.DataTextField = "product_name";
//                    ddlProduct.DataValueField = "id";
//                    ddlProduct.DataBind();
//                    con.Close();
//                }
//            }

//            // Optionally, you can add a default item at the beginning of the dropdown list.
//            ddlProduct.Items.Insert(0, new ListItem("-- Select Product --", "0"));
//        }
//        protected void btnInsert_Click(object sender, EventArgs e)
//        {
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_InsertSalesTran", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@sales_date", Convert.ToDateTime(txtSalesDate.Text));
//                    cmd.Parameters.AddWithValue("@product_id", Convert.ToInt32(ddlProduct.SelectedValue));
//                    cmd.Parameters.AddWithValue("@sales_qty", Convert.ToInt32(txtSalesQty.Text));
//                    cmd.Parameters.AddWithValue("@sales_price", Convert.ToDecimal(txtSalesPrice.Text));
//                    con.Open();
//                    cmd.ExecuteNonQuery();
//                    con.Close();
//                    BindGridView(); // Method to refresh data grid after insert
//                }
//            }
//        }

//        protected void btnUpdate_Click(object sender, EventArgs e)
//        {
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_UpdateSalesTran", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtId.Text));
//                    cmd.Parameters.AddWithValue("@sales_date", Convert.ToDateTime(txtSalesDate.Text));
//                    cmd.Parameters.AddWithValue("@product_id", Convert.ToInt32(ddlProduct.SelectedValue));
//                    cmd.Parameters.AddWithValue("@sales_qty", Convert.ToInt32(txtSalesQty.Text));
//                    cmd.Parameters.AddWithValue("@sales_price", Convert.ToDecimal(txtSalesPrice.Text));
//                    con.Open();
//                    cmd.ExecuteNonQuery();
//                    con.Close();
//                    BindGridView(); // Refresh data grid after update
//                }
//            }
//        }


//        protected void btnDelete_Click(object sender, EventArgs e)
//        {
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_DeleteSalesTran", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtId.Text));
//                    con.Open();
//                    cmd.ExecuteNonQuery();
//                    con.Close();
//                    BindGridView(); // Refresh data grid after delete
//                }
//            }
//        }


//        protected void btnGet_Click(object sender, EventArgs e)
//        {
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_GetSalesTranById", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@id", Convert.ToInt32(txtId.Text));
//                    con.Open();
//                    SqlDataReader reader = cmd.ExecuteReader();
//                    if (reader.Read())
//                    {
//                        txtSalesDate.Text = Convert.ToDateTime(reader["sales_date"]).ToString("yyyy-MM-dd");
//                        ddlProduct.SelectedValue = reader["product_id"].ToString();
//                        txtSalesQty.Text = reader["sales_qty"].ToString();
//                        txtSalesPrice.Text = reader["sales_price"].ToString();
//                    }
//                    con.Close();
//                }
//            }
//        }
        

//        private void BindGridView()
//        {
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_GetAllSalesTran", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
//                    {
//                        DataTable dt = new DataTable();
//                        sda.Fill(dt);
//                        GridView1.DataSource = dt;
//                        GridView1.DataBind();
//                    }
//                }
//            }
//        }

//        // Row Editing
//        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
//        {
//            GridView1.EditIndex = e.NewEditIndex;
//            BindProductDropdown();
//            BindGridView();
//        }

//        // Row Updating
//        // Row Updating
//        protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
//        {

//            // Access the row that is being updated
//            GridViewRow row = GridView1.Rows[e.RowIndex];

//            // Retrieve the values from the controls
//            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);
//            DateTime salesDate = Convert.ToDateTime((row.FindControl("txtSalesDate") as TextBox).Text);
//            int salesQty = Convert.ToInt32((row.FindControl("txtSalesQty") as TextBox).Text);
//            decimal salesPrice = Convert.ToDecimal((row.FindControl("txtSalesPrice") as TextBox).Text);

//            // Fetch the dropdown value correctly
//            DropDownList ddlProduct = (DropDownList)row.FindControl("ddlProduct");
//            int productId = Convert.ToInt32(ddlProduct.SelectedValue);

//            // Update the database
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_UpdateSalesTran", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@id", id);
//                    cmd.Parameters.AddWithValue("@sales_date", salesDate);
//                    cmd.Parameters.AddWithValue("@product_id", productId);
//                    cmd.Parameters.AddWithValue("@sales_qty", salesQty);
//                    cmd.Parameters.AddWithValue("@sales_price", salesPrice);
//                    con.Open();
//                    cmd.ExecuteNonQuery();
//                    con.Close();
//                }
//            }

//            // Reset the EditIndex and rebind the GridView
//            GridView1.EditIndex = -1;
//            BindGridView();
//        }

//        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
//        {
//            if (e.Row.RowType == DataControlRowType.DataRow)
//            {
//                // Check if the row is in edit mode
//                if (e.Row.RowState.HasFlag(DataControlRowState.Edit))
//                {
//                    // Find the dropdown in the editing template
//                    DropDownList ddlProduct = (DropDownList)e.Row.FindControl("ddlProduct");

//                    // Check if the dropdown is not null
//                    if (ddlProduct != null)
//                    {
//                        // Populate the dropdown
//                        BindProductDropdown(ddlProduct);

//                        // Set the selected value
//                        int productId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "product_id"));
//                        ddlProduct.SelectedValue = productId.ToString();
//                    }
//                }
//            }
//        }
//        private void BindProductDropdown(DropDownList ddl)
//        {
//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("SELECT id, product_name FROM product", con))
//                {
//                    con.Open();
//                    SqlDataReader reader = cmd.ExecuteReader();
//                    ddl.DataSource = reader;
//                    ddl.DataTextField = "product_name";
//                    ddl.DataValueField = "id";
//                    ddl.DataBind();
//                    con.Close();
//                }
//            }

//            // Optionally, you can add a default item at the beginning of the dropdown list.
//            ddl.Items.Insert(0, new ListItem("-- Select Product --", "0"));
//        }




//        // Row Canceling Edit
//        protected void GridView1_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
//        {
//            GridView1.EditIndex = -1;
//            BindGridView();
//        }

//        // Row Deleting
//        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
//        {
//            int id = Convert.ToInt32(GridView1.DataKeys[e.RowIndex].Values[0]);

//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("sp_DeleteSalesTran", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;
//                    cmd.Parameters.AddWithValue("@id", id);
//                    con.Open();
//                    cmd.ExecuteNonQuery();
//                    con.Close();
//                }
//            }
//            BindGridView();
//        }



//    }
//}
