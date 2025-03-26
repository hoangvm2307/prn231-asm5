
using ChildTracking.Services;
using ChildTracking.SoapService.HoangVM.SoapServices;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IChildService, ChildService>();
builder.Services.AddScoped<GrowthRecordService>();
builder.Services.AddScoped<IChildSoapService, ChildSoapService>();
builder.Services.AddSoapCore();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSoapEndpoint<IChildSoapService>("/childSoapService.asmx", new SoapEncoderOptions());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
