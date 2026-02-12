using GarageLog.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageLog.Services
{
    public class ModService
    {
        private readonly GarageLogContext _db;

        public ModService(GarageLogContext db)
        {
            _db = db;
        }

        public async Task<List<ModItem>> GetAllModsAsync()
        {
            return await _db.ModItems
                .Include(m => m.Car)
                .ToListAsync();
        }

        public async Task<ModItem> AddModAsync(ModItem mod)
        {
            _db.ModItems.Add(mod);
            await _db.SaveChangesAsync();
            return mod;
        }

        public async Task<ModItem?> UpdateModAsync(int id, ModItem updatedMod)
        {
            var mod = await _db.ModItems.FindAsync(id);
            if (mod == null) return null;

            mod.Name = updatedMod.Name;
            mod.Category = updatedMod.Category;
            mod.Cost = updatedMod.Cost;
            mod.Status = updatedMod.Status;
            mod.Notes = updatedMod.Notes;
            mod.Link = updatedMod.Link;
            mod.CompletedDate = updatedMod.CompletedDate;

            await _db.SaveChangesAsync();
            return mod;
        }

        public async Task<bool> DeleteModAsync(int id)
        {
            var mod = await _db.ModItems.FindAsync(id);
            if (mod == null) return false;

            _db.ModItems.Remove(mod);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ModItem?> GetModByIdAsync(int id)
        {
            return await _db.ModItems
                .Include(m => m.Car)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

    }
}
