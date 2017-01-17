using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assignment.RestService.BusinessLogic.Objects;
using Assignment.RestService.Objects.Json;
using Newtonsoft.Json.Linq;

namespace Assignment.RestService.BusinessLogic
{
    public class DataBusinessLogic : IBusinessLogic
    {
        private Dictionary<string, Data> m_Data;
        private string[] supportedRelation = { "left", "right" };

        public DataBusinessLogic()
        {
            m_Data = new Dictionary<string, Data>();
        }

        public bool IsReady(string id)
        {
            return IsExists(id) && HaveValue(id);
        }

        private bool IsExists(string id)
        {
            return m_Data.ContainsKey(id);
        }

        private bool HaveValue(string id)
        {
            return m_Data[id].DataLeft != null && m_Data[id].DataRight != null;
        }

        public void ClearData()
        {
            m_Data.Clear();
        }

        public bool StoreData(string id, string relation, Stream bodyStream)
        {
            var body = bodyStream.ConvertToString();
            var internalReferenceObject = new InternalReferenceObject { Id = id, Relation = relation, Body = body };

            if (!supportedRelation.Contains(relation) || !ValidInput(body))
            {
                return false;
            }

            if (IsExists(id))
            {
                ModifyData(internalReferenceObject);
            }
            else
            {
                CreateNewData(internalReferenceObject);
            }

            return true;
        }

        private bool ValidInput(string body)
        {
            return body.IsValidJson() && GetJsonData(body).IsBase64Encoded();
        }

        private static string GetJsonData(string body)
        {
            var jObject = JToken.Parse(body);
            var data = jObject["data"].Value<string>();

            return data;
        }

        private void ModifyData(InternalReferenceObject internalReferenceObject)
        {
            var data = m_Data[internalReferenceObject.Id];

            SetDataMessage(internalReferenceObject, data);

            m_Data.Remove(internalReferenceObject.Id);
            m_Data.Add(internalReferenceObject.Id, data);
        }

        private void CreateNewData(InternalReferenceObject internalReferenceObject)
        {
            var data = new Data();

            SetDataMessage(internalReferenceObject, data);

            m_Data.Add(internalReferenceObject.Id, data);
        }

        private static void SetDataMessage(InternalReferenceObject internalReferenceObject, Data data)
        {
            var message = new Message { data = GetJsonData(internalReferenceObject.Body) };

            switch (internalReferenceObject.Relation)
            {
                case "left":
                    data.DataLeft = message;
                    break;
                case "right":
                    data.DataRight = message;
                    break;
            }
        }

        public DiffResult DifferentCalculation(string id)
        {
            var data = m_Data[id];
            var dataLeft = Convert.FromBase64String(data.DataLeft.data);
            var dataRight = Convert.FromBase64String(data.DataRight.data);

            if (dataLeft.Length != dataRight.Length)
            {
                return new DiffResult { diffResultType = DiffResultType.SizeDoNotMatch.ToString() };
            }

            var diffs = FindDifferent(dataLeft, dataRight);

            return diffs.Count == 0 ?  new DiffResult { diffResultType = DiffResultType.Equals.ToString() }
                : new DiffResult { diffResultType = DiffResultType.ContentDoNotMatch.ToString(), diffs = diffs };
        }

        private List<Diffs> FindDifferent(byte[] dataLeft, byte[] dataRight)
        {
            var diffs = new List<Diffs>();

            var diffFound = false;
            var diffOffset = 0;
            var diffLength = 0;

            for (var i = 0; i < dataLeft.Length; i++)
            {
                if (dataLeft[i] != dataRight[i])
                {
                    if (diffFound)
                    {
                        diffLength += 1;
                    }
                    else
                    {
                        diffFound = true;
                        diffOffset = i;
                        diffLength = 1;
                    }
                }
                else if (diffFound)
                {
                    diffs.Add(new Diffs{offset = diffOffset, length = diffLength});
                    diffFound = false;
                }
            }

            if (diffFound)
            {
                diffs.Add(new Diffs { offset = diffOffset, length = diffLength });
            }

            return diffs;
        }

        private class InternalReferenceObject
        {
            internal string Id;
            internal string Relation;
            internal string Body;
        }
    }
}
