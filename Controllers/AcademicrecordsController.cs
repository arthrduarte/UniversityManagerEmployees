using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using lab6.DataAccess;

namespace LabAssignment6.Controllers
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

            var records = from r in _context.Academicrecords.Include(a => a.CourseCodeNavigation).Include(a => a.Student)
                          select r;


            switch (sortOrder)
            {
                case "course_desc":
                    records = records.OrderByDescending(r => r.Grade == null).ThenByDescending(r => r.CourseCodeNavigation.Title);
                    break;
                case "student_desc":
                    records = records.OrderByDescending(r => r.Grade == null).ThenByDescending(r => r.Student.Name);
                    break;

            }

            return View(await records.AsNoTracking().ToListAsync());
        }
        // GET: Academicrecords/EditAll
        public async Task<IActionResult> EditAll(string sortOrder)
        {
            ViewData["CourseSort"] = String.IsNullOrEmpty(sortOrder) ? "course_desc" : "";
            ViewData["StudentSort"] = sortOrder == "Student" ? "student_desc" : "Student";

            var records = from r in _context.Academicrecords.Include(a => a.CourseCodeNavigation).Include(a => a.Student)
                          select r;

            records = records.OrderBy(r => r.Grade == null).ThenBy(r => r.CourseCodeNavigation.Title);

            switch (sortOrder)
            {
                case "course_desc":
                    records = records.OrderByDescending(r => r.Grade == null).ThenByDescending(r => r.CourseCodeNavigation.Title);
                    break;

                case "student_desc":
                    records = records.OrderByDescending(r => r.Grade == null).ThenByDescending(r => r.Student.Name);
                    break;

            }

            return View(await records.AsNoTracking().ToListAsync());
        }

        // POST: Academicrecords/EditAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(List<Academicrecord> academicRecords)
        {
            if (ModelState.IsValid)
            {
                foreach (var record in academicRecords)
                {
                    if (record.StudentId != null && record.CourseCode != null)
                    {
                        var existingRecord = await _context.Academicrecords.FindAsync(record.StudentId, record.CourseCode);
                        if (existingRecord != null)
                        {
                            existingRecord.Grade = record.Grade;
                            _context.Entry(existingRecord).State = EntityState.Modified;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(academicRecords);
        }


        // GET: Academicrecords/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (academicrecord == null)
            {
                return NotFound();
            }

            return View(academicrecord);
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
        // GET: Academicrecords/Edit/5
        // GET: Academicrecords/Edit/5
        public async Task<IActionResult> Edit(string studentId, string courseCode)
        {
            if (studentId == null || courseCode == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.StudentId == studentId && m.CourseCode == courseCode);
            if (academicrecord == null)
            {
                return NotFound();
            }
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Title", academicrecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", academicrecord.StudentId);
            return View(academicrecord);
        }

        // POST: Academicrecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string studentId, string courseCode, [Bind("CourseCode,StudentId,Grade")] Academicrecord academicrecord)
        {
            if (studentId != academicrecord.StudentId || courseCode != academicrecord.CourseCode)
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
                    if (!AcademicrecordExists(academicrecord.StudentId, academicrecord.CourseCode))
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
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Title", academicrecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", academicrecord.StudentId);
            return View(academicrecord);
        }




        // GET: Academicrecords/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (academicrecord == null)
            {
                return NotFound();
            }

            return View(academicrecord);
        }

        // POST: Academicrecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var academicrecord = await _context.Academicrecords.FindAsync(id);
            if (academicrecord != null)
            {
                _context.Academicrecords.Remove(academicrecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AcademicrecordExists(string studentId, string courseCode)
        {
            return _context.Academicrecords.Any(e => e.StudentId == studentId && e.CourseCode == courseCode);
        }
    }
}
