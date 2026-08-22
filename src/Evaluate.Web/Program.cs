using Evaluate.Application;
using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Evaluations.Queries.GetStudentReportCardPdf;
using Evaluate.Infrastructure;
using Evaluate.Infrastructure.Identity;
using Evaluate.Infrastructure.Persistence;
using Evaluate.Web.Components;
using Evaluate.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<ConfirmService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EvaluateDbContext>();
    await DbSeeder.SeedAsync(dbContext);

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var gradingStrategy = scope.ServiceProvider.GetRequiredService<IGradingStrategy>();
    await SchoolDataSeeder.SeedAsync(dbContext, userManager, roleManager, gradingStrategy);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/api/report-cards/{studentId:int}/current", async (int studentId, IMediator mediator, HttpContext httpContext) =>
{
    try
    {
        var pdfBytes = await mediator.Send(new GetStudentReportCardPdfQuery(studentId));
        // "inline" opens the PDF in the browser tab as a preview; the browser's own viewer
        // still lets the user save it from there under the suggested filename.
        httpContext.Response.Headers.ContentDisposition = $"inline; filename=\"report-card-{studentId}.pdf\"";
        return Results.File(pdfBytes, "application/pdf");
    }
    catch (ReportCardNotReadyException ex)
    {
        return Results.Conflict(ex.Message);
    }
});

app.Run();
