using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Buffers.Text;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Projekt_Recept.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : Controller
    {
        MySqlConnection Connection = new MySqlConnection("server=localhost;uid=root;pwd=;database=recept");

    }
}
