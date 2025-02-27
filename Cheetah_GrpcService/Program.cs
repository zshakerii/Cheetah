using Cheetah_Business.Repository;
using Cheetah_DataAccess.Data;
using Cheetah_DataAccess.Repository;
using Cheetah_GrpcService.Middleware;
using Cheetah_GrpcService.Services;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var provider = builder.Configuration.GetValue("Provider", "Npgsql");

if (provider is "Npgsql")
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    builder.Services.AddDbContext<ApplicationDbContext>(
        b => b.UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration.GetConnectionString("Npgsql")
        , x => x.MigrationsAssembly("Cheetah_DataAccess_Npgsql")
        ),
        ServiceLifetime.Transient
        );
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(
        b => b.UseLazyLoadingProxies()
        .UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"),
        x => x.MigrationsAssembly("Cheetah_DataAccess_SqlServer")),
        ServiceLifetime.Transient
        );
}

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped(typeof(ITableCRUD), typeof(TableCRUD));
builder.Services.AddScoped(typeof(IWorkItem), typeof(WorkItem));
builder.Services.AddScoped(typeof(IView), typeof(View));
builder.Services.AddScoped(typeof(ISync), typeof(Sync));
builder.Services.AddScoped(typeof(ICartable), typeof(Cartable));
builder.Services.AddScoped(typeof(ICopyClass), typeof(CopyClass));

//builder.Services.AddGrpc();

builder.Services.AddGrpc(options =>
{
    {
        options.Interceptors.Add<ServerLoggerInterceptor>();
        options.EnableDetailedErrors = true;
    }
});



builder.Services.AddTransient<ExceptionMiddleware>();
var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<RequestService>();
app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionMiddleware>();
app.Run();
