using LogStorageService.Models;

namespace LogStorageService.Interfaces
{
    public interface ILogService
    {
        Task<IEnumerable<LogModel>> GetLogsAsync();      
        Task<LogModel> CreateLogAsync(LogModel logModel);
         
    }
}
