using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Observability;
using UserManagement.API.DbContexts;
using UserManagement.API.Services;

var builder = WebApplication.CreateBuilder(args);

//setup logging, tracing and metrics
var logger = LoggerSetup.Init(builder);
TracingSetup.Init(builder, logger);
MetricsSetup.Init(builder, logger); 

// Add services to the container.
builder.Services.AddDbContext<UserManagementContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMyOrigin",
    builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddAutoMapper(typeof(Program));
// Add services to the container
builder.Services.AddScoped<IUserManagementRepository, UserManagementRepository>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.Audience = "usermanagementapi";
        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowMyOrigin");
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
