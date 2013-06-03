using Machine.Specifications;

using Yea.RuleEngine.Sample;

namespace Yea.RuleEngine.Tests
{
    public class AccountSubject
    {
        protected static Account _account;

        private Establish context = () => { _account = new Account(1); };

    }

    public class When_Account_Charge_100_Spec : AccountSubject
    {

        private Because of = () => _account.Charge(100);

        private It account_type_should_be_silver = () => _account.Type.ShouldEqual(MemberType.Silver);

        private It account_money_should_be_101 = () => _account.Money.ShouldEqual(101);
    }

    public class When_Account_Consume_10000_Sepc : AccountSubject
    {
        private Because of = () =>
        {
            _account.Charge(10000);
            _account.Consume(10000);
        };

        private It account_type_should_be_platinum = () => _account.Type.ShouldEqual(MemberType.Platinum);

        private It account_discount_should_be_80 = () => _account.Discount.ShouldEqual(80);

        private It account_money_should_be_2001 = () => _account.Money.ShouldEqual(2001);
    }
}