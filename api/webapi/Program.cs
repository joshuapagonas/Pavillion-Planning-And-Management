using Microsoft.EntityFrameworkCore;
using PPM;
using PPM.Interfaces;
using PPM.Repositories;
using PPM.Services;
using BCrypt.Net;

string password_hash = BCrypt.Net.BCrypt.HashPassword("my password");
if (BCrypt.Net.BCrypt.Verify("my password", password_hash)) {
    Console.WriteLine("\n\nPassword is correct.");
    Console.WriteLine(password_hash);
    Console.WriteLine("\n\n");
}
else {
    Console.WriteLine("\n\nNo Hash\n\n");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PPMDBContext>(options => {
    options.UseSqlServer(
       builder.Configuration.GetConnectionString("PPMDatabase"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<IParkRepository, ParkRepository>();
builder.Services.AddScoped<ParkService>();

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
