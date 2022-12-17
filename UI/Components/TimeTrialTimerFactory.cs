using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class TimeTrialTimerFactory : IComponentFactory
    {
        public string ComponentName => "Time Trial Timer";
        public string Description => "Displays the total time from the in-game timer";

        public ComponentCategory Category => ComponentCategory.Information;
        public IComponent Create(LiveSplitState state) => new TimeTrialTimerComponent(state);

        public string UpdateName => ComponentName;
        public string UpdateURL => "https://raw.githubusercontent.com/Toyro98/LiveSplit.MirrorsEdge.TimeTrialTimer/main/";
        public string XMLURL => UpdateURL + "Components/LiveSplit.MirrorsEdge.TimeTrialTimer.xml";

        public Version Version => Version.Parse("1.1.1");
    }
}