using System;
using System.Linq;
using Guidance.Core;
using NUnit.Framework;

namespace Guidance.Tests
{
    public class GuidanceEvaluatorTests
    {
        static GuidanceRule Rule(string id, int priority = 0, bool showOnce = true, params ICondition[] conditions) =>
            new GuidanceRule(id, new GuidancePrompt("target", id), conditions, priority, showOnce);

        [Test]
        public void Candidates_OrderedByPriority_ThenDeclarationOrder()
        {
            var evaluator = new GuidanceEvaluator(
                new[] { Rule("a"), Rule("b", priority: 5), Rule("c") },
                new InMemoryProgressStore());

            var ids = evaluator.Candidates(new GuidanceContext()).Select(r => r.Id);

            CollectionAssert.AreEqual(new[] { "b", "a", "c" }, ids);
        }

        [Test]
        public void Candidates_SkipUnsatisfiedRules()
        {
            var evaluator = new GuidanceEvaluator(
                new[] { Rule("needs-home", conditions: Condition.Is("screen", "home")) },
                new InMemoryProgressStore());

            Assert.IsEmpty(evaluator.Candidates(new GuidanceContext().Set("screen", "shop")));
            Assert.IsNotEmpty(evaluator.Candidates(new GuidanceContext().Set("screen", "home")));
        }

        [Test]
        public void ShowOnce_HiddenAfterMarkShown_RepeatableStays()
        {
            var once = Rule("once");
            var repeat = Rule("repeat", showOnce: false);
            var evaluator = new GuidanceEvaluator(new[] { once, repeat }, new InMemoryProgressStore());
            var context = new GuidanceContext();

            evaluator.MarkShown(once);
            evaluator.MarkShown(repeat);

            CollectionAssert.AreEqual(new[] { "repeat" }, evaluator.Candidates(context).Select(r => r.Id));
        }

        [Test]
        public void DuplicateIds_Throw()
        {
            Assert.Throws<ArgumentException>(() =>
                new GuidanceEvaluator(new[] { Rule("x"), Rule("x") }, new InMemoryProgressStore()));
        }
    }
}
