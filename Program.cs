using asistenciaBack.Attendance.Presentation;
using asistenciaBack.Identity.Presentation;
using asistenciaBack.Membership.Presentation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddControllers();
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddMembershipModule();
builder.Services.AddAttendanceModule();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Asistencia API",
        Version = "v1",
        Description = "API de asistencia eclesiástica — Identity + Membership + Attendance"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Pega solo el token JWT del login (sin la palabra Bearer)."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

app.UseForwardedHeaders();

var enableSwagger = app.Environment.IsDevelopment()
    || string.Equals(app.Configuration["EnableSwagger"], "true", StringComparison.OrdinalIgnoreCase);

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Asistencia API v1");
        options.RoutePrefix = "swagger";
    });
}

// En Render el TLS lo termina el proxy; no forzar redirect HTTPS dentro del contenedor.
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.SeedIdentityAsync();

app.Run();
