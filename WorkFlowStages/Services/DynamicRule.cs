using NRules.Fluent.Dsl;
using NRules.RuleModel;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkFlowStages.Model;

namespace WorkFlowStages.Services
{
    public class DynamicRule : Rule
    {
        private readonly RuleDefinition _ruleDefinition;

        public DynamicRule(RuleDefinition ruleDefinition)
        {
            _ruleDefinition = ruleDefinition;
        }

        public override void Define()
        {
            Customer customer = null;

            var conditionExpressions = _ruleDefinition.Conditions.Select(c =>
                CreateConditionExpression(c)).ToList();

            When()
                .Match<Customer>(() => customer, c =>
                    conditionExpressions.All(expr => expr(c))
                );

            foreach (var action in _ruleDefinition.Actions)
            {
                Then()
                    .Do(ctx => Console.WriteLine(action.Message));
            }
        }

        private Func<Customer, bool> CreateConditionExpression(Condition condition)
        {
            return condition.Operator switch
            {
                ">" => c => Convert.ToDouble(GetPropertyValue(c, condition.Field)) > Convert.ToDouble(condition.Value),
                "<" => c => Convert.ToDouble(GetPropertyValue(c, condition.Field)) < Convert.ToDouble(condition.Value),
                "=" => c => Convert.ToDouble(GetPropertyValue(c, condition.Field)) == Convert.ToDouble(condition.Value),
                _ => throw new InvalidOperationException($"Unsupported operator: {condition.Operator}")
            };
        }

        private decimal GetPropertyValue(Customer customer, string field)
        {
            return field switch
            {
                "Age" => customer.Age,
                "TotalIncome" => customer.TotalIncome,
                _ => throw new InvalidOperationException($"Unsupported field: {field}")
            };
        }
    }
}