using System;
using System.Collections.Generic;
using System.Text;

namespace OneSmallStep.Database.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

        public int IngredientCategoryId { get; set; }
        public IngredientCategory IngredientCategory { get; set; }
    }
}
