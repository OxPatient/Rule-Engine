namespace Yea.Rule
{
    public static class RuleFlow
    {
        public static FlowNode<T> For<T>()
        {
            return new FlowNode<T>();
        }
    }
}