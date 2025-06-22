using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class SignDto : IBaseDto
    {
        // 다른 필드들 예: 사번, 서명코드 등...
        public string? UserId { get; set; }
        public string? SignCd { get; set; }
        public string? SignId { get; set; }

        public string? AppAlarmId { get; set; }

        public string? MesAlarmId { get; set; }


        // key1~key5는 상황에 따라 값이 없을 수 있으므로 string?으로 선언
        public string? Key1 { get; set; }
        public string? Key2 { get; set; }
        public string? Key3 { get; set; }
        public string? Key4 { get; set; }
        public string? Key5 { get; set; }
    }
}

