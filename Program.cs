
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// Add services to builder
builder.Services.AddControllers().AddJsonOptions(options=>options.JsonSerializerOptions.ReferenceHandler= System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "apiFlight", Version = "v2" });
});

//builder.Services.AddControllers();
//---------------------------------//
//tao builder
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "api"); });
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//khoi chay
app.Run();

