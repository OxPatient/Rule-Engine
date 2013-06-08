namespace Yea.Infrastructure.Rule
{
    public class OrRule :  IRule
    {
        protected OrRule()
        {
        }

        public OrRule(IRule ruleLeft, IRule ruleRigth)
        {
            Guard.NotNull(ruleLeft, "ruleLeft");
            Guard.NotNull(ruleRigth, "ruleRigth");
            LeftRule = ruleLeft;
            RigthRule = ruleRigth;
        }

        public virtual IRule LeftRule { get; protected set; }
        public virtual IRule RigthRule { get; protected set; }

        public virtual bool Evaluate(object instance)
        {
            return LeftRule.Evaluate(instance) || RigthRule.Evaluate(instance);
        }

        public override string ToString()
        {
            return string.Format("({0} || {1})",LeftRule,RigthRule);
        }
    }
}