using CodeHub.Helpers;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Background;

namespace CodeHub.Models
{
    class BackgroundTaskBuilderModel
    {
        private ICollection<IBackgroundCondition> _Conditions;

        public string Name { get; private set; }
        public IBackgroundTrigger Trigger { get; private set; }

        public BackgroundTaskBuilderModel(string name)
        {
            SetName(name);
        }

        public BackgroundTaskBuilderModel(string name, IBackgroundTrigger trigger)
            : this(name)
        {
            SetTrigger(trigger);
        }

        public BackgroundTaskBuilderModel(string name, params IBackgroundCondition[] conditions)
            : this(name)
        {
            SetConditions(conditions);
        }

        public BackgroundTaskBuilderModel(string name, IBackgroundTrigger trigger, params IBackgroundCondition[] conditions)
            : this(name, trigger)
        {
            SetConditions(conditions);
        }

        public void SetName(string name)
        {
            Name = StringHelper.IsNullOrEmptyOrWhiteSpace(name) ? throw new ArgumentNullException(nameof(name)) : name;
        }

        public void SetTrigger(IBackgroundTrigger trigger)
        {
            Trigger = trigger ?? throw new ArgumentNullException(nameof(trigger));
        }

        public void AddCondition(IBackgroundCondition condition)
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            _Conditions = _Conditions ?? new UniqueCollection<IBackgroundCondition>();

            if (!_Conditions.Contains(condition))
            {
                _Conditions.Add(condition);
            }
        }

        public void AddConditions(params IBackgroundCondition[] conditions)
        {
            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }

            _Conditions = _Conditions ?? new UniqueCollection<IBackgroundCondition>();

            foreach (var condition in conditions)
            {
                if (!_Conditions.Contains(condition))
                {
                    _Conditions.Add(condition);
                }
            }
        }

        public ref readonly ICollection<IBackgroundCondition> GetConditions()
        {
            return ref _Conditions;
        }

        public void CombineConditions(params IBackgroundCondition[] conditions)
        {
            _Conditions = _Conditions.Combine(conditions);
        }

        public void RemoveConditions(params IBackgroundCondition[] conditions)
        {
            if (conditions == null)
            {
                throw new ArgumentNullException(nameof(conditions));
            }

            _Conditions = _Conditions ?? new UniqueCollection<IBackgroundCondition>();

            foreach (var condition in conditions)
            {
                if (_Conditions.Contains(condition))
                {
                    _Conditions.Remove(condition);
                }
            }
        }

        public void SetConditions(params IBackgroundCondition[] conditions)
        {
            _Conditions = conditions ?? throw new ArgumentNullException(nameof(conditions));
        }
    }
}
