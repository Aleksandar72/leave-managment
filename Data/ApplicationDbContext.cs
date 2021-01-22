using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LeaveManagment.Models;

namespace LeaveManagment.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<EmployeeReport>(eb =>
                {
                    eb.HasNoKey();
                }).Entity<RequestStatistic>(eb =>
                {
                    eb.HasNoKey();
                });
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveAllocation> LeaveAllocations { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveTypes>  LeaveTypes { get; set; }
        public DbSet<EmployeeReport> EmployeesReport { get; set; }
        public DbSet<RequestStatistic> RequestStatistics { get; set; }

    }
}
