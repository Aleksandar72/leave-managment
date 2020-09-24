using LeaveManagment.Contract;
using LeaveManagment.Data;
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
        public bool Create(LeaveType entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(LeaveType entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<LeaveType> FindAll()
        {
            throw new NotImplementedException();
        }

        public LeaveType FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool Update(LeaveType entity)
        {
            throw new NotImplementedException();
        }
    }
}
