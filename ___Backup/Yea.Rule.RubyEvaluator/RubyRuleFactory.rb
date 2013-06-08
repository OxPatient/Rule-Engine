include Yea::Rule
include Yea::Rule::RubyEvaluator
include $ruleAssembly$

class RubyRuleFactory

	def Create()
		internalCreate $contextType$, do |this| $condition$ end
	end

	def internalCreate(type, &condition)
		RubyRule.of(type).new condition
	end
end

