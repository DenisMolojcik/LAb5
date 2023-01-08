using System;

namespace MedicalInstitution.ViewsModels
{
    public class MedicianView
    {
        public int MedicianId { get; set; }
        public string Name { get; set; }
        public string Indication { get; set; }
        public string Contraindicat { get; set; }
        public string Manufacturer { get; set; }
        public string Packaging { get; set; }
        public string Dasage { get; set; }
        public int Cost { get; set; }
        public DateTime Date { get; set; }
    }
}
