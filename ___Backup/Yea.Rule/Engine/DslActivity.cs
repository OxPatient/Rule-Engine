namespace Yea.Rule.Engine
{
    public class DslActivity : IActivity
    {
        public DslActivity()
        {
        }

        public DslActivity(string statement)
        {
            DslStatement = statement;
        }

        public string DslStatement { get; set; }

        public virtual void Execute(object context)
        {
            if (EvaluatorAccessPoint.DslConditionEvaluator != null)
                EvaluatorAccessPoint.DslConditionEvaluator.Evaluate(DslStatement, context);
        }
    }
}