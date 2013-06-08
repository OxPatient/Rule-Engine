using System;
using Yea.Rule.Engine;

namespace Yea.Rule
{
    public class FlowNode<T> : FlowElement<T>
    {
        private ProcessNode<T> _actionNode;
        private DecisionNode<T> _decisionNode;

        public FlowNode()
        {
        }

        internal FlowNode(FlowElement<T> master) : base(master)
        {
        }

        internal override void Evaluate(T instance)
        {
            if (_decisionNode != null) _decisionNode.Evaluate(instance);
            if (_actionNode != null) _actionNode.Evaluate(instance);
        }

        public DecisionNode<T> Decide(Func<T, bool> func)
        {
            _decisionNode = new ConditionDecisionNode<T>(new FuncCondition<T>(func), Master);
            return _decisionNode;
        }

        public DecisionNode<T> Decide(ICondition condition)
        {
            _decisionNode = new ConditionDecisionNode<T>(condition, Master);
            return _decisionNode;
        }

        public ProcessNode<T> Do(Action<FlowNode<T>> action)
        {
            _actionNode = new FlowProcessNode<T>(action, Master);
            return _actionNode;
        }

        public ProcessNode<T> Do(Action<T> action)
        {
            _actionNode = new ActivityProcessNode<T>(new ActionActivity<T>(action), Master);
            return _actionNode;
        }

        public ProcessNode<T> Do(IActivity activity)
        {
            _actionNode = new ActivityProcessNode<T>(activity, Master);
            return _actionNode;
        }
    }
}