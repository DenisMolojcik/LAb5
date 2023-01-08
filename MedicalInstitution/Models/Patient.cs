using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalInstitution.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateHospitalisation { get; set; }
        public DateTime DateDischarge { get; set; }
        public string Diagnos { get; set; }
        public string Department { get; set; }
        public string ResultTreatment { get; set; }
        public ICollection<Therapy> Therapies { get; set; }
        public Patient()
        {   
            Therapies = new List<Therapy>();
        }
    }
}
