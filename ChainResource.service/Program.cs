using ChainResource.service.BL;
using ChainResource.service.BL.Interfaces;
using ChainResource.service.Configuration;
using ChainResource.service.DAL;
using ChainResource.service.DAL.Interfaces;
using ChainResource.service.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MemoryDal<ExchangeRateList>>();
builder.Services.AddSingleton<FileSystemDal<ExchangeRateList>>();
builder.Services.AddSingleton<WebServiceDal<ExchangeRateList>>();

builder.Services.AddSingleton<IStorageFactory<ExchangeRateList>, StorageFactory<ExchangeRateList>>();

builder.Services.AddSingleton<IChainResource<ExchangeRateList>, ChainResource<ExchangeRateList>>();

builder.Services.AddHttpClient();
//Configuyrations

builder.Services.Configure<FileSystemConfiguration>(builder.Configuration.GetSection(FileSystemConfiguration.ConfigName));
builder.Services.Configure<MemoryConfiguration>(builder.Configuration.GetSection(MemoryConfiguration.ConfigName));
builder.Services.Configure<WebServiceConfiguration>(builder.Configuration.GetSection(WebServiceConfiguration.ConfigName));
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
