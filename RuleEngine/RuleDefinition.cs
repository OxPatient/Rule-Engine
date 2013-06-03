#region Usings

using System;
using Yea.RuleEngine.Utils;

#endregion

namespace Yea.RuleEngine
{
    public class RuleDefinition<T> : IRuleDefinition<T>, ILeftSide<T>, IRightSide<T>
    {
#if DEBUG
        public RuleDefinition()
#else
        internal RuleDefinition()
#endif   
        {
            Rules = new RuleSet<T>();
        }

        public RuleSet<T> Rules { get; private set; }
#if DEBUG
        public Rule<T> CurrentRule { get; set; }
#else
        private Rule<T> CurrentRule { get; set; }
#endif 

#if DEBUG
        public Rule<T>.Tuple<T> CurrentTuple { get; set; }
#else
        private Rule<T>.Tuple<T> CurrentTuple { get; set; }
#endif

        public IRuleDefinition<T> OnPassed(RuleEevnt<T> evt)
        {
            CurrentRule.RulePassed += evt;
            return this;
        }

        public IRuleDefinition<T> OnFailed(RuleEevnt<T> evt)
        {
            CurrentRule.RuleFailed += evt;
            return this;
        }

        public IRuleDefinition<T> OnBeginRuleInvoke(RuleEevnt<T> evt)
        {
            CurrentRule.BeginRuleInvoke += evt;
            return this;
        }

        public IRuleDefinition<T> OnEndRuleInvoke(RuleEevnt<T> evt)
        {
            CurrentRule.EndRuleInvoke += evt;
            return this;
        }
#if DEBUG
        public IRuleDefinition<T> NewRule()
#else
        internal IRuleDefinition<T> NewRule()
#endif
        {
            CurrentRule = new Rule<T>();
            Rules.AddRule(CurrentRule);

            return this;
        }

        #region Implementation of ILeftSide

        public IRightSide<T> When(ICondition<T> condition)
        {
            Guard.NotNull(condition, "condition");
            CurrentTuple.Condition = condition;

            return this;
        }

        public IRightSide<T> When(Func<T, bool> condition)
        {
            Guard.NotNull(condition, "condition");

            var func = new FuncCondition<T>(condition);

            CurrentTuple.Condition = func;

            return this;
        }

        #endregion

        #region Implementation of IRightSide

        public IRuleDefinition<T> Then(IHandler<T> action)
        {
            Guard.NotNull(action, "action");

            CurrentTuple.Action = action;
            CurrentRule.Tuples.Add(CurrentTuple);

            return this;
        }

        public IRuleDefinition<T> Then(Action<T> action)
        {
            Guard.NotNull(action, "action");
            var act = new ActionHandler<T>(action);
            CurrentTuple.Action = act;
            CurrentRule.Tuples.Add(CurrentTuple);

            return this;
        }

        #endregion

        #region Implementation of IRuleDefinition

        public ILeftSide<T> NewTuple()
        {
            CurrentTuple = new Rule<T>.Tuple<T>();
            return this;
        }

        public IRuleDefinition<T> Id(Guid id)
        {
            CurrentRule.Id = id;
            return this;
        }

        public IRuleDefinition<T> Id(string id)
        {
            Guid guid = Guid.Parse(id);
            CurrentRule.Id = guid;
            return this;
        }

        public IRuleDefinition<T> Describe(string description)
        {
            CurrentRule.Description = description;
            return this;
        }

        public IRuleDefinition<T> Priority(int priotiry)
        {
            Guard.NotOutOfRange(priotiry, "priotiry", 0, 1000);
            CurrentRule.Priority = priotiry;
            return this;
        }

        public IRuleDefinition<T> RuleType(NoMatchOption type)
        {
            CurrentRule.RuleType = type;
            return this;
        }

        public IRuleDefinition<T> ExclusiveRule(Guid ruleId)
        {
            Guard.NotDefault(ruleId, "ruleId");
            CurrentRule.AddExclusionRule(ruleId);
            return this;
        }

        public IRuleDefinition<T> ExclusiveRule(string ruleId)
        {
            Guard.NotDefault(ruleId, "ruleId");
            Guid id = Guid.Parse(ruleId);
            CurrentRule.AddExclusionRule(id);
            return this;
        }
        #endregion
    }
}