using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalInstitution.Data;
using MedicalInstitution.Models;
using MedicalInstitution.Models.Enums;
using MedicalInstitution.Services;
using MedicalInstitution.ViewsModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Infrastructure;
using X.PagedList;
using MedicalInstitution.Infrastructure;

namespace MedicalInstitution.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly Context _context;

        public DoctorsController(Context context)
        {
            _context = context;
        }

        // GET: Doctors
        public IActionResult Index(SortStateDoctor sortStateDoctor, string currentFilter, string searchDoctorName, int? page, bool reset = false)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchDocName");
                HttpContext.Session.Remove("sortStateDoctor");
            }
            if (searchDoctorName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchDocName", searchDoctorName);
            }
            else if (HttpContext.Session.Keys.Contains("searchDocName"))
            {
                searchDoctorName = HttpContext.Session.GetString("searchDocName");
            }
            else
            {
                searchDoctorName = currentFilter;
            }
            if (sortStateDoctor != SortStateDoctor.Default)
            {
                HttpContext.Session.Set("sortStateDoctor", sortStateDoctor);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateDoctor"))
            {
                sortStateDoctor = HttpContext.Session.Get<SortStateDoctor>("sortStateDoctor");
            }
            ViewBag.CurrentFilter = searchDoctorName;
            IEnumerable<Doctor> doctors = _context.GetService<ICached<Doctor>>().GetList();
            doctors = Sort(doctors, sortStateDoctor);
            ViewBag.CurrentSort = sortStateDoctor;
            doctors = Search(doctors, searchDoctorName);
            int pageNumber = page ?? 1;
            return View(doctors.ToPagedList(pageNumber, 15));
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,Name,Age,Gender,Position")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<Doctor>>().AddList("CachedDoctor");
                return RedirectToAction(nameof(Index));
            }
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,Name,Age,Gender,Position")] Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<Doctor>>().AddList("CachedDoctor");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
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
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            _context.GetService<ICached<Doctor>>().AddList("CachedDoctor");
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<Doctor> Search(IEnumerable<Doctor> doctors, string searchDoctorName)
        {
            if (!String.IsNullOrEmpty(searchDoctorName))
            {
                doctors = doctors.Where(s => s.Name.Contains(searchDoctorName));
            }
            return doctors;
        }

        private IEnumerable<Doctor> Sort(IEnumerable<Doctor> doctors, SortStateDoctor sortStateDoctor)
        {
            ViewData["DocName"] = sortStateDoctor == SortStateDoctor.NameAsc ? SortStateDoctor.NameDesc : SortStateDoctor.NameAsc;
            ViewData["DocAge"] = sortStateDoctor == SortStateDoctor.AgeAsc ? SortStateDoctor.AgeDesc : SortStateDoctor.AgeAsc;
            ViewData["DocGender"] = sortStateDoctor == SortStateDoctor.GenderAsc ? SortStateDoctor.GenderDesc : SortStateDoctor.GenderAsc;
            ViewData["DocPosition"] = sortStateDoctor == SortStateDoctor.PositionAsc ? SortStateDoctor.PositionDesc : SortStateDoctor.PositionAsc;
            doctors = sortStateDoctor switch
            {
                SortStateDoctor.NameAsc => doctors.OrderBy(t => t.Name),
                SortStateDoctor.NameDesc => doctors.OrderByDescending(t => t.Name),
                SortStateDoctor.AgeAsc => doctors.OrderBy(t => t.Age),
                SortStateDoctor.AgeDesc => doctors.OrderByDescending(t => t.Age),
                SortStateDoctor.GenderAsc => doctors.OrderBy(t => t.Gender),
                SortStateDoctor.GenderDesc => doctors.OrderBy(t => t.Gender),
                SortStateDoctor.PositionAsc => doctors.OrderBy(t => t.Position),
                SortStateDoctor.PositionDesc => doctors.OrderBy(t => t.Position),
                _ => doctors.OrderByDescending(t => t.DoctorId),
            };
            return doctors;
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}
