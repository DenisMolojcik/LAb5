using MedicalInstitution.Data;
using MedicalInstitution.Infrastructure;
using MedicalInstitution.Models;
using MedicalInstitution.Models.Enums;
using MedicalInstitution.Services;
using MedicalInstitution.ViewsModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using X.PagedList;

namespace MedicalInstitution.Controllers
{
    public class TherapyController : Controller
    {
        private readonly Context _context;
        public TherapyController(Context context)
        {
            _context = context;
        }
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 240)]
        public IActionResult Index(SortStateTherapy sortStateTherapy, string currentFilter, string searchDoctorName, int? page, bool reset = false)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchDoc");
                HttpContext.Session.Remove("sortStateTherapy");
            }
            if (searchDoctorName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchDoc", searchDoctorName);               
            }
            else if (HttpContext.Session.Keys.Contains("searchDoc"))
            {
                searchDoctorName = HttpContext.Session.GetString("searchDoc");
            }
            else
            {
                searchDoctorName = currentFilter;
            }
            if (sortStateTherapy != SortStateTherapy.Default)
            {
                HttpContext.Session.Set("sortStateTherapy", sortStateTherapy);
            }
            else if (HttpContext.Session.Keys.Contains("sortStateTherapy"))
            {
                sortStateTherapy = HttpContext.Session.Get<SortStateTherapy>("sortStateTherapy");
            }
            ViewBag.CurrentFilter = searchDoctorName;
            IEnumerable<TherapyView> therapyViews = default; 
            ICached<Therapy> cached = _context.GetService<ICached<Therapy>>();
            therapyViews = GetTherapyViews(cached.GetList());
            therapyViews = Sort(therapyViews, sortStateTherapy);
            ViewBag.CurrentSort = sortStateTherapy;
            therapyViews = Search(therapyViews, searchDoctorName);
            int pageNumber = page ?? 1;
            return View(therapyViews.ToPagedList(pageNumber, 15));
        }
        private IEnumerable<TherapyView> GetTherapyViews(IEnumerable<Therapy> therapies)
        {
            IEnumerable<TherapyView> therapyViews = from t in therapies
                                                    join d in _context.Diseases
                                                    on t.DiseaseId equals d.DiseaseId
                                                    join m in _context.Medicianes
                                                    on t.MedicianId equals m.MedicianId
                                                    join doc in _context.Doctors
                                                    on t.DoctorId equals doc.DoctorId
                                                    join p in _context.Patients
                                                    on t.PatientId equals p.PatientId
                                                    select new TherapyView
                                                    {
                                                        Id = t.Id,
                                                        DiseaseName = d.Name,
                                                        MedicianName = m.Name,
                                                        DoctorName = doc.Name,
                                                        PatientName = p.Name,
                                                        Date = t.Date
                                                    };
            return therapyViews;
        }

        public ActionResult Delete(int id)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    Therapy therapy = _context.Therapies.FirstOrDefault(i => i.Id.Equals(id));
                    _context.Remove(therapy);
                    _context.SaveChanges();
                    _context.GetService<ICached<Therapy>>().AddList("TherapyPatient");
                }
                catch(DbUpdateConcurrencyException)
                {
                    return RedirectToAction("HandleError",
                        new RouteValueDictionary(new { Controller = "Home", Action = "HandleError", statuscode = Convert.ToInt32(HttpStatusCode.NotFound) }));
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("HandleError",
                    new RouteValueDictionary(new { Controller = "Home", Action = "HandleError", statuscode = Convert.ToInt32(HttpStatusCode.NotFound) }));
            }
            var thr = _context.Therapies.Find(id);
            if (thr == null)
            {
                return RedirectToAction("HandleError",
                    new RouteValueDictionary(new { Controller = "Home", Action = "HandleError", statuscode = Convert.ToInt32(HttpStatusCode.NotFound) }));
            }
            Therapy therapy = _context.GetService<ICached<Therapy>>().GetList("cachedComputerOrders").FirstOrDefault(c => c.Id == id);
            IEnumerable<PatientView> patientViews = GetPatientViews(_context.Patients);
            ViewData["PatientId"] = new SelectList(patientViews, "PatientId", "Name");
            ViewData["MedicianId"] = new SelectList(_context.Medicianes, "MedicianId", "Name");
            ViewData["DiseaseId"] = new SelectList(_context.Diseases, "DiseaseId", "Name");
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
            return View(therapy);
        }

        [HttpPost]
        public IActionResult Edit(int id, [Bind("Id, DiseaseId, MedicianId, PatientId, DoctorId, Date")] Therapy therapy)
        {
            if (id != therapy.Id)
            {
                return RedirectToAction("HandleError",
                    new RouteValueDictionary(new { Controller = "Home", Action = "HandleError", statuscode = Convert.ToInt32(HttpStatusCode.NotFound) }));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(therapy);
                    _context.SaveChanges();
                    _context.GetService<ICached<Therapy>>().AddList("TherapyPatient");
                }
                catch(DbUpdateConcurrencyException)
                {
                    return RedirectToAction("HandleError",
                        new RouteValueDictionary(new { Controller = "Home", Action = "HandleError", statuscode = Convert.ToInt32(HttpStatusCode.BadRequest)}));
                }
                return RedirectToAction("Index");
            }
            IEnumerable<PatientView> patientViews = GetPatientViews(_context.Patients);
            ViewData["PatientId"] = new SelectList(patientViews, "PatientId", "Name");
            ViewData["MedicianId"] = new SelectList(_context.Medicianes, "MedicianId", "Name");
            ViewData["DiseaseId"] = new SelectList(_context.Diseases, "DiseaseId", "Name");
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
            return View(therapy);
        }

        [HttpGet]
        public IActionResult Create()
        {
            IEnumerable<PatientView> patientViews = GetPatientViews(_context.Patients);
            ViewData["PatientId"] = new SelectList(patientViews, "PatientId", "Name");
            ViewData["MedicianId"] = new SelectList(_context.Medicianes, "MedicianId", "Name");
            ViewData["DiseaseId"] = new SelectList(_context.Diseases, "DiseaseId", "Name");
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create([Bind("Id, DiseaseId, MedicianId, PatientId, DoctorId, Date")] Therapy therapy)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    _context.Add(therapy);
                    _context.SaveChanges();
                    _context.GetService<ICached<Therapy>>().AddList("TherapyPatient");
                }
                catch(DbUpdateConcurrencyException)
                {
                    return RedirectToAction("HandleError",
                        new RouteValueDictionary(new { Controller = "Home", Action = "HandleError", statuscode = Convert.ToInt32(HttpStatusCode.BadRequest) }));
                }
                return RedirectToAction("Index");
            }
            IEnumerable<PatientView> patientViews = GetPatientViews(_context.Patients);
            ViewData["PatientId"] = new SelectList(patientViews, "PatientId", "Name");
            ViewData["MedicianId"] = new SelectList(_context.Medicianes, "MedicianId", "Name");
            ViewData["DiseaseId"] = new SelectList(_context.Diseases, "DiseaseId", "Name");
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
            return View(therapy);
        }
        private IEnumerable<TherapyView> Search(IEnumerable<TherapyView> therapyViews, string searchDoctorName)
        {
            if (!String.IsNullOrEmpty(searchDoctorName))
            {
                therapyViews = therapyViews.Where(s => s.DoctorName.Contains(searchDoctorName));
            }
            return therapyViews;
        }
        private IEnumerable<TherapyView> Sort(IEnumerable<TherapyView> therapyViews, SortStateTherapy sortStateTherapy)
        {
            ViewData["DiseaseName"] = sortStateTherapy == SortStateTherapy.DiseaseNameAsc ? SortStateTherapy.DiseaseNameDesc : SortStateTherapy.DiseaseNameAsc;
            ViewData["MedicianName"] = sortStateTherapy == SortStateTherapy.MedicianNameAsc ? SortStateTherapy.MedicianNameDesc : SortStateTherapy.MedicianNameAsc;
            ViewData["DoctorName"] = sortStateTherapy == SortStateTherapy.DoctorNameAsc ? SortStateTherapy.DoctorNameDesc : SortStateTherapy.DoctorNameAsc;
            therapyViews = sortStateTherapy switch
            {
                SortStateTherapy.DiseaseNameAsc => therapyViews.OrderBy(t => t.DiseaseName),
                SortStateTherapy.DiseaseNameDesc => therapyViews.OrderByDescending(t => t.DiseaseName),
                SortStateTherapy.MedicianNameAsc => therapyViews.OrderBy(t => t.MedicianName),
                SortStateTherapy.MedicianNameDesc => therapyViews.OrderByDescending(t => t.MedicianName),
                SortStateTherapy.DoctorNameAsc => therapyViews.OrderBy(t => t.DoctorName),
                _ => therapyViews.OrderByDescending(t => t.Id),
            };
            return therapyViews;
        }

        private IEnumerable<PatientView> GetPatientViews(IEnumerable<Patient> patients)
        {
            IEnumerable<PatientView> views = from p in patients
                                             select new PatientView
                                             {
                                                 PatientId = p.PatientId,
                                                 Age = p.Age,
                                                 DateDischarge = p.DateDischarge,
                                                 Diagnos = p.Diagnos,
                                                 DateHospitalisation = p.DateHospitalisation,
                                                 Department = p.Department,
                                                 Gender = p.Gender,
                                                 PhoneNumber = p.PhoneNumber,
                                                 ResultTreatment = p.ResultTreatment,
                                                 Name = p.Name + " " + p.Surname + " " +p.Lastname
                                             };
            return views;
                                                
        }
    }
}
