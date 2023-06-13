using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Application.Container;
using Microsoft.AspNetCore.Authentication.Cookies;
using Web.Mvc.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.EventsType = typeof(CustomCookieAuthenticationEvents);
        options.LoginPath = @"/User/Login";
        options.LogoutPath = @"/User/Logout";
    });

builder.Services.AddScoped<CustomCookieAuthenticationEvents>();

/*
builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("Admin",
    //    policy => policy.AddRequirements(new BeAdminRequirement("Admin"))
    //);
    //options.AddPolicy("User",
    //    policy => policy.AddRequirements(new BeUserRequirement("Admin"))
    //);
    options.AddPolicy("IsAdmin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));

    options.AddPolicy("IsUser", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == ClaimTypes.Role) &&
                c.Value != "Admin")));
});*/

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new ApplicationConfigureServiceContainer());
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