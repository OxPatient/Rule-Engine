#region Usings

using System.Collections.Generic;

#endregion

namespace Yea.RuleEngine
{
    public abstract class RuleBase<T>
    {
        private readonly RuleDefinition<T> _definition;

        private bool _buildFlag;

        protected RuleBase()
        {
            _definition = new RuleDefinition<T>();
        }

        protected IRuleDefinition<T> NewRule
        {
            get
            {
                _definition.Define();
                return _definition;
            }
        }

        public IList<Rule<T>> Rules
        {
            get
            {
                Build();
                return _definition.Rules.Lists;
            }
        }

        protected abstract void Define();

        public void Build()
        {
            if (!_buildFlag)
            {
                Define();
                _buildFlag = true;
            }
        }
    }
}