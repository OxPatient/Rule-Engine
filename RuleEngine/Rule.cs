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
            Id = Guid.NewGuid();
            Tuples = new List<Tuple<T>>();
        }

        /// <summary>
        ///     优先级，默认为500
        /// </summary>
        public int Priority { get; internal set; }

        public string Description { get; internal set; }
        internal IList<Tuple<T>> Tuples { get; set; }
        public bool Enabled { get; internal set; }
        public NoMatchOption RuleType { get; internal set; }
        public RuleEngine<T> Engine { get; internal set; }
        public RuleBase<T> Base { get; internal set; }
        public IList<Guid> ExclusionRules { get; private set; }
        public Guid Id { get; internal set; }

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
#if DEBUG
        public bool Decide(T obj)
#else
        private bool Decide(T obj)
#endif
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
            if (Base.IsBreakOnFirstMatch)
                Base.CanExcute = false;
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
                      new RuleEventArgs { Message = ex.Message });
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
#if DEBUG
        public class Tuple<T>
#else
        internal class Tuple<T>
#endif
        {
            internal ICondition<T> Condition { get; set; }
            internal IHandler<T> Action { get; set; }
#if DEBUG
            public bool Decide(T obj)
#else
            internal bool Decide(T obj)
#endif
            {
                return Condition.Decide(obj);
            }
#if DEBUG
            public void Handle(T obj)
#else
            internal void Handle(T obj)
#endif
            {
                if (Decide(obj))
                {
                    Action.Handle(obj);
                }
            }
        }

        #endregion
    }
}