#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Yea.RuleEngine
{
    /// <summary>
    ///     规则实体
    /// </summary>
    public sealed class Rule<T>
    {
        internal Rule()
        {
            RuleType = NoMatchOption.Continue;
            Priority = 500;
            Enabled = true;
            RuleId = Guid.NewGuid();
            Tuples = new List<Tuple>();
        }

        /// <summary>
        ///     优先级，默认为500
        /// </summary>
        public int Priority { get; internal set; }

        public string Description { get; internal set; }
        internal IList<Tuple> Tuples { get; set; }
        public bool Enabled { get; internal set; }
        public NoMatchOption RuleType { get; internal set; }
        public RuleEngine<T> Engine { get; internal set; }
        public IList<Guid> ExclusionRules { get; private set; }
        internal Guid RuleId { get; set; }

        internal void Init()
        {
            if (ExclusionRules != null)
                foreach (var rule in ExclusionRules)
                    Engine.SetRuleDisabled(rule);
        }

        internal void AddExclusionRule(Guid rule)
        {
            if (ExclusionRules == null)
                ExclusionRules = new List<Guid>(8);

            ExclusionRules.Add(rule);
        }

        private bool Decide(T obj)
        {
            return Tuples.All(tuple => tuple.Decide(obj));
        }

        public void Handle(T obj)
        {
            if (!Enabled) return;
            if (!Decide(obj))
            {
                if (RuleType == NoMatchOption.Break)
                    Engine.Stop();
                return;
            }

            Engine.ActiveRules.AddRule(this);
        }

        #region Events

        public event RuleEevnt<T> RulePassed;

        internal void OnRulePassed()
        {
            FireEvent(RulePassed, this);
        }

        public event RuleEevnt<T> RuleFailed;

        internal void OnRuleFailed(Exception ex)
        {
            FireEvent(RuleFailed, this,
                      new RuleEventArgs {Message = ex.Message});
        }

        public event RuleEevnt<T> BeginRuleInvoke;

        internal void OnBeginRuleInvoke()
        {
            FireEvent(BeginRuleInvoke, this);
        }

        public event RuleEevnt<T> EndRuleInvoke;

        internal void OnEndRuleInvoke()
        {
            FireEvent(EndRuleInvoke, this);
        }

        private static void FireEvent(RuleEevnt<T> ruleDelegate, Rule<T> sender, RuleEventArgs args)
        {
            if (ruleDelegate != null)
            {
                ruleDelegate(sender, args);
            }
        }

        private static void FireEvent(RuleEevnt<T> ruleDeleget, Rule<T> sender)
        {
            FireEvent(ruleDeleget, sender, new RuleEventArgs());
        }

        #endregion

        #region Tuple

        /// <summary>
        ///     规则元组
        /// </summary>
        internal class Tuple
        {
            internal ICondition<T> Condition { get; set; }
            internal ICommand<T> Action { get; set; }

            internal bool Decide(T obj)
            {
                return Condition.Decide(obj);
            }

            internal void Handle(T obj)
            {
                if (Decide(obj))
                {
                    Action.Execute(obj);
                }
            }
        }

        #endregion
    }
}