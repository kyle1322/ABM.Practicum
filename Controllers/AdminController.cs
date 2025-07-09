using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Timesheets_APP.Data;
using Timesheets_APP.ViewModels;

namespace Timesheets_APP.Controllers
{
    public class AdminController : Controller
    {
        private readonly TimesheetDbContext _db;

        public AdminController(TimesheetDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new AdminLoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var user = await _db.Users
                .SingleOrDefaultAsync(u => u.LoginId == vm.Username);

            if (user != null && ComputeMd5(vm.Password) == user.Password)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.LoginId),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var ci = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(ci));
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(DateTime? date, int? empId, string empName)
        {
            var query = _db.Timesheets
                .Include(t => t.Emp)
                .Include(t => t.TimesheetsItems)
                .AsQueryable();

            if (date.HasValue)
            {
                var d = date.Value.Date;
                query = query.Where(t => t.TsDate == d);
            }

            if (empId.HasValue)
                query = query.Where(t => t.EmpId == empId.Value);

            if (!string.IsNullOrWhiteSpace(empName))
                query = query.Where(t => t.Emp.EmpName.Contains(empName));

            var list = await query
                .OrderBy(t => t.TsDate)
                .ThenBy(t => t.StartTime)
                .Select(t => new TimesheetViewModel
                {
                    TsId = t.TsId,
                    EmpId = t.EmpId,
                    EmpName = t.Emp.EmpName,
                    TsDate = t.TsDate,
                    Approved = t.TsApproved == "Y",
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    Hours = t.Hours,
                    Minutes = t.Minutes,
                    Overtime = t.Overtime == "Y",
                    Modified = t.Modified,
                    Items = t.TimesheetsItems
                        .Select(i => new TimesheetsItemViewModel
                        {
                            TrId = i.TrId,
                            TimeFrom = i.TimeFrom,
                            TimeOut = i.TimeOut,
                            Description = i.Description
                        })
                        .ToList()
                })
                .ToListAsync();

            ViewData["FilterDate"] = date?.ToString("yyyy-MM-dd") ?? "";
            return View(list);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ViewTimesheet(int tsId)
        {
            var ts = await _db.Timesheets
                .Include(t => t.Emp)
                .Include(t => t.TimesheetsItems)
                .FirstOrDefaultAsync(t => t.TsId == tsId);

            if (ts == null)
                return NotFound();

            var start = TimeSpan.Parse(ts.StartTime);
            var end = TimeSpan.Parse(ts.EndTime);
            var span = end - start;

            var vm = new TimesheetViewModel
            {
                TsId = ts.TsId,
                EmpId = ts.EmpId,
                EmpName = ts.Emp.EmpName,
                TsDate = ts.TsDate,
                Approved = ts.TsApproved == "Y",
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
                Hours = (byte)span.Hours,
                Minutes = (byte)span.Minutes,
                Overtime = ts.Overtime == "Y",
                Modified = ts.Modified,
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ViewTimesheet(TimesheetViewModel vm)
        {
            var ts = await _db.Timesheets.FindAsync(vm.TsId);
            if (ts == null)
                return NotFound();

            ts.StartTime = vm.StartTime;
            ts.EndTime = vm.EndTime;
            var span = TimeSpan.Parse(vm.EndTime) - TimeSpan.Parse(vm.StartTime);
            ts.Hours = (byte)span.Hours;
            ts.Minutes = (byte)span.Minutes;
            ts.TsApproved = vm.Approved ? "Y" : "N";
            ts.Modified = DateTime.Now;

            foreach (var item in vm.Items)
            {
                var ent = await _db.TimesheetsItems.FindAsync(item.TrId);
                if (ent != null)
                {
                    ent.TimeFrom = item.TimeFrom;
                    ent.TimeOut = item.TimeOut;
                    ent.Description = item.Description;
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditTimesheet(int tsId)
        {
            var ts = await _db.Timesheets
                .Include(t => t.Emp)
                .Include(t => t.TimesheetsItems)
                .FirstOrDefaultAsync(t => t.TsId == tsId);

            if (ts == null)
                return NotFound();

            var start = TimeSpan.Parse(ts.StartTime);
            var end = TimeSpan.Parse(ts.EndTime);

            var vm = new TimesheetViewModel
            {
                TsId = ts.TsId,
                EmpId = ts.EmpId,
                EmpName = ts.Emp.EmpName,
                TsDate = ts.TsDate,
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
                Hours = (byte)(end - start).Hours,
                Minutes = (byte)(end - start).Minutes,
                Overtime = ts.Overtime == "Y",
                Approved = ts.TsApproved == "Y",
                Modified = ts.Modified,
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

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTimesheet(TimesheetViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var ts = await _db.Timesheets
                .Include(t => t.TimesheetsItems)
                .FirstOrDefaultAsync(t => t.TsId == vm.TsId);

            if (ts == null)
                return NotFound();

            ts.StartTime = vm.StartTime;
            ts.EndTime = vm.EndTime;
            var span2 = TimeSpan.Parse(vm.EndTime) - TimeSpan.Parse(vm.StartTime);
            ts.Hours = (byte)span2.Hours;
            ts.Minutes = (byte)span2.Minutes;
            ts.Overtime = vm.Overtime ? "Y" : "N";
            ts.TsApproved = vm.Approved ? "Y" : "N";
            ts.Modified = DateTime.Now;

            foreach (var slotVm in vm.Items)
            {
                var slot = ts.TimesheetsItems.FirstOrDefault(i => i.TrId == slotVm.TrId);
                if (slot != null)
                {
                    slot.TimeFrom = slotVm.TimeFrom;
                    slot.TimeOut = slotVm.TimeOut;
                    slot.Description = slotVm.Description;
                }
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { date = ts.TsDate });
        }

        private static string ComputeMd5(string input)
        {
            using var md5 = MD5.Create();
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
