#region Usings

using System;
using System.Diagnostics;

#endregion

namespace Yea.RuleEngine.Sample
{
    public class MemberTypeRule : RuleBase<Account>
    {
        public MemberTypeRule()
        {
            IsBreakOnFirstMatch = true;
        }

        #region Overrides of RuleBase<Account>

        protected override void Define()
        {
            NewRule
                .Priority(5)
                .Describe("test rule")
                .OnBeginRuleInvoke((sender, e) => Debug.WriteLine(sender.Description))
                .OnEndRuleInvoke((serder, e) => Debug.WriteLine("rule invoked"))
                .OnFailed((sender, e) => Debug.WriteLine("rule failed"))
                .OnPassed((sender, e) => Debug.WriteLine("rule passed."))
                .NewTuple()
                .When(new FuncCondition<Account>(p => p.TotalMoney > 100))
                .Then(new ActionHandler<Account>(p =>
                    {
                        p.Type = MemberType.Silver;
                        Console.WriteLine("change member type to Silver.");
                    }));

            NewRule
                .Priority(6)
                .NewTuple()
                .When(p => p.TotalMoney > 1000)
                .Then(p =>
                    {
                        p.Type = MemberType.Gold;
                        Console.WriteLine("change member type to Gold.");
                    });

            NewRule
                .Priority(7)
                .NewTuple()
                .When(p => p.TotalMoney > 10000)
                .Then(p =>
                    {
                        p.Type = MemberType.Platinum;
                        Console.WriteLine("change member type to Platinum.");
                    });

            NewRule
                .Priority(8)
                .NewTuple()
                .When(p => p.TotalMoney > 100000)
                .Then(p =>
                    {
                        p.Type = MemberType.Diamond;
                        Console.WriteLine("change member type to Diamond.");
                    });
        }

        #endregion
    }

    public class ConsumeRule : RuleBase<Account>
    {
        public ConsumeRule()
        {
            IsBreakOnFirstMatch = true;
        }

        #region Overrides of RuleBase<Account>

        protected override void Define()
        {
            NewRule.Id(Guid.NewGuid())
                .Describe("钻石会员7折，如单次消费金额达到1万，则返现500")
                .NewTuple()
                .When(new FuncCondition<Account>(p => p.Type == MemberType.Diamond))
                .Then(new ActionHandler<Account>(p => p.SetDiscount(70)));

            NewRule.NewTuple()
                   .When(p => p.Type == MemberType.Platinum)
                   .Then(p =>
                       {
                           p.SetDiscount(80);
                           Console.WriteLine("discount 80");
                       });

            NewRule.NewTuple()
                   .When(new FuncCondition<Account>(p => p.Type == MemberType.Gold))
                   .Then(new ActionHandler<Account>(p =>
                       {
                           p.SetDiscount(90);
                           Console.WriteLine("discount 90");
                       }));

            NewRule.NewTuple()
                   .When(new FuncCondition<Account>(p => p.Type == MemberType.Silver))
                   .Then(new ActionHandler<Account>(p =>
                       {
                           p.SetDiscount(95);
                           Console.WriteLine("discount 95");
                       }));
        }

        #endregion
    }
}