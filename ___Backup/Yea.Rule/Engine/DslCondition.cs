namespace Yea.Rule.Engine
{
    public class DslCondition : ICondition
    {
        public DslCondition()
        {
        }

        public DslCondition(string statement)
        {
            DslStatement = statement;
        }

        public string DslStatement { get; set; }

        public virtual bool Evaluate(object context)
        {
            return EvaluatorAccessPoint.DslConditionEvaluator != null
                   && EvaluatorAccessPoint.DslConditionEvaluator.Evaluate(DslStatement, context);
        }
    }
}