using CloudinaryDotNet;
using Movie_Ticket_Booking.Models;
using Movie_Ticket_Booking.Service;


var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
/*builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));*/
builder.Services.AddSingleton<VoucherService>();
builder.Services.AddSingleton<GenreService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<NewsService>();
builder.Services.AddSingleton<TheatreService>();
builder.Services.AddSingleton<MovieService>();
builder.Services.AddSingleton<CloudinaryService>();


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
