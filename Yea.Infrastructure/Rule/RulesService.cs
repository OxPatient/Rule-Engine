using System.Collections.Generic;
using System.Linq;

namespace Yea.Infrastructure.Rule
{
    public class RulesService : IRulesService
    {
        public IEnumerable<T> Filter<T>(IEnumerable<T> origin, IRule rule)
        {
            return origin.Where(x => rule.Evaluate(x));
        }

        public bool IsValid<T>(T origin, IRule rule)
        {
            return rule.Evaluate(origin);
        }

        public IEnumerable<T> Filter<T>(IEnumerable<T> origin, IEnumerable<IRule> rules)
        {
            return origin.Where(entidad => rules.All(rule => rule.Evaluate(entidad)));
        }

        public bool IsValid<T>(T origin, IEnumerable<IRule> rules)
        {
            return rules.All(x => x.Evaluate(origin));
        }
    }
}