using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab6.DataAccess;

namespace lab6.Controllers
{
    public class AcademicrecordsController : Controller
    {
        private readonly StudentrecordContext _context;

        public AcademicrecordsController(StudentrecordContext context)
        {
            _context = context;
        }

        // GET: Academicrecords
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["CourseSort"] = String.IsNullOrEmpty(sortOrder) ? "course_desc" : "";
            ViewData["StudentSort"] = sortOrder == "Student" ? "student_desc" : "Student";

            var recordsContext = from r in _context.Academicrecords.
                          Include(a => a.CourseCodeNavigation).
                          Include(a => a.Student)
                          select r;

            switch (sortOrder)
            {
                case "course_desc":
                    recordsContext = recordsContext.OrderBy(r => r.Grade == null ? 0 : 1).ThenByDescending(r => r.CourseCodeNavigation.Title);
                    break;
                case "student_desc":
                    recordsContext = recordsContext.OrderBy(r => r.Grade == null ? 0 : 1).ThenByDescending(r => r.Student.Name);
                    break;
                default:
                    recordsContext = recordsContext.OrderBy(r => r.Grade == null ? 0 : 1).ThenByDescending(r => r.CourseCodeNavigation.Title);
                    break;
            }

            return View(await recordsContext.AsNoTracking().ToListAsync());
        }

       

        // GET: Academicrecords/Create
        public IActionResult Create()
        {
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            return View();
        }

        // POST: Academicrecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseCode,StudentId,Grade")] Academicrecord academicrecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicrecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicrecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicrecord.StudentId);
            return View(academicrecord);
        }

        // GET: Academicrecords/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords.FindAsync(id);
            if (academicrecord == null)
            {
                return NotFound();
            }
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicrecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicrecord.StudentId);
            return View(academicrecord);
        }

        // POST: Academicrecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CourseCode,StudentId,Grade")] Academicrecord academicrecord)
        {
            if (id != academicrecord.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicrecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicrecordExists(academicrecord.StudentId))
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
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicrecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicrecord.StudentId);
            return View(academicrecord);
        }

        private bool AcademicrecordExists(string id)
        {
            return _context.Academicrecords.Any(e => e.StudentId == id);
        }
    }
}
