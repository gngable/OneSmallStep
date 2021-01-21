using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EventAggregator.Blazor;
using Microsoft.EntityFrameworkCore;
using OneSmallStep.Database;
using OneSmallStep.Database.Models;
using OneSmallStep.Events;
using Toolbelt.Blazor.SpeechSynthesis;

namespace OneSmallStep.ViewModels
{
    public class RecipeViewModel : INotifyPropertyChanged
    {
        private readonly SpeechSynthesis _speechSynthesis;
        private readonly IEventAggregator _eventAggregator;
        private string _timerLabel;
        private DateTime? _timerStart = null;
        private DateTime? _timerEnd = null;
        private bool _timerRunning;
        private const int ThinkTime = 500;

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

        public string TimerLabel
        {
            get => _timerLabel;
            set
            {
                _timerLabel = value;
                RaisePropertyChanged();
            }
        }

        public bool TimerRunning
        {
            get => _timerRunning;
            set
            {
                _timerRunning = value;
                RaisePropertyChanged();
            }
        }


        public RecipeViewModel(SpeechSynthesis speechSynthesis, IEventAggregator eventAggregator)
        {
            _speechSynthesis = speechSynthesis;
            _eventAggregator = eventAggregator;
        }

        public async Task Load()
        {
            try
            {
                StopTimer();
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

        public async Task StartRecipe()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

            CurrentState = Ingredients.Any() ? State.IngredientStart : State.StepStart;

            _speechSynthesis.Speak(CurrentState == State.IngredientStart ? IngredientStartText : StepStartText);

            CurrentIndex = 0;
        }

        public async Task StartIngredients()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

            CurrentState = State.Ingredients;
            CurrentIngredient = Ingredients.First();
            IngredientChecked = false;

            _speechSynthesis.Speak(IngredientLabel);
        }

        public async Task NextIngredient()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

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

        public async Task PreviousIngredient()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

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

        public async Task StartSteps()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

            CurrentState = State.Steps;
            CurrentStep = Steps.First();

            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public async Task NextStep()
        {
            StopTimer();

            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

            CurrentIndex++;

            if (CurrentIndex >= Steps.Count)
            {
                await _eventAggregator.PublishAsync(new RecipeFinishedEvent());

                return;
            }

            CurrentStep = Steps[CurrentIndex];

            if (CurrentStep.TimerSeconds.HasValue)
            {
                StartTimer(CurrentStep.TimerSeconds.Value);
            }

            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public void RepeatStep()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();
            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public async Task PreviousStep()
        {
            StopTimer();

            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

            if (CurrentIndex == 0)
            {
                CurrentState = State.StepStart;
                return;
            }

            CurrentIndex--;

            CurrentStep = Steps[CurrentIndex];

            _speechSynthesis.Speak(CurrentStep.Text);
        }

        public async Task StepStartBack()
        {
            if (_speechSynthesis.Speaking) _speechSynthesis.Cancel();

            await _eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(ThinkTime);
            await _eventAggregator.PublishAsync(new ThinkingDoneEvent());

            CurrentState = State.Ingredients;

            CurrentIndex = Ingredients.Count - 1;

            CurrentIngredient = Ingredients[CurrentIndex];
            IngredientChecked = false;

            _speechSynthesis.Speak(IngredientLabel);
        }

        private void StartTimer(int seconds)
        {
            _timerStart = DateTime.Now;
            _timerEnd = _timerStart.Value.AddSeconds(seconds);
            TimerRunning = true;

            Task.Run(TimerLoop);
        }

        private void StopTimer()
        {
            TimerRunning = false;
        }

        private async Task TimerLoop()
        {
            try
            {
                bool endReached = false;
                DateTime lastEndNotify = DateTime.Now;

                while (TimerRunning)
                {
                    if (DateTime.Now > _timerEnd.Value && !endReached)
                    {
                        TimerLabel = "00:00";
                        endReached = true;
                        _speechSynthesis.Speak("Time is up");
                        lastEndNotify = DateTime.Now;
                    }
                    else if (DateTime.Now > _timerEnd.Value && (DateTime.Now - lastEndNotify).TotalSeconds >= 10)
                    {
                        _speechSynthesis.Speak("Time is up");
                        lastEndNotify = DateTime.Now;
                    }
                    else
                    {
                        var ts = (_timerEnd.Value - DateTime.Now);

                        string time = "";

                        if (ts.Hours > 0)
                        {
                            time = $"{ts.Hours} hours, {ts.Minutes} minutes, {ts.Seconds} seconds left";
                        }
                        else if (ts.Minutes > 0)
                        {
                            time = $"{ts.Minutes} minutes, {ts.Seconds} seconds left";
                        }
                        else
                        {
                            time = $"{ts.Seconds} seconds left";
                        }

                        if (!string.Equals(time, TimerLabel))
                        {
                            TimerLabel = time;
                        }
                    }

                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
