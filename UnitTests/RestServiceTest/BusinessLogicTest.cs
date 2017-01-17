using System;
using System.Collections.Generic;
using Assignment.RestService.BusinessLogic;
using Assignment.RestService.BusinessLogic.Objects;
using Assignment.RestService.Objects.Converter;
using Assignment.RestService.Objects.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Assignment.RestServiceTest
{
    public class BusinessLogicTest
    {
        private DataBusinessLogic m_dataBusinessLogic;
        private Dictionary<string, Data> m_Data;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            m_dataBusinessLogic = new DataBusinessLogic();
            RefreshData();
        }

        [SetUp]
        public void SetUp()
        {
            m_dataBusinessLogic.ClearData();
        }

        [Test]
        public void GetJsonData()
        {
            var data = TestHelpers.RunStaticMethod(typeof(DataBusinessLogic), "GetJsonData", "{ \"data\": \"AAAAAA==\"}");
            Assert.AreEqual("AAAAAA==", data);
        }

        [TestCase("{ \"data\": \"AAAAAA==\" }")]
        [TestCase("{ \"data\": \"123456==\" }")]
        public void ValidInput(string input)
        {
            var result = TestHelpers.RunInstanceMethod(m_dataBusinessLogic, "ValidInput", input);
            Assert.AreEqual(true, result);
        }

        [TestCase("{ \"data\": \"AAAAAA==\"")]
        [TestCase("{ \"data\": \"AAAAAA==\"")]
        [TestCase("{ \"data\": \"AAAA=\" }")]
        [TestCase("")]
        [TestCase("{ \"data\": null }")]
        public void InValidInput(string input)
        {
            var result = TestHelpers.RunInstanceMethod(m_dataBusinessLogic, "ValidInput", input);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void StoreData_CreateNewData()
        {
            StoreData("1", "left", "AAAAAA==");

            Assert.AreEqual("AAAAAA==", m_Data["1"].DataLeft.data);
        }

        [Test]
        public void StoreData_ModifyData()
        {
            StoreData("2", "left", "AAAAAA==");
            StoreData("2", "right", "BBBBBB==");

            Assert.AreEqual("AAAAAA==", m_Data["2"].DataLeft.data);
            Assert.AreEqual("BBBBBB==", m_Data["2"].DataRight.data);

            StoreData("2", "left", "CCCCCC==");

            Assert.AreEqual("CCCCCC==", m_Data["2"].DataLeft.data);
            Assert.AreEqual("BBBBBB==", m_Data["2"].DataRight.data);
        }

        [Test]
        public void DifferentCalculation_ModifyData()
        {
            StoreData("2", "left", "AAAAAA==");
            StoreData("2", "right", "BBBBBB==");

            var diffResult = TestHelpers.RunInstanceMethod(m_dataBusinessLogic, "DifferentCalculation", "2");
            Assert.IsNotNull(diffResult);
        }

        [Test]
        public void FindDifferent_Equals()
        {
            var diffs = FindDifferent("AAAAAA==", "AAAAAA==");

            Assert.AreEqual(0, diffs.Count);
        }

        [Test]
        public void FindDifferent_NotEquals()
        {
            var diffs = FindDifferent("AAAAAA==", "AQABAQ==");

            Assert.AreEqual(2, diffs.Count);
        }

        private DiffResult CreateDiffResult()
        {
            return new DiffResult
            {
                diffResultType = DiffResultType.SizeDoNotMatch.ToString()
            };
        }

        private DiffResult CreateDiffResult_WithDiffs()
        {
            return new DiffResult
            {
                diffResultType = DiffResultType.ContentDoNotMatch.ToString(),
                diffs = new List<Diffs> { new Diffs { offset = 0, length = 1 }, new Diffs { offset = 2, length = 2 } }
            };
        }
        
        private static string SampleDiffResultWithDiffs
        {
            get
            {
                return "{\"diffResultType\":\"ContentDoNotMatch\",\"diffs\":[{\"offset\": 0,\"length\": 1},{\"offset\": 2,\"length\": 2}]}";
            }
        }

        [Test]
        public void DiffResultConverter_SerializeObject()
        {
            var diffResult = CreateDiffResult();

            var diffResultString = JsonConvert.SerializeObject(diffResult, new DiffsConverter());
            Assert.AreEqual("{\"diffResultType\":\"SizeDoNotMatch\",\"diffs\":null}", diffResultString);
        }

        [Test]
        public void DiffsConverter_DeSerializeObject()
        {
            var diffResult = JsonConvert.DeserializeObject<DiffResult>(SampleDiffResultWithDiffs, new DiffsConverter());

            Assert.IsNotNull(diffResult);

            Assert.AreEqual("ContentDoNotMatch", diffResult.diffResultType);
            Assert.AreEqual(2, diffResult.diffs.Count);

            Assert.AreEqual(0, diffResult.diffs[0].offset);
            Assert.AreEqual(1, diffResult.diffs[0].length);

            Assert.AreEqual(2, diffResult.diffs[1].offset);
            Assert.AreEqual(2, diffResult.diffs[1].length);
        }


        [Test]
        public void DiffsConverter_DeSerializeObject_NoDiffs()
        {
            var diffResult = JsonConvert.DeserializeObject<DiffResult>("{\"diffResultType\":\"SizeDoNotMatch\"}", new DiffsConverter());

            Assert.IsNotNull(diffResult);

            Assert.AreEqual("SizeDoNotMatch", diffResult.diffResultType);
            Assert.AreEqual(null, diffResult.diffs);
        }

        [Test]
        public void DiffsConverter_DeSerializeObject_OnlyDiffs()
        {
            var diffs = JsonConvert.DeserializeObject<Diffs[]>("[{\"offset\": 0,\"length\": 1},{\"offset\": 2,\"length\": 2}]", new DiffsConverter());

            Assert.IsNotNull(diffs);
            
            Assert.AreEqual(2, diffs.Length);

            Assert.AreEqual(0, diffs[0].offset);
            Assert.AreEqual(1, diffs[0].length);

            Assert.AreEqual(2, diffs[1].offset);
            Assert.AreEqual(2, diffs[1].length);
        }

        private List<Diffs> FindDifferent(string left, string right)
        {
            return (List<Diffs>)TestHelpers.RunInstanceMethod(m_dataBusinessLogic, "FindDifferent", Convert.FromBase64String(left), Convert.FromBase64String(right));
        }

        private void StoreData(string id, string relation, string dataValue)
        {
            var str = "{ \"data\": \"" + dataValue + "\" }";
            m_dataBusinessLogic.StoreData(id, relation, str.ConvertToStream());

            RefreshData();
        }

        private void RefreshData()
        {
            m_Data = TestHelpers.GetMemberValue(m_dataBusinessLogic, "m_Data") as Dictionary<string, Data>;
        }
    }
}
