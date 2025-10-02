using ProductionTrackerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddScoped<IProductionService, ExcelProductionService>();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
   
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Production Tracker API V1");
    c.RoutePrefix = string.Empty; 
});


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
