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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagment.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly UserManager<LeaveManagment.Data.Employee> _userManager;
        private readonly IMapper _mapper;
        public LeaveRequestController(ILeaveTypeRepository leaveTyperepo,
                               ILeaveRequestRepository leaveRequestRepo,
                               ILeaveAllocationRepository leaveAllocationRepo,
                               IMapper mapper,
                               UserManager<Employee> userManager)
        {
            _leaveTypeRepo = leaveTyperepo;
            _leaveRequestRepo = leaveRequestRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
            _userManager = userManager;
            _mapper = mapper;
        }
        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public ActionResult Index()
        {
            var leaverequest = _mapper.Map<List<LeaveRequestVM>>(_leaveRequestRepo.FindAll());
            var model = new AdminLeaveRequestsViewVM
            {
                TotalRequest = leaverequest.Count,
                ApprowedRequest = leaverequest.Where(q => q.Approved == true).Count(),
                PendingRequest = leaverequest.Where(q => q.Approved == null).Count(),
                RejectedRequest = leaverequest.Where(q => q.Approved == false).Count(),
                leaveRequests = leaverequest
            };
            return View(model);
        }

        // GET: LeaveRequestController/Details/5
        public ActionResult Details(int id)
        {
            var model = _mapper.Map<LeaveRequestVM>(_leaveRequestRepo.FindById(id));
            return View(model);
        }
        public ActionResult MyLeave()
        {
            var leaveAlloc = _mapper.Map<List<LeaveAllocationVM>>(_leaveAllocationRepo.GetLeaveAllocationsByEmployee(_userManager.GetUserId(User)));
            var leaveRequest = _mapper.Map<List<LeaveRequestVM>>(_leaveRequestRepo.GetLeaveRequestByEmployeeId(_userManager.GetUserId(User)));
            var model = new EmployeeLeaveRequestViewVM
            {
                leaveAllocations = leaveAlloc,
                leaveRequests = leaveRequest
            };
            return View(model);

        }
        public ActionResult ApproveRequest(int id)
        {
            try
            {
               // var model = _mapper.Map<LeaveRequestVM>(_leaveRequestRepo.FindById(id));
               
                var leaveReq = _leaveRequestRepo.FindById(id);
                var leaveAlloc = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(leaveReq.RequestingEmployeeId,leaveReq.LeaveTypeId);
                if (!(leaveAlloc.NumberOfDays > ((int)(leaveReq.EndDate.Date - leaveReq.StartDate.Date).TotalDays)))
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
                leaveAlloc.NumberOfDays -= ((int)(leaveReq.EndDate.Date - leaveReq.StartDate.Date).TotalDays);

                leaveReq.Approved = true;
                leaveReq.ApprovedById = _userManager.GetUserId(User);
                leaveReq.DateActioned = DateTime.Now;
                var isSuccess = _leaveRequestRepo.Update(leaveReq);
                if (!isSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                _leaveAllocationRepo.Update(leaveAlloc);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(Index));
            }
            
           
        }
        public ActionResult RejectRequest(int id)
        {
            try
            {
                var leaveReq = _leaveRequestRepo.FindById(id);
                leaveReq.Approved = false;
                leaveReq.ApprovedById = _userManager.GetUserId(User);
                leaveReq.DateActioned = DateTime.Now;
                var isSuccess = _leaveRequestRepo.Update(leaveReq);
                if (!isSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(Index));
            }
        }

        // GET: LeaveRequestController/Create
        public ActionResult Create()
        {
            var leavetype = _leaveTypeRepo.FindAll();
            var listitem = leavetype.Select(q => new SelectListItem { Text = q.Name, Value = q.Id.ToString() });
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = listitem
            };
            return View(model);
        }

        // POST: LeaveRequestController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            var startDate = Convert.ToDateTime(model.StartDate);      
            var endDate = Convert.ToDateTime(model.EndDate);      
            
            try
            {
                var leavetype = _leaveTypeRepo.FindAll();
                var listitem = leavetype.Select(q => new SelectListItem { Text = q.Name, Value = q.Id.ToString() });
                var createmodel = new CreateLeaveRequestVM
                {
                    LeaveTypes = listitem
                };
                model.LeaveTypes = createmodel.LeaveTypes;
                if (!ModelState.IsValid || !(DateTime.Compare(startDate, endDate) < 0))
                {
                    return View(model);

                }

                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId);
                int daysRequested = (int)(endDate.Date - startDate.Date).TotalDays;
                if (daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "To much days requested");
                    return View(model);
                }
                var leaveRequest = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    DateRequested = DateTime.Now,
                    Approved = null,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId
                };
                var leaveRequestModel = _mapper.Map<LeaveRequest>(leaveRequest);
                if (!_leaveRequestRepo.Create(leaveRequestModel))
                {
                    ModelState.AddModelError("", "Error");
                    return View(model);
                }

                return RedirectToAction(nameof(Index),"Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequestController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: LeaveRequestController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequestController/Delete/5
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
