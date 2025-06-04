using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI.Context;
using WebAPI.Services.IServices;
using WebAPI.Services.Services;

var builder = WebApplication.CreateBuilder(args);

//  Se obtiene la clave secreta para JWT desde appsettings.json o se usa una por defecto
var key = builder.Configuration["Jwt:Key"] ?? "clave_super_secreta_1234567890_ABCDEF";

//  Configuración de autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opt =>
{
    opt.RequireHttpsMetadata = false; // Permitir HTTP sin certificado (solo en desarrollo)
    opt.SaveToken = true;             // Guardar el token en la sesión HTTP
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,       // No se valida el emisor (útil para entornos internos)
        ValidateAudience = false,     // No se valida el público
        ValidateLifetime = true,      // Se valida que el token no haya expirado
        ValidateIssuerSigningKey = true, // Se valida la firma del token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // Clave usada para firmar
    };
});

//  Se habilita autorización (para usar [Authorize] en los controladores)
builder.Services.AddAuthorization();

//  Configuración de Swagger para probar la API con tokens JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi29AV", Version = "v1" });

    // Definición del esquema de seguridad (Bearer)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce tu token JWT en este formato: **Bearer {token}**"
    });

    // Requerimiento de seguridad para usar el esquema definido
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//  Servicios y dependencias
builder.Services.AddControllers();             // Controladores de la API
builder.Services.AddEndpointsApiExplorer();    // Habilita los endpoints
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Base de datos

//  Servicios de usuarios y roles
builder.Services.AddTransient<IUserServices, UserServices>();
builder.Services.AddTransient<IRolServices, RolServices>();

var app = builder.Build();

//  Middleware del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Interfaz gráfica de Swagger
}

app.UseHttpsRedirection();

app.UseAuthentication(); // ¡IMPORTANTE! Siempre antes de Authorization
app.UseAuthorization();

app.MapControllers(); // Mapeo de endpoints de los controladores

app.Run(); // Ejecuta la aplicación
