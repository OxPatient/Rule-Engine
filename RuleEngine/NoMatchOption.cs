namespace Yea.RuleEngine
{
    /// <summary>
    ///     规则类型
    ///     <remarks>
    ///         用来区别规则执行中遇到不匹配时的后续处理方式
    ///     </remarks>
    /// </summary>
    public enum NoMatchOption
    {
        /// <summary>
        ///     继续
        /// </summary>
        Continue = 0,

        /// <summary>
        ///     跳出
        /// </summary>
        Break = 1,
        /*/// <summary>
        /// 回滚
        /// </summary>
        RollBack,*/
    }
}