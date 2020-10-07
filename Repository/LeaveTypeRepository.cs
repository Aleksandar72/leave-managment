using LeaveManagment.Contract;
using LeaveManagment.Data;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public bool Create(LeaveTypes entity)
        {

            _db.LeaveTypes.Add(entity);
            return Save();
        }

        public bool Delete(LeaveTypes entity)
        {
            _db.LeaveTypes.Remove(entity);
            return Save();
        }

        public ICollection<LeaveTypes> FindAll()
        {
            return _db.LeaveTypes.ToList();
        }

        public LeaveTypes FindById(int Id)
        {
            return _db.LeaveTypes.Find(Id);
        }

        public bool isExist(int Id)
        {
            return _db.LeaveTypes.Any(q => q.Id == Id);
        }

        public bool Save()
        {
           return _db.SaveChanges() > 0;
        }

        public bool Update(LeaveTypes entity)
        {
            _db.LeaveTypes.Update(entity);
            return Save();
        }
    }
}
