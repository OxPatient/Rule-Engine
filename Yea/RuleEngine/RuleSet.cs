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
            Lists = new List<Rule<T>>(8);
        }

        internal IList<Rule<T>> Lists { get; set; }

        internal void AddRule(Rule<T> rule)
        {
            if (!Contains(rule))
                Lists.Add(rule);
            else
                throw new Exception("不能向规则集合中添加重复的规则");
        }

        internal void AddRuleSet(RuleSet<T> ruleSet)
        {
            foreach (var rule in ruleSet.Lists)
                AddRule(rule);
        }

        internal bool Contains(Rule<T> rule)
        {
            return Lists.Any(r => r.RuleId == rule.RuleId);
        }

        public bool ContainsRule(Guid ruleId)
        {
            return Lists.Any(r => r.RuleId == ruleId);
        }
    }
}