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
        private readonly ILeaveTypeRepository _leaveTyperepo;
        private readonly ILeaveAllocationRepository _leaveAllocationrepo;
        private readonly UserManager<LeaveManagment.Data.Employee> _userManager;
        private readonly IMapper _mapper;

        public LeaveAllocationController(ILeaveTypeRepository leaveTyperepo,
                               ILeaveAllocationRepository leaveAllocationrepo,
                               IMapper mapper,
                               UserManager<LeaveManagment.Data.Employee> userManager)
        {
            _leaveTyperepo = leaveTyperepo;
            _leaveAllocationrepo = leaveAllocationrepo;
            _userManager = userManager;
            _mapper = mapper;
        }
        // GET: LeaveAllocation
        public async Task<ActionResult> Index()
        {
            var leavetype = await _leaveTyperepo.FindAll();
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
            var leavetype = await _leaveTyperepo.FindById(id);
            var employees = await _userManager.GetUsersInRoleAsync("Employee");
            foreach (var emp in employees)
            {
                if (await _leaveAllocationrepo.CheckAllocation(id, emp.Id))
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
                await _leaveAllocationrepo.Create(alloc);
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
            var allocation = _mapper.Map<List<LeaveAllocationVM>>(await _leaveAllocationrepo.GetLeaveAllocationsByEmployee(id));
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
            var model = _mapper.Map<EditLeaveAllocationVM>(await _leaveAllocationrepo.FindById(id));
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

                var defaultEntity = await _leaveAllocationrepo.FindById(editLeaveAllocationVM.Id);
                defaultEntity.NumberOfDays = editLeaveAllocationVM.NumberOfDays;
                var isSuccess = await _leaveAllocationrepo.Update(defaultEntity);
                if (!isSuccess)
                {
                    ModelState.AddModelError("","Something went wrong...");
                    return View(editLeaveAllocationVM);
                }
               
                return RedirectToAction(nameof(Details), new { id = editLeaveAllocationVM.EmployeeId });
            }
            catch
            {
                return View();
            }
        }
    }
}
