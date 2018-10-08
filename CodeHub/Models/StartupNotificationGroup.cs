using CodeHub.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeHub.Models
{
    public class StartupNotificationGroup : BindableBase, IStartupNotification
    {
        private readonly string _name;
        public string Name => _name;

        public override string ToString()
        {
            return Name;
        }

        public IList<IStartupNotification> Children { get; private set; }

        public StartupNotificationGroup(string name, IEnumerable<IStartupNotification> children)
        {
            _name = name;
            Children = new List<IStartupNotification>(children);
        }

        public bool Contains(Type scenarioType)
        {
            return FindScenario(scenarioType) != null;
        }

        public StartupNotification FindScenario(Type scenarioType)
        {
            foreach (var child in Children)
            {
                var found = child.FindScenario(scenarioType);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        public StartupNotificationGroup FindParent(IStartupNotification ofScenario)
        {
            foreach (var child in Children)
            {
                if (child == ofScenario)
                {
                    return this;
                }

                if (child is StartupNotificationGroup)
                {
                    var recursiveFound = (child as StartupNotificationGroup).FindParent(ofScenario);
                    if (recursiveFound != null)
                    {
                        return recursiveFound;
                    }
                }
            }

            return null;
        }

        public IList<StartupNotificationGroup> GetPathToScenario(StartupNotification scenario)
        {
            IList<StartupNotificationGroup> answer = new List<StartupNotificationGroup>() { this };

            GetPathToScenarioHelper(scenario, answer);

            return answer;
        }

        private void GetPathToScenarioHelper(StartupNotification scenario, IList<StartupNotificationGroup> listToAppendTo)
        {
            foreach (var child in Children.OfType<StartupNotification>())
            {
                if (child == scenario)
                {
                    return;
                }
            }

            foreach (var groupChild in Children.OfType<StartupNotificationGroup>())
            {
                if (groupChild.Contains(scenario.UIElementType))
                {
                    listToAppendTo.Add(groupChild);
                    groupChild.GetPathToScenarioHelper(scenario, listToAppendTo);
                }
            }
        }
    }
}
