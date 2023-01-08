using MedicalInstitution.Data;
using MedicalInstitution.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicalInstitution.Services
{
    public class CachedMedician : ICached<Medician>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Context _context;
        public CachedMedician(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IEnumerable<Medician> GetList()
        {
            return _context.Medicianes.ToList();
        }
        public void AddList(string key)
        {
            IEnumerable<Medician> medicians = _context.Medicianes.ToList();
            if (medicians != null)
            {
                _memoryCache.Set(key, medicians, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(264)
                });
            }
        }
        public IEnumerable<Medician> GetList(string key)
        {
            IEnumerable<Medician> medicians;
            if (!_memoryCache.TryGetValue(key, out medicians))
            {
                medicians = _context.Medicianes.ToList();
                if (medicians != null)
                {
                    _memoryCache.Set(key, medicians, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(264)));
                }
            }
            return medicians;
        }
    }
}
