using MedicalInstitution.Data;
using MedicalInstitution.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MedicalInstitution.Services
{
    public class CachedTherapy : ICached<Therapy>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Context _context;
        public CachedTherapy(Context context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }
        public IEnumerable<Therapy> GetList()
        {
            return _context.Therapies.ToList();
        }
        public void AddList(string key)
        {
            IEnumerable<Therapy> therapies = _context.Therapies.ToList();
            if (therapies != null)
            {
                _memoryCache.Set(key, therapies, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(264)
                });
            }
        }
        public IEnumerable<Therapy> GetList(string key)
        {
            IEnumerable<Therapy> therapies;
            if (!_memoryCache.TryGetValue(key, out therapies))
            {
                therapies = _context.Therapies.ToList();
                if (therapies != null)
                {
                    _memoryCache.Set(key, therapies, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(264)));
                }
            }
            return therapies;
        }
    }
}
