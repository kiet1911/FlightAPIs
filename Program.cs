
using FlightAPIs.Internal.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FlightAPIs.Helper;
using System.Security.Claims;

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
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(
//        policy =>
//        {

//            // Ví d?: "http://127.0.0.1:5500", "http://localhost:5500"
//            policy.AllowAnyOrigin()
//                  .AllowAnyHeader() 
//                  .AllowAnyMethod(); 

//        });
//});
//add logging 
builder.Services.AddLogging();

builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "FlightAPI", Version = "v1" });
    //o.SwaggerDoc("v1", new OpenApiInfo { Title = "FlightAPI", Version = "v1" });
    //add scheme to security defition
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
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
                Type = ReferenceType.SecurityScheme,
                 Id = "Bearer"
            }
        },
        new string[] {}
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
builder.Services.AddAuthorization(option => 
{
    option.AddPolicy("roleSecurity", policy => { policy.RequireRole("Admin","Employee"); });
});

builder.Services.AddScoped<TokenProvider>();
//---------------------------------//
//create builder
var app = builder.Build();
// Configure the HTTP request pipeline.
//check development in properties launchSettings.json
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FlightAPI V1");
    });
    //app.MapGet("/", async context =>
    //{
    //    await Task.Run(() => context.Response.Redirect("./swagger/index.html"));
    //}
    //);
    
//}
app.UseRouting();
//app.UseCors();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//runn app 
app.Run();
