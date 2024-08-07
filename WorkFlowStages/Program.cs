using DocuSign.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRules;
using NRules.Fluent;
using NRules.RuleModel;
using WorkFlowStages.Data;
using WorkFlowStages.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddDbContext<WorkFlowStages.Data.AppContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("WebApiDatabase"));
});

// Register services
builder.Services.AddSingleton<SchemaValidator>();
builder.Services.AddScoped<ValidationService>();
builder.Services.AddScoped<DocuSignService>();
builder.Services.AddScoped<WorkFlowService>();
builder.Services.AddScoped<RuleEngineService>();


/*builder.Services.AddSingleton<IRuleRepository>(provider =>
{
    var repository = new RuleRepository();
    repository.Load(x => x.From(typeof(FormFieldValidationRule).Assembly));
    return repository;
});*/

/*builder.Services.AddSingleton(provider =>
{
    var repository = provider.GetRequiredService<IRuleRepository>();
    return repository.Compile();
});*/

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
