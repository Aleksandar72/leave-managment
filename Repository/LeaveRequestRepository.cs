using LeaveManagment.Contract;
using LeaveManagment.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LeaveManagment.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(LeaveRequest entity)
        {
            await _db.LeaveRequests.AddAsync(entity);
            return await Save();
        }
        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return await Save();
        }
        public async Task<ICollection<LeaveRequest>> FindAll()
        {
            return await _db.LeaveRequests.Include(q => q.RequestingEmployee).Include(q => q.LeaveTypes).Include(q => q.ApprovedBy).ToListAsync();
        }

        public async Task<LeaveRequest> FindById(int Id)
        {
            return await _db.LeaveRequests.Include(q => q.RequestingEmployee).Include(q => q.LeaveTypes).Include(q => q.ApprovedBy).FirstOrDefaultAsync(q => q.Id == Id);
        }

        public async Task<ICollection<LeaveRequest>> GetLeaveRequestByEmployeeId(string employeeId)
        {
            var list = await FindAll();
            return list.Where(q => q.RequestingEmployeeId == employeeId).ToList();
        }

        public async Task<bool> isExist(int Id)
        {
            return await _db.LeaveRequests.AnyAsync(q => q.Id == Id);
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}
