using MedicalInstitution.Data;
using MedicalInstitution.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MedicalInstitution.Services
{
    public class CachedDoctor : ICached<Doctor>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Context _context;
        public CachedDoctor(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IEnumerable<Doctor> GetList()
        {
            return _context.Doctors.ToList();
        }
        public void AddList(string key)
        {
            IEnumerable<Doctor> doctors = _context.Doctors.ToList();
            if (doctors != null)
            {
                _memoryCache.Set(key, doctors, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(264)
                });
            }
        }
        public IEnumerable<Doctor> GetList(string key)
        {
            IEnumerable<Doctor> doctors;
            if (!_memoryCache.TryGetValue(key, out doctors))
            {
                doctors = _context.Doctors.ToList();
                if (doctors != null)
                {
                    _memoryCache.Set(key, doctors, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(264)));
                }
            }
            return doctors;
        }
    }
}
