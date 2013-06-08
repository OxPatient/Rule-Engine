using System;

namespace Yea.Rule.RubyEvaluator
{
    public interface ICondition
    {
        bool Evaluate(object arg);
    }

    public class RubyRule<TContext> : ICondition
    {
        private readonly Predicate<object> _evaluation;

        public RubyRule(Func<TContext, bool> evaluation)
        {
            _evaluation = x => (x is Func<TContext>) ?
                                        evaluation.Invoke(((Func<TContext>)x).Invoke()) :
                                   (x is TContext) && evaluation.Invoke((TContext)x);
        }

        public bool Evaluate(object arg)
        {
            return _evaluation(arg);
        }
    }
}
