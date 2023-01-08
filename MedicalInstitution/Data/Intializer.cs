using MedicalInstitution.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicalInstitution.Data
{
    public static class Intializer
    {
        public static void Intialize(Context context)
        {
            context.Database.EnsureCreated();
            if (context.Therapies.Any())
            {
                return;
            }
            List<string> Name = new List<string> { "Артемий", "Александр", "Николай", "Игорь", "Денис" };
            List<string> Surname = new List<string> { "Рябый", "Поминдеев", "Ялченко", "Шух", "Михайловский", "Молойчик" };
            List<string> Lastname = new List<string> { "Викторович", "Владимирович", "Юрьевич", "Николаевич" };
            List<string> Gender = new List<string> { "Мужской", "Женский" };
            for (int id = 1; id <= 15; id++)
            {
                string manufacturer = "manufacturer" + new Random().Next(100).ToString();
                int cost = new Random().Next(100);
                DateTime date = DateTime.Now.AddDays(new Random().Next(1000));
                context.CostMedicianes.Add(new CostMediciane { Manufacturer = manufacturer, Cost = cost, Date = date});
            }
            context.SaveChanges();
            for (int id = 1; id <= 100; id++)
            {
                string medician = "medician" + new Random().Next(100).ToString();
                string indication = "indication" + new Random().Next(100).ToString();
                string contraindicat = "contraindicat" + new Random().Next(100).ToString();
                string manufacturer = "manufacturer" + new Random().Next(100).ToString();
                string packaging = "packaging" + new Random().Next(100).ToString();
                string dasage = "dasage" + new Random().Next(100).ToString();
                int medicianCost = new Random().Next(1, 15);
                context.Medicianes.Add(new Medician { Name = medician, Indication = indication, Contraindicat = contraindicat, Manufacturer = manufacturer, Packaging = packaging, Dasage = dasage, CostMedicianeId = medicianCost });
            }
            context.SaveChanges();
            for (int id = 1; id <= 100; id++)
            {
                string disease = "disease" + new Random().Next(100).ToString();
                string symptom = "symptom" + new Random().Next(100).ToString();
                string duration = "duration" + new Random().Next(100).ToString();
                string consequence = "consequence" + new Random().Next(100).ToString();
                context.Diseases.Add(new Disease { Name = disease, Symptom = symptom, Duration = duration, Consequence = consequence });
            }
            for (int id = 1; id <= 100; id++)
            {
                string name = Name[new Random().Next(Name.Count)] + " " + Surname[new Random().Next(Surname.Count)];
                string gender = Gender[new Random().Next(Gender.Count)];
                string position = "position" + new Random().Next(100).ToString();
                int age = new Random().Next(1, 92);
                context.Doctors.Add(new Doctor { Name = name, Gender = gender, Position = position, Age = age });
            }
            for (int id = 1; id <= 100; id++)
            {
                string name = Name[new Random().Next(Name.Count)];
                string surname = Surname[new Random().Next(Surname.Count)];
                string lastname = Lastname[new Random().Next(Lastname.Count)];
                string gender = Gender[new Random().Next(Gender.Count)];
                int age = new Random().Next(1, 92);
                string phone = "phone" + new Random().Next(100).ToString();
                DateTime date = DateTime.Now.AddDays(new Random().Next(1000));
                DateTime date1 = DateTime.Now.AddDays(new Random().Next(1000));
                string diagnos = "diagnos" + new Random().Next(100).ToString();
                string department = "department" + new Random().Next(100).ToString();
                string resultTreatment = "resultTreatment" + new Random().Next(100).ToString();
                context.Patients.Add(new Patient { Name = name, Surname = surname, Lastname = lastname, Gender = gender, Age = age, PhoneNumber = phone, DateDischarge = date, DateHospitalisation = date1, Diagnos = diagnos, Department = department, ResultTreatment = resultTreatment });
            }
            context.SaveChanges();
            for (int id = 1; id <= 100; id++)
            {
                int id1 = new Random().Next(1, 100);
                int id2 = new Random().Next(1, 100);
                int id3 = new Random().Next(1, 100);
                int id4 = new Random().Next(1, 100);
                DateTime date = DateTime.Now.AddDays(new Random().Next(1000));
                context.Therapies.Add(new Therapy { DiseaseId = id1, DoctorId = id2, MedicianId = id3, PatientId = id4, Date = date });
            }
            context.SaveChanges();
        }
    }
}
