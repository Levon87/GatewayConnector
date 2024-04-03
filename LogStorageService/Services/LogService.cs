using LogStorageService.Data;
using LogStorageService.Interfaces;
using LogStorageService.Models;
using Microsoft.EntityFrameworkCore;

namespace LogStorageService.Services
{
    public class LogService(LogStorageDbContext context) : ILogService
    {
        private readonly LogStorageDbContext _context = context;
        public async Task<IEnumerable<LogModel>> GetLogsAsync()
        {
            try
            {
                return await _context.Logs.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при получении записей: " + ex.Message);
            }
        }

        public async Task<LogModel> CreateLogAsync(LogModel logModel)
        {
            try
            {
                _context.Logs.Add(logModel);
                await _context.SaveChangesAsync();

                return logModel;
            }

            catch (Exception ex)
            {
                throw new Exception("Ошибка при добавлении записи: " + ex.Message);
            }
        }
    }
}
