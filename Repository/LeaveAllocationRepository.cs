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
        public bool Create(Data.LeaveAllocation entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Data.LeaveAllocation entity)
        {
            throw new NotImplementedException();
        }

        public ICollection<Data.LeaveAllocation> FindAll()
        {
            throw new NotImplementedException();
        }

        public Data.LeaveAllocation FindById(int Id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public bool Update(Data.LeaveAllocation entity)
        {
            throw new NotImplementedException();
        }
    }
}
