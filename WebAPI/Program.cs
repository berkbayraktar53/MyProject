using Autofac;
using Core.Utilities.Security.Jwt;
using Microsoft.IdentityModel.Tokens;
using Core.Utilities.Security.Encryption;
using Business.DependencyResolvers.Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//////////////////////////////////////////////////////////////////////////////////////////////
var builder = WebApplication.CreateBuilder(args);
#region Autofac Integration
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(options => options.RegisterModule(new AutofacBusinessModule()));
#endregion
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    builder.WithOrigins("http://localhost:3000"));
});
#region Jwt Integration
var tokenOptions = new TokenOptions();
builder.Configuration.GetSection("TokenOptions").Bind(tokenOptions);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };
    }
);
#endregion
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(builder => builder.WithOrigins("http://localhost:3000").AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();