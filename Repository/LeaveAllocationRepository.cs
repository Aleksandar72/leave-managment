using LeaveManagment.Contract;
using LeaveManagment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CheckAllocation(int leavetypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            var list = await FindAll();
            return list.Where(q => q.LeaveTypeId == leavetypeId && q.EmployeeId == employeeId && q.Period == period).Any();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            await _db.LeaveAllocations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            return await _db.LeaveAllocations.Include(q => q.LeaveType).Include(q => q.LeaveType).ToListAsync();
        }

        public async Task<LeaveAllocation> FindById(int Id)
        {
            return await _db.LeaveAllocations.Include(q => q.LeaveType).Include(q => q.Employee).FirstOrDefaultAsync(q => q.Id == Id);
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string employeeId)
        {
            var period = DateTime.Now.Year;
            var list = await FindAll();
            return list.Where(q => q.EmployeeId == employeeId && q.Period == period).ToList();
        }

        public async Task<LeaveAllocation> GetLeaveAllocationsByEmployeeAndType(string employeeId, int leavetypeId)
        {
            var period = DateTime.Now.Year;
            var item = await FindAll();
            return item.FirstOrDefault(q => q.EmployeeId == employeeId && q.Period == period && q.LeaveTypeId == leavetypeId);
        }

        public async Task<bool> isExist(int Id)
        {
            return await _db.LeaveAllocations.AnyAsync(q => q.Id == Id);
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
