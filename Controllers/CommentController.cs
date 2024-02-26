using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Relational;
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

namespace Projekt_Recept.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : Controller
    {
        MySqlConnection Connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=recept");

        [HttpGet("GetCommentsFromRecipeId/{RecipeId}")]
        public ActionResult<List<Comment>> GetCommentsFromRecipeId(int Id)
        {
            List<Comment> comments = new List<Comment>();

            try
            {
                Connection.Open();
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "SELECT * FROM `comment` WHERE RecipeId = @RecipeId;";
                command.Parameters.AddWithValue("@Id", Id);
                MySqlDataReader data = command.ExecuteReader();
                while (data.Read())
                {
                    Comment comment = new Comment();
                    
                    comment.UserId = data.GetInt32("userId");
                    comment.Content = data.GetString("content");
                    comment.TimeStamp = data.GetString("timestamp");
                    comment.RecipeId = data.GetInt32("RecipeId");

                    comments.Add(comment);
                }
                return StatusCode(200, comments);
            }
            catch (Exception ex)
            {
                Connection.Close();
                return StatusCode(500, ex.Message);
            }
        }

    }
}
