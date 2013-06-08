using System;

namespace Yea.Rule
{
    public abstract class DecisionNode<T> : FlowElement<T>
    {
        private FalseTrueNode<T> _falseNode;
        private TrueFalseNode<T> _trueNode;

        protected DecisionNode(FlowElement<T> master) : base(master)
        {
        }

        public TrueFalseNode<T> WhenTrue(Action<FlowNode<T>> action)
        {
            _trueNode = new TrueFalseNode<T>(action, Master);
            return _trueNode;
        }

        public FalseTrueNode<T> WhenFalse(Action<FlowNode<T>> action)
        {
            _falseNode = new FalseTrueNode<T>(action, Master);
            return _falseNode;
        }

        protected abstract bool Condition(T instance);

        internal override void Evaluate(T instance)
        {
            DicisionBranchNode<T> branchNode = _trueNode ?? (_falseNode ?? (DicisionBranchNode<T>) null);
            if (branchNode == null) return;

            if (Condition(instance))
                branchNode.Evaluate(instance);
            else
                branchNode.EvaluateOtherResult(instance);
        }
    }
}