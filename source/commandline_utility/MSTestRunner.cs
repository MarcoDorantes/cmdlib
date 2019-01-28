using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using static System.Console;

namespace utility
{
    public class MSTestRunner
    {
        public System.IO.FileInfo assembly;
        public List<string> methods;
        public string[] exceptcategory;
        public void tests()
        {
            //Trace.Listeners.Clear();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Trace.AutoFlush = true;

            //assembly = new System.IO.FileInfo(@"..\..\..\..\..\..\AcceptanceTests\Main\Source\SqlWriterSpecs\bin\Release\SqlWriterSpecs.dll");

            if (assembly == null)
            {
                Escape.Write("$yellow|usage with -assembly:<file>\n");
                return;
            }
            else if (!assembly.Exists)
            {
                Escape.Write("$red|{0} not found\n\n$yellow|usage: -tests -assembly=<full.file.path>\n", assembly.FullName);
                return;
            }
            Escape.Write("Tests methods in assembly $cyan|{0}:\n", assembly.FullName);
            if (methods != null)
            {
                Escape.Write("\t$cyan|Looking for these methods:\n");
                methods.ForEach(m => Escape.Write("\t$cyan|{0}:\n", m));
                Escape.Write("\n");
            }

            var name = AssemblyName.GetAssemblyName(assembly.FullName);
            var mstest_container = Assembly.Load(name);
            int passed = 0, failed = 0, total = 0;
            foreach (Type testclass in mstest_container.GetTypes().Where((classtype) => SpecFilter.IsTestClass(classtype)))
            {
                Console.WriteLine("\tClass: {0}", testclass.FullName);
                object instance = Activator.CreateInstance(testclass);

                MethodInfo class_init = SpecFilter.GetMethod(testclass, "ClassInitializeAttribute");
                MethodInfo class_clean = SpecFilter.GetMethod(testclass, "ClassCleanupAttribute");
                MethodInfo init = SpecFilter.GetMethod(testclass, "TestInitializeAttribute");
                MethodInfo clean = SpecFilter.GetMethod(testclass, "TestCleanupAttribute");

                if (class_init != null) class_init.Invoke(instance, new object[] { null });
                foreach (var method in testclass.GetMethods().Where((m) => SpecFilter.IsTestMethod(m, methods, exceptcategory)))
                {
                    Console.Write("\t\tExecuting {0}", method.Name);
                    try
                    {
                        ++total;
                        if (init != null) init.Invoke(instance, null);
                        method.Invoke(instance, null);
                        if (clean != null) clean.Invoke(instance, null);
                        Escape.Write(" - $green|OK\n");
                        ++passed;
                    }
                    catch (Exception ex)
                    {
                        ++failed;
                        Escape.Write(" - $red|Failed$reset|: $red|{0}$reset|\n", innerest(ex));
                    }
                }
                if (class_clean != null) class_clean.Invoke(instance, null);

                foreach (var method in testclass.GetMethods().Where((m) => SpecFilter.IsExcludedTestMethod(m, methods, exceptcategory)))
                {
                    WriteLine("\t\tExcluded {0}", method.Name);
                }
            }
            foreach (Type testclass in mstest_container.GetTypes().Where((classtype) => SpecFilter.IsExcludedTestClass(classtype)))
            {
                WriteLine("\tClass: {0} - EXCLUDED", testclass.FullName);
            }
            Escape.Write("\n$cyan|Total: {0} $green|Passed: {1} $red|Failed: {2}\n", total, passed, failed);
        }
        private string innerest(Exception ex, int level = 0)
        {
            if (ex == null) return "";
            string msg = $"\n\t| [Level {level}] " + ex.Message + " (" + ex.GetType().FullName + ")";
            if (ex is TargetInvocationException) return innerest(ex.InnerException, ++level);
            return msg + innerest(ex.InnerException, ++level);
        }
    }

    public static class SpecFilter
    {
        public static bool IsTestClass(Type type)
        {
            object[] attrs = type.GetCustomAttributes(true);
            bool testclass = attrs.Any(T => T.GetType().Name == "TestClassAttribute");
            bool noIgnore = !attrs.Any(T => T.GetType().Name == "IgnoreAttribute");
            return testclass && noIgnore;
        }

        public static bool IsExcludedTestClass(Type type)
        {
            object[] attrs = type.GetCustomAttributes(true);
            bool testclass = attrs.Any(T => T.GetType().Name == "TestClassAttribute");
            bool ignore = attrs.Any(T => T.GetType().Name == "IgnoreAttribute");
            return testclass && ignore;
        }

        public static bool IsTestMethod(MethodInfo method, IEnumerable<string> included_methods, params string[] exclude_categories)
        {
            if (included_methods != null)
            {
                return included_methods.Any(m => string.Compare(m, method.Name, true) == 0);
            }

            if (exclude_categories == null) exclude_categories = new string[0];
            object[] attrs = method.GetCustomAttributes(true);

            bool testmethod = attrs.Any(T => T.GetType().Name == "TestMethodAttribute");
            return
                testmethod &&
                !attrs.Any(T => T.GetType().Name == "IgnoreAttribute") &&
                !attrs.Any(T => T.GetType().Name == "TestCategoryAttribute" && hasListPropertyValue(T, "TestCategories", exclude_categories));
        }

        public static bool IsExcludedTestMethod(MethodInfo method, IEnumerable<string> included_methods, params string[] exclude_categories)
        {
            if (included_methods != null)
            {
                return !included_methods.Any(m => string.Compare(m, method.Name, true) == 0);
            }

            if (exclude_categories == null) exclude_categories = new string[0];
            object[] attrs = method.GetCustomAttributes(true);

            bool testmethod = attrs.Any(T => T.GetType().Name == "TestMethodAttribute");
            return
                testmethod &&
                attrs.Any(T => T.GetType().Name == "IgnoreAttribute") &&
                attrs.Any(T => T.GetType().Name == "TestCategoryAttribute" && hasListPropertyValue(T, "TestCategories", exclude_categories));
        }

        public static MethodInfo GetMethod(Type testclass, string attributename)
        {
            return testclass.GetMethods().Where((m) => hasAttribute(m, attributename)).FirstOrDefault();
        }

        private static bool hasAttribute(MethodInfo method, string attributename)
        {
            return method.GetCustomAttributes(true).Any(T => string.Compare(T.GetType().Name, attributename, true) == 0);
        }

        private static bool hasListPropertyValue(object attribute_instance, string propertyname, string[] excluded_propertyvalues)
        {
            Type type = attribute_instance.GetType();
            object value = type.InvokeMember(propertyname, BindingFlags.GetProperty, null, attribute_instance, null);
            var list = value as IList<string>;
            if (list != null)
                return list.Any(category => excluded_propertyvalues.Any(excluded => string.Compare(category, excluded, true) == 0));
            else
                return false;
        }
    }
}