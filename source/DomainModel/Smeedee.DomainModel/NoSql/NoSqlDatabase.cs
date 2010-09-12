using System.Collections.Generic;
using System.Dynamic;

namespace Smeedee.DomainModel.NoSql
{
    public class NoSqlDatabase : DynamicObject
    {
        public NoSqlDatabase()
        {
            Collections = new Dictionary<string, Collection>();
        }

        public virtual string Name { get; set; }

        /// <summary>
        /// Collections in NoSql are roughly equals to Tables in relational databases.
        /// </summary>
        public virtual IDictionary<string, Collection> Collections { get; set; }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetCollection(binder.Name);

            return true;
        }

        public virtual Collection GetCollection(string name)
        {
            if (Collections.ContainsKey(name))
            {
                return  Collections[name];
            }
            else
            {
                Collections.Add(name, new Collection()
                {
                    Name = name,
                    NoSqlDatabase = this
                });
                return Collections[name];
            }
        }
    }
}
