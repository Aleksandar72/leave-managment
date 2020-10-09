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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<LeaveManagment.Data.Employee> _userManager;
        private readonly IMapper _mapper;

        public LeaveAllocationController(
                               IUnitOfWork unitOfWork,
                               IMapper mapper,
                               UserManager<LeaveManagment.Data.Employee> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        // GET: LeaveAllocation
        public async Task<ActionResult> Index()
        {
            var leavetype = await _unitOfWork.LeaveTypes.FindAll();
            var mappedLeaveTypes = _mapper.Map<List<LeaveTypes>,List<LeaveTypesVM>>(leavetype.ToList());
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };
            return View(model);
        }
        public async Task<ActionResult> SetLeave(int id)
        {
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                if (await _unitOfWork.LeaveAllocations.isExist(q => q.LeaveTypeId == id && q.EmployeeId == emp.Id && q.Period == DateTime.Now.Year))
                    continue;
                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leavetype.DefaultDays,
                    Period = DateTime.Now.Year
                };
                var alloc = _mapper.Map<LeaveAllocation>(allocation);
                await _unitOfWork.LeaveAllocations.Create(alloc);
                await _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<ActionResult> ListEmployees()
        {
            var employees =await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);
        }

        // GET: LeaveAllocation/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var employee = _mapper.Map<EmployeeVM>(await _userManager.FindByIdAsync(id));
            var allocation = _mapper.Map<List<LeaveAllocationVM>>(await _unitOfWork.LeaveAllocations.FindAll(expression: q => q.EmployeeId == id && q.Period == DateTime.Now.Year, includes: new List<string> { "LeaveTypes" }));
            var model = new ViewAllocationVM
            {
                Employee = employee,
                LeaveAllocations = allocation
            };
            return View(model);
        }


        // GET: LeaveAllocation/Create
        public ActionResult Create()
        {
            return View();
        }
        // GET: LeaveAllocation/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var model = _mapper.Map<EditLeaveAllocationVM>(await _unitOfWork.LeaveAllocations.Find(expression: q => q.Id == id, includes: new List<string> { "Employee" }));
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM editLeaveAllocationVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(editLeaveAllocationVM);

                var defaultEntity = await _unitOfWork.LeaveAllocations.Find(q => q.Id == editLeaveAllocationVM.Id);
                defaultEntity.NumberOfDays = editLeaveAllocationVM.NumberOfDays;
               
                _unitOfWork.LeaveAllocations.Update(defaultEntity);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Details), new { id = editLeaveAllocationVM.EmployeeId });
            }
            catch
            {
                return View();
            }
        }
    }
}
