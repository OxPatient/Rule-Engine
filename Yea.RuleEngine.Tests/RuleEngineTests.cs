using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Machine.Specifications;

using Yea.RuleEngine.Sample;

namespace Yea.RuleEngine.Tests
{
    public class RuleEngineSubject
    {
        protected static RuleEngine<Account> _engine;

        Establish context = () =>
        {
            _engine = new RuleEngine<Account>();
        };
    }

    public class When_RuleEngine_Add_Ruleset : RuleEngineSubject
    {
        Because of = () =>
        {
            _engine.AddRuleSet(new MemberTypeRule());
            _engine.AddRuleSet(new ConsumeRule());
        };

        It rule_engine_rulebases_count_should_be_2 = () => { _engine.RuleBases.Count.ShouldEqual(2); };

        It rule_engine_active_rules_count_should_be_0 = () =>
            {
                _engine.ActiveRules.Count()
                    .ShouldEqual(0);
            };
    }

    public class When_RuleEngine_Running : RuleEngineSubject
    {
        static Account _account = new Account(88888);
        Because of = () =>
            {
                _engine.AddRuleSet(new MemberTypeRule());
                _engine.AddRuleSet(new ConsumeRule());
                _engine.Start(_account);
            };

        It rule_engine_active_rules_count_should_be_2 = () =>
            {
                _engine.ActiveRules.Count()
                    .ShouldEqual(2);
            };

        It account_discount_should_be_80 = () => { _account.Discount.ShouldEqual(80); };
    }
}
