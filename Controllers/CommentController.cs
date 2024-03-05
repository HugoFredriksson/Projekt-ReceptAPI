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
    public class CommentController : Controller
    {
        MySqlConnection Connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=recept");

        [HttpGet("{RecipeId}")]
        public ActionResult<List<Comment>> GetCommentsFromRecipeId(int RecipeId)
        {
            List<Comment> comments = new List<Comment>();

            try
            {
                Connection.Open();
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "SELECT * FROM `comment` WHERE RecipeId = @RecipeId";
                command.Parameters.AddWithValue("@RecipeId", RecipeId);
                MySqlDataReader data = command.ExecuteReader();
                while (data.Read())
                {
                    Comment comment = new Comment();

                    comment.CommentId = data.GetInt32("CommentId");
                    comment.UserId = data.GetInt32("UserId");
                    comment.RecipeId = data.GetInt32("RecipeId");
                    comment.TimeStamp = data.GetString("Timestamp");
                    comment.Content = data.GetString("Content");
                    comment.UserName = data.GetString("UserName");

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
        [HttpDelete("DeleteComment")]
        public ActionResult DeleteComment(Comment comment)
        {
            try
            {
                Connection.Open();
                MySqlCommand command = Connection.CreateCommand();
                command.Prepare();
                command.CommandText = "DELETE FROM comment WHERE CommentId = @CommentId";
                command.Parameters.AddWithValue("CommentId", comment.CommentId);
                command.ExecuteNonQuery();

                Connection.Close();
                return StatusCode(200, $"Lyckades ta bort kommentar, KommentarId = {comment.CommentId} ");
            }
            catch (Exception exception)
            {
                Connection.Close();
                return StatusCode(500, exception.Message);
            }
        }

        [HttpPost("CreateComment")]
            public ActionResult CreateComment(Comment comment)
            {
                //Checks if user is logged in
                string authorization = Request.Headers["Authorization"];
                User user = (User)UserController.sessionId[authorization];

                //Attempts to insert comment values into database
                try
                {
                    Connection.Open();
                    MySqlCommand command = Connection.CreateCommand();
                    command.Prepare();
                    command.CommandText = "INSERT INTO `comment` (`UserId`, `RecipeId`, `TimeStamp`, `Content`) VALUES(@UserId, @RecipeId, (SELECT CURRENT_TIMESTAMP), @Content);";
                    command.Parameters.AddWithValue("@UserId", comment.UserId);
                    command.Parameters.AddWithValue("@RecipeId", comment.RecipeId);
                    command.Parameters.AddWithValue("@TimeStamp", comment.TimeStamp);
                    command.Parameters.AddWithValue("@Content", comment.Content);
                    command.ExecuteNonQuery();

                    Connection.Close();
                    return StatusCode(200, $"Lyckades skapa kommentar på blog = {comment.RecipeId} Content = {comment.RecipeId}");
                }
                catch (Exception exception)
                {
                    Connection.Close();
                    return StatusCode(500, exception.Message);
                }
        }
    }
    
}
