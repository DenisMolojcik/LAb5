using MedicalInstitution.Data;
using MedicalInstitution.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MedicalInstitution.Services
{
    public class CachedPatient : ICached<Patient>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Context _context;
        public CachedPatient(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IEnumerable<Patient> GetList()
        {
            return _context.Patients.ToList();
        }
        public void AddList(string key)
        {
            IEnumerable<Patient> patients = _context.Patients.ToList();
            if (patients != null)
            {
                _memoryCache.Set(key, patients, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(264)
                });
            }
        }
        public IEnumerable<Patient> GetList(string key)
        {
            IEnumerable<Patient> patients;
            if (!_memoryCache.TryGetValue(key, out patients))
            {
                patients = _context.Patients.ToList();
                if (patients != null)
                {
                    _memoryCache.Set(key, patients, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(264)));
                }
            }
            return patients;
        }
    }
}
