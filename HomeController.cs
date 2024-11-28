using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment environment;

        public HomeController(IWebHostEnvironment env)
        {
            environment = env;
        }

        private string GetConnectionString(string databaseFileName)
        {
            return environment.IsDevelopment()
                ? @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={environment.ContentRootPath}{databaseFileName};Integrated Security=True;Connect Timeout=15"
                : @$"Data Source=.\SQLEXPRESS;Integrated Security=SSPI;AttachDBFilename={environment.ContentRootPath}{databaseFileName};User Instance=true;Connect Timeout=15";
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            userModel userModel = new userModel();
            DataTable LocalTbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(GetConnectionString("myStore.mdf")))
            {
                sqlCon.Open();
                string query = "SELECT * FROM tblUser WHERE UID = @Id";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("Id", id);
                sqlDa.Fill(LocalTbl);
            }
            if (LocalTbl.Rows.Count == 1)
            {
                userModel.UID = Convert.ToInt32(LocalTbl.Rows[0][0].ToString());
                userModel.Username = LocalTbl.Rows[0][3].ToString();
                userModel.Password = LocalTbl.Rows[0][4].ToString();
                userModel.Förnamn = LocalTbl.Rows[0][1].ToString();
                userModel.Efternamn = LocalTbl.Rows[0][4].ToString();
                userModel.Picture = LocalTbl.Rows[0][5].ToString();
                return View(userModel);
            }
            else
            {
                return RedirectToAction("index");
            }
        }


        public ActionResult create()
        {
            return View();
        }
        public ActionResult Index()
        {
            DataTable localTbl = new DataTable();
            using (SqlConnection connection = new SqlConnection(GetConnectionString("WebApplication1"))) 
            {
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM tblUser", connection);
                connection.Open();
                sqlDa.Fill(localTbl);
            }
            return View(localTbl);
        }

        public ActionResult Delete(int id)
        {
            using (SqlConnection sqlConn = new SqlConnection(GetConnectionString("myStore.mdf")))
            {
                sqlConn.Open();

                string query = "DELETE FROM tblUser WHERE UID = @Id";
                SqlCommand sqlCmd = new SqlCommand(query, sqlConn);
                sqlCmd.Parameters.AddWithValue("@Id", id);
                sqlCmd.ExecuteReader();
            }
            return RedirectToAction("Index");
        }



    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]  
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
