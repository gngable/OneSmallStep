using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventAggregator.Blazor;
using Microsoft.EntityFrameworkCore;
using OneSmallStep.Database;
using OneSmallStep.Database.Models;
using OneSmallStep.Events;
using Toolbelt.Blazor.SpeechSynthesis;

namespace OneSmallStep.ViewModels
{
    public class RecipeViewModel
    {
        private readonly SpeechSynthesis _speechSynthesis;
        private readonly IEventAggregator _eventAggregator;

        public const string IngredientStartText = "Let's get some things together";
        public const string StepStartText = "Let's start cooking!";

        public enum State
        {
            Start,
            IngredientStart,
            Ingredients,
            StepStart,
            Steps,
        }

        public int RecipeId { get; set; }

        public Recipe Recipe { get; protected set; }

        public List<Ingredient> Ingredients { get; protected set; }
        public Ingredient CurrentIngredient { get; protected set; }
        public string IngredientLabel => $"{CurrentIngredient.Name}{(CurrentIngredient.Quantity > 0 ? $" ({CurrentIngredient.Quantity}{(!string.IsNullOrWhiteSpace(CurrentIngredient.Unit) ? $" {CurrentIngredient.Unit}" : "")})" : "")}";
        public bool IngredientChecked { get; set; }
        public List<Step> Steps { get; protected set; }
        public Step CurrentStep { get; protected set; }

        public State CurrentState { get; set; } = State.Start;
        public int CurrentIndex { get; set; }

        public RecipeViewModel(SpeechSynthesis speechSynthesis, IEventAggregator eventAggregator)
        {
            _speechSynthesis = speechSynthesis;
            _eventAggregator = eventAggregator;
        }

        public async Task Load()
        {
            try
            {
                if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();
                using var context = new OneSmallStepContext { DatabasePath = "C:\\Git\\Personal\\OneSmallStep\\OneSmallStep.Database\\sktl.db" };
                Recipe = context.Recipes.FirstOrDefault(r => r.Id == RecipeId);
                Ingredients = context.Ingredients.Where(i => i.RecipeId == RecipeId).OrderBy(i => i.IngredientCategory.Rank).ThenBy(i => i.Name).ToList();
                Steps = context.Steps.Where(s => s.RecipeId == RecipeId).OrderBy(s => s.Rank).ToList();
                CurrentState = State.Start;

                _speechSynthesis.Speak(Recipe.Description);
            }
            catch (Exception ex)
            {
            }
        }

        public void StartRecipe()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            CurrentState = Ingredients.Any() ? State.IngredientStart : State.StepStart;

            _speechSynthesis.Speak(CurrentState == State.IngredientStart ? IngredientStartText : StepStartText);

            CurrentIndex = 0;
        }

        public void StartIngredients()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();
            CurrentState = State.Ingredients;
            CurrentIngredient = Ingredients.First();
            IngredientChecked = false;

            _speechSynthesis.Speak(IngredientLabel);
        }

        public void NextIngredient()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            CurrentIndex++;

            if (CurrentIndex >= Ingredients.Count)
            {
                CurrentState = State.StepStart;

                _speechSynthesis.Speak(StepStartText);

                CurrentIndex = 0;

                return;
            }

            CurrentIngredient = Ingredients[CurrentIndex];
            IngredientChecked = false;

            _speechSynthesis.Speak(IngredientLabel);
        }

        public void RepeatIngredient()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();
            _speechSynthesis.Speak(IngredientLabel);
        }

        public void PreviousIngredient()
        {
            if (CurrentIndex == 0)
            {
                CurrentState = State.IngredientStart;
                return;
            }

            CurrentIndex--;

            CurrentIngredient = Ingredients[CurrentIndex];
            IngredientChecked = false;

            _speechSynthesis.Speak(IngredientLabel);
        }

        public void StartSteps()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            CurrentState = State.Steps;
            CurrentStep = Steps.First();

            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public async Task NextStep()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            CurrentIndex++;

            if (CurrentIndex >= Steps.Count)
            {
                await _eventAggregator.PublishAsync(new RecipeFinishedEvent());

                return;
            }

            CurrentStep = Steps[CurrentIndex];

            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public void RepeatStep()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();
            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public void PreviousStep()
        {
            if (CurrentIndex == 0)
            {
                CurrentState = State.StepStart;
                return;
            }

            CurrentIndex--;

            CurrentStep = Steps[CurrentIndex];

            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public void StepStartBack()
        {
            CurrentState = State.Ingredients;

            CurrentIndex = Ingredients.Count - 1;

            CurrentIngredient = Ingredients[CurrentIndex];
            IngredientChecked = false;

            _speechSynthesis.Speak(IngredientLabel);
        }
    }
}
