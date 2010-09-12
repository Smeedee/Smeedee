using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smeedee.Framework;

namespace Smeedee.DomainModel.NoSql
{
    public class Document
    {
        private JObject jsonObject = null;
        private JObject JSONObject
        {
            get
            {
                if (jsonObject == null)
                    jsonObject = JObject.Parse(JSON);

                return jsonObject;
            }
        }

        public virtual Guid Id { get; set; }
        public virtual string JSON { get; set; }
        public virtual Collection Collection { get; set; }

        public virtual JToken this[string key]
        {
            get
            {
                var obj = JSONObject[key];
                return obj;
            }
            set
            {
                JSONObject[key] = value;
                JSON = JSONObject.ToString();
            }
        }

        public Document()
        {
            JSON = "{}";
            Id = Guid.NewGuid();
        }

        public virtual T ToObject<T>()
        {
            return JsonConvert.DeserializeObject<T>(JSON);
        }

        public static Document Parse(string JSON)
        {
            return new Document()
            {
                JSON = JSON
            };
        }

        public static Document FromObject(Object obj)
        {
            Guard.Requires<ArgumentException>(!(obj is IEnumerable), "Document doesn't support list objects, must be an plain old object");

            var newDoc = new Document();
            newDoc.jsonObject = JObject.FromObject(obj);
            newDoc.JSON = newDoc.jsonObject.ToString();

            return newDoc;
        }

        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return string.Format("{0}.{1}.{2}", 
                (this.Collection != null && this.Collection.NoSqlDatabase != null) ? this.Collection.NoSqlDatabase.Name : string.Empty,
                (this.Collection != null) ? this.Collection.Name : string.Empty, 
                Id).GetHashCode();
        }
    }
}
