using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab6.DataAccess;
using lab6.Models.ViewModel;

namespace lab6.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly StudentrecordContext _context;

        public EmployeesController(StudentrecordContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees.Include(e => e.Roles).ToListAsync();
            return View(employees);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            var model = new SelectEmployeeRole
            {
                Employee = new Employee(),
                Roles = _context.Roles.Select(r => new RoleSelection
                {
                    RoleId = r.Id,
                    RoleName = r.Role1,
                    IsSelected = false
                }).ToList()
            };
            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SelectEmployeeRole model)
        {
            if (ModelState.IsValid)
            {
                foreach (var roleSelection in model.Roles)
                {
                    if (roleSelection.IsSelected)
                    {
                        var role = await _context.Roles.FindAsync(roleSelection.RoleId);
                        if (role != null)
                        {
                            model.Employee.Roles.Add(role);
                        }
                    }
                }
                _context.Add(model.Employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.Include(e => e.Roles).FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new SelectEmployeeRole
            {
                Employee = employee,
                Roles = _context.Roles.AsEnumerable().Select(r => new RoleSelection
                {
                    RoleId = r.Id,
                    RoleName = r.Role1,
                    IsSelected = employee.Roles.Any(er => er.Id == r.Id)
                }).ToList()
            };

            return View(model);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SelectEmployeeRole model)
        {
            if (id != model.Employee.Id)
            {
                return NotFound();
            }

            if (model.Roles == null)
            {
                model.Roles = new List<RoleSelection>();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var employeeToUpdate = await _context.Employees.Include(e => e.Roles).FirstOrDefaultAsync(e => e.Id == id);
                    if (employeeToUpdate == null)
                    {
                        return NotFound();
                    }

                    employeeToUpdate.Name = model.Employee.Name;
                    employeeToUpdate.UserName = model.Employee.UserName;
                    employeeToUpdate.Password = model.Employee.Password;

                    employeeToUpdate.Roles.Clear();
                    foreach (var roleSelection in model.Roles)
                    {
                        if (roleSelection.IsSelected)
                        {
                            var role = await _context.Roles.FindAsync(roleSelection.RoleId);
                            if (role != null)
                            {
                                employeeToUpdate.Roles.Add(role);
                            }
                        }
                    }

                    _context.Update(employeeToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(model.Employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
