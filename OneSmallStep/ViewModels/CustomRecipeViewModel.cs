using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OneSmallStep.ViewModels
{
    public class CustomRecipeViewModel
    {
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
        public List<string> ParsedTools { get; protected set; }

        public async Task Initialize()
        {
            TextEntry = string.Empty;
            ParsedTools = new List<string>();

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
                        break;
                }

                case CustomRecipeViewModel.Step.EquipmentConfirm:
                {
                    Title = "Equipment";
                    SubTitle = "Here's what I understood. Does this look right?";
                    break;
                }

                case CustomRecipeViewModel.Step.Ingredients:
                {
                    Title = "Ingredients";
                    SubTitle = "Now Enter the ingredients";
                        break;
                }

                case CustomRecipeViewModel.Step.IngredientsConfirm:
                {
                    Title = "Ingredients";
                    SubTitle = "Here's what I understood. Does this look right?";
                        break;
                }

                case CustomRecipeViewModel.Step.Steps:
                {
                    Title = "Recipe";
                    SubTitle = "Now Enter the recipe steps";
                        break;
                }

                case CustomRecipeViewModel.Step.StepsConfirm:
                {
                    Title = "Recipe";
                    SubTitle = "Here's what I understood. Does this look right?";
                        break;
                }


            }
        }
    }
}
