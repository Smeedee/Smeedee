using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;
using Smeedee.Framework;

namespace Smeedee.DomainModel.NoSql
{
    /// <summary>
    /// Has many documents. 
    /// </summary>
    public class Collection 
    {
        public virtual string Name { get; set; }
        public virtual IList<Document> Documents { get; set; }
        public virtual NoSqlDatabase NoSqlDatabase { get; set; }

        public Collection()
        {
            Documents = new List<Document>();
        }


        public virtual void Insert(Document doc)
        {
            Guard.Requires<ArgumentNullException>(doc != null);

            doc.Collection = this;
            Documents.Add(doc);
        }

        public virtual void Delete(Document doc)
        {
            Guard.Requires<ArgumentNullException>(doc != null);
            Guard.Requires<ArgumentException>(Documents.Contains(doc));

            Documents.Remove(doc);
        }

        public static Collection Parse(string JSON)
        {
            var collection = new Collection();

            var array = JArray.Parse(JSON);
            foreach (var item in array)
            {
                collection.Insert(Document.Parse(item.ToString()));
            }

            return collection;
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
            return string.Format("{0}.{1}", 
                (this.NoSqlDatabase != null) ? this.NoSqlDatabase.Name : string.Empty, 
                Name).GetHashCode();
        }
    }
}
