using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OneSmallStep.ViewModels
{
    public class CustomRecipeViewModel
    {
        private string _equipmentText;
        private string _ingredientText;
        private string _stepText;

        public enum Step
        {
            Equipment,
            EquipmentConfirm,
            Ingredients,
            IngredientsConfirm,
            Steps,
            StepsConfirm,
        }

        public string Title { get; set; }
        public string SubTitle { get; set; }

        public Step CurrentStep { get; set; }

        public string TextEntry { get; set; }
        public List<string> ConfirmList { get; protected set; }

        public List<string> ParsedEquipment { get; protected set; }
        public List<string> ParsedIngredients { get; protected set; }
        public List<string> ParsedSteps { get; protected set; }

        public async Task Initialize()
        {
            TextEntry = string.Empty;
            ParsedEquipment = new List<string>();

            CurrentStep = Step.Equipment;

            await SetupStep();
        }

        public async Task Back()
        {
            switch (CurrentStep)
            {
                case CustomRecipeViewModel.Step.Equipment:
                {
                    CurrentStep = Step.Equipment;
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
                    CurrentStep = Step.StepsConfirm;
                    break;
                }
            }

            await SetupStep();
        }

        private async Task SetupStep()
        {
            switch (CurrentStep)
            {
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
    }
}
