#region Usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace Yea.RuleEngine
{
    public class RuleEngine<T>
    {
        private readonly bool _rollBackFlag;
        private bool _breakFlag;

        /// <summary>
        ///     规则引擎
        ///     <remarks>
        ///         当<see cref="rollBackWhenAnyRuleFailed" />为false时，只执行所有匹配规则,此为默认设置。
        ///         当<see cref="rollBackWhenAnyRuleFailed" />为true时，当存在任意不匹配规则时，则不执行任何规则，
        ///         直接退出引擎执行。
        ///     </remarks>
        /// </summary>
        /// <param name="rollBackWhenAnyRuleFailed">决定当有规则不匹配时，规则引擎执行逻辑</param>
        public RuleEngine(bool rollBackWhenAnyRuleFailed = false)
        {
            RuleBases = new List<RuleBase<T>>();
            ActiveRules = new RuleSet<T>();
            _rollBackFlag = rollBackWhenAnyRuleFailed;
        }

        public List<RuleBase<T>> RuleBases { get; internal set; }
        public RuleSet<T> ActiveRules { get; internal set; }

        private void Init()
        {
            _breakFlag = false;
            foreach (var rule in RuleBases)
            {
                foreach (var r in rule.Rules)
                {
                    r.Enabled = true;
                    r.Engine = this;
                }
            }
        }

        /// <summary>
        ///     开始执行
        /// </summary>
        public void Start(T obj)
        {
            Init();

            foreach (var ruleset in RuleBases)
            {
                foreach (var rule in ruleset.Rules.OrderByDescending(r => r.Priority))
                {
                    if (!ruleset.CanExcute)
                        break;

                    if (_breakFlag)
                    {
                        if (_rollBackFlag)
                            return;
                        break;
                    }

                    rule.Handle(obj);
                }
            }

            ExecActiveRules(obj);
        }

        private void ExecActiveRules(T obj)
        {
            foreach (var rule in ActiveRules.Items)
            {
                rule.Init();
                rule.OnBeginRuleInvoke();

                try
                {
                    foreach (var tuple in rule.Tuples)
                        tuple.Handle(obj);

                    rule.OnRulePassed();
                }
                catch (Exception ex)
                {
                    rule.OnRuleFailed(ex);
                }
                finally
                {
                    rule.OnEndRuleInvoke();
                }
            }
        }

        /// <summary>
        ///     终止执行
        /// </summary>
        internal void Stop()
        {
            _breakFlag = true;
        }

        internal void SetRuleDisabled(Guid id)
        {
            foreach (var rule in RuleBases)
            {
                foreach (var r in rule.Rules.Where(r => r.Id == id))
                {
                    r.Enabled = false;
                }
            }
        }

        public RuleEngine<T> AddRuleSet(RuleBase<T> ruleBase)
        {
            ruleBase.Build();

            RuleBases.Add(ruleBase);

            return this;
        }
    }
}