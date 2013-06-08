#region Usings

using System;

#endregion

namespace Yea.RuleEngine
{
    public interface ICommand<in T>
    {
        void Execute(T obj);
    }

    public interface ICondition<in T>
    {
        bool Decide(T obj);
    }

    public interface IRuleDefinition<T>
    {
        IRuleDefinition<T> Id(Guid id);
        IRuleDefinition<T> Id(string id);
        IRuleDefinition<T> Describe(string description);
        IRuleDefinition<T> Priority(int priotiry);
        IRuleDefinition<T> RuleType(NoMatchOption type);
        IRuleDefinition<T> ExclusiveRule(Guid rule);
        IRuleDefinition<T> ExclusiveRule(string ruleId);
        ILeftSide<T> NewTuple();

        IRuleDefinition<T> OnPassed(RuleEevnt<T> evt);
        IRuleDefinition<T> OnFailed(RuleEevnt<T> evt);
        IRuleDefinition<T> OnBeginRuleInvoke(RuleEevnt<T> evt);
        IRuleDefinition<T> OnEndRuleInvoke(RuleEevnt<T> evt);
    }

    public interface ILeftSide<T>
    {
        IRightSide<T> When(ICondition<T> condition);
        IRightSide<T> When(Func<T, bool> condition);
    }

    public interface IRightSide<T>
    {
        IRuleDefinition<T> Then(ICommand<T> action);
        IRuleDefinition<T> Then(Action<T> action);
    }
}