using MedicalInstitution.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalInstitution.Middleware
{
    public class DBMiddleware
    {
        private readonly RequestDelegate _next;
        public DBMiddleware(RequestDelegate next)
        {
            // инициализация базы данных 
            _next = next;
        }
        public Task Invoke(HttpContext context)
        {
            if (!(context.Session.Keys.Contains("starting")))
            {
                ApplicationInitializer.Initialize(context).Wait();
                context.Session.SetString("starting", "Yes");
            }
            return _next.Invoke(context);
        }
    }
}
