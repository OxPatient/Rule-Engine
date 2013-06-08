using System;
using System.Collections.Generic;
using System.Linq;
using Yea.Infrastructure.Rule.Rules;

namespace Yea.Infrastructure.Rule
{
    public class Engine
    {
        public static IEnumerable<Type> GetRuleImplementations()
        {
            return typeof(EqualsRule).Assembly.GetExportedTypes().Where(x => x.Namespace == typeof(EqualsRule).Namespace);    
        }
        
        public static void Configure()
        {
            Environment.CurrentValueProviderFactory = new ValueProviders.DefaultValueProviderFactory();
        }
    }
}