using System;

namespace Yea.Rule
{
    public class FlowProcessNode<T> : ProcessNode<T>
    {
        private readonly Action<FlowNode<T>> _action;

        public FlowProcessNode(Action<FlowNode<T>> action, FlowElement<T> master)
            : base(master)
        {
            _action = action;
        }

        internal override void Evaluate(T instance)
        {
            if (_action != null)
            {
                var flowNode = new FlowNode<T>(Master);
                _action(flowNode);
                flowNode.Evaluate(instance);
            }
            base.Evaluate(instance);
        }
    }
}