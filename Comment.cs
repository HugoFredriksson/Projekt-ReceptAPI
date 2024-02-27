using Projekt_Recept.Controllers;

namespace Projekt_Recept
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RecipeId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Content { get; set; }
        
    }
}
