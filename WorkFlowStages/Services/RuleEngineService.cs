using NRules;
using NRules.Fluent;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WorkFlowStages.Model;

namespace WorkFlowStages.Services
{
    public class RuleEngineService
    {
        private readonly ISessionFactory _sessionFactory;

        public RuleEngineService()
        {
            string json = File.ReadAllText("./rules.json");
            RuleSet ruleSet = JsonConvert.DeserializeObject<RuleSet>(json);

            var repository = new RuleRepository();

            foreach (var ruleDefinition in ruleSet.Rules)
            {
                RegisterRule(repository, ruleDefinition);
            }

            _sessionFactory = repository.Compile();
        }

        private void RegisterRule(RuleRepository repository, RuleDefinition ruleDefinition)
        {
            var rule = new DynamicRule(ruleDefinition);
//repository.Load(x => x.Instance(rule));
        }

        public void Evaluate(Customer customer)
        {
            var session = _sessionFactory.CreateSession();
            session.Insert(customer);
            session.Fire();
        }
    }
}
