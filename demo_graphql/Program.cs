using demo_graphql.Extension;
using demo_graphql.Filters;
using RestSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(CustomAuthorization));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.ConfigureSwagger();
builder.Services.ConfigureAppSettings(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IRestClient>(_ => new RestClient()); //configuration["OrderServiceEndpoint"])); 
builder.Services.AddTransient<IRestRequest>(_ => new RestRequest());

builder.Services.AddHttpClient();
builder.Services.AddCors(options =>
      {
          options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
      });

var app = builder.Build();


app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");


app.Run();