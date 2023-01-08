using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalInstitution.Models
{
    public class Medician
    {
        public int MedicianId { get; set; }
        public string Name { get; set; }
        public string Indication { get; set; }
        public string Contraindicat { get; set; }
        public string Manufacturer { get; set; }
        public string Packaging { get; set; }
        public string Dasage { get; set; }
        public int CostMedicianeId { get; set; }
        public CostMediciane CostMediciane { get; set; }
        public ICollection<Therapy> Therapies { get; set; }
        public Medician()
        {
            Therapies = new List<Therapy>();
        }


    }
}
