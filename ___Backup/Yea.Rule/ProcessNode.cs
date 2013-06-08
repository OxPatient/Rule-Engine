using System;

namespace Yea.Rule
{
    public class ProcessNode<T> : FlowElement<T>
    {
        private Action<FlowNode<T>> _continuedAction;
        private Action<T> _finalAction;
        private FlowNode<T> _then;

        public ProcessNode(FlowElement<T> master) : base(master)
        {
        }

        public FlowNode<T> Then
        {
            get { return _then ?? (_then = new FlowNode<T>(Master)); }
        }

        internal override void Evaluate(T instance)
        {
            if (_then != null) _then.Evaluate(instance);
        }
    }
}