using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultipleDataGenerator.Models;
using MultipleDataGenerator.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<DataGeneratorDatabaseSettings>(
    builder.Configuration.GetSection(nameof(DataGeneratorDatabaseSettings)));

builder.Services.AddSingleton<DataGeneratorService>();

/*builder.Services.AddSingleton<IDataGeneratorDatabaseSettings>(provider =>
    provider.GetRequiredService<IOptions<DataGeneratorDatabaseSettings>>().Value);
*/
//builder.Services.AddScoped<DataGeneratorService>();
/*builder.Services.AddDbContext<MultipleDataDbContext>(opts => {
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:MultipleDataConnection"]);
});*/

var app = builder.Build();

app.UseStaticFiles();
app.MapDefaultControllerRoute();

app.Run();
