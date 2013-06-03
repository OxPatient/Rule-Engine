#region Usings

using Yea.RuleEngine.Utils;

#endregion

namespace Yea.RuleEngine.Sample
{
    public class Account
    {
        public Account(int m)
        {
            Charge(m);
            Discount = 100;
        }

        /// <summary>
        ///     当前金额
        /// </summary>
        public int Money { get; private set; }

        /// <summary>
        ///     累计消费金额
        /// </summary>
        public int TotalMoney { get; private set; }

        /// <summary>
        ///     会员类型
        /// </summary>
        public MemberType Type { get; set; }

        /// <summary>
        ///     折扣率
        /// </summary>
        public int Discount { get; private set; }


        public void SetDiscount(int d)
        {
            Guard.NotOutOfRange(d, "d", 0, 100);
            Discount = d;
        }

        public void Charge(int m)
        {
            Money += m;
            TotalMoney += m;

            var engine = new RuleEngine<Account>();
            engine.AddRuleSet(new MemberTypeRule())
                  .Start(this);
        }

        public void Consume(int m)
        {
            var engine = new RuleEngine<Account>();
            engine.AddRuleSet(new ConsumeRule())
                  .Start(this);

            Money -= m*Discount/100;
        }
    }

    public enum MemberType
    {
        Normal = 0,
        Silver,
        Gold,
        Platinum,
        Diamond
    }
}