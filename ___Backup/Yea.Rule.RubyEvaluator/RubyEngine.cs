using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using IronRuby;
using Microsoft.Scripting.Hosting;
using Yea.Rule.Engine;

namespace Yea.Rule.RubyEvaluator
{
    public class RubyEngine
    {
        private readonly string _dslFileName;
        private readonly List<Assembly> _assemblies;
        private readonly Type _contextType;
        private readonly string _condition;
        private ScriptEngine _engine;
        private ScriptSource _source;

        public RubyEngine(Type type, string condition)
        {
            _contextType = type;
            _condition = condition;
            _dslFileName = "RubyRuleFactory.rb";
            _assemblies = new List<Assembly>
                                  {
                                      typeof(RuleBase).Assembly,
                                      typeof(RubyEngine).Assembly,
                                      type.Assembly
                                  };
        }


        public ICondition Create()
        {
            _engine = Ruby.CreateEngine();

            string rubyRuleTemplate;
            using (var stream = Assembly.GetAssembly(this.GetType()).GetManifestResourceStream(_dslFileName))
            {
                using (var reader = new StreamReader(stream))
                {
                    rubyRuleTemplate = reader.ReadToEnd();
                }
            }
            rubyRuleTemplate = rubyRuleTemplate.Replace("$ruleAssembly$", _contextType.Namespace.Replace(".", "::"));
            rubyRuleTemplate = rubyRuleTemplate.Replace("$contextType$", _contextType.Name);
            rubyRuleTemplate = rubyRuleTemplate.Replace("$condition$", _condition);
            _source = _engine.CreateScriptSourceFromString(rubyRuleTemplate);

            var scope = _engine.CreateScope();
            _assemblies.ForEach(a => _engine.Runtime.LoadAssembly(a));

            _engine.Execute(_source.GetCode(), scope);
            var @class = _engine.Runtime.Globals.GetVariable("RubyRuleFactory");

            var installer = _engine.Operations.CreateInstance(@class);
            var rule = installer.Create();

            return (ICondition)rule;
        }
    }
}