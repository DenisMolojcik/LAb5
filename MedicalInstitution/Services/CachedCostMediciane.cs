using MedicalInstitution.Data;
using MedicalInstitution.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MedicalInstitution.Services
{
    public class CachedCostMediciane : ICached<CostMediciane>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Context _context;
        public CachedCostMediciane(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IEnumerable<CostMediciane> GetList()
        {
            return _context.CostMedicianes.ToList();
        }
        public void AddList(string key)
        {
            IEnumerable<CostMediciane> medicianes = _context.CostMedicianes.ToList();
            if (medicianes != null)
            {
                _memoryCache.Set(key, medicianes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(264)
                });
            }
        }
        public IEnumerable<CostMediciane> GetList(string key)
        {
            IEnumerable<CostMediciane> medicianes;
            if (!_memoryCache.TryGetValue(key, out medicianes))
            {
                medicianes = _context.CostMedicianes.ToList();
                if (medicianes != null)
                {
                    _memoryCache.Set(key, medicianes, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(264)));
                }
            }
            return medicianes;
        }
    }
}
