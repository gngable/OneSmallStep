using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using EventAggregator.Blazor;
using OneSmallStep.Database;
using OneSmallStep.Database.Models;
using OneSmallStep.Events;
using OneSmallStep.Utilities;

namespace OneSmallStep.ViewModels
{
    public class CustomRecipeViewModel
    {
        private readonly IEventAggregator _eventAggregator;
        private string _recipeDescription;
        private string _equipmentText;
        private string _ingredientText;
        private string _stepText;
        private int _recipeId;

        public enum Step
        {
            Title,
            Equipment,
            EquipmentConfirm,
            Ingredients,
            IngredientsConfirm,
            Steps,
            StepsConfirm,
            Code
        }

        public string Title { get; set; }
        public string SubTitle { get; set; }

        public Step CurrentStep { get; set; }

        public string RecipeTitle { get; set; }
        public string TextEntry { get; set; }
        public List<string> ConfirmList { get; protected set; }

        public List<string> ParsedEquipment { get; protected set; }
        public List<string> ParsedIngredients { get; protected set; }
        public List<string> ParsedSteps { get; protected set; }
        public string Code { get; set; }
        public bool CanGoBack => CurrentStep != Step.Title && CurrentStep != Step.Code;

        public CustomRecipeViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public async Task Initialize()
        {
            TextEntry = string.Empty;
            ParsedEquipment = new List<string>();

            CurrentStep = Step.Title;
            Code = string.Empty;

            await SetupStep();
        }

        public async Task Back()
        {
            switch (CurrentStep)
            {
                case CustomRecipeViewModel.Step.Title:
                {
                    CurrentStep = Step.Title;
                    break;
                }

                case CustomRecipeViewModel.Step.Equipment:
                {
                    CurrentStep = Step.Title;
                    break;
                }

                case CustomRecipeViewModel.Step.EquipmentConfirm:
                {
                    CurrentStep = Step.Equipment;
                    break;
                }

                case CustomRecipeViewModel.Step.Ingredients:
                {
                    CurrentStep = Step.EquipmentConfirm;
                        break;
                }

                case CustomRecipeViewModel.Step.IngredientsConfirm:
                {
                    CurrentStep = Step.Ingredients;
                        break;
                }

                case CustomRecipeViewModel.Step.Steps:
                {
                    CurrentStep = Step.IngredientsConfirm;
                        break;
                }

                case CustomRecipeViewModel.Step.StepsConfirm:
                {
                    CurrentStep = Step.Steps;
                        break;
                }
            }

            await SetupStep();
        }

        public async Task Forward()
        {
            switch (CurrentStep)
            {
                case CustomRecipeViewModel.Step.Title:
                {
                    _recipeDescription = TextEntry;
                    CurrentStep = Step.Equipment;

                    if (!string.IsNullOrWhiteSpace(Code))
                    {
                        using var context = new OneSmallStepContext { DatabasePath = "C:\\Git\\Personal\\OneSmallStep\\OneSmallStep.Database\\sktl.db" };

                        var recipe = context.Recipes.FirstOrDefault(r => r.AccessCode == Code);

                        if (recipe != null)
                        {
                            await _eventAggregator.PublishAsync(new StartRecipeEvent { RecipeId = recipe.Id });
                        }
                    }

                    break;
                }

                case CustomRecipeViewModel.Step.Equipment:
                {
                    _equipmentText = TextEntry;
                    ParsedEquipment = ParseList(_equipmentText);
                    ConfirmList = ParsedEquipment;
                    CurrentStep = Step.EquipmentConfirm;
                    break;
                }

                case CustomRecipeViewModel.Step.EquipmentConfirm:
                {
                    CurrentStep = Step.Ingredients;
                    break;
                }

                case CustomRecipeViewModel.Step.Ingredients:
                {
                    _ingredientText = TextEntry;
                    ParsedIngredients = ParseList(_ingredientText);
                    CurrentStep = Step.IngredientsConfirm;
                    break;
                }

                case CustomRecipeViewModel.Step.IngredientsConfirm:
                {
                    CurrentStep = Step.Steps;
                    break;
                }

                case CustomRecipeViewModel.Step.Steps:
                {
                    _stepText = TextEntry;
                    ParsedSteps = ParseList(_stepText, true);
                    CurrentStep = Step.StepsConfirm;
                    break;
                }

                case CustomRecipeViewModel.Step.StepsConfirm:
                {
                    CurrentStep = Step.Code;
                    Code = AlphaStringGenerator.RandomString(4);
                    break;
                }

                case CustomRecipeViewModel.Step.Code:
                {
                    //Start Transient
                    await _eventAggregator.PublishAsync(new StartRecipeEvent { RecipeId = _recipeId });
                    return;
                }
            }

            await SetupStep();
        }

