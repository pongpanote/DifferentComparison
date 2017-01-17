using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using Assignment.RestService.BusinessLogic;
using Assignment.RestService.Objects.Json;

namespace Assignment.RestService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IService
    {
        private IBusinessLogic m_BusinessLogic;

        public Service()
            : this(new DataBusinessLogic())
        { }

        public Service(IBusinessLogic businessLogic)
        {
            m_BusinessLogic = businessLogic;
            ClearData();
        }

        private void ClearData()
        {
            m_BusinessLogic.ClearData();
        }

        public DiffResult DifferentCalculation(string id)
        {
            var context = WebOperationContext.Current;

            if (!m_BusinessLogic.IsReady(id))
            {
                context.OutgoingResponse.SetStatusAsNotFound();
                return null;
            }

            return m_BusinessLogic.DifferentCalculation(id);
        }

        public void PutMessage(string id, string relation, Stream bodyStream)
        {
            var context = WebOperationContext.Current;

            if (!m_BusinessLogic.StoreData(id, relation, bodyStream))
            {
                context.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
            }
            else
            {
                context.OutgoingResponse.SetStatusAsCreated(context.IncomingRequest.UriTemplateMatch.BaseUri);
            }
        }
    }
}
