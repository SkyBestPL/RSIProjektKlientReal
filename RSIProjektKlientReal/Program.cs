using JavaEventService;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<EventServiceClient>(_ =>
{
    var binding = new BasicHttpBinding
    {
        MessageEncoding = WSMessageEncoding.Mtom,
        MaxReceivedMessageSize = 10 * 1024 * 1024, // np. 10 MB
        Security = new BasicHttpSecurity { Mode = BasicHttpSecurityMode.None } // lub Transport dla HTTPS
    };

    var endpoint = new EndpointAddress("http://localhost:8080/RSIProjekt_war_exploded/eventService");

    return new EventServiceClient(binding, endpoint);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
