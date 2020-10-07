using LeaveManagment.Contract;
using LeaveManagment.Data;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagment.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(LeaveTypes entity)
        {

            await _db.LeaveTypes.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveTypes entity)
        {
             _db.LeaveTypes.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveTypes>> FindAll()
        {
            return await _db.LeaveTypes.ToListAsync();
        }

        public async Task<LeaveTypes> FindById(int Id)
        {
            return await _db.LeaveTypes.FindAsync(Id);
        }

        public async Task<bool> isExist(int Id)
        {
            return await _db.LeaveTypes.AnyAsync(q => q.Id == Id);
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
           return changes > 0;
        }

        public async Task<bool> Update(LeaveTypes entity)
        {
            _db.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
