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
        public bool Create(LeaveRequest entity)
        {
            _db.LeaveRequests.Add(entity);
            return Save();
        }
        public bool Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return Save();
        }
        public ICollection<LeaveRequest> FindAll()
        {
            return _db.LeaveRequests.Include(q => q.RequestingEmployee).Include(q => q.LeaveTypes).Include(q => q.ApprovedBy).ToList();
        }

        public LeaveRequest FindById(int Id)
        {
            return _db.LeaveRequests.Include(q => q.RequestingEmployee).Include(q => q.LeaveTypes).Include(q => q.ApprovedBy).FirstOrDefault(q => q.Id == Id);
        }

        public ICollection<LeaveRequest> GetLeaveRequestByEmployeeId(string employeeId)
        {
            return FindAll().Where(q => q.RequestingEmployeeId == employeeId).ToList();
        }

        public bool isExist(int Id)
        {
            return _db.LeaveRequests.Any(q => q.Id == Id);
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }

        public bool Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return Save();
        }
    }
}
