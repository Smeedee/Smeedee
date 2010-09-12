using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Smeedee.Tasks.Framework.TaskAttributes;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.Learning_Tests
{
    [TestFixture]
    class ReflectionTests
    {
        [Ignore]
        [Test]
        public void How_to_collect_all_items_tagged_with_the_TaskAttribute()
        {
            var tasks = GetTypesWithTaskAttribute(Assembly.LoadFrom(@"c:\dfdfd\fdf"));
            foreach (var task in tasks)
            {
                Console.WriteLine(task.FullName);
            }

            // 1) go to DB and find these:

            // "GitChangeSetTask" - googlecode.com - username, pass
            // "GitChangeSetTask" - github.com

            // 2) Scan assemblies and load 2 instances

            // 3) 
            
            tasks.Count().ShouldBe(2);
        }

        static IEnumerable<Type> GetTypesWithTaskAttribute(Assembly assembly)
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(TaskAttribute), true).Length > 0);
        }
    }
}
