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
        public async Task<ActionResult> Index()
        {
            var leaverequest = _mapper.Map<List<LeaveRequestVM>>(await _leaveRequestRepo.FindAll());
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
        public async Task<ActionResult> Details(int id)
        {
            var model = _mapper.Map<LeaveRequestVM>(await _leaveRequestRepo.FindById(id));
            return View(model);
        }
        public async Task<ActionResult> MyLeave()
        {
            var leaveAlloc = _mapper.Map<List<LeaveAllocationVM>>(await _leaveAllocationRepo.GetLeaveAllocationsByEmployee(_userManager.GetUserId(User)));
            var leaveRequest = _mapper.Map<List<LeaveRequestVM>>(await _leaveRequestRepo.GetLeaveRequestByEmployeeId(_userManager.GetUserId(User)));
            var model = new EmployeeLeaveRequestViewVM
            {
                leaveAllocations = leaveAlloc,
                leaveRequests = leaveRequest
            };
            return View(model);

        }
        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {

                var leaveReq = await _leaveRequestRepo.FindById(id);
                var leaveAlloc = await _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(leaveReq.RequestingEmployeeId, leaveReq.LeaveTypeId);
                if (!(leaveAlloc.NumberOfDays > ((int)(leaveReq.EndDate.Date - leaveReq.StartDate.Date).TotalDays)))
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
                leaveAlloc.NumberOfDays -= ((int)(leaveReq.EndDate.Date - leaveReq.StartDate.Date).TotalDays);

                leaveReq.Approved = true;
                leaveReq.ApprovedById = _userManager.GetUserId(User);
                leaveReq.DateActioned = DateTime.Now;
                var isSuccess = await _leaveRequestRepo.Update(leaveReq);
                if (!isSuccess)
                {
                    return RedirectToAction(nameof(Index));
                }
                await _leaveAllocationRepo.Update(leaveAlloc);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                return RedirectToAction(nameof(Index));
            }


        }
        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var leaveReq = await _leaveRequestRepo.FindById(id);
                leaveReq.Approved = false;
                leaveReq.ApprovedById = _userManager.GetUserId(User);
                leaveReq.DateActioned = DateTime.Now;
                var isSuccess = await _leaveRequestRepo.Update(leaveReq);
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
        public async Task<ActionResult> Create()
        {
            var leavetype = await _leaveTypeRepo.FindAll();
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
        public async Task<ActionResult> Create(CreateLeaveRequestVM model)
        {
            var startDate = Convert.ToDateTime(model.StartDate);
            var endDate = Convert.ToDateTime(model.EndDate);

            try
            {
                var leavetype = await _leaveTypeRepo.FindAll();
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
                var allocation = await _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId);
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
                if (!await _leaveRequestRepo.Create(leaveRequestModel))
                {
                    ModelState.AddModelError("", "Error");
                    return View(model);
                }

                return RedirectToAction(nameof(Index), "Home");
            }
            catch
            {
                return View();
            }
        }
    }
}
