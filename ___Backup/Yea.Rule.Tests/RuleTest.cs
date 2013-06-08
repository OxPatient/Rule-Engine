using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yea.Rule.Engine;

namespace Yea.Rule.Tests
{
    public class Order
    {
        public double TotalPrice;
        public double Discount;

        public string Message;
    }

    [TestClass]
    public class RuleTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var order = new Order() { TotalPrice = 200, Discount = 0 };
            EvaluatorAccessPoint.DslConditionEvaluator = new RubyEvaluator.RubyEvaluator();

            var businessRule = "(this.TotalPrice > 100)";
            var businessAction = @"this.Discount = 10";
            var condition = new DslCondition { DslStatement = businessRule };
            var rule = new ActivityRule(condition, new DslActivity
            {
                DslStatement = businessAction
            });
            bool res = rule.Evaluate(order);
            Assert.AreEqual(res, true);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var flow = RuleFlow.For<Order>()
            .Do(a =>
                a.TotalPrice = 10)
            .Then
            .Decide(o => o.TotalPrice > 10)
                .WhenTrue(a => a.Do(t => t.TotalPrice = 10)
                    .Then
                        //.Decide(new DslCondition("this.TotalPrice > 20"))
                        .Decide(p => p.TotalPrice > 20)
                            .WhenTrue(b => b.Do(t => t.TotalPrice = 20))
                            .WhenFalse(b => b.Do(t => t.TotalPrice = 30))
                )
                .WhenFalse(a1 => a1.Do(t => t.TotalPrice = 30)
                    .Then.Do(new DslActivity("this.TotalPrice > 50"))
                        .Then.Do(t => t.TotalPrice = 50).Then
                            .Decide(aa => aa.TotalPrice > 40).WhenFalse(a => a.Do(a2 => a2.Message = "test")));

            var order = new Order { TotalPrice = 16 };

            flow.Execute(order);

            Assert.AreEqual("test", order.Message);
        }
    }
}
