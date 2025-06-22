using MesApplicationAPI.Interface;

namespace MesApplicationAPI.Dto
{
    public class AlarmControlDto : IBaseDto
    {
        public string? UserId { get; set; }

        public string? AppAlarmId { get; set; }

        public string? AlarmStatus { get; set; }
    }
}
