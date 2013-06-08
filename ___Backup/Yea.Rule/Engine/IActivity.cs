namespace Yea.Rule.Engine
{
    public interface IActivity
    {
        void Execute(object context);
    }

    public interface IActivity<T> : IActivity
    {
        void Execute(T context);
    }
}