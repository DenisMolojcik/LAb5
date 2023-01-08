using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalInstitution.Models
{
    public class Disease
    {
        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public string Symptom { get; set; }
        public string Duration { get; set; }
        public string Consequence { get; set; }

        public ICollection<Therapy> Therapies { get; set; }
        public Disease()
        {
            Therapies = new List<Therapy>();
        }
    }
}
