namespace Yea.Rule.Engine
{
    public abstract class RuleBase
    {
        public string Name { get; set; }
        public abstract bool Evaluate<T>(T context);

        public static RuleBase operator &(RuleBase rule1, RuleBase rule2)
        {
            return new OperationRule(rule1, rule2, Operation.And);
        }

        public static RuleBase operator |(RuleBase rule1, RuleBase rule2)
        {
            return new OperationRule(rule1, rule2, Operation.Or);
        }
    }
}