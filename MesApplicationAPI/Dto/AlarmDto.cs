using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class AlarmDto : IBaseDto
    {
        public int AppAlarmId { get; set; }

        public int MesAlarmId { get; set; }
        public string FcmToken { get; set; }

        public string UserId { get; set; }

        public string UserNm { get; set; }

        public string Title { get; set; }
        public string Content1 { get; set; }
        public string Content2 { get; set; }
        public string Content3 { get; set; }

        public string Content4 { get; set; }

        public string Content5 { get; set; }
        public string SignCd { get; set; }

        public string SignId { get; set; }

        public string Key1 { get; set; }
        public string Key2 { get; set; }
        public string Key3 { get; set; }
        public string Key4 { get; set; }
        public string Key5 { get; set; }

        public string CreateTime { get; set; }
    }
}
