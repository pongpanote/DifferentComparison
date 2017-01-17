using System;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Assignment.RestService;

namespace Assignment.RestServiceStart
{
    class Program
    {
        private const string HOST_URI = "http://localhost:1234";

        static void Main(string[] args)
        {
            using (var webServiceHost = new WebServiceHost(typeof(Service), new Uri(HOST_URI)))
            {
                var serviceDebugBehavior = webServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
                serviceDebugBehavior.HttpHelpPageEnabled = false;

                webServiceHost.Open();

                Console.WriteLine("RestService is up and running\r\n" +
                                  "Hosting at '" + HOST_URI + "'\r\n\r\n" +
                                  "Press any key to quit. ");
                Console.ReadLine();

                webServiceHost.Close();
            }
        }
    }
}
