
using DinSko.Repositories;
using DinSko.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// add services to the container.
builder.Services.AddRazorPages();

// store temporary login attempt data
builder.Services.AddMemoryCache();

// configure cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";

        // log when an authenticated user is denied access
        options.Events.OnRedirectToAccessDenied = context =>
        {
            // get the logger from the application services
            ILogger<Program> logger =
                context.HttpContext.RequestServices
                    .GetRequiredService<ILogger<Program>>();

            // get the current user's ID
            string userId = context.HttpContext.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? "Unknown";

            // log the denied access attempt
            logger.LogWarning(
                "Access denied for User ID {UserId} when accessing {Path}.",
                userId,
                context.Request.Path);

            // redirect the user to the Access Denied page
            context.Response.Redirect(context.RedirectUri);

            return Task.CompletedTask;
        };
    });

// dependency injection for repositories and services

// product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

// product variant
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<ProductVariantService>();

// cart
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<CartService>();

// order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();

// user
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();