using System;
using System.Linq;                                    // for Where/Select/ToList
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Timesheets_APP.Data;
using Timesheets_APP.Models;
using Timesheets_APP.ViewModels;

namespace Timesheets_APP.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public EmployeeController(ApplicationDbContext db)
            => _db = db;

        // ─── GET: /Employee/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
            => View(new EmployeeLoginViewModel());

        // ─── POST: /Employee/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(EmployeeLoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var emp = _db.Employees.Find(vm.EmpId);
            if (emp == null)
            {
                ModelState.AddModelError("", "Invalid employee ID");
                return View(vm);
            }

            // TODO: issue your auth cookie/session here

            return RedirectToAction(nameof(Index), new { empId = vm.EmpId });
        }

        // ─── GET: /Employee/Index?empId=#
        [HttpGet]
        public IActionResult Index(int empId)
        {
            var emp = _db.Employees.Find(empId);
            if (emp == null)
                return RedirectToAction("Select", "Account");

            // build a flat list of TimesheetViewModel
            var list = _db.Timesheets
                .Where(ts => ts.EmpId == empId)
                .Select(ts => new TimesheetViewModel
                {
                    TsId = ts.TsId,
                    EmpId = ts.EmpId,
                    EmpName = emp.EmpName,
                    Date = ts.TsDate,
                    Approved = ts.TsApproved == "Y"
                })
                .ToList();

            var vm = new EmployeeDashboardViewModel
            {
                EmpId = empId,
                EmpName = emp.EmpName,
                Timesheets = list
            };

            return View(vm);
        }

        // ─── GET: /Employee/AddTimesheet?empId=#
        [HttpGet]
        public IActionResult AddTimesheet(int empId)
        {
            var emp = _db.Employees.Find(empId);
            if (emp == null)
                return NotFound();

            // prepare your date‐picker options, defaults, etc.
            var vm = new AddTimesheetViewModel
            {
                EmpId = emp.EmpId,
                EmpName = emp.EmpName,
                TsDate = DateTime.Today,
                StartTime = "09:00",
                EndTime = "17:00",
                TsDateOptions = Enumerable
                    .Range(0, 30)
                    .Select(d => DateTime.Today.AddDays(-d))
                    .Select(dt => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = dt.ToString("yyyy-MM-dd"),
                        Text = dt.ToString("yyyy-MM-dd")
                    })
                    .ToList()
            };

            return View(vm);
        }

        // ─── POST: /Employee/AddTimesheet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTimesheet(AddTimesheetViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // repopulate TsDateOptions if needed...
                return View(vm);
            }

            var start = TimeSpan.Parse(vm.StartTime);
            var end = TimeSpan.Parse(vm.EndTime);
            var span = end - start;

            var ts = new Timesheet
            {
                EmpId = vm.EmpId,
                TsDate = vm.TsDate,
                TsApproved = "N",
                StartTime = vm.StartTime,
                EndTime = vm.EndTime,
                Hours = (byte)span.Hours,
                Minutes = (byte)span.Minutes,
                Overtime = "N",
                Modified = DateTime.Now
            };

            _db.Timesheets.Add(ts);
            _db.SaveChanges();

            foreach (var item in vm.Items)
            {
                _db.TimesheetsItems.Add(new TimesheetsItem
                {
                    TsId = ts.TsId,
                    TimeFrom = item.TimeFrom,
                    TimeOut = item.TimeOut,
                    Description = item.Description
                });
            }
            _db.SaveChanges();

            return RedirectToAction(nameof(Index),
                                    new { empId = vm.EmpId });
        }


        [HttpGet]
        public IActionResult ViewTimesheet(int empId, int tsId)
        {
            var ts = _db.Timesheets
                        .Include(t => t.TimesheetsItems)    // load the child rows
                        .Include(t => t.Emp)
                        .FirstOrDefault(t => t.EmpId == empId && t.TsId == tsId);

            if (ts == null) return NotFound();

            var vm = new TimesheetViewModel
            {
                TsId = ts.TsId,
                EmpId = ts.EmpId,
                EmpName = ts.Emp.EmpName,
                TsDate = ts.TsDate,
                Approved = ts.TsApproved == "Y",
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
                Hours = ts.Hours,
                Minutes = ts.Minutes,
                Overtime = ts.Overtime == "Y",
                Modified = ts.Modified,

                // NEW: map each TimesheetsItem into your view-model list
                Items = ts.TimesheetsItems
                              .Select(i => new TimesheetsItemViewModel
                              {
                                  TrId = i.TrId,
                                  TimeFrom = i.TimeFrom,
                                  TimeOut = i.TimeOut,
                                  Description = i.Description
                              })
                              .ToList()
            };

            return View(vm);
        }
    }
}
