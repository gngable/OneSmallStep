using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventAggregator.Blazor;
using OneSmallStep.Events;

namespace OneSmallStep.Utilities
{
    public static class Thinking
    {
        public static async Task Think(this IEventAggregator eventAggregator)
        {
            await eventAggregator.PublishAsync(new ThinkingStartEvent());
            await Task.Delay(Globals.ThinkDelay);
            await eventAggregator.PublishAsync(new ThinkingDoneEvent());
        }
    }
}
