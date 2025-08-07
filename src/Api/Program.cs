var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();  // ตัวช่วยเปิด metadata สำหรับ Swagger
builder.Services.AddSwaggerGen();             // เปิดใช้งาน Swagger

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();       // เปิด endpoint /swagger/v1/swagger.json
    app.UseSwaggerUI();     // เปิด UI /swagger/index.html
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
