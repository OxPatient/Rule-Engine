#region Usings

using System;

#endregion

namespace Yea.RuleEngine
{
    public delegate void RuleEevnt<T>(Rule<T> sender, RuleEventArgs e);

    public class RuleEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}