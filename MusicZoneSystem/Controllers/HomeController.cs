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
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Requisition()
        {
            string mainconn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            string sqlquery = "SELECT * FROM [dbo].[supplier]";
            SqlCommand sqlcomm = new SqlCommand(sqlquery, sqlconn);
            sqlconn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(sqlcomm);
            DataSet ds = new DataSet();
            sda.Fill(ds);

            List<supplierDetails> lemp = new List<supplierDetails>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                // Assuming supplierDetails has properties corresponding to your database columns
                supplierDetails supplier = new supplierDetails
                {
                    // Populate properties based on your database columns
                    sup_id = Convert.ToInt32(dr["sup_id"]),
                    sup_company = dr["sup_company"].ToString(),
                };

                lemp.Add(supplier);
            }

            sqlconn.Close();

            // Pass the list to the view
            return View(lemp);
        }
        public ActionResult Supplier()
        {
            string mainconn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            string sqlquery = "SELECT * FROM [dbo].[supplier]";
            SqlCommand sqlcomm = new SqlCommand(sqlquery, sqlconn);
            sqlconn.Open();
            SqlDataAdapter sda = new SqlDataAdapter(sqlcomm);
            DataSet ds = new DataSet();
            sda.Fill(ds);

            List<supplierDetails> lemp = new List<supplierDetails>();

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                // Assuming supplierDetails has properties corresponding to your database columns
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

            sqlconn.Close();

            // Pass the list to the view
            return View(lemp);
        }

        public ActionResult SupplierDetails(int supId)
        {
            string mainconn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            SqlConnection sqlconn = new SqlConnection(mainconn);
            string sqlquery = "SELECT * FROM [dbo].[supplier] WHERE sup_id = @supId";
            SqlCommand sqlcomm = new SqlCommand(sqlquery, sqlconn);
            sqlcomm.Parameters.AddWithValue("@supId", supId);
            sqlconn.Open();

            SqlDataReader reader = sqlcomm.ExecuteReader();

            if (reader.HasRows)
            {
                // Read the first row (assuming sup_id is unique)
                reader.Read();

                // Create a supplierDetails object
                supplierDetails supplier = new supplierDetails
                {
                    sup_id = Convert.ToInt32(reader["sup_id"]),
                    sup_company = reader["sup_company"].ToString(),
                    sup_address = reader["sup_address"].ToString(),
                    sup_lname = reader["sup_lname"].ToString(),
                    sup_fname = reader["sup_fname"].ToString(),
                    sup_mi = reader["sup_mi"].ToString(),
                    sup_email = reader["sup_email"].ToString(),
                    sup_phone = reader["sup_phone"].ToString(),
                };

                sqlconn.Close();

                // Pass the supplierDetails object to the view
                return View(supplier);
            }
            else
            {
                // Handle the case where the supplier with the given supId is not found
                sqlconn.Close();
                return HttpNotFound();
            }
        }

        public ActionResult Profile()
        {
            return View();
        }
    }
}