using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalInstitution.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Position { get; set; }

        public ICollection<Therapy> Therapies { get; set; }
        public Doctor()
        {
            Therapies = new List<Therapy>();
        }
    }
}
