using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Machine.Specifications;

using Yea.RuleEngine.Sample;

namespace Yea.RuleEngine.Tests
{
    public class RuleBaseSubject
    {
        protected static RuleBase<Account> _rule;

        Establish context = () =>
            {
                _rule = new MemberTypeRule();
            };
    }

    public class When_Rule_Define_Spec : RuleBaseSubject
    {
        Because of = () => { _rule.Build(); };

        It rule_count_should_be_4 = () => { _rule.Rules.Count.ShouldEqual(4); };

        It rule_break_on_first_match_should_be_true = () => { _rule.IsBreakOnFirstMatch.ShouldEqual(true); };

        It rule_canExecute_should_be_ture = () => { _rule.CanExcute.ShouldEqual(true); };
    }
}
