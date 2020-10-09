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
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<LeaveManagment.Data.Employee> _userManager;
        private readonly IMapper _mapper;
        public LeaveRequestController(
                               IUnitOfWork unitOfWork,
                               IMapper mapper,
                               UserManager<Employee> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequestController
        public async Task<ActionResult> Index()
        {
            var leaverequest = _mapper.Map<List<LeaveRequestVM>>(await _unitOfWork.LeaveRequests.FindAll(includes : new List<string> { "RequestingEmployee", "LeaveTypes" }));
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
            var model = _mapper.Map<LeaveRequestVM>(await _unitOfWork.LeaveRequests.Find(q => q.Id == id, new List<string> { "ApprovedBy", "RequestingEmployee", "LeaveTypes" }));
            return View(model);
        }
        public async Task<ActionResult> MyLeave()
        {
            var leaveAlloc = _mapper.Map<List<LeaveAllocationVM>>(await _unitOfWork.LeaveAllocations.FindAll(q => q.EmployeeId == _userManager.GetUserId(User), includes: new List<string> { "LeaveTypes" }));
            var leaveRequest = _mapper.Map<List<LeaveRequestVM>>(await _unitOfWork.LeaveRequests.FindAll(q => q.RequestingEmployeeId == _userManager.GetUserId(User), includes: new List<string> { "ApprovedBy" }));
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
                var leaveReq = await _unitOfWork.LeaveRequests.Find(q => q.Id == id);
                var leaveAlloc = await _unitOfWork.LeaveAllocations.Find(q => q.EmployeeId == leaveReq.RequestingEmployeeId && q.LeaveTypeId == leaveReq.LeaveTypeId && q.Period == DateTime.Now.Year);
                if (!(leaveAlloc.NumberOfDays > ((int)(leaveReq.EndDate.Date - leaveReq.StartDate.Date).TotalDays)))
                {
                    return RedirectToAction(nameof(Details), new { id });
                }
                leaveAlloc.NumberOfDays -= ((int)(leaveReq.EndDate.Date - leaveReq.StartDate.Date).TotalDays);

                leaveReq.Approved = true;
                leaveReq.ApprovedById = _userManager.GetUserId(User);
                leaveReq.DateActioned = DateTime.Now;
                _unitOfWork.LeaveRequests.Update(leaveReq);
                _unitOfWork.LeaveAllocations.Update(leaveAlloc);
                await _unitOfWork.Save();
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
                var leaveReq = await _unitOfWork.LeaveRequests.Find(q => q.Id == id);
                leaveReq.Approved = false;
                leaveReq.ApprovedById = _userManager.GetUserId(User);
                leaveReq.DateActioned = DateTime.Now;
               
                _unitOfWork.LeaveRequests.Update(leaveReq);
                await _unitOfWork.Save();
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
            var leavetype = await _unitOfWork.LeaveTypes.FindAll();
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
                var leavetype = await _unitOfWork.LeaveTypes.FindAll();
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
                var allocation = await _unitOfWork.LeaveAllocations.Find(q => q.EmployeeId == employee.Id && q.LeaveTypeId == model.LeaveTypeId && q.Period == DateTime.Now.Year);
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
                await _unitOfWork.LeaveRequests.Create(leaveRequestModel);
                await _unitOfWork.Save();

                return RedirectToAction(nameof(Index), "Home");
            }
            catch
            {
                return View();
            }
        }
    }
}
