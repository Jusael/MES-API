using MesApplicationAPI.Dto;

namespace MesApplicationAPI.Interface
{
    public interface IAlarmService
    {
        Task<List<AlarmDto>> Select();

        Task<int?> UpdateAsync(string kind, IBaseDto dto); 
    }
}
