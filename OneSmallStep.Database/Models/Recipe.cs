using System;
using System.Collections.Generic;
using System.Text;

namespace OneSmallStep.Database.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public List<Ingredient> Ingredients { get; } = new List<Ingredient>();
    }
}
