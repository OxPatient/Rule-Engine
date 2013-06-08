using Yea.Rule.Engine;

namespace Yea.Rule
{
    public class ActivityProcessNode<T> : ProcessNode<T>
    {
        private readonly IActivity _activity;

        public ActivityProcessNode(IActivity activity, FlowElement<T> master)
            : base(master)
        {
            _activity = activity;
        }

        internal override void Evaluate(T instance)
        {
            if (_activity != null) _activity.Execute(instance);
            base.Evaluate(instance);
        }
    }
}