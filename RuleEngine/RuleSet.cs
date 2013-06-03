#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Yea.RuleEngine
{
    public class RuleSet<T>
    {
        public RuleSet()
        {
            Items = new List<Rule<T>>(8);
        }

        internal IList<Rule<T>> Items { get; set; }

        internal void AddRule(Rule<T> rule)
        {
            if (!Contains(rule))
                Items.Add(rule);
            else
                throw new Exception("不能向规则集合中添加重复的规则");
        }

        internal void AddRuleSet(RuleSet<T> ruleSet)
        {
            foreach (var rule in ruleSet.Items)
                AddRule(rule);
        }

        internal bool Contains(Rule<T> rule)
        {
            return Items.Any(r => r.Id == rule.Id);
        }

        public bool ContainsRule(Guid ruleId)
        {
            return Items.Any(r => r.Id == ruleId);
        }

        public int Count()
        {
            return Items.Count;
        }
    }
}