using SustiVest.Web;
using SustiVest.Data.Services;
using SustiVest.Data.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Authentication / Authorisation via extension methods 
builder.Services.AddCookieAuthentication();
//builder.Services.AddPolicyAuthorisation();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    // Configure connection string for selected database in appsettings.json

    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
    //  sqliteOptions => sqliteOptions.MigrationsAssembly("SustiVest.Data"));
    //options.UseMySql(builder.Configuration.GetConnectionString("MySql"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql")));
    //options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
    //options.UseSqlServer(Ybuilder.Configuration.GetConnectionString("SqlServer"));
});

// Add UserService to DI   
builder.Services.AddTransient<IUserService, UserServiceDb>();
builder.Services.AddTransient<IMailService, SmtpMailService>();
builder.Services.AddTransient<ICompanyService, CompanyServiceDb>();
builder.Services.AddTransient<IAssessmentsService, AssessmentServiceDb>();
// builder.Services.AddTransient<IAnalystsService, AnalystServiceDb>();
builder.Services.AddTransient<IOfferService, OfferServiceDb>();

builder.Services.AddScoped<Permissions>();


// ** Required to enable asp-authorize Taghelper **            
builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// else 
// {
//     // seed users in development mode - using service provider to get UserService from DI
//     using var scope = app.Services.CreateScope();
//     Seeder.Seed(
//         scope.ServiceProvider.GetService<IUserService>(),
//         scope.ServiceProvider.GetService<ICompanyService>(),
//         scope.ServiceProvider.GetService<IAnalystsService>(),
//         scope.ServiceProvider.GetService<IAssessmentService>()

//     );
// }
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ** configure cors to allow full cross origin access to any webapi end points **
//app.UseCors(c => c.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// ** turn on authentication/authorisation **
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// app.MapControllerRoute(
//     name: "company-details",
//     pattern: "Company/CompanyDetails/{cr_no}", // Define your custom URL format here
//     defaults: new { controller = "Company", action = "CompanyDetails" }
// );

// app.MapControllerRoute(
//     name: "FinanceRequest",
//     pattern: "{controller=FinanceRequest}/{action=Index}/{request_No?}"); // Define your custom URL format here



app.Run();
