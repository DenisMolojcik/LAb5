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
using MedicalInstitution.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using X.PagedList;
using MedicalInstitution.Infrastructure;

namespace MedicalInstitution.Controllers
{
    public class MediciansController : Controller
    {
        private readonly Context _context;

        public MediciansController(Context context)
        {
            _context = context;
        }

        // GET: Medicians
        public async Task<IActionResult> Index(SortStateMedician sortStateMedician, string currentFilter, string searchMedicianName, int? page, bool reset = false)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchMedicianName");
                HttpContext.Session.Remove("sortStateMedician");
            }
            if (searchMedicianName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchMedicianName", searchMedicianName);
            }
            else if (HttpContext.Session.Keys.Contains("searchMedicianName"))
            {
                searchMedicianName = HttpContext.Session.GetString("searchMedicianName");
            }
            else
            {
                searchMedicianName = currentFilter;
            }
            if (sortStateMedician != SortStateMedician.Default)
            {
                HttpContext.Session.Set("sortStateMedician", sortStateMedician);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateMedician"))
            {
                sortStateMedician = HttpContext.Session.Get<SortStateMedician>("sortStateMedician");
            }
            ViewBag.CurrentFilter = searchMedicianName;
            ICached<Medician> cached = _context.GetService<ICached<Medician>>();
            IEnumerable<MedicianView>  medicianViews = GetMedicianViews(cached.GetList());
            medicianViews = await Sort(medicianViews, sortStateMedician).ToListAsync();
            ViewBag.CurrentSort = sortStateMedician;
            medicianViews = Search(medicianViews, searchMedicianName);
            int pageNumber = page ?? 1;
            return View(medicianViews.ToPagedList(pageNumber, 15));
        }

        // GET: Medicians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medician = await _context.Medicianes
                .Include(m => m.CostMediciane)
                .FirstOrDefaultAsync(m => m.MedicianId == id);
            if (medician == null)
            {
                return NotFound();
            }

            return View(medician);
        }

        // GET: Medicians/Create
        public IActionResult Create()
        {
            ViewData["CostMedicianeId"] = new SelectList(_context.CostMedicianes, "CostMedicianeId", "Manufacturer");
            return View();
        }

        // POST: Medicians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicianId,Name,Indication,Contraindicat,Manufacturer,Packaging,Dasage,CostMedicianeId")] Medician medician)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medician);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<Medician>>().AddList("CachedMedician");
                return RedirectToAction(nameof(Index));
            }
            ViewData["CostMedicianeId"] = new SelectList(_context.CostMedicianes, "CostMedicianeId", "Manufacturer");
            return View(medician);
        }

        // GET: Medicians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medician = await _context.Medicianes.FindAsync(id);
            if (medician == null)
            {
                return NotFound();
            }
            ViewData["CostMedicianeId"] = new SelectList(_context.CostMedicianes, "CostMedicianeId", "Manufacturer");
            return View(medician);
        }

        // POST: Medicians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicianId,Name,Indication,Contraindicat,Manufacturer,Packaging,Dasage,CostMedicianeId")] Medician medician)
        {
            if (id != medician.MedicianId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medician);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<Medician>>().AddList("CachedMedician");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicianExists(medician.MedicianId))
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
            ViewData["CostMedicianeId"] = new SelectList(_context.CostMedicianes, "CostMedicianeId", "Manufacturer");
            return View(medician);
        }

        // GET: Medicians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medician = await _context.Medicianes
                .Include(m => m.CostMediciane)
                .FirstOrDefaultAsync(m => m.MedicianId == id);
            if (medician == null)
            {
                return NotFound();
            }

            return View(medician);
        }

        // POST: Medicians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medician = await _context.Medicianes.FindAsync(id);
            _context.Medicianes.Remove(medician);
            await _context.SaveChangesAsync();
            _context.GetService<ICached<Medician>>().AddList("CachedMedician");
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<MedicianView> Search(IEnumerable<MedicianView> medicianViews, string searchMedicianName)
        {
            if (!String.IsNullOrEmpty(searchMedicianName))
            {
                medicianViews = medicianViews.Where(s => s.Name.Contains(searchMedicianName));
            }
            return medicianViews;
        }
        private IEnumerable<MedicianView> Sort(IEnumerable<MedicianView> medicianViews, SortStateMedician sortStateMedician)
        {
            ViewData["MedName"] = sortStateMedician == SortStateMedician.NameAsc ? SortStateMedician.NameDesc : SortStateMedician.NameAsc;
            ViewData["MedIndication"] = sortStateMedician == SortStateMedician.IndicationAsc ? SortStateMedician.IndicationDesc : SortStateMedician.IndicationAsc;
            ViewData["MedCost"] = sortStateMedician == SortStateMedician.CostAsc ? SortStateMedician.CostDesc : SortStateMedician.CostAsc;
            medicianViews = sortStateMedician switch
            {
                SortStateMedician.NameAsc => medicianViews.OrderBy(t => t.Name),
                SortStateMedician.NameDesc => medicianViews.OrderByDescending(t => t.Name),
                SortStateMedician.IndicationAsc => medicianViews.OrderBy(t => t.Indication),
                SortStateMedician.IndicationDesc => medicianViews.OrderByDescending(t => t.Indication),
                SortStateMedician.CostAsc => medicianViews.OrderBy(t => t.Cost),
                SortStateMedician.CostDesc => medicianViews.OrderByDescending(t => t.Cost),
                _ => medicianViews.OrderByDescending(t => t.MedicianId),
            };
            return medicianViews;
        }

        private IEnumerable<MedicianView> GetMedicianViews(IEnumerable<Medician> medicians)
        {
            IEnumerable<MedicianView> views = from m in medicians
                                              join c in _context.CostMedicianes
                                              on m.CostMedicianeId equals c.CostMedicianeId
                                              select new MedicianView
                                             {
                                                MedicianId = m.MedicianId,
                                                Manufacturer = m.Manufacturer,
                                                Contraindicat = m.Contraindicat,
                                                Cost = c.Cost,
                                                Dasage = m.Dasage,
                                                Date = c.Date,
                                                Indication = m.Indication,
                                                Name = m.Name,
                                                Packaging = m.Packaging
                                             };
            return views;

        }

        private bool MedicianExists(int id)
        {
            return _context.Medicianes.Any(e => e.MedicianId == id);
        }
    }
}
