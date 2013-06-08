using System;

namespace Yea.Infrastructure.Rule
{
    public class RulesSet 
    {
        public virtual string Description { get; set; }
        public virtual IRule ParentRule { get; set; }
        public virtual Type RootType { get; set; }
    }
}