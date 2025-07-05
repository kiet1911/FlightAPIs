
using FlightAPIs.Internal.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FlightAPIs.Helper;

var builder = WebApplication.CreateBuilder(args);
// Add services to builder
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//add controller with anti cycles
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
//add openapi to support swagger
//builder.Services.AddOpenApi(
//    o =>
//    {
//        o.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
//        o.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
        
//    }
//    );
//add endpoint
//builder.Services.AddEndpointsApiExplorer();
//add swagger gen options
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "FlightAPI", Version = "v1" });
    //o.SwaggerDoc("v1", new OpenApiInfo { Title = "FlightAPI", Version = "v1" });
    //add scheme to security defition
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    //
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            },
            In = ParameterLocation.Header,
        },
       new List<string>()
    }
    });

});
//add authorization and authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!)),
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateIssuer = true,
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateAudience = true,
        };
        //event 
        options.Events = new JwtBearerEvents
        {
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(new
                {
                    status = 403,
                    error = "You don't have permission"
                }.ToJson());

            }
            
        };
    }
);
builder.Services.AddAuthorization();
builder.Services.AddScoped<TokenProvider>();
//---------------------------------//
//create builder
var app = builder.Build();
// Configure the HTTP request pipeline.
//check development in properties launchSettings.json
if (app.Environment.IsDevelopment())
{
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightAPI V1");
    });
    app.MapGet("/", async context =>
    {
        await Task.Run(() => context.Response.Redirect("./swagger/index.html"));
    }
    );
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//runn app 
app.Run();
//if use openapi
//internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
//{
//    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
//    {
//        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
//        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
//        {
//            var securityScheme = new OpenApiSecurityScheme
//            {
//                Type = SecuritySchemeType.Http,
//                In = ParameterLocation.Header,
//                Scheme = "Bearer",
//                BearerFormat = "JWT",
//                Description = "JWT Authorization header using the Bearer scheme.",
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            };
//            document.Components ??= new OpenApiComponents();
//            document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
//            document.Components.SecuritySchemes["Bearer"] = securityScheme;

//            document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
//            document.SecurityRequirements.Add(new OpenApiSecurityRequirement
//            {
//                { securityScheme, Array.Empty<string>() }
//            });
//        }
//    }
//}