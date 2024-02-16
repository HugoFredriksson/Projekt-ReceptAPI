using Projekt_Recept.Controllers;

namespace Projekt_Recept
{
    public class Category
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Categories { get; set; } = string.Empty;
    }
}
