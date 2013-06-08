using System;

namespace Yea.Rule
{
    public class DicisionBranchNode<T> : FlowProcessNode<T>
    {
        protected ProcessNode<T> OnOtherResult;

        public DicisionBranchNode(Action<FlowNode<T>> action, FlowElement<T> master)
            : base(action, master)
        {
        }

        public void EvaluateOtherResult(T instance)
        {
            if (OnOtherResult != null) OnOtherResult.Evaluate(instance);
        }
    }

    public class TrueFalseNode<T> : DicisionBranchNode<T>
    {
        public TrueFalseNode(Action<FlowNode<T>> action, FlowElement<T> master)
            : base(action, master)
        {
        }

        public ProcessNode<T> WhenFalse(Action<FlowNode<T>> action)
        {
            OnOtherResult = new FlowProcessNode<T>(action, Master);
            return OnOtherResult;
        }
    }

    public class FalseTrueNode<T> : DicisionBranchNode<T>
    {
        public FalseTrueNode(Action<FlowNode<T>> action, FlowElement<T> master)
            : base(action, master)
        {
        }

        public ProcessNode<T> WhenTrue(Action<FlowNode<T>> action)
        {
            OnOtherResult = new FlowProcessNode<T>(action, Master);
            return OnOtherResult;
        }
    }
}