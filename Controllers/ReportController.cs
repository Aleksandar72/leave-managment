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
        private readonly IUnitOfWorkSql _unitOfWorkSql;
        public ReportController(IUnitOfWorkSql unitOfWorkSql)
        {
            _unitOfWorkSql = unitOfWorkSql;
           
        }
        public async Task<ActionResult> EmployeeReport()
        {
            var model = await _unitOfWorkSql.EmployeeReport.ExecuteSql(SqlQuerys.EmployeeStoredProcedure);
           
            return View(model);
        }
        public async Task<ActionResult> RequestReport(Data.ProcedureParams.ResponseStatisticParams responseStatisticParams)
        {
            
            if (responseStatisticParams.StartDate == null || responseStatisticParams.EndDate == null)
            {
                responseStatisticParams.StartDate = new DateTime(2020,01,01);
                responseStatisticParams.EndDate = new DateTime(2020, 05, 01);
            }
            var model = await _unitOfWorkSql.RequestReport.ExecuteSql(SqlQuerys.RequestStoredProcedure, responseStatisticParams);

            return View(model);
        }
        


    }
}
