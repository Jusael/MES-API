using Google.Apis.Auth.OAuth2;
using Google.Apis.Util;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MesApplicationAPI.Helpers.cs
{

    /// <summary>
    /// FCM 서버에 데이터를 쏘기 위해서는 접근 토큰을 받는 스태틱 클래스
    /// </summary>
    public static class GoogleAuthHelper
    {
        private static string _jsonKeyPath = string.Empty;

        public static void Init(IConfiguration config)
        {
            _jsonKeyPath = config["Firebase:JsonKeyPath"]!;
        }

        private static readonly string[] _scopes = new[] { "https://www.googleapis.com/auth/firebase.messaging" };

        public static async Task<string> GetAccessTokenAsync()
        {
            GoogleCredential credential;

            using (var stream = new FileStream(_jsonKeyPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                                             .CreateScoped(_scopes);
            }

            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }
    }
}
