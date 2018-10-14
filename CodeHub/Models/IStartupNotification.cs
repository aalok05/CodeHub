using System;

namespace CodeHub.Models
{
    public interface IStartupNotification
    {
        string Name { get; }

        bool Contains(Type scenarioType);

        StartupNotification FindScenario(Type scenarioType);
    }
}
