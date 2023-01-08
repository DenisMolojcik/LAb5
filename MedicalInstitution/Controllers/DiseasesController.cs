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
using MedicalInstitution.ViewsModels;
using Microsoft.AspNetCore.Http;
using MedicalInstitution.Infrastructure;
using MedicalInstitution.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using X.PagedList;

namespace MedicalInstitution.Controllers
{
    public class DiseasesController : Controller
    {
        private readonly Context _context;

        public DiseasesController(Context context)
        {
            _context = context;
        }

        // GET: Diseases
        public IActionResult Index(SortStateDisease sortStateDisease, string currentFilter, string searchName, int? page, bool reset)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchName");
                HttpContext.Session.Remove("sortStateDisease");
            }
            if (searchName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchName", searchName);
            }
            else if (HttpContext.Session.Keys.Contains("searchName"))
            {
                searchName = HttpContext.Session.GetString("searchName");
            }
            else
            {
                searchName = currentFilter;
            }
            if (sortStateDisease != SortStateDisease.Default)
            {
                HttpContext.Session.Set("sortStateDisease", sortStateDisease);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateDisease"))
            {
                sortStateDisease = HttpContext.Session.Get<SortStateDisease>("sortStateDisease");
            }
            ViewBag.CurrentFilter = searchName;
            ICached<Disease> cached = _context.GetService<ICached<Disease>>();
            IEnumerable<Disease> diseases = cached.GetList();
            diseases = Sort(diseases, sortStateDisease);
            ViewBag.CurrentSort = sortStateDisease;
            diseases = Search(diseases, searchName);
            int pageNumber = page ?? 1;
            return View(diseases.ToPagedList(pageNumber, 15));
        }

        // GET: Diseases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases
                .FirstOrDefaultAsync(m => m.DiseaseId == id);
            if (disease == null)
            {
                return NotFound();
            }

            return View(disease);
        }

        // GET: Diseases/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diseases/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DiseaseId,Name,Symptom,Duration,Consequence")] Disease disease)
        {
            if (ModelState.IsValid)
            {
                _context.Add(disease);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<Disease>>().AddList("CachedDisease");
                return RedirectToAction(nameof(Index));
                
            }
            return View(disease);
        }

        // GET: Diseases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases.FindAsync(id);
            if (disease == null)
            {
                return NotFound();
            }
            return View(disease);
        }

        // POST: Diseases/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DiseaseId,Name,Symptom,Duration,Consequence")] Disease disease)
        {
            if (id != disease.DiseaseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disease);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<Disease>>().AddList("CachedDisease");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiseaseExists(disease.DiseaseId))
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
            return View(disease);
        }

        // GET: Diseases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disease = await _context.Diseases
                .FirstOrDefaultAsync(m => m.DiseaseId == id);
            if (disease == null)
            {
                return NotFound();
            }

            return View(disease);
        }

        // POST: Diseases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var disease = await _context.Diseases.FindAsync(id);
            _context.Diseases.Remove(disease);
            await _context.SaveChangesAsync();
            _context.GetService<ICached<Disease>>().AddList("CachedDisease");
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<Disease> Search(IEnumerable<Disease> diseases, string searchName)
        {
            if (!String.IsNullOrEmpty(searchName))
            {
                diseases = diseases.Where(s => s.Name.Contains(searchName));
            }
            return diseases;
        }

        private IEnumerable<Disease> Sort(IEnumerable<Disease> diseases, SortStateDisease sortStateDisease)
        {
            ViewData["DsName"] = sortStateDisease == SortStateDisease.NameAsc ? SortStateDisease.NameDesc : SortStateDisease.NameAsc;
            ViewData["DsDuration"] = sortStateDisease == SortStateDisease.DurationAsc ? SortStateDisease.DurationDesc : SortStateDisease.DurationAsc;
            ViewData["DsSymptom"] = sortStateDisease == SortStateDisease.SymptomAsc ? SortStateDisease.SymptomDesc : SortStateDisease.SymptomAsc;
            ViewData["DsConsequence"] = sortStateDisease == SortStateDisease.ConsequenceAsc ? SortStateDisease.ConsequenceDesc : SortStateDisease.ConsequenceAsc;
            diseases = sortStateDisease switch
            {
                SortStateDisease.NameAsc => diseases.OrderBy(t => t.Name),
                SortStateDisease.NameDesc => diseases.OrderByDescending(t => t.Name),
                SortStateDisease.DurationAsc => diseases.OrderBy(t => t.Duration),
                SortStateDisease.DurationDesc => diseases.OrderByDescending(t => t.Duration),
                SortStateDisease.SymptomAsc => diseases.OrderBy(t => t.Symptom),
                SortStateDisease.SymptomDesc => diseases.OrderBy(t => t.Symptom),
                SortStateDisease.ConsequenceAsc => diseases.OrderBy(t => t.Consequence),
                SortStateDisease.ConsequenceDesc => diseases.OrderBy(t => t.Consequence),
                _ => diseases.OrderByDescending(t => t.DiseaseId),
            };
            return diseases;
        }

        private bool DiseaseExists(int id)
        {
            return _context.Diseases.Any(e => e.DiseaseId == id);
        }
    }
}
