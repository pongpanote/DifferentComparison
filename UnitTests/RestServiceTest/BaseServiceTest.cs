using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using Assignment.RestService;
using NUnit.Framework;

namespace Assignment.RestServiceTest
{
    public abstract class BaseServiceTest
    {
        private ServiceHost m_ServiceHost;
        private Service m_Service;
        private const string HOST_URI = "http://localhost:1234";
        private const string TEXT_CONTENT_TYPE = "text/plain; charset=utf-8";
        internal const string XML_CONTENT_TYPE = "text/xml; charset=utf-8";
        internal const string JSON_CONTENT_TYPE = "application/json";

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp()
        {
            m_Service = new Service();
            m_ServiceHost = new WebServiceHost(m_Service, new Uri(HOST_URI));

            var serviceDebugBehavior = m_ServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            serviceDebugBehavior.HttpHelpPageEnabled = false;

            m_ServiceHost.Open();
        }

        [SetUp]
        public virtual void SetUp()
        {
            TestHelpers.RunInstanceMethod(m_Service, "ClearData", null);
        }

        [TearDown]
        public virtual void TearDown()
        { }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown()
        {
            m_ServiceHost.Close();
        }

        protected HttpWebRequest CreatHttpWebRequest(string uri, HttpMethod method = HttpMethod.GET, string contentType = TEXT_CONTENT_TYPE, string body = null)
        {
            var request = (HttpWebRequest) WebRequest.Create(HOST_URI + "/" + uri);

            request.ContentType = contentType;
            request.KeepAlive = false;
            request.Timeout = 5000;
            request.Method = method.ToString();

            if (body != null)
            {
                SetRequestBody(request, body);
            }

            return request;
        }

        private void SetRequestBody(WebRequest request, string body)
        {
            var dataByte = Encoding.UTF8.GetBytes(body);
            request.ContentLength = dataByte.Length;

            var requestStream = request.GetRequestStream();
            requestStream.Write(dataByte, 0, dataByte.Length);
        }
    }

    public enum HttpMethod
    { 
        GET, 
        POST, 
        PUT, 
        DELETE
    }
}