namespace Yea.Rule.Engine
{
    public class ActivityRule : ConditionalRule
    {
        private readonly IActivity _activity;

        public ActivityRule(ICondition condition,
                            IActivity activity) : base(condition)
        {
            _activity = activity;
        }

        public override bool Evaluate<T>(T context)
        {
            if (base.Evaluate(context))
            {
                _activity.Execute(context);
                return true;
            }
            return false;
        }
    }
}