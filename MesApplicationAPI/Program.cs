using MesApplicationAPI.Dao;
using MesApplicationAPI.Helpers.cs;
using MesApplicationAPI.Interface;
using MesApplicationAPI.Services;
using MesApplicationAPI.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5216); // 외부 모든 IP에서 수신 가능
});

AppSettings.Init(builder.Configuration); // SecretKey 전역 초기화

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(AppSettings.SecretKey))
    };
});


#region - CONFIG -

DbHelper.Init(builder.Configuration); 
TokenHelper.Init(builder.Configuration);
GoogleAuthHelper.Init(builder.Configuration);
FcmHelper.Init(builder.Configuration);

#endregion

#region - SERVICE -

//SERVICE
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SignService>();
builder.Services.AddScoped<IAlarmService, AlarmService>();
builder.Services.AddScoped<ExcuteSpService>();
builder.Services.AddScoped<AlarmService>();
builder.Services.AddScoped<ReceiptService>();
builder.Services.AddScoped<ReceiptPackService>();

builder.Services.AddScoped<CommonService>();
#endregion


#region - DAO -
//DAO
builder.Services.AddScoped<UserDao>();
builder.Services.AddScoped<SignDao>();
builder.Services.AddScoped<AlarmDao>();
builder.Services.AddScoped<EsSpMappingDao>();
builder.Services.AddScoped<SpExecLogDao>();
builder.Services.AddScoped<ReceiptDao>();
builder.Services.AddScoped<ReceiptPackDao>();
builder.Services.AddScoped<CommonLocationDao>();
#endregion


#region - SCHEDULER -

//프로그램이 실행될때, 스케쥴러가 바로 사용될수있도록 
builder.Services.AddHostedService<AlarmScheduler>();

builder.Services.AddHostedService<SpRetryScheduler>();


#endregion

var app = builder.Build();
app.MapControllers();
app.Run();