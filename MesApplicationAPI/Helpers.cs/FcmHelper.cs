// FCM 푸시 전송을 위한 static 유틸 클래스
using MesApplicationAPI.Dto;
using MesApplicationAPI.Helpers.cs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static Google.Apis.Requests.BatchRequest;

public static class FcmHelper
{

    private static string _projectId = string.Empty;

    public static void Init(IConfiguration config)
    {
        _projectId = config["Firebase:ProjectId"]!;
    }

    // 단일 메시지를 FCM HTTP v1 API로 전송
    public static async Task<bool> SendAsync(AlarmDto item)
    {
        try
        {
            var client = new HttpClient(); // HTTP 요청 클라이언트 생성
            var accessToken = await GoogleAuthHelper.GetAccessTokenAsync(); // 엑세스 토큰 발급
            string url = $"https://fcm.googleapis.com/v1/projects/{_projectId}/messages:send";
            HttpResponseMessage response = null;

            Console.WriteLine($"전송 토큰: {item.FcmToken}");

            // 전송할 메시지 , 전자서명할때 사용할 키값 전달
            // 전송할 메세지는 무조건 string타입 고정
            var message = new
            {
                message = new
                {
                    //보낼 대상은 MESSAGE안에서 처리함에 누락 금지
                    token = item.FcmToken,

                    // 앱에서 표시될 알람 타이틀과 본문
                    notification = new
                    {
                        title = item.Title
                        ,
                        body = $"{item.Content1}\n{item.Content2}\n{item.Content3}\n{item.UserNm}"
                    },
                    data = new
                    {
                        // 클릭 구분자 플러터가 반응하기 위한 필수값
                        // Content도 데이터로 보내줘야 App 로컬 DB에 저장이 가능하다.
                        click_action = "FLUTTER_NOTIFICATION_CLICK",
                        title = item.Title,
                        content1 = item.Content1,
                        content2 = item.Content2,
                        content3 = item.Content3,
                        content4 = item.Content4,
                        content5 = item.Content5,
                        appAlarmId = item.AppAlarmId.ToString(),
                        userId = item.UserId,
                        userNm = item.UserNm,
                        signCd = item.SignCd,
                        signId = item.SignId,                          
                        key1 = item.Key1,
                        key2 = item.Key2,
                        key3 = item.Key3,
                        key4 = item.Key4,
                        key5 = item.Key5
                    }
                }
            };


            // HTTP 요청을 세팅후,
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken); // 인증 토큰 추가
            request.Content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json"); // JSON 직렬화 후 본문으로 설정

            //전송한다.
            response = await client.SendAsync(request);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"🔍 응답 내용: {responseBody}");

            //200~299면 true 아니면 false
            if (!response.IsSuccessStatusCode) // 200~
                throw new Exception(response.IsSuccessStatusCode.ToString());

            return response.IsSuccessStatusCode; // 성공 여부 반환 (true = 200 OK)

        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine($"HTTP 요청 예외: {httpEx.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"일반 예외: {ex.Message}");
            return false;
        }
    }
}
