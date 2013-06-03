#region Usings

using System.Collections.Generic;

#endregion

namespace Yea.RuleEngine
{
    public abstract class RuleBase<T>
    {
        private readonly RuleDefinition<T> _definition;

        private bool _buildFlag;

        public bool IsBreakOnFirstMatch { get; set; }

        protected RuleBase()
        {
            _definition = new RuleDefinition<T>();
            CanExcute = true;
        }

#if DEBUG
        public bool CanExcute { get; set; }
#else
        internal bool CanExcute { get; set; }
#endif
        
        protected IRuleDefinition<T> NewRule
        {
            get
            {
                _definition.NewRule();

                return _definition;
            }
        }

        public IList<Rule<T>> Rules
        {
            get
            {
                Build();
                foreach (var rule in _definition.Rules.Items)
                {
                    rule.Base = this;
                }
                return _definition.Rules.Items;
            }
        }

        protected abstract void Define();

#if DEBUG
        public void Build()
#else
        internal void Build()
#endif
        {
            if (!_buildFlag)
            {
                Define();
                _buildFlag = true;
            }
        }
    }
}