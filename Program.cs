using tema2mvc.Services;
using tema2mvc.Models;
using LiteDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ILiteDatabase, LiteDatabaseObject>();
builder.Services.AddSingleton<IDataAccess<UserDAO>, DataAccess<UserDAO>>();
builder.Services.AddSingleton<IDataAccess<MenuItemDAO>, DataAccess<MenuItemDAO>>();
builder.Services.AddSingleton<IDataAccess<OrderDAO>, DataAccess<OrderDAO>>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
