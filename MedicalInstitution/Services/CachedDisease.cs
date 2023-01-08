using MedicalInstitution.Data;
using MedicalInstitution.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MedicalInstitution.Services
{
    public class CachedDisease : ICached<Disease>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Context _context;
        public CachedDisease(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IEnumerable<Disease> GetList()
        {
            return _context.Diseases.ToList();
        }
        public void AddList(string key)
        {
            IEnumerable<Disease> diseases = _context.Diseases.ToList();
            if (diseases != null)
            {
                _memoryCache.Set(key, diseases, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(264)
                });
            }
        }
        public IEnumerable<Disease> GetList(string key)
        {
            IEnumerable<Disease> diseases;
            if (!_memoryCache.TryGetValue(key, out diseases))
            {
                diseases = _context.Diseases.ToList();
                if (diseases != null)
                {
                    _memoryCache.Set(key, diseases, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(264)));
                }
            }
            return diseases;
        }
    }
}
