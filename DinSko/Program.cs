using DinSko.Repositories;
using DinSko.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Dependency Injection for Repositories and Services

// Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

// ProductVariant
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<ProductVariantService>();

// Cart
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<CartService>();

// Order
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();

// User
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
