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
        private readonly ILeaveTypeRepository _repo;
        private readonly IMapper _mapper;

        public LeaveTypeController(ILeaveTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        // GET: LeaveTypeController
        public async Task<ActionResult> Index()
        {
            var leavetype = await _repo.FindAll();
            var model = _mapper.Map<List<LeaveTypes> , List<LeaveTypesVM>>(leavetype.ToList());
            return View(model);
        }

        // GET: LeaveTypeController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (! await _repo.isExist(id))
            {
                return NotFound();
            }
            var leavetype = await _repo.FindById(id);
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
                if (!await _repo.Create(leavetypes))
                {
                    ModelState.AddModelError("", "Something went wrong");
                }
                
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
            if (!await _repo.isExist(id))
            {
                return NotFound();
            }
            var leavetype = await _repo.FindById(id);
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
                if (await _repo.Update(leavetypes))
                {
                    ModelState.AddModelError("", "Something went wrong");
                }
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
            if (!await _repo.isExist(id))
            {
                return NotFound();
            }
            var leavetype = await _repo.FindById(id);
            return View(_mapper.Map<LeaveTypesVM>(leavetype));
        }

        // POST: LeaveTypeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, LeaveTypesVM model)
        {
            try
            {
               
                var leavetype = await _repo.FindById(id);
                var isSuccess = await _repo.Delete(leavetype);
                if (!isSuccess)
                {
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
