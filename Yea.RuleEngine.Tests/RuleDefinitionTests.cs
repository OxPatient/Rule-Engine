using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Machine.Specifications;

using Yea.RuleEngine.Sample;

namespace Yea.RuleEngine.Tests
{
    public class Rule_Definition_Subject
    {
        protected static RuleDefinition<Account> _define;

        Establish context = () => { _define = new RuleDefinition<Account>(); };
    }

    public class When_Rule_Definition_Do_NewRule_Spec : Rule_Definition_Subject
    {
        Because of = () => { _define.NewRule(); };
        It rule_count_should_equal_1 = () => { _define.Rules.Count().ShouldEqual(1); };
    }

    public class When_Rule_Definition_Do_Define_Spec : Rule_Definition_Subject
    {
        Because of = () =>
        {
            _define.NewRule().Describe("test rule description.")
                             .Priority(100)
                             .RuleType(NoMatchOption.Break)
                             .Id("FBA5CDB9-FDA3-49A1-926F-1018BDFF6A3B");
        };

        It rule_description_should_be_string = () => { _define.CurrentRule.Description.ShouldBe(typeof(string)); };

        It rule_description_shouled_equal_test_rule_description = () =>
        { _define.CurrentRule.Description.ShouldEqual("test rule description."); };

        It rule_priotiry_should_equal_100 = () => { _define.CurrentRule.Priority.ShouldEqual(100); };

        It rule_id_should_equal_ = () =>
            {
                _define.CurrentRule.Id.ToString().ToUpper()
                    .ShouldEqual("FBA5CDB9-FDA3-49A1-926F-1018BDFF6A3B");
            };

        It rule_type_should_equal_break = () => { _define.CurrentRule.RuleType.ShouldEqual(NoMatchOption.Break); };
    }

    public class When_Rule_Definition_Do_When_Then_Spec : Rule_Definition_Subject
    {
        static Account account = new Account(1111);

        Because of = () =>
           {
               _define.NewRule()
                   .NewTuple()
                   .When(p => p.TotalMoney > 1000)
                   .Then(p => p.SetDiscount(95));

           };

        It rule_decide_should_be_true = () =>
           {
               _define.CurrentTuple.Decide(account)
                   .ShouldEqual(true);
           };

        It account_discount_should_be_95 = () =>
        {
            if (_define.CurrentTuple.Decide(account))
                _define.CurrentTuple.Handle(account);
            account.Discount.ShouldEqual(95);
        };
    }
}
