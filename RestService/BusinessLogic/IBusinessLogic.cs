using System.IO;
using Assignment.RestService.Objects.Json;

namespace Assignment.RestService.BusinessLogic
{
    public interface IBusinessLogic
    {
        bool IsReady(string id);
        bool StoreData(string id, string relation, Stream bodyStream);
        void ClearData();
        DiffResult DifferentCalculation(string id);
    }
}
