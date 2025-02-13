using PustokApp.Data;
using PustokApp.Models;

namespace PustokApp.Service
{
    public class LayoutService
    {
        private readonly PustokDbContext _context;

        public LayoutService(PustokDbContext context)
        {
            _context = context;
        }

        public List<Genre> GetAllGenres()
        {
            return _context.Genres.ToList();
        } 
        
        public Dictionary<string, string> GetAllSettings()
        {
            return _context.Settings.ToDictionary(s=>s.Key, s=>s.Value);
        }
    }
}
