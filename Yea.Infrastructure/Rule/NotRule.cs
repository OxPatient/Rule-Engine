namespace Yea.Infrastructure.Rule
{
    public class NotRule : IRule
    {
        protected NotRule()
        {
        }

        public NotRule(IRule rule)
        {
            Guard.NotNull(rule, "rule");
            LeftRule = rule;
        }

        public virtual IRule LeftRule { get; protected set; }

        public virtual bool Evaluate(object instance)
        {
            return !LeftRule.Evaluate(instance);
        }

        public override string ToString()
        {
            return string.Format("(!{0})", LeftRule);
        }

    }
}