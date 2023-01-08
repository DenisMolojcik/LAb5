using MedicalInstitution.Data;
using MedicalInstitution.Models;
using MedicalInstitution.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;
using System.Linq;

public class CachedUser : ICached<AppUser>
{
    private readonly IMemoryCache _memoryCache;
    private readonly ApplicationDbContext _context;
    public CachedUser(ApplicationDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }
    public IEnumerable<AppUser> GetList()
    {
        return _context.Users.ToList();
    }
    public void AddList(string key)
    {
        IEnumerable<AppUser> users = _context.Users.ToList();
        if (users != null)
        {
            _memoryCache.Set(key, users, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(264)
            });
        }
    }
    public IEnumerable<AppUser> GetList(string key)
    {
        IEnumerable<AppUser> users;
        if (!_memoryCache.TryGetValue(key, out users))
        {
            users = _context.Users.ToList();
            if (users != null)
            {
                _memoryCache.Set(key, users, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(264)));
            }
        }
        return users;
    }
}