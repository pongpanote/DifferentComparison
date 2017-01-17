using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Assignment.RestService.Objects.Json;
using Assignment.RestService.Properties;

namespace Assignment.RestService
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet(UriTemplate = Configuration.URI_PREFIX + "{id}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DiffResult DifferentCalculation(string id);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = Configuration.URI_PREFIX + "{id}/{relation}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        void PutMessage(string id, string relation, Stream bodyStream);
    }
}
