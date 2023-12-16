using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicZoneSystem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;


namespace MusicZoneSystem.Controllers
{
    public class HomeController : Controller
    {
        string mainconn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Requisition()
        {
            try
            {
                List<requisitionItemLists> productList = new List<requisitionItemLists>();
                List<viewRequisition> itemList = new List<viewRequisition>();

                using (var db = new SqlConnection(mainconn))
                {
                    db.Open();

                    // Fetch product data
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT * FROM [dbo].[product]";

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            sda.Fill(ds);

                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                requisitionItemLists product = new requisitionItemLists
                                {
                                    ReqItem_ID = Convert.ToInt32(dr["prod_id"]),
                                    ReqItem_Item = dr["prod_name"].ToString(),
                                    ReqItem_Desc = dr["prod_desc"].ToString(),
                                    ReqItem_Price = Convert.ToInt32(dr["prod_price"]),
                                };
                                productList.Add(product);
                            }
                        }
                    }

                    // Fetch canvas data
                    using (var cmd = db.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SELECT p.prod_name, p.prod_desc, c.canvas_quantity, c.canvas_unit, p.prod_price, c.canvas_total FROM canvas c JOIN product p ON c.prod_id = p.prod_id";

                        using (SqlDataReader canvasReader = cmd.ExecuteReader())
                        {
                            while (canvasReader.Read())
                            {
                                viewRequisition canvas = new viewRequisition
                                {
                                    CanvasItem = canvasReader["prod_name"].ToString(),
                                    CanvasDesc = canvasReader["prod_desc"].ToString(),
                                    CanvasQuantity = Convert.ToInt32(canvasReader["canvas_quantity"]),
                                    CanvasUnit = canvasReader["canvas_unit"].ToString(),
                                    CanvasPrice = Convert.ToDecimal(canvasReader["prod_price"]),
                                    CanvasTotal = Convert.ToDecimal(canvasReader["canvas_total"]),
                                };

                                itemList.Add(canvas);
                            }
                        }
                    }
                }

                requisitionModel combine = new requisitionModel
                {
                    requisitionItemList = productList,
                    canvas = itemList
                };

