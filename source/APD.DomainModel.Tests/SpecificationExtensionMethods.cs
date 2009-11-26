using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;


namespace APD.DomainModel.Tests
{
    //TODO: extract this functionality out to TinyBDD specifications
    public static class SpecificationExtensionMethods
    {
        public static void ShouldHave(this IEnumerable list, object obj)
        {
            foreach (var item in list)
                if (item == obj)
                    return;

            Assert.Fail("Object not found in list");
        }
    }
}
