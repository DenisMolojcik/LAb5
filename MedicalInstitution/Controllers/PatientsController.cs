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
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;
using X.PagedList;
using MedicalInstitution.Infrastructure;

namespace MedicalInstitution.Controllers
{
    public class PatientsController : Controller
    {
        private readonly Context _context;

        public PatientsController(Context context)
        {
            _context = context;
        }

        // GET: Patients
        public IActionResult Index(SortStatePatient sortStatePatient, string currentFilter, string searchPatientName, int? page, bool reset = false)
        {
            if (reset)
            {
                HttpContext.Session.Remove("searchPatientName");
                HttpContext.Session.Remove("sortStatePatient");
            }
            if (searchPatientName != null)
            {
                page = 1;
                HttpContext.Session.SetString("searchPatientName", searchPatientName);
            }
            else if (HttpContext.Session.Keys.Contains("searchPatientName"))
            {
                searchPatientName = HttpContext.Session.GetString("searchPatientName");
            }
            else
            {
                searchPatientName = currentFilter;
            }
            if (sortStatePatient != SortStatePatient.Default)
            {
                HttpContext.Session.Set("sortStatePatient", sortStatePatient);
            }
            else if (HttpContext.Session.Keys.Contains("sortStatePatient"))
            {
                sortStatePatient = HttpContext.Session.Get<SortStatePatient>("sortStatePatient");
            }
            ViewBag.CurrentFilter = searchPatientName;
            IEnumerable<Patient> patients = _context.GetService<ICached<Patient>>().GetList();
            patients = Sort(patients, sortStatePatient);
            ViewBag.CurrentSort = sortStatePatient;
            patients = Search(patients, searchPatientName);
            int pageNumber = page ?? 1;
            return View(patients.ToPagedList(pageNumber, 15));
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,Surname,Name,Lastname,Age,Gender,PhoneNumber,DateHospitalisation,DateDischarge,Diagnos,Department,ResultTreatment")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                _context.GetService<ICached<Patient>>().AddList("CachedPatient");
                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,Surname,Name,Lastname,Age,Gender,PhoneNumber,DateHospitalisation,DateDischarge,Diagnos,Department,ResultTreatment")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                    _context.GetService<ICached<Patient>>().AddList("CachedPatient");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
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
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            _context.GetService<ICached<Patient>>().AddList("CachedPatient");
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<Patient> Search(IEnumerable<Patient> patients, string searchPatientSurname)
        {
            if (!String.IsNullOrEmpty(searchPatientSurname))
            {
                patients = patients.Where(s => s.Surname.Contains(searchPatientSurname));
            }
            return patients;
        }

        private IEnumerable<Patient> Sort(IEnumerable<Patient> patients, SortStatePatient sortStatePatient)
        {
            ViewData["PatName"] = sortStatePatient == SortStatePatient.NameAsc ? SortStatePatient.NameDesc : SortStatePatient.NameAsc;
            ViewData["PatSurname"] = sortStatePatient == SortStatePatient.SurnameAsc ? SortStatePatient.SurnameDesc : SortStatePatient.SurnameAsc;
            ViewData["PatGender"] = sortStatePatient == SortStatePatient.GenderAsc ? SortStatePatient.GenderDesc : SortStatePatient.GenderAsc;
            ViewData["PatAge"] = sortStatePatient == SortStatePatient.AgeAsc ? SortStatePatient.AgeDesc : SortStatePatient.AgeAsc;
            ViewData["PatDiagnos"] = sortStatePatient == SortStatePatient.DiagnosAsc ? SortStatePatient.DiagnosDesc : SortStatePatient.DiagnosAsc;
            ViewData["PatLastname"] = sortStatePatient == SortStatePatient.LastNameAsc ? SortStatePatient.LastNameDesc : SortStatePatient.LastNameAsc;
            ViewData["PatPhoneNumber"] = sortStatePatient == SortStatePatient.PhoneNumberAsc ? SortStatePatient.PhoneNumberDesc : SortStatePatient.PhoneNumberAsc;
            ViewData["PatDepartment"] = sortStatePatient == SortStatePatient.DepartmentAsc ? SortStatePatient.DepartmentDesc : SortStatePatient.DepartmentAsc;
            ViewData["PatResultTreatment"] = sortStatePatient == SortStatePatient.ResultTreatmentAsc ? SortStatePatient.ResultTreatmentDesc : SortStatePatient.ResultTreatmentAsc;
            ViewData["PatDischarge"] = sortStatePatient == SortStatePatient.DateDischargeAsc ? SortStatePatient.DateDischargeDesc : SortStatePatient.DateDischargeAsc;
            ViewData["PatDateHospit"] = sortStatePatient == SortStatePatient.DateHospitalisationAsc ? SortStatePatient.DateHospitalisationDesc : SortStatePatient.DateHospitalisationAsc;
            patients = sortStatePatient switch
            {
                SortStatePatient.NameAsc => patients.OrderBy(t => t.Name),
                SortStatePatient.NameDesc => patients.OrderByDescending(t => t.Name),
                SortStatePatient.SurnameAsc => patients.OrderBy(t => t.Surname),
                SortStatePatient.SurnameDesc => patients.OrderByDescending(t => t.Surname),
                SortStatePatient.GenderAsc => patients.OrderBy(t => t.Gender),
                SortStatePatient.GenderDesc => patients.OrderByDescending(t => t.Gender),
                SortStatePatient.AgeAsc => patients.OrderBy(t => t.Age),
                SortStatePatient.AgeDesc => patients.OrderByDescending(t => t.Age),
                SortStatePatient.DiagnosAsc => patients.OrderBy(t => t.Diagnos),
                SortStatePatient.DiagnosDesc => patients.OrderByDescending(t => t.Diagnos),
                SortStatePatient.LastNameAsc => patients.OrderBy(t => t.Lastname),
                SortStatePatient.LastNameDesc => patients.OrderByDescending(t => t.Lastname),
                SortStatePatient.PhoneNumberAsc => patients.OrderBy(t => t.PhoneNumber),
                SortStatePatient.PhoneNumberDesc => patients.OrderByDescending(t => t.PhoneNumber),
                SortStatePatient.ResultTreatmentAsc => patients.OrderBy(t => t.ResultTreatment),
                SortStatePatient.ResultTreatmentDesc => patients.OrderByDescending(t => t.ResultTreatment),
                SortStatePatient.DepartmentAsc => patients.OrderBy(t => t.Department),
                SortStatePatient.DepartmentDesc => patients.OrderByDescending(t => t.Department),
                SortStatePatient.DateDischargeAsc => patients.OrderBy(t => t.DateDischarge),
                SortStatePatient.DateDischargeDesc => patients.OrderByDescending(t => t.DateDischarge),
                SortStatePatient.DateHospitalisationAsc => patients.OrderBy(t => t.DateHospitalisation),
                SortStatePatient.DateHospitalisationDesc => patients.OrderByDescending(t => t.DateHospitalisation),
                _ => patients.OrderByDescending(t => t.PatientId),
            };
            return patients;
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
