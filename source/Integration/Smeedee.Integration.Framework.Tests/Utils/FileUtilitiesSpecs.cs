using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Smeedee.Integration.Framework.Utils;
using Smeedee.Tests;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Framework.Tests.Utils
{
    [TestFixture]
    class FileUtilitiesSpecs : Shared
    {

        [Test]
        public void assure_it_fetches_all_classes_implementing_a_specific_interface()
        {
            Given("three classes of type IDummyType exists");

            When(it_fetches_classes);

            Then("all classes which implements the interface should be found", () =>
            {
                types.Where(t => t == typeof (DummyClass1)).Count().ShouldBe(1);
                types.Where(t => t == typeof(DummyClass2)).Count().ShouldBe(1);
            });
        }

        [Test]
        public void assure_it_fetches_all_subclasses_from_a_base_class()
        {
            Given("a type which inherits from a base class");

            When("it fetches classes", () =>
            {
                types = fileUtilities.FindImplementationsOrSubclassesOf<DummyTypeBase>(Directory.GetCurrentDirectory());
            });

            Then("all classes which inherits from base class should be found", () =>
            {
                types.Where(t => t == typeof(DummyClass1)).Count().ShouldBe(1);
            });
        }

        [Test]
        public void assure_it_fetches_all_subclasses_from_an_abstract_base_class()
        {
            Given("a type which inherits from an abstract base class");

            When("it fetches classes", () =>
            {
                types = fileUtilities.FindImplementationsOrSubclassesOf<AbstractDummyType>(Directory.GetCurrentDirectory());
            });

            Then("all classes which inherits from base class should be found", () =>
            {
                types.Where(t => t == typeof(DummyClass2)).Count().ShouldBe(1);
            });
        }

        [Test]
        public void assure_does_not_fetch_structs()
        {
            Given("two structs of type IDummyType and three classes of type IDummyType exists");

            When(it_fetches_classes);

            Then("it should not return the structs", () => types.Count().ShouldNotBe(4));
        }

        [Test]
        public void assure_loads_files_from_folder()
        {
            Given(there_is_one_dll_in_our_folder);

            When("we load dlls from the folder");

            Then(only_one_dll_file_is_loaded);
        }

        [Test]
        public void assure_loads_only_dlls_from_folder()
        {
            Given(there_is_one_dll_and_other_files_in_our_folder);

            When("we load dlls from the folder");

            Then(only_one_dll_file_is_loaded);
        }
    }

    public class Shared : SmeedeeScenarioTestClass
    {
        private static string folderName = @"c:\myTestFolder123XXYYFEWFSAA_SA";
        protected static IEnumerable<Type> types;
        protected static IFileUtilities fileUtilities;


        [SetUp]
        public void SetUp()
        {
            types = new List<Type>();
            fileUtilities = new FileUtilities();
        }

        [TearDown]
        public void TearDown()
        {
            if (!Directory.Exists(folderName))
                return;
            foreach (var file in Directory.EnumerateFiles(folderName))
            {
                File.Delete(file);
            }
            Directory.Delete(folderName);

            
        }

        public static void AddEmptyFilesToFolder(params string[] files)
        {
            Directory.CreateDirectory(folderName);
            foreach (var file in files)
            {
                TextWriter tw = new StreamWriter(folderName + "\\" + file);
                tw.WriteLine("saf");
                tw.Close();
            }
        }

        protected When it_fetches_classes = () =>
        {
            types = fileUtilities.FindImplementationsOrSubclassesOf<IDummyType>(Directory.GetCurrentDirectory());
        };


        public Then only_one_dll_file_is_loaded = () =>
        {
            fileUtilities.GetDLLs(folderName).Count().ShouldBe(1);
        };


        public Context there_is_one_dll_in_our_folder = () =>
        {
            AddEmptyFilesToFolder("bla.dll");
        };

        public Context there_is_one_dll_and_other_files_in_our_folder = () =>
        {
            AddEmptyFilesToFolder("bla.dll", "h.dll.txt", "blah.jpeg");
        };

    }

    public interface IDummyType
    {
    }

    public class DummyClass1 : DummyTypeBase, IDummyType
    {
        
    }

    public class DummyClass2 : AbstractDummyType, IDummyType
    {
        
    }

    public struct DummyStruct1 : IDummyType
    {
        
    }

    public struct DummyStruct2 : IDummyType
    {
        
    }

    public class DummyTypeBase
    {
        
    }

    public abstract class AbstractDummyType
    {
        
    }

}
