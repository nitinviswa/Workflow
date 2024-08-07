using NRules;
using NRules.Fluent;
using NRules.Fluent.Dsl;
using NRules.RuleModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WorkFlowStages.Dto;
using WorkFlowStages.Model;

namespace WorkFlowStages.Services
{
    public class ValidationService
    {
        public List<ValidationError> Validate(FormRequestDto customer, Rootobject formFields)
    {
        var payload = new Dictionary<string, string>
        {
            { "first_name", customer.FirstName },
            { "dob", customer.Dob },
            { "email", customer.Email },
            { "age", customer.Age.ToString() },
            { "total_income", customer.TotalIncome.ToString() }
        };

        var validator = new Validator(formFields.form.fields.ToList(), payload);
        return validator.Validate();
    }

  /*  public void EvaluateBusinessRules(Customer customer)
    {
        var repository = new RuleRepository();
        repository.Load(x => x.From(typeof(LoanEligibilityRule).Assembly));
        var factory = repository.Compile();
        var session = factory.CreateSession();

        session.Insert(customer);
        session.Fire();
    }

    public bool EvaluateDecisionTable(Customer customer)
    {
        var decisionTable = new LoanDecisionTable();
        return decisionTable.Evaluate(customer);
    }*/
    }
}