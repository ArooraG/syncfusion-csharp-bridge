using Syncfusion.Licensing;

// 1. License Key Environment Variable se lena
var licenseKey = Environment.GetEnvironmentVariable("Ngo9BigBOggjHTQxAR8/V1JFaF1cX2hIf0x1WmFZfVtgdVRMY11bRHFPMyBoS35Rc0RjW3hfcXdURmdYVE1xVEFc");
if (!string.IsNullOrEmpty(licenseKey))
{
    SyncfusionLicenseProvider.RegisterLicense(licenseKey);
}

// 2. Default ASP.NET startup code
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Neeche ki do lines mein AddOpenApi tha, jo error de raha tha, isliye hata diya
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Default environment check yahan se shuru hota hai
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); // Yeh line ConvertController ko enable karti hai

app.Run();