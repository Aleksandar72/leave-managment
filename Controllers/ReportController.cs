using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagment.Contract;
using LeaveManagment.Data;
using LeaveManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace LeaveManagment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        // private readonly IMapper _mapper;
        private readonly IUnitOfWorkSql _unitOfWorkSql;

        public ReportController(IUnitOfWorkSql unitOfWorkSql)
        {
            _unitOfWorkSql = unitOfWorkSql;
           
        }
        // GET: LeaveTypeController
        public async Task<ActionResult> EmployeeReport()
        {
            var model = await _unitOfWorkSql.EmployeeReport.ExecuteSql("EXEC REPORT.EmployeeStatistic");
           
            return View(model);
        }

       
    }
}
