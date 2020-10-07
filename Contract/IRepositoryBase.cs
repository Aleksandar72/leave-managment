using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagment.Contract
{
     public interface IRepositoryBase<T> where T : class
    {
       Task<ICollection<T>> FindAll();
       Task<bool> isExist(int Id); 
       Task<T> FindById(int Id);
       Task<bool> Create(T entity);
       Task<bool> Update(T entity);
       Task<bool> Delete(T entity);
       Task<bool> Save();
    }
}
