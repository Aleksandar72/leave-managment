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
    public class LeaveTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LeaveTypeController(/*ILeaveTypeRepository repo,*/ IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        // GET: LeaveTypeController
        public async Task<ActionResult> Index()
        {
            var leavetype = await _unitOfWork.LeaveTypes.FindAll();
            var model = _mapper.Map<List<LeaveTypes> , List<LeaveTypesVM>>(leavetype.ToList());
            return View(model);
        }

        // GET: LeaveTypeController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (!await _unitOfWork.LeaveTypes.isExist(q => q.Id == id))
            {
                return NotFound();
            }
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            var model = _mapper.Map<LeaveTypesVM>(leavetype);
            return View(model);
        }

        // GET: LeaveTypeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LeaveTypesVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leavetypes = _mapper.Map<LeaveTypes>(model);
                leavetypes.DateCreated = DateTime.Now;
                await _unitOfWork.LeaveTypes.Create(leavetypes);
                await _unitOfWork.Save();
                
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveTypeController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            await _unitOfWork.LeaveTypes.isExist(q => q.Id == id);
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            return View(_mapper.Map<LeaveTypesVM>(leavetype));
        }

        // POST: LeaveTypeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, LeaveTypesVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var leavetypes = _mapper.Map<LeaveTypes>(model);
                 _unitOfWork.LeaveTypes.Update(leavetypes);
                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveTypeController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (!await _unitOfWork.LeaveTypes.isExist(q => q.Id == id))
            {
                return NotFound();
            }
            var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
            return View(_mapper.Map<LeaveTypesVM>(leavetype));
        }

        // POST: LeaveTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, LeaveTypesVM model)
        {
            try
            {
                var leavetype = await _unitOfWork.LeaveTypes.Find(q => q.Id == id);
                _unitOfWork.LeaveTypes.Delete(leavetype);
                await _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
