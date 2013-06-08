namespace Yea.Rule
{
    public abstract class FlowElement<T>
    {
        protected FlowElement<T> Master;

        protected FlowElement()
        {
            Master = this;
        }

        protected FlowElement(FlowElement<T> master)
        {
            Master = master;
        }

        internal abstract void Evaluate(T instance);

        public virtual void Execute(T instance)
        {
            if (Master != null)
                Master.Evaluate(instance);
            else
                Evaluate(instance);
        }
    }
}