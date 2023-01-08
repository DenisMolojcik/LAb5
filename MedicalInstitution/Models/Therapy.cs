using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalInstitution.Models
{
    public class Therapy
    {
        public int Id { get; set; }
        public int DiseaseId { get; set; }
        public Disease Disease { get; set; }
        public int MedicianId { get; set; }
        public Medician Medician { get; set; }
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime Date { get; set; }
    }
}
