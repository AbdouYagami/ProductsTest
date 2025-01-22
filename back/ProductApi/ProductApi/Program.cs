using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProductApi.Repositories;  
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// On autoriste les méthodes HTTP pour pas avoir d'erreurs ainsi que les headers 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyMethod()  
              .AllowAnyHeader();  
    });
});

// Ajouter les services nécessaires
// Notamment le "Bearer" pour l'ajout du token JWT
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Veuillez entrer votre token JWT",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eH$23!V@dKf7bJq#9tLmN*PzR4sT5xYA"))
    };
});

// on enregistre chaque repository (panier liste envie et produits) 
builder.Services.AddSingleton<JsonCartRepository>();   
builder.Services.AddSingleton<JsonWishlistRepository>();   
builder.Services.AddSingleton<JsonProductRepository>();   

var app = builder.Build();

// j'ai fais le test via le SWAGGER donc ça sert à afficher le bouton pour mettre le token
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Permet de spécifier l'URL de l'API Swagger avec l'option OAuth
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
    });
}

 
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();  
app.UseAuthorization();
app.MapControllers();

app.Run();