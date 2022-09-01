using InfoTrack.GoogleRank;
using InfoTrack.GoogleRank.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<GoogleRankService>(client =>
{
    client.BaseAddress = new Uri("https://google.com");
});
builder.Services.AddOptions<GoogleSettings>()
    .Configure<IConfiguration>((settings, configuration) => configuration.Bind(settings));

builder.Services.AddSingleton<IHtmlScraper, DirtyHtmlScraper>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program { }