                return View(combine);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred while processing your request: {ex.Message}";
                return View("Error");
            }
        }

        private void InsertCanvasRecord(SqlConnection connection, SqlTransaction transaction, int quantity, decimal total, int selectedProduct, string unit)
        {
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandType = CommandType.Text;
                command.CommandText = "INSERT INTO [dbo].[canvas] (canvas_quantity, canvas_status, canvas_total, prod_id, canvas_unit) VALUES (@quantity, @stat, @total, @product, @unit)";

                command.Parameters.AddWithValue("@quantity", quantity);
                command.Parameters.AddWithValue("@stat", 0);
                command.Parameters.AddWithValue("@total", total);
                command.Parameters.AddWithValue("@product", selectedProduct);
                command.Parameters.AddWithValue("@unit", unit);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected < 1)
                {
                    throw new Exception("INSERT operation unsuccessful.");
                }
            }
        }
        [HttpPost]
        public ActionResult AddRequisition(requisitionItemLists model)
        {
            int product = model.ReqItem_Prod;
            int quantity = model.ReqItem_Qt;
            var totalValue = model.ReqItem_Total;
            string unit = model.ReqItem_Unit;
            using (var connection = new SqlConnection(mainconn))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        InsertCanvasRecord(connection, transaction, quantity, totalValue, product, unit);
                        transaction.Commit();
                        return RedirectToAction("Requisition");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                        return View("Index");
                    }
                }
            }
        }
        public ActionResult Supplier()
        {
            using (var db = new SqlConnection(mainconn))
            {
                db.Open();
                using (var cmd = db.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM [dbo].[supplier]";
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);

                    List<supplierDetails> lemp = new List<supplierDetails>();

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        supplierDetails supplier = new supplierDetails
                        {
                            // Populate properties based on your database columns
                            sup_id = Convert.ToInt32(dr["sup_id"]),
                            sup_company = dr["sup_company"].ToString(),
                            sup_address = dr["sup_address"].ToString(),
                            sup_lname = dr["sup_lname"].ToString(),
                            sup_fname = dr["sup_fname"].ToString(),
                            sup_mi = dr["sup_mi"].ToString(),
                            sup_email = dr["sup_email"].ToString(),
                            sup_phone = dr["sup_phone"].ToString(),
                        };

                        lemp.Add(supplier);
                    }

                    db.Close();

                    // Pass the list to the view
                    return View(lemp);
                }
            }
        }
        public ActionResult SupplierDetails(int supId)
        {
            using (var db = new SqlConnection(mainconn))
            {
                db.Open();
                using (var cmd1 = db.CreateCommand())
                {
                    try
                    {
                        cmd1.CommandType = CommandType.Text;
                        cmd1.CommandText = "SELECT * FROM [dbo].[product] WHERE sup_id = @supId;";
                        cmd1.Parameters.AddWithValue("@supId", supId);

                        SqlDataReader reader = cmd1.ExecuteReader();

                        if (reader.HasRows)
                        {
                            // Close the previous reader
                            reader.Close();

                            using (var cmd2 = db.CreateCommand())
                            {
                                cmd2.CommandType = CommandType.Text;
                                cmd2.CommandText = "SELECT * FROM [dbo].[supplier] WHERE sup_id = @supId;";
                                cmd2.Parameters.AddWithValue("@supId", supId);

                                SqlDataReader supplierReader = cmd2.ExecuteReader();

                                if (supplierReader.HasRows)
                                {
                                    supplierReader.Read();

                                    supplierDetails supplier = new supplierDetails
                                    {
                                        sup_id = Convert.ToInt32(supplierReader["sup_id"]),
                                        sup_company = supplierReader["sup_company"].ToString(),
                                        sup_address = supplierReader["sup_address"].ToString(),
                                        sup_lname = supplierReader["sup_lname"].ToString(),
                                        sup_fname = supplierReader["sup_fname"].ToString(),
                                        sup_mi = supplierReader["sup_mi"].ToString(),
                                        sup_email = supplierReader["sup_email"].ToString(),
                                        sup_phone = supplierReader["sup_phone"].ToString(),
                                    };

                                    supplierReader.Close();  // Close the supplierReader

                                    // Create a list to store product details
                                    List<productLists> productList = new List<productLists>();

                                    // Fetch all products associated with the supplier
                                    using (var cmd3 = db.CreateCommand())
                                    {
                                        cmd3.CommandType = CommandType.Text;
                                        cmd3.CommandText = "SELECT * FROM [dbo].[product] WHERE sup_id = @supId;";
                                        cmd3.Parameters.AddWithValue("@supId", supId);

                                        SqlDataReader productReader = cmd3.ExecuteReader();

                                        while (productReader.Read())
                                        {
                                            productLists product = new productLists
                                            {
                                                prod_id = Convert.ToInt32(productReader["prod_id"]),
                                                prod_code = productReader["prod_code"].ToString(),
                                                prod_name = productReader["prod_name"].ToString(),
                                                prod_desc = productReader["prod_desc"].ToString(),
                                                prod_price = productReader["prod_price"].ToString(),
                                            };

                                            productList.Add(product);
                                        }

                                        // Close the productReader
                                        productReader.Close();

                                        // Pass both objects to the view
                                        var combinedData = new Tuple<supplierDetails, List<productLists>>(supplier, productList);
                                        return View(combinedData);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception appropriately
                        return View("Error"); // You might want to create an error view
                    }
                    finally
                    {
                        // Close the connection in a finally block to ensure it's closed even in case of an exception
                        db.Close();
                    }
                }

                // Handle the case where the supplier with the given supId is not found
                return HttpNotFound();
            }
        }

        //[HttpDelete]
        //public ActionResult DeleteItem(int itemId)
        //{
        //    using (var db = new SqlConnection(mainconn))
        //    {
        //        db.Open();
        //        using (var cmd = db.CreateCommand())
        //        {
        //            cmd.CommandType = CommandType.Text;
        //            cmd.CommandText = "DELETE FROM [dbo].canvas where canvas_id = @canvasId";
        //            cmd.Parameters.AddWithValue("@canvasId", itemId);

        //            try
        //            {
        //                // Execute the DELETE statement.
        //                int rowsAffected = cmd.ExecuteNonQuery();

        //                // Check if any rows were affected to determine if the deletion was successful.
        //                if (rowsAffected > 0)
        //                {
        //                    return RedirectToAction("Requisition");
        //                }
        //                else
        //                {
        //                    return View("Index");
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                return View("Index");
        //            }
        //        }
        //    }
        //}
        public ActionResult Profile()
        {
            return View();
        }
    }
}