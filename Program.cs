using DemoWUITGallery.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<PhotoGalleryContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("WUITPhotoGallery");
    options.UseSqlite(connectionString);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PhotoGalleries}/{action=Index}/{id?}");

app.Run();
