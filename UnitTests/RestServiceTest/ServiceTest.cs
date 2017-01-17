using System;
using System.Net;
using Assignment.RestService.Objects.Converter;
using Assignment.RestService.Objects.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Assignment.RestServiceTest
{
    public class ServiceTest : BaseServiceTest
    {
        [Test]
        public void Get_Nothing_404NotFound()
        {
            try
            {
                var getRequest = CreatHttpWebRequest("v1/diff/1");
                var getResponse = (HttpWebResponse) getRequest.GetResponse();
                Console.WriteLine(getResponse.ToString());
            }
            catch (WebException exception)
            {
                Assert.AreEqual(exception.Message, "The remote server returned an error: (404) Not Found.");
            }
        }

        [Test]
        public void Equals()
        {
            var request = CreatHttpWebRequest("v1/diff/1/left", HttpMethod.PUT, JSON_CONTENT_TYPE, "{ \"data\" : \"AAAAAA==\" }");
            var response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            request = CreatHttpWebRequest("v1/diff/1/right", HttpMethod.PUT, JSON_CONTENT_TYPE, "{ \"data\" : \"AAAAAA==\" }");
            response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            request = CreatHttpWebRequest("v1/diff/1");
            response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = response.GetBodyAsString();
            var diffResult = JsonConvert.DeserializeObject<DiffResult>(body, new DiffsConverter());

            Assert.AreEqual(DiffResultType.Equals.ToString(), diffResult.diffResultType);
            Assert.IsNull(diffResult.diffs);
        }

        [Test]
        public void SizeDoNotMatch()
        {
            var request = CreatHttpWebRequest("v1/diff/1/left", HttpMethod.PUT, JSON_CONTENT_TYPE, "{ \"data\" : \"AAAAAA==\" }");
            var response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            request = CreatHttpWebRequest("v1/diff/1/right", HttpMethod.PUT, JSON_CONTENT_TYPE, "{ \"data\" : \"AAA=\" }");
            response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            request = CreatHttpWebRequest("v1/diff/1");
            response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = response.GetBodyAsString();
            var diffResult = JsonConvert.DeserializeObject<DiffResult>(body, new DiffsConverter());

            Assert.AreEqual(DiffResultType.SizeDoNotMatch.ToString(), diffResult.diffResultType);
            Assert.IsNull(diffResult.diffs);
        }

        [Test]
        public void ContentDoNotMatch()
        {
            var request = CreatHttpWebRequest("v1/diff/1/left", HttpMethod.PUT, JSON_CONTENT_TYPE, "{ \"data\" : \"AAAAAA==\" }");
            var response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            request = CreatHttpWebRequest("v1/diff/1/right", HttpMethod.PUT, JSON_CONTENT_TYPE, "{ \"data\" : \"AQABAQ==\" }");
            response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            request = CreatHttpWebRequest("v1/diff/1");
            response = (HttpWebResponse)request.GetResponse();
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = response.GetBodyAsString();
            var diffResult = JsonConvert.DeserializeObject<DiffResult>(body, new DiffsConverter());

            Assert.AreEqual(DiffResultType.ContentDoNotMatch.ToString(), diffResult.diffResultType);
            Assert.AreEqual(2, diffResult.diffs.Count);
        }
    }
}
