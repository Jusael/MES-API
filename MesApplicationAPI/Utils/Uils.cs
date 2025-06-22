using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace MesApplicationAPI.Utils
{
    public class Uils
    {
        /// <summary>
        /// dto를 Dictionary로 변환 함수
        /// userId -> USER_ID 로 변환해서 Dictionary에 add
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public static  Dictionary<string, object> DtoToDictionary(object dto)
        {
            var dict = new Dictionary<string, object>();
            var props = dto.GetType().GetProperties();

            foreach (var prop in props)
            {
                var key = $"@{ToSnakeCase(prop.Name)}"; // 자동 변환 적용!
                var value = prop.GetValue(dto) ?? DBNull.Value;
                dict[key] = value;
            }

            return dict;
        }

        private static string ToSnakeCase(string input)
        {
            return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToUpper();
        }

    }

    /// <summary>
    /// API에 전송하기 위해 토큰 발급
    /// </summary>
    public static class TokenHelper
    {
        private static string _secretKey = "";

        public static void Init(IConfiguration config)
        {
            _secretKey = config["JwtSettings:SecretKey"];
        }

        public static string GenerateToken(string userId, int expireDays)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expireDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }


}