        private async Task SetupStep()
        {
            switch (CurrentStep)
            {
                case CustomRecipeViewModel.Step.Title:
                {
                    Title = "Recipe";
                    SubTitle = "What's the name of the recipe?";
                    TextEntry = _recipeDescription;
                    break;
                }

                case CustomRecipeViewModel.Step.Equipment:
                {
                    Title = "Equipment";
                    SubTitle = "Enter what tools you'll need below (Like pots, measuring cups, etc)";
                    TextEntry = _equipmentText;
                    break;
                }

                case CustomRecipeViewModel.Step.EquipmentConfirm:
                {
                    Title = "Equipment";
                    SubTitle = "Here's what I understood. Does this look right?";
                    ConfirmList = ParsedEquipment;
                    break;
                }

                case CustomRecipeViewModel.Step.Ingredients:
                {
                    Title = "Ingredients";
                    SubTitle = "Now Enter the ingredients";
                    TextEntry = _ingredientText;
                    break;
                }

                case CustomRecipeViewModel.Step.IngredientsConfirm:
                {
                    Title = "Ingredients";
                    SubTitle = "Here's what I understood. Does this look right?";
                    ConfirmList = ParsedIngredients;
                    break;
                }

                case CustomRecipeViewModel.Step.Steps:
                {
                    Title = "Recipe";
                    SubTitle = "Now Enter the recipe steps";
                    TextEntry = _stepText;
                        break;
                }

                case CustomRecipeViewModel.Step.StepsConfirm:
                {
                    Title = "Recipe";
                    SubTitle = "Here's what I understood. Does this look right?";
                    ConfirmList = ParsedSteps;
                    break;
                }

                case CustomRecipeViewModel.Step.Code:
                {
                    Title = $"Code = {Code}";
                    SubTitle = "You can use this code for 2 days to access this recipe, or just click next to start now!";
                    await SaveRecipe();
                    break;
                }
            }
        }

        private List<string> ParseList(string input, bool includePeriod = false)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<string>();
            }

            var list = input.Split('\n').ToList();
            list = list.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).ToList();

            if (includePeriod)
            {
                list = list.SelectMany(l => l.Split('.')).Select(l => l.Trim()).ToList();
            }

            return list;
        }

        private async Task SaveRecipe()
        {
            using var context = new OneSmallStepContext { DatabasePath = "C:\\Git\\Personal\\OneSmallStep\\OneSmallStep.Database\\sktl.db" };

            var recipe = new Recipe();
            recipe.AccessCode = Code;
            recipe.ExpirationDate = DateTime.UtcNow.AddDays(2);

            recipe.Title = RecipeTitle;
            recipe.Description = _recipeDescription;

            var category = context.Category.First(c => c.Title == "Transient");

            recipe.CategoryId = category.Id;

            await context.Recipes.AddAsync(recipe);
            await context.SaveChangesAsync();

            recipe = context.Recipes.First(r => r.AccessCode == Code);
            _recipeId = recipe.Id;
            var equipmentCategory = context.IngredientCategories.First(c => c.Name == "Tools");

            foreach (var equipment in ParsedEquipment)
            {
                var ingredient = new Ingredient();
                ingredient.Name = equipment;
                ingredient.IngredientCategoryId = equipmentCategory.Id;
                ingredient.RecipeId = recipe.Id;

                await context.Ingredients.AddAsync(ingredient);
            }

            await context.SaveChangesAsync();

            var ingredientCategory = context.IngredientCategories.First(c => c.Name == "Ingredients");

            foreach (var ingredient in ParsedIngredients)
            {
                var ing = new Ingredient();
                ing.Name = ingredient;
                ing.IngredientCategoryId = ingredientCategory.Id;
                ing.RecipeId = recipe.Id;

                await context.Ingredients.AddAsync(ing);
            }

            await context.SaveChangesAsync();

            int i = 0;
            foreach (var step in ParsedSteps)
            {
                var s = new OneSmallStep.Database.Models.Step();

                s.Text = step;
                s.Rank = i++;
                s.RecipeId = recipe.Id;

                await context.Steps.AddAsync(s);
            }

            await context.SaveChangesAsync();
        }
    }
}
