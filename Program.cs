using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Common.Infrastructure;
using RealEstateAPI.Features.Properties.Application.Commands;
using RealEstateAPI.Features.Properties.Domain;
using RealEstateAPI.Features.Properties.Infrastructure;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // This will help with enums and better JSON formatting
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Real Estate API - Smart Apartment Data",
        Version = "v1",
        Description = "Property Management API using Vertical Slice Architecture, DDD, and CQRS patterns",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Keven Bardales",
            Email = "keven.bardales@gmail.com"
        }
    });

    // Add XML comments if you have them
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure Entity Framework with InMemory Database
// Sin llaves extras, todo en una línea
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




// Register Repository
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();

// Register MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreatePropertyCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// Registra los validators manualmente
builder.Services.AddScoped<IValidator<CreatePropertyCommand>, CreatePropertyCommandValidator>();
builder.Services.AddScoped<IValidator<UpdatePropertyCommand>, UpdatePropertyCommandValidator>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        // Will throw if cannot connect
        var canConnect = await context.Database.CanConnectAsync();
        Console.WriteLine($"Database connection ok: {canConnect}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Real Estate API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.DefaultModelsExpandDepth(-1); // Hide schemas section by default
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();


// Add a welcome endpoint
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.Run();

// Validation Pipeline Behavior
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                // Log validation errors
                var logger = LoggerFactory.Create(builder => builder.AddConsole())
                    .CreateLogger<ValidationBehavior<TRequest, TResponse>>();
                logger.LogWarning("Validation errors: {@Errors}", failures);
            }
        }

        return await next();
    }
}