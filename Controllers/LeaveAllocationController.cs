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
        public ActionResult Index()
        {
            var leavetype = _leaveTyperepo.FindAll().ToList();
            var mappedLeaveTypes = _mapper.Map<List<LeaveTypes>,List<LeaveTypesVM>>(leavetype);
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };
            return View(model);
        }
        public ActionResult SetLeave(int id)
        {
            var leavetype = _leaveTyperepo.FindById(id);
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            foreach (var emp in employees)
            {
                if (_leaveAllocationrepo.CheckAllocation(id, emp.Id))
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
                _leaveAllocationrepo.Create(alloc);
            }
            return RedirectToAction(nameof(Index));
        }
        public ActionResult ListEmployees()
        {
            var employees = _userManager.GetUsersInRoleAsync("Employee").Result;
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);
        }

        // GET: LeaveAllocation/Details/5
        public ActionResult Details(string id)
        {
            var employee = _mapper.Map<EmployeeVM>(_userManager.FindByIdAsync(id).Result);
            var allocation = _mapper.Map<List<LeaveAllocationVM>>(_leaveAllocationrepo.GetLeaveAllocationsByEmployee(id));
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

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Edit/5
        public ActionResult Edit(int id)
        {
            var model = _mapper.Map<EditLeaveAllocationVM>(_leaveAllocationrepo.FindById(id));
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditLeaveAllocationVM editLeaveAllocationVM)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(editLeaveAllocationVM);

                var defaultEntity = _leaveAllocationrepo.FindById(editLeaveAllocationVM.Id);
                defaultEntity.NumberOfDays = editLeaveAllocationVM.NumberOfDays;
                var isSuccess =  _leaveAllocationrepo.Update(defaultEntity);
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

        // GET: LeaveAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
