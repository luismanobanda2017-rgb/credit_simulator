using System.Text;
using AuthService.Data;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var jwt = builder.Configuration.GetSection("Jwt");
var jwtKey = jwt["Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");

builder.Services.AddDbContext<AuthDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHttpClient<LoginAuditClient>(client =>
{
	client.BaseAddress = new Uri(builder.Configuration["AuditService:BaseUrl"] ?? "http://localhost:5004/");
	client.Timeout = TimeSpan.FromSeconds(2);
});
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
		ValidateIssuer = true,
		ValidIssuer = jwt["Issuer"],
		ValidateAudience = true,
		ValidAudience = jwt["Audience"],
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
	await db.Database.ExecuteSqlRawAsync("""
		DO $$
		BEGIN
			IF EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema = 'public' AND table_name = 'users' AND column_name = 'status' AND data_type = 'boolean') THEN
				ALTER TABLE users ALTER COLUMN status DROP DEFAULT;
				ALTER TABLE users ALTER COLUMN status TYPE SMALLINT USING CASE WHEN status THEN 1 ELSE 0 END;
			END IF;
			ALTER TABLE users ALTER COLUMN status SET DEFAULT 1;
		END $$;
		""");
	await db.Database.ExecuteSqlRawAsync("ALTER TABLE users DROP COLUMN IF EXISTS failed_login_attempts;");
}
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
