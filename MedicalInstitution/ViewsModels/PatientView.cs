using System;

namespace MedicalInstitution.ViewsModels
{
    public class PatientView
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateHospitalisation { get; set; }
        public DateTime DateDischarge { get; set; }
        public string Diagnos { get; set; }
        public string Department { get; set; }
        public string ResultTreatment { get; set; }
    }
}
