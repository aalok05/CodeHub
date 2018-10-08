using System;
using CodeHub.Helpers;

namespace CodeHub.Models
{
    public class StartupNotification : BindableBase, IStartupNotification
    {
        /// <summary>
        /// Simply returns the PageUri, since that's guaranteed to be unique
        /// </summary>
        public string Id
        {
            get { return UIElementType.FullName; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        public Type UIElementType { get; private set; }

        public StartupNotification(string name, Type uiElementType)
        {
            _name = name;
            UIElementType = uiElementType;
        }

        public override string ToString()
        {
            return Name + " - " + Id;
        }

        public bool Contains(Type scenarioType)
        {
            return FindScenario(scenarioType) != null;
        }

        public StartupNotification FindScenario(Type scenarioType)
        {
            if (UIElementType == scenarioType)
            {
                return this;
            }

            return null;
        }
    }
}
