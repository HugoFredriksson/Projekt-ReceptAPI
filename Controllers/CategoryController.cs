using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Encoders;
using Projekt_Recept;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Buffers.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Projekt_Recept.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        MySqlConnection Connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=recept");

        [HttpPost("AddCategoryToRecipe")]
        public ActionResult AddCategoryToRecipe(Category category)
        {
            //string authorization = Request.Headers["Authorization"];
            //User user = (User)UserController.sessionId[authorization];

            try
            {
                Connection.Open();
                string userHeader = Request.Headers["Authorization"];
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "INSERT INTO `category` (`Id`, `RecipeId`, `Category`) VALUES (@Id, @RecipeId, @Category)";
                command.Parameters.AddWithValue("Id", category.Id);
                command.Parameters.AddWithValue("RecipeId", category.RecipeId);
                command.Parameters.AddWithValue("Category", category.Categories);

                int rows = command.ExecuteNonQuery();

                if (rows == 0)
                {
                    Connection.Close();
                    return StatusCode(500, "Image rows was zero");
                }

            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
            Connection.Close();
            return StatusCode(201, "Lade Till Kategori");
        }
    }
}
   