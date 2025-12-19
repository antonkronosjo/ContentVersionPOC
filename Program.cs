using ContentVersionsPOC.Data;
using ContentVersionsPOC.Repositories;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ContentVersionsPOCContext>(options => options.UseSqlite("Data Source=app19.db"));
builder.Services.AddControllers();
builder.Services.AddTransient<IContentRepository, ContentRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContentVersionsPOCContext>();
    db.Database.EnsureCreated();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
//app.UseSpa(spa =>
//{
//    spa.Options.SourcePath = "ClientApp";

//    if (app.Environment.IsDevelopment())
//    {
//        spa.UseReactDevelopmentServer(npmScript: "start");
//    }
//});
app.MapFallbackToFile("index.html");
app.Run();

