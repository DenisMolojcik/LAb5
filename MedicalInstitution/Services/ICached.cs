using System.Collections;
using System.Collections.Generic;

namespace MedicalInstitution.Services
{
    public interface ICached<T>
    {
        public IEnumerable<T> GetList();
        public void AddList(string key);
        public IEnumerable<T> GetList(string key);
    }
}
