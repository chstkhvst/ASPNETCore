using ASPNETCore.Application.Services;
using ASPNETCore.Domain.Entities;
using ASPNETCore.Domain.Interfaces;
using ASPNETCore.Infrastructure.Data;
using ASPNETCore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<IREObjectRepository, REObjectRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<CatalogService>();

// Регистрация репозитория объектов.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]))
        };

        //// Обработка события 401
        //options.Events = new JwtBearerEvents
        //{
        //    OnChallenge = async context =>
        //    {
        //        context.HandleResponse();
        //        context.Response.StatusCode = 401;
        //        context.Response.ContentType = "application/json";
        //        await context.Response.WriteAsync("{\"error\":\"Unauthorized access\"}");
        //    }
        //};
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin"));
    options.AddPolicy("RequireRoleUser", policy => policy.RequireRole("User"));
});

builder.Services.AddScoped<REObjectServices>();
builder.Services.AddScoped<ContractServices>();
builder.Services.AddScoped<ReservationServices>();
// Регистрация сервиса объектов.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:3000") // Указываем фронт
//                  .AllowAnyHeader()
//                  .AllowAnyMethod();
//        });
//});

var app = builder.Build();

app.UseAuthorization();

// Настройка middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Использование страницы разработки.

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management API V1");
        c.RoutePrefix = "swagger";
        // Swagger будет доступен на главной странице.
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowAll");
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await DbInitializer.Initialize(context, userManager, roleManager);
}

app.Run();
