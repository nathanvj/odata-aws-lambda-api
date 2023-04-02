using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.NewtonsoftJson;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRouting(o => o.LowercaseUrls = true)
    .AddControllers()
    .AddOData(o =>
    {
        o.AddRouteComponents("api/v1", ODataEdmBuilder.GetEdmModel()).Select().OrderBy().Count().Filter().Expand().SetMaxTop(250);
    })
    .AddODataNewtonsoftJson()
    .AddNewtonsoftJson();

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();
app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.Run();

namespace ODataAPI
{
    public class TicketsController : ODataController
    {
        private readonly IQueryable<Ticket> _ticketsQueryable =
            new List<Ticket>()
            {
                new (new Guid("f447e347-246b-4989-9168-90cc670267e2"), "Ticket 1", DateTime.UtcNow.Subtract(TimeSpan.FromHours(0))),
                new (new Guid("442ba2c3-45cc-4f04-9922-29095a53a92b"), "Ticket 2", DateTime.UtcNow.Subtract(TimeSpan.FromHours(1))),
                new (new Guid("edea7155-2d3d-4308-87e0-52d695bc9510"), "Ticket 3", DateTime.UtcNow.Subtract(TimeSpan.FromHours(2))),
                new (new Guid("d0d5f399-98ea-4ec5-b641-e3371be34919"), "Ticket 4", DateTime.UtcNow.Subtract(TimeSpan.FromHours(3))),
                new (new Guid("258be4be-7463-4764-8038-710f1082e7b2"), "Ticket 5", DateTime.UtcNow.Subtract(TimeSpan.FromHours(4))),
            }
            .AsQueryable();

        /// <summary>
        /// Get all tickets.
        /// </summary>
        /// <param name="options">The odata query options.</param>
        [HttpGet]
        [CustomEnableQuery]
        public IActionResult Get(ODataQueryOptions<Ticket> options)
        {
            return Ok(_ticketsQueryable);
        }
    }

    public class CustomEnableQueryAttribute : EnableQueryAttribute
    {
        /// <summary>
        /// See https://github.com/OData/WebApi/issues/1227
        /// </summary>
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            actionExecutedContext.HttpContext.Response.StatusCode = 200;
            base.OnActionExecuted(actionExecutedContext);
        }
    }

    public class Ticket
    {
        public Ticket(Guid id, string subject, DateTime createdAt)
        {
            Id = id;
            Subject = subject;
            CreatedAt = createdAt;
        }

        public Guid Id { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public static class ODataEdmBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder conventionModelBuilder = new();
            conventionModelBuilder.EntitySet<Ticket>("Tickets");
            conventionModelBuilder.EnableLowerCamelCase();
            return  conventionModelBuilder.GetEdmModel();
        }
    }
}