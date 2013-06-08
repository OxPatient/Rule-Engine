using System.Collections.Generic;

namespace Yea.Infrastructure.Rule
{
    public interface IRulesService
    {
        bool IsValid<T>(T origin, IRule rule);
        bool IsValid<T>(T origin, IEnumerable<IRule> rules);
        IEnumerable<T> Filter<T>(IEnumerable<T> origin, IRule rule);
        IEnumerable<T> Filter<T>(IEnumerable<T> origin, IEnumerable<IRule> rules);
    }
}