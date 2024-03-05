namespace Projekt_Recept
{
    public class PostRecipe
    {
        public string title {  get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string ingredients { get; set; } = string.Empty;
        public string[] categories { get; set; } = new string[0];
        public string imageUrl { get; set; } = string.Empty;
    }
}
