using System;
using System.Collections.Generic;
using System.Linq;
using Yea.Infrastructure.Rule.Rules;

namespace Yea.Infrastructure.Rule
{
    public class RulesFactory
    {
        readonly Dictionary<string, Type> types = new Dictionary<string, Type>();
        
        public RulesFactory()
        {
            foreach (var t in typeof (EqualsRule).Assembly.GetExportedTypes().Where(x => x.Namespace == typeof (EqualsRule).Namespace))
            {
                string key = ((AbstractRule) Activator.CreateInstance(t)).GetOperation();
                types.Add(key,t);
            }
        }
        
        public string[] GetRules()
        {
            return types.Keys.OrderByDescending(x=> x).ToArray();
        }

        public IRule GetRule(string key, Type type, string property, string expectedValue)
        {
            Type t = types[key];
            var constructor = t.GetConstructor(new[] { typeof(string), typeof(Type), typeof(string) });
            if (constructor == null)
            {
                throw new ArgumentException("regla no valida");    
            }

            return (IRule)constructor.Invoke(new object[] { property, type, expectedValue });
        }
    }
}