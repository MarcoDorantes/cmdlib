using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//[assembly: System.Reflection.AssemblyVersion("3.3.0.109")]
//[assembly: System.Reflection.AssemblyFileVersion("3.3.0.109")]

namespace ContractSpec
{
    [TestClass]
    public class SwitchSpec
    {
        #region Regex
        static readonly string Switch_Regex;

        static SwitchSpec()
        {
            Switch_Regex = utility.Switch.regexpr;
        }

        [TestMethod]
        public void match1()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/n=");
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("n", opt.Value);
        }

        [TestMethod]
        public void NamedNonValue()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abcdef");
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abcdef", opt.Value);
        }

        [TestMethod]
        public void NamedSwitch()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abc-");
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abc", opt.Value);
            System.Text.RegularExpressions.Group turn = m.Groups["turn"];
            Assert.IsTrue(turn.Success);
            Assert.AreEqual<string>("-", turn.Value);
        }

        [TestMethod]
        public void NamedSwitchOff()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abc");
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abc", opt.Value);
            System.Text.RegularExpressions.Group turn = m.Groups["turn"];
            Assert.IsFalse(turn.Success);
            Assert.AreEqual<string>(string.Empty, turn.Value);
        }

        [TestMethod]
        public void NamedSwitchNoValue()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abc+");
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abc", opt.Value);
            System.Text.RegularExpressions.Group turn = m.Groups["turn"];
            Assert.IsTrue(turn.Success);
            Assert.AreEqual<string>("+", turn.Value);
            System.Text.RegularExpressions.Group val = m.Groups["value"];
            Assert.IsFalse(val.Success);
            Assert.AreEqual<string>(string.Empty, val.Value);
        }

        [TestMethod]
        public void NamedEqualValue()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abc=def");
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abc", opt.Value);
            System.Text.RegularExpressions.Group val = m.Groups["value"];
            Assert.IsTrue(val.Success);
            Assert.AreEqual<string>("def", val.Value);
        }

        [TestMethod]
        public void NamedSwitchValue()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abc:val");
            Assert.IsTrue(m.Success);
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abc", opt.Value);
            System.Text.RegularExpressions.Group turn = m.Groups["turn"];
            Assert.IsFalse(turn.Success);
            System.Text.RegularExpressions.Group val = m.Groups["value"];
            Assert.IsTrue(val.Success);
            Assert.AreEqual<string>("val", val.Value);
        }

        [TestMethod]
        public void NamedSwitchSpaceInValue()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abc:string value");
            Assert.IsTrue(m.Success);
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abc", opt.Value);
            System.Text.RegularExpressions.Group turn = m.Groups["turn"];
            Assert.IsFalse(turn.Success);
            System.Text.RegularExpressions.Group val = m.Groups["value"];
            Assert.IsTrue(val.Success);
            Assert.AreEqual<string>("string value", val.Value);
        }

        [TestMethod]
        public void UriValue()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("http://host/app");
            System.Text.RegularExpressions.Group opt = m.Groups["switch"];
            Assert.IsFalse(opt.Success);
            Assert.AreEqual<string>(string.Empty, opt.Value);
            opt = m.Groups["name"];
            Assert.IsFalse(opt.Success);
            Assert.AreEqual<string>(string.Empty, opt.Value);
        }

        [TestMethod]
        public void NamedSwitchValues()
        {
            System.Text.RegularExpressions.Regex expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = expr.Match("/abc=");
            Assert.IsTrue(m.Success);
            System.Text.RegularExpressions.Group opt = m.Groups["name"];
            Assert.IsTrue(opt.Success);
            Assert.AreEqual<string>("abc", opt.Value);
            System.Text.RegularExpressions.Group turn = m.Groups["turn"];
            Assert.IsFalse(turn.Success);
            System.Text.RegularExpressions.Group val = m.Groups["value"];
            Assert.IsTrue(val.Success);
            Assert.AreEqual<string>(string.Empty, val.Value);
        }

        class Part
        {
            public Part(string name, bool matched, string value) { Name = name; Matched = matched; Value = value; }
            public string Name, Value;
            public bool Matched;
            public override string ToString()
            {
                return string.Format("({0},{1},{2})", Name, Matched, Value);
            }
        }

        void eval(System.Text.RegularExpressions.Match m, params Part[] parts)
        {
            foreach (Part p in parts)
            {
                System.Text.RegularExpressions.Group g = m.Groups[p.Name];
                Assert.AreEqual<bool>(p.Matched, g.Success, "Expected:" + p.ToString() + " Actual:" + g.Success);
                Assert.AreEqual<string>(p.Value, g.Value, "Expected:" + p.Value + " Actual:" + g.Value);
            }
        }

        [TestMethod]
        public void Named()
        {
            var expr = new System.Text.RegularExpressions.Regex(Switch_Regex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            eval(expr.Match("/abc="), new Part[] { new Part("name", true, "abc"), new Part("turn", false, string.Empty), new Part("value", true, string.Empty) });
            eval(expr.Match("/abc:"), new Part[] { new Part("name", true, "abc"), new Part("turn", false, string.Empty), new Part("value", true, string.Empty) });
            eval(expr.Match("/abc"), new Part[] { new Part("name", true, "abc"), new Part("turn", false, string.Empty), new Part("value", false, string.Empty) });
            eval(expr.Match("/a12+"), new Part[] { new Part("name", true, "a12"), new Part("turn", true, "+"), new Part("value", false, string.Empty) });
            eval(expr.Match("/a12-"), new Part[] { new Part("name", true, "a12"), new Part("turn", true, "-"), new Part("value", false, string.Empty) });
            eval(expr.Match("/a12=b34"), new Part[] { new Part("name", true, "a12"), new Part("turn", false, string.Empty), new Part("value", true, "b34") });
            eval(expr.Match("-abc="), new Part[] { new Part("name", true, "abc"), new Part("turn", false, string.Empty), new Part("value", true, string.Empty) });
            eval(expr.Match("-abc:"), new Part[] { new Part("name", true, "abc"), new Part("turn", false, string.Empty), new Part("value", true, string.Empty) });
            eval(expr.Match("-abc"), new Part[] { new Part("name", true, "abc"), new Part("turn", false, string.Empty), new Part("value", false, string.Empty) });
            eval(expr.Match("-a12+"), new Part[] { new Part("name", true, "a12"), new Part("turn", true, "+"), new Part("value", false, string.Empty) });
            eval(expr.Match("-a12-"), new Part[] { new Part("name", true, "a12"), new Part("turn", true, "-"), new Part("value", false, string.Empty) });
            eval(expr.Match("-a12=b34"), new Part[] { new Part("name", true, "a12"), new Part("turn", false, string.Empty), new Part("value", true, "b34") });
            eval(expr.Match("/12=\"def\""), new Part[] { new Part("name", true, "12"), new Part("turn", false, string.Empty), new Part("value", true, "\"def\"") });
            eval(expr.Match("/12:\"def\""), new Part[] { new Part("name", true, "12"), new Part("turn", false, string.Empty), new Part("value", true, "\"def\"") });
            eval(expr.Match("val"), new Part[] { new Part("name", false, string.Empty), new Part("turn", false, string.Empty), new Part("value", false, string.Empty) });
        }

        [TestMethod]
        public void namespace_discriminator()
        {
            var expr = new System.Text.RegularExpressions.Regex(Switch_Regex);

            var match1 = expr.Match("-a");
            System.Text.RegularExpressions.Group ns1 = match1.Groups["switch"];
            Assert.AreEqual<int>(1, ns1.Length);
            Assert.AreEqual<string>("-", ns1.Value);

            var match2 = expr.Match("--arg");
            System.Text.RegularExpressions.Group ns2 = match2.Groups["switch"];
            Assert.AreEqual<int>(2, ns2.Length);
            Assert.AreEqual<string>("--", ns2.Value);

            var match2b = expr.Match("//arg");
            System.Text.RegularExpressions.Group ns2b = match2.Groups["switch"];
            Assert.AreEqual<int>(2, ns2b.Length);
            Assert.AreEqual<string>("--", ns2b.Value);

            var match3 = expr.Match("-/-x");
            System.Text.RegularExpressions.Group ns3 = match3.Groups["switch"];
            Assert.AreEqual<int>(3, ns3.Length);
            Assert.AreEqual<string>("-/-", ns3.Value);

            var match3b = expr.Match("-//x");
            System.Text.RegularExpressions.Group ns3b = match3b.Groups["switch"];
            Assert.AreEqual<int>(3, ns3b.Length);
            Assert.AreEqual<string>("-//", ns3b.Value);
        }
        #endregion

        #region Basics: string and bool
        [TestMethod]
        public void opts1()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/n", "-d-", "/fi-", "/fd+", "-name:", "/last:3", "pos0" });
            Assert.IsTrue(opts.Is("n"));
            Assert.IsTrue(opts.Is("fd"));
            Assert.IsFalse(opts.Is("d"));
            Assert.IsFalse(opts.Is("fi"));
            Assert.IsFalse(opts.Is("m"));
            Assert.IsTrue(opts.Is("name"));
            Assert.AreEqual<string>(string.Empty, opts["name"]);
            Assert.IsTrue(opts.Is("last"));
            Assert.AreEqual<string>("3", opts["last"]);
            Assert.IsFalse(opts.Is("ot"));
            Assert.IsNull(opts["ot"]);
            Assert.IsFalse(opts.Is("pos0"));
        }

        [TestMethod]
        public void dot_in()
        {
            var opts = new utility.Switch(new string[] { "/map", "name1.dot" });
            Assert.AreEqual<string>("name1.dot", opts.SuffixOf("map").Aggregate((whole, next) => whole + next));
        }

        [TestMethod]
        public void dot_in_in()
        {
            var opts = new utility.Switch(new string[] { "/map=-key name1.dot" });
            Dictionary<string, List<string>> dic = opts.AsType<Dictionary<string, List<string>>>("map");
            Assert.AreEqual<int>(1, dic.Count);
            Assert.AreEqual<string>("name1.dot", dic["key"][0]);
        }

        [TestMethod]
        public void dot_in_key_in()
        {
            var opts = new utility.Switch(new string[] { "/map=-key1.dot value" });
            Dictionary<string, List<string>> dic = opts.AsType<Dictionary<string, List<string>>>("map");
            Assert.AreEqual<int>(1, dic.Count);
            Assert.AreEqual<string>("value", dic["key1.dot"][0]);
        }

        [TestMethod]
        public void NamedDotted()
        {
            utility.Switch opts = new utility.Switch(new string[] { "-key1.dot" });
            Assert.IsTrue(opts.Is("key1.dot"));
        }

        [TestMethod]
        public void NamedSpaced()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/n", "-op=a b c", "-x" });
            Assert.IsTrue(opts.Is("n"));
            Assert.IsTrue(opts.Is("x"));
            Assert.IsTrue(opts.Is("op"));
            Assert.AreEqual<string>("a b c", opts["op"]);
        }

        [TestMethod]
        public void Urls()
        {
            string uri1 = "http://host/app";
            string uri2 = "ftp://x/a";
            string uri3 = @"\\server\share";
            utility.Switch opts = new utility.Switch(new string[] { uri1, uri2, uri3 });
            Assert.AreEqual<string>(uri1, opts[0]);
            Assert.AreEqual<string>(uri2, opts[1]);
            Assert.AreEqual<string>(uri3, opts[2]);
        }

        [TestMethod]
        public void positioned1()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/n", "-d-", "/fi-", "-name:", "/last:3", "pos0" });
            Assert.AreEqual<string>("pos0", opts[0]);
            Assert.IsNull(opts[1]);
        }

        [TestMethod]
        public void duplicated_turn()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/n", "-n-", "-m-", "-m+" });
            Assert.IsFalse(opts.Is("n"));
            Assert.IsTrue(opts.Is("m"));
        }

        [TestMethod]
        public void duplicated_named()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/a:1", "-a=2", "/a:3" });
            Assert.AreEqual<string>("3", opts["a"]);
        }

        [TestMethod]
        public void CaseSenseTurn()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/N", "-n-" }, StringComparer.CurrentCulture);
            Assert.IsTrue(opts.Is("N"));
            Assert.IsFalse(opts.Is("n"));
        }

        [TestMethod]
        public void CaseSenseValue()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/N:uno", "-n:dos" }, StringComparer.CurrentCulture);
            Assert.IsTrue(opts.Is("N"));
            Assert.IsTrue(opts.Is("n"));
            Assert.AreEqual<string>("uno", opts["N"]);
            Assert.AreEqual<string>("dos", opts["n"]);
        }

        [TestMethod]
        public void CaseInsenseValue()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/N:uno", "-n=dos" });
            Assert.IsTrue(opts.Is("N"));
            Assert.IsTrue(opts.Is("n"));
            Assert.AreEqual<string>("dos", opts["N"]);
            Assert.AreEqual<string>("dos", opts["n"]);
        }

        [TestMethod]
        public void iterate_all()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/N:uno", "pos0", "-n:dos", "pos1", "/x-" }, StringComparer.CurrentCulture);
            Assert.AreEqual<int>(5, opts.Count);
            StringBuilder concat = new StringBuilder();
            List<string> all = new List<string>();
            foreach (string arg in opts)
            {
                concat.Append(arg);
                all.Add(arg);
            }
            Assert.AreEqual<int>(5, all.Count);
            Assert.AreEqual<string>("/N:unopos0-n:dospos1/x-", concat.ToString());
        }

        [TestMethod]
        public void iterate_bytype()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/N=uno", "pos0", "-n+", "pos1" }, StringComparer.CurrentCulture);
            Assert.AreEqual<string>("pos0", opts[0]);
            Assert.AreEqual<string>("pos1", opts[1]);
            Assert.AreEqual<int>(4, opts.Count);
            Assert.AreEqual<int>(2, opts.IndexedArguments.Count);
            StringBuilder concat = new StringBuilder();
            List<string> all = new List<string>();
            foreach (string arg in opts.IndexedArguments)
            {
                concat.Append(arg);
                all.Add(arg);
            }
            Assert.AreEqual<int>(2, all.Count);
            Assert.AreEqual<string>("pos0pos1", concat.ToString());

            Assert.AreEqual<int>(2, opts.NamedArguments.Count);
            concat = new StringBuilder();
            List<KeyValuePair<string, string>> allturns = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, string> arg in opts.NamedArguments)
            {
                concat.Append(arg.Key);
                concat.Append(arg.Value);
                allturns.Add(arg);
            }
            Assert.AreEqual<int>(2, all.Count);
            Assert.AreEqual<string>("Nunon+", concat.ToString());
        }

        [TestMethod]
        public void iterate_bytype_no_named_args()
        {
            utility.Switch opts = new utility.Switch(new string[] { "pos0", "pos1" }, StringComparer.CurrentCulture);
            Assert.AreEqual<string>("pos0", opts[0]);
            Assert.AreEqual<string>("pos1", opts[1]);
            Assert.AreEqual<int>(2, opts.Count);
            Assert.AreEqual<int>(2, opts.IndexedArguments.Count);
            Assert.AreEqual<int>(0, opts.NamedArguments.Count);
        }

        [TestMethod]
        public void intravalspace()
        {
            /*
                  mixx.exe uno dos /op:"string value"
                  uno
                  dos
                  /op:string value
            */
            utility.Switch opts = new utility.Switch(new string[] { "/N=uno", "/op:string value" });
            Assert.AreEqual<string>("uno", opts["N"]);
            Assert.AreEqual<string>("string value", opts["op"]);
            Assert.AreEqual<int>(2, opts.NamedArguments.Count);
        }

        [TestMethod]
        public void suffix_of_named()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/favroot:path", "-exclude", "xd x", "asd", "dos", "/out:file.html", "four" });
            Assert.AreEqual<string>("path", opts["favroot"]);
            Assert.IsTrue(opts.Is("exclude"));
            foreach (string arg in opts.SuffixOf("none"))
                Assert.Fail("a non-existing argument 'none'");
            var args = new List<string>();
            foreach (string arg in opts.SuffixOf("favroot"))
                args.Add(arg);
            Assert.AreEqual<int>(0, args.Count);
            foreach (string arg in opts.SuffixOf("exclude"))
                args.Add(arg);
            var all = new StringBuilder();
            args.ForEach((x) => { all.Append(x); });
            Assert.AreEqual<int>(3, args.Count);
            Assert.AreEqual<string>("xd xasddos", all.ToString());

            var four = new StringBuilder();
            foreach (string arg in opts.SuffixOf("out"))
                four.Append(arg);
            Assert.AreEqual<string>("four", four.ToString());
        }

        [TestMethod]
        public void suffix_of_positioned()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/favroot:path", "source", "dest", "/out:file.html", "/mod=m1", "four", "/x-" });
            Assert.AreEqual<int>(3, opts.IndexedArguments.Count);
            Assert.AreEqual<string>("source", opts[0]);
            foreach (var arg in opts.SuffixOf(10))
                Assert.Fail("a non-existing positioned argument 10");
            foreach (var arg in opts.SuffixOf(-1))
                Assert.Fail("a non-existing positioned argument -1");
            var args = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, string> arg in opts.SuffixOf(0))
                args.Add(arg);
            Assert.AreEqual<int>(0, args.Count);
            foreach (KeyValuePair<string, string> arg in opts.SuffixOf(1))
                args.Add(arg);
            var all = new StringBuilder();
            args.ForEach((x) => { all.Append(x.Key); all.Append(x.Value); });
            Assert.AreEqual<int>(2, args.Count);
            Assert.AreEqual<string>("outfile.htmlmodm1", all.ToString());
        }

        [TestMethod]
        public void prefix_of_named()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/favroot:path", "-exclude", "xd x", "asd", "dos", "/out:file.html", "four" });
            Assert.AreEqual<string>("path", opts["favroot"]);
            Assert.IsTrue(opts.Is("exclude"));
            foreach (string arg in opts.PrefixOf("none"))
                Assert.Fail("a non-existing argument 'none'");
            var args = new List<string>();
            foreach (string arg in opts.PrefixOf("favroot"))
                args.Add(arg);
            Assert.AreEqual<int>(0, args.Count);
            foreach (string arg in opts.PrefixOf("out"))
                args.Add(arg);
            var all = new StringBuilder();
            args.ForEach((x) => { all.Append(x); });
            Assert.AreEqual<int>(3, args.Count);
            Assert.AreEqual<string>("dosasdxd x", all.ToString());
        }

        [TestMethod]
        public void prefix_of_positioned()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/favroot:path", "source", "dest", "/out:file.html", "/mod=m1", "four", "/x-" });
            Assert.AreEqual<int>(3, opts.IndexedArguments.Count);
            Assert.AreEqual<string>("four", opts[2]);
            foreach (var arg in opts.PrefixOf(10))
                Assert.Fail("a non-existing positioned argument 10");
            foreach (var arg in opts.PrefixOf(-1))
                Assert.Fail("a non-existing positioned argument -1");
            var args = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, string> arg in opts.PrefixOf(1))
                args.Add(arg);
            Assert.AreEqual<int>(0, args.Count);
            foreach (KeyValuePair<string, string> arg in opts.PrefixOf(0))
                args.Add(arg);
            var all = new StringBuilder();
            args.ForEach((x) => { all.Append(x.Key); all.Append(x.Value); });
            Assert.AreEqual<int>(1, args.Count);
            Assert.AreEqual<string>("favrootpath", all.ToString());

            args = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, string> arg in opts.PrefixOf(2))
                args.Add(arg);
            all = new StringBuilder();
            args.ForEach((x) => { all.Append(x.Key); all.Append(x.Value); });
            Assert.AreEqual<int>(2, args.Count);
            Assert.AreEqual<string>("modm1outfile.html", all.ToString());
        }

        [TestMethod]
        public void help()
        {
            var args = new string[] { "-?" };
            var opts = new utility.Switch(args);
            Assert.IsTrue(opts.Is("?"));
        }

        [TestMethod]
        public void help2()
        {
            var args = new string[] { "/?" };
            var opts = new utility.Switch(args);
            Assert.IsTrue(opts.Is("?"));
        }

        [TestMethod]
        public void FolderArgIsPresent()
        {
            var args = new[] { "-folder", "." };
            var opts = new utility.Switch(args, StringComparer.OrdinalIgnoreCase);

            Assert.IsTrue(opts.Is("folder"));
            if (!opts.Is("folder"))
            {
                throw new ArgumentException();
            }
            Assert.IsTrue(opts["folder"] == null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void FolderArgIsNotPresent()
        {
            var args = new[] { "-other", "." };
            var opts = new utility.Switch(args);

            Assert.IsFalse(opts.Is("folder"));
            if (!opts.Is("folder"))
            {
                throw new ArgumentException();
            }
        }

        [TestMethod]
        public void FolderAsSuffix()
        {
            var args = new[] { "-folder", "." };
            var opts = new utility.Switch(args);
            string folder = opts.SuffixOf("folder").First();
            Assert.AreEqual<string>(".", folder);
        }
        #endregion

        #region Prefix/Suffix whitespace
        //The input is processed as literal value.
        //Switch presupposes the parsing rules of Win32 API parser (see class SystemArgumentParser).

        [TestMethod]
        public void whitespace_prefix()
        {
            var opts = new utility.Switch(new string[] { " /arg=value" });
            Assert.IsFalse(opts.Is("arg"));
        }
        [TestMethod]
        public void whitespace_suffix()
        {
            var opts = new utility.Switch(new string[] { "/arg=value " });
            Assert.IsTrue(opts.Is("arg"));
            Assert.AreEqual<string>("value ", opts["arg"]);
        }
        #endregion

        #region Schema types
        class PersonType
        {
            public string Name { get; set; }
            public string Last { get; set; }
            public int Age;
        }

        class AgentType
        {
            public int ID;
            public string[] Goals;
        }

        public class DeviceValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        class WnClass
        {
            public int Value;
            public double N;
            public bool Log;
            public bool Trace;
            public bool? Optional;
            public string Title;
            public int[] Sizes;
            public char Sign;
            public DateTime End;
            public Uri Url;
            public NormalizationForm EnumValue;
            public AgentType Agent;
            public double[] Fracs;
            public decimal[] Prices;
            public AgentType[] Agents;
            public Uri[] Dirs;

            public string Name { get; set; }
            public bool IsActive { get; set; }
            public bool Debug { get; set; }
            public bool Throw { get; set; }
            public decimal Price { get; set; }
            public string[] Names { get; set; }
            public bool[] Flags { get; set; }
            public DateTime Start { get; set; }
            public List<int> Limits { get; set; }
            public List<PersonType> HeadPersons { get; set; }
            public HashSet<string> Excludes { get; set; }
            public HashSet<PersonType> ExcludePersons { get; set; }
            public PersonType Person { get; set; }
            public PersonType[] Persons { get; set; }
            public Dictionary<string, string> StringMap { get; set; }
            public Dictionary<string, List<string>> Map { get; set; }
            public Dictionary<string, List<decimal>> Marks { get; set; }
            public Dictionary<int, List<string>> Notes { get; set; }
            public Dictionary<string, HashSet<int>> Tones { get; set; }

            public Guid ID { get; set; }
            public DeviceValue[] Values { get; set; }
            public NormalizationForm[] EnumValues { get; set; }
            public Guid[] IDs { get; set; }

            public void Method1() { Method1Result = Name; }
            public void Method2() { Method2Result = Method1Result + Name; }
            public string Method1Result;
            public string Method2Result;
        }
        #endregion

        #region Object data type

        [TestMethod]
        public void Object_value()
        {
            string[] args =
      {
        "/value=2", "-isactive", "/Log-", "-trace+", "-debug-", "/throw+","-price:3.4", "-Name:name1",
        "/title=uno dos", "/n:3.2", "-start:2008-10-10", "/end=2008-10-10T23:45:45","-sign=X",
        "-id:077e5f6f-49ba-4583-a383-64ec3156fe83", "-url=http://hostname/path/",
        "/enumvalue=FormC"
      };
            WnClass WnObject = utility.Switch.AsType(args, new WnClass());
            Assert.AreEqual<int>(2, WnObject.Value);
            Assert.AreEqual<string>("name1", WnObject.Name);
            Assert.AreEqual<string>("uno dos", WnObject.Title);
            Assert.AreEqual<double>(3.2, WnObject.N);
            Assert.AreEqual<decimal>(3.4M, WnObject.Price);
            Assert.AreEqual<char>('X', WnObject.Sign);
            Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10), WnObject.Start);
            Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10, 23, 45, 45), WnObject.End);
            Assert.AreEqual<Guid>(new Guid("077e5f6f-49ba-4583-a383-64ec3156fe83"), WnObject.ID);
            Assert.AreEqual<Uri>(new Uri("http://hostname/path/"), WnObject.Url);
            Assert.AreEqual<NormalizationForm>(NormalizationForm.FormC, WnObject.EnumValue);
            Assert.IsTrue(WnObject.IsActive);
            Assert.IsFalse(WnObject.Log);
            Assert.IsTrue(WnObject.Trace);
            Assert.IsFalse(WnObject.Debug);
            Assert.IsTrue(WnObject.Throw);
        }

        [TestMethod]
        public void Object_flag()
        {
            string[] args = { "-trace", "-debug-", "/throw+" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsFalse(WnObject.IsActive);
            Assert.IsTrue(WnObject.Trace);
            Assert.IsFalse(WnObject.Debug);
            Assert.IsTrue(WnObject.Throw);
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Object_misvalue()
        {
            var opts = new utility.Switch(new string[] { "-value:t" });
            Assert.AreEqual<int>(0, opts.AsType<int>("Value"));
            Assert.Fail();
        }

        #region .NET value types
        public struct Value
        {
            public string name;
        }
        [TestMethod, ExpectedException(typeof(InvalidCastException))]
        public void valuetype()
        {
            var opts = new utility.Switch(new string[] { "-name:n2" });
            Value x = opts.AsType<Value>("name");
            Assert.Fail();
        }
        #endregion

        [TestMethod]
        public void Object_valuearray()
        {
            string id1 = "f9b4ffc3-0dc1-4311-95eb-3985c4438f33";
            string id2 = "ed923e5e-ebf3-4321-8771-4050e7103962";
            string[] args =
      {
        "z","/names","uno", "dos", "tres", "-x", "-sizes", "1", "2",
        "/flags","true","False","True","false","-fracs","1.1", "2.2",
        "-prices","12.20","31.42","-EnumValues","FormC","FormKD",
        "-dirs","http://x/a","http://x/b", "/IDs", id1, id2
      };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.AreEqual<int>(3, WnObject.Names.Length);
            Assert.AreEqual<int>(2, WnObject.Sizes.Length);
            Assert.AreEqual<int>(4, WnObject.Flags.Length);
            Assert.AreEqual<int>(2, WnObject.Fracs.Length);
            Assert.AreEqual<int>(2, WnObject.Prices.Length);
            Assert.AreEqual<int>(2, WnObject.Prices.Length);
            Assert.AreEqual<decimal>(43.62M, WnObject.Prices[0] + WnObject.Prices[1]);
            Assert.AreEqual<int>(2, WnObject.EnumValues.Length);
            Assert.AreEqual<int>(2, WnObject.Dirs.Length);
            Assert.AreEqual<string>("http://x/a", WnObject.Dirs[0].ToString());
            Assert.AreEqual<string>("http://x/b", WnObject.Dirs[1].ToString());
            Assert.AreEqual<int>(2, WnObject.IDs.Length);
            Assert.AreEqual<string>(id1, WnObject.IDs[0].ToString());
            Assert.AreEqual<string>(id2, WnObject.IDs[1].ToString());
            Assert.IsTrue(WnObject.Flags[0] && !WnObject.Flags[1] && WnObject.Flags[2] && !WnObject.Flags[3]);
        }

        [TestMethod]
        public void Object_valuearrayNone()
        {
            var opts = new utility.Switch(new string[] { "-names" });
            WnClass WnObject = utility.Switch.AsType<WnClass>(opts);
            Assert.AreEqual<int>(0, WnObject.Names.Length);
            Assert.IsNull(WnObject.Prices);
        }

        [TestMethod]
        public void Object_collectionListOfValueTypes()
        {
            string[] args = { "z", "/limits", "0", "1", "2", "-x", "-sizes", "1", "2", "/flags", "true", "False", "True", "false", "-fracs", "1.1", "2.2" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.AreEqual(3, WnObject.Limits.Count);
        }

        [TestMethod]
        public void Object_collectionListOfPersons()
        {
            string[] args = { "z", "/limits", "0", "1", "2", "-x", "-sizes", "1", "2", "-HeadPersons:-name:name1 /last=last1 -age=24;-name:name2 /last=last2 -age=34", "/flags", "true", "False", "True", "false", "-fracs", "1.1", "2.2" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.AreEqual(2, WnObject.HeadPersons.Count);
            Assert.AreEqual<string>("name1", WnObject.HeadPersons[0].Name);
            Assert.AreEqual<string>("last1", WnObject.HeadPersons[0].Last);
            Assert.AreEqual<int>(24, WnObject.HeadPersons[0].Age);
            Assert.AreEqual<string>("name2", WnObject.HeadPersons[1].Name);
            Assert.AreEqual<string>("last2", WnObject.HeadPersons[1].Last);
            Assert.AreEqual<int>(34, WnObject.HeadPersons[1].Age);
        }

        [TestMethod]
        public void Object_collectionHashSetOfString()
        {
            string[] args = { "z", "/limits", "0", "1", "2", "-x", "-sizes", "1", "2", "/flags", "true", "False", "True", "false", "-fracs", "1.1", "2.2", "/excludes", "exclude1", "exclude2" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.AreEqual(2, WnObject.Excludes.Count);
        }

        [TestMethod]
        public void Object_collectionHashSetOfPerson()
        {
            string[] args = { "/ExcludePersons:-name:name1 /last=last1 -age=24;-name:name2 /last=last2 -age=34" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.AreEqual(2, WnObject.ExcludePersons.Count);
            int index = 0;
            foreach (PersonType p in WnObject.ExcludePersons)
            {
                if (index == 0)
                {
                    Assert.AreEqual<string>("name1", p.Name);
                    Assert.AreEqual<string>("last1", p.Last);
                    Assert.AreEqual<int>(24, p.Age);
                }
                else
                {
                    Assert.AreEqual<string>("name2", p.Name);
                    Assert.AreEqual<string>("last2", p.Last);
                    Assert.AreEqual<int>(34, p.Age);
                }
                ++index;
            }
        }

        [TestMethod]
        public void Object_subobject1()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/person: -name:name1 /last=last1 -age=24" });
            WnClass WnObject = utility.Switch.AsType<WnClass>(opts);
            Assert.AreEqual("name1", WnObject.Person.Name);
            Assert.AreEqual("last1", WnObject.Person.Last);
            Assert.AreEqual(24, WnObject.Person.Age);
        }

        [TestMethod]
        public void Object_subobject2()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/person:-name:name1 /last=last1 -age=24", "/Agent=-id=2 -goals uno dos" });
            WnClass WnObject = utility.Switch.AsType<WnClass>(opts);
            Assert.AreEqual("name1", WnObject.Person.Name);
            Assert.AreEqual("last1", WnObject.Person.Last);
            Assert.AreEqual(24, WnObject.Person.Age);
            Assert.AreEqual(2, WnObject.Agent.ID);
            Assert.AreEqual(2, WnObject.Agent.Goals.Length);
        }

        [TestMethod]
        public void Object_objectarray0()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/persons=-name:name1 /last=last1 -age=24 ; -name:name2 /last=last2 -age=34" });
            Assert.AreEqual<string>("-name:name1 /last=last1 -age=24 ; -name:name2 /last=last2 -age=34", opts["Persons"]);
            Assert.AreEqual<int>(1, opts.Count);
        }

        [TestMethod]
        public void Object_objectarray1()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/persons:-name:name1 /last=last1 -age=24;-name:name2 /last=last2 -age=34" });
            WnClass WnObject = utility.Switch.AsType<WnClass>(opts);
            Assert.AreEqual<int>(2, WnObject.Persons.Length);
            Assert.AreEqual<string>("name1", WnObject.Persons[0].Name);
            Assert.AreEqual<string>("last1", WnObject.Persons[0].Last);
            Assert.AreEqual<int>(24, WnObject.Persons[0].Age);
            Assert.AreEqual<string>("name2", WnObject.Persons[1].Name);
            Assert.AreEqual<string>("last2", WnObject.Persons[1].Last);
            Assert.AreEqual<int>(34, WnObject.Persons[1].Age);
        }

        [TestMethod]
        public void Object_objectarray2()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/agents:-id=2 -goals uno dos;-id=3 -goals tres cuatro" });
            WnClass WnObject = utility.Switch.AsType<WnClass>(opts);
            Assert.AreEqual(2, WnObject.Agents.Length);
            Assert.AreEqual(2, WnObject.Agents[0].ID);
            Assert.AreEqual(2, WnObject.Agents[0].Goals.Length);
            Assert.AreEqual(3, WnObject.Agents[1].ID);
            Assert.AreEqual(2, WnObject.Agents[1].Goals.Length);
        }

        [TestMethod]
        public void Object_objectarray3()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/values= /key=k1 /value=n1;/key=k2 /value=n2" });
            WnClass WnObject = utility.Switch.AsType<WnClass>(opts);
            Assert.AreEqual<int>(2, WnObject.Values.Length);
            Assert.AreEqual<string>("k1", WnObject.Values[0].Key);
            Assert.AreEqual<string>("n1", WnObject.Values[0].Value);
            Assert.AreEqual<string>("k2", WnObject.Values[1].Key);
            Assert.AreEqual<string>("n2", WnObject.Values[1].Value);
        }

        [TestMethod]
        public void Object_objectarray4()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/values= /key=k1 /value=n1" });
            WnClass WnObject = utility.Switch.AsType<WnClass>(opts);
            Assert.AreEqual<int>(1, WnObject.Values.Length);
            Assert.AreEqual<string>("k1", WnObject.Values[0].Key);
            Assert.AreEqual<string>("n1", WnObject.Values[0].Value);
        }

        [TestMethod]
        public void Object_objectarray5()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/values= /key=k1 /value=n1" });
            DeviceValue[] values = opts.AsType<DeviceValue[]>("values");
            Assert.AreEqual<int>(1, values.Length);
            Assert.AreEqual<string>("k1", values[0].Key);
            Assert.AreEqual<string>("n1", values[0].Value);
        }

        [TestMethod]
        public void Object_method()
        {
            string[] args = { "-Method1", "-x", "-name:name1" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.AreEqual("name1", WnObject.Method1Result);
        }

        [TestMethod]
        public void Object_method2()
        {
            string[] args = { "-Method1", "-Method2", "-x", "-name:name1" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.AreEqual("name1", WnObject.Method1Result);
            Assert.AreEqual("name1name1", WnObject.Method2Result);
        }

        [TestMethod, Ignore]
        public void Object_valid()
        {
            string[] args = { "-op:Method1", "-x", "-name:name1" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            //Assert only valid schema
        }

        [TestMethod]
        public void Object_DictionaryStringToString()
        {
            string[] args = new string[] { "/StringMap=-k1=1 -k2:2 /k3:3 -k4=4" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsNotNull(WnObject.StringMap);
            Assert.AreEqual<int>(4, WnObject.StringMap.Count);
            Assert.AreEqual<string>("1", WnObject.StringMap["k1"]);
            Assert.AreEqual<string>("2", WnObject.StringMap["k2"]);
            Assert.AreEqual<string>("3", WnObject.StringMap["k3"]);
            Assert.AreEqual<string>("4", WnObject.StringMap["k4"]);
        }

        [TestMethod]
        public void Object_DictionaryStringToLongString()
        {
            string[] args = new string[] { "/StringMap=\"-Filter:TipoReg = 'O1' OR TipoReg = 'P '\"" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsNotNull(WnObject.StringMap);
            Assert.AreEqual<int>(1, WnObject.StringMap.Count);
            Assert.AreEqual<string>("TipoReg = 'O1' OR TipoReg = 'P '", WnObject.StringMap["Filter"]);
        }

        [TestMethod]
        public void Object_DictionaryStringToStringList()
        {
            string[] args = { "-map:-mapping1 mappedone mappedtwo mappedtree -mapping2 mapped1 mapped2" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsNotNull(WnObject.Map);
            Assert.AreEqual<int>(2, WnObject.Map.Count);
            Assert.AreEqual<int>(3, WnObject.Map["mapping1"].Count);
            Assert.AreEqual<int>(2, WnObject.Map["mapping2"].Count);
            Assert.AreEqual<string>("mappedone", WnObject.Map["mapping1"][0]);
            Assert.AreEqual<string>("mappedtwo", WnObject.Map["mapping1"][1]);
            Assert.AreEqual<string>("mappedtree", WnObject.Map["mapping1"][2]);
            Assert.AreEqual<string>("mapped1", WnObject.Map["mapping2"][0]);
            Assert.AreEqual<string>("mapped2", WnObject.Map["mapping2"][1]);
        }

        [TestMethod]
        public void Object_DictionaryStringToDecimalList()
        {
            string[] args = { "-Marks:-g1 9.9 7.9 -g2 7 6.5 10" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsNotNull(WnObject.Marks);
            Assert.AreEqual<int>(2, WnObject.Marks.Count);
            Assert.AreEqual<int>(2, WnObject.Marks["g1"].Count);
            Assert.AreEqual<int>(3, WnObject.Marks["g2"].Count);
            Assert.AreEqual<decimal>(9.9M, WnObject.Marks["g1"][0]);
            Assert.AreEqual<decimal>(7.9M, WnObject.Marks["g1"][1]);
            Assert.AreEqual<decimal>(7M, WnObject.Marks["g2"][0]);
            Assert.AreEqual<decimal>(6.5M, WnObject.Marks["g2"][1]);
            Assert.AreEqual<decimal>(10M, WnObject.Marks["g2"][2]);
        }

        [TestMethod]
        public void Object_DictionaryIntToStringList()
        {
            string[] args = { "-Notes:-1 a b -2 x y z" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsNotNull(WnObject.Notes);
            Assert.AreEqual<int>(2, WnObject.Notes.Count);
            Assert.AreEqual<int>(2, WnObject.Notes[1].Count);
            Assert.AreEqual<int>(3, WnObject.Notes[2].Count);
            Assert.AreEqual<string>("a", WnObject.Notes[1][0]);
            Assert.AreEqual<string>("b", WnObject.Notes[1][1]);
            Assert.AreEqual<string>("x", WnObject.Notes[2][0]);
            Assert.AreEqual<string>("y", WnObject.Notes[2][1]);
            Assert.AreEqual<string>("z", WnObject.Notes[2][2]);
        }

        [TestMethod]
        public void Object_DictionaryStringToIntHashSet()
        {
            string[] args = { "-tones:-a 1 2 -b 3 4 5" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsNotNull(WnObject.Tones);
            Assert.AreEqual<int>(2, WnObject.Tones.Count);
            Assert.AreEqual<int>(2, WnObject.Tones["a"].Count);
            Assert.AreEqual<int>(3, WnObject.Tones["b"].Count);
            Assert.IsTrue(WnObject.Tones["a"].Contains(1));
            Assert.IsTrue(WnObject.Tones["a"].Contains(2));
            Assert.IsTrue(WnObject.Tones["b"].Contains(3));
            Assert.IsTrue(WnObject.Tones["b"].Contains(4));
            Assert.IsTrue(WnObject.Tones["b"].Contains(5));
        }

        #endregion

        #region Value data type

        [TestMethod]
        public void Value_basic()
        {
            string[] args =
            {
        "/value=2", "-isactive", "/Log-", "-trace+", "-debug-", "/throw+", "-Name:name1",
        "/title=uno dos", "/n:3.2", "-start:2008-10-10", "/end=2008-10-10T23:45:45",
        "-id:077e5f6f-49ba-4583-a383-64ec3156fe83", "-url=http://hostname/path/",
        "/enumvalue=blue"
            };
            var opts = new utility.Switch(args);
            Assert.AreEqual<int>(2, opts.AsType<int>("value"));
            Assert.AreEqual<int>(0, opts.AsType<int>("novalue"));
            Assert.AreEqual<double>(3.2, opts.AsType<double>("n"));
            Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10), opts.AsType<DateTime>("Start"));
            Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10, 23, 45, 45), opts.AsType<DateTime>("End"));
            Assert.AreEqual<Guid>(new Guid("077e5f6f-49ba-4583-a383-64ec3156fe83"), opts.AsType<Guid>("ID"));
            Assert.AreEqual<Uri>(new Uri("http://hostname/path/"), opts.AsType<Uri>("Url"));
            Assert.AreEqual<ConsoleColor>(ConsoleColor.Blue, opts.AsType<ConsoleColor>("EnumValue"));
        }

        [TestMethod]
        public void Value_basicmix()
        {
            var id = new Guid("92d833ea-c99d-499d-846d-0e6eb7155f6a");
            var opts = new utility.Switch(new string[] { "/color=red", "92d833ea-c99d-499d-846d-0e6eb7155f6a" });
            Assert.AreEqual<Guid>(id, opts.AsType<Guid>(0));
            Assert.AreEqual<ConsoleColor>(ConsoleColor.Red, opts.AsType<ConsoleColor>("color"));
        }

        [TestMethod]
        public void Value_Parse()
        {
            var opts = new utility.Switch(new string[] { "01:30:00" });
            TimeSpan span = opts.AsType<TimeSpan>(0);
            Assert.AreEqual<int>(30, span.Minutes);
        }

        [TestMethod]
        public void Value_ParseNamed()
        {
            var opts = new utility.Switch(new string[] { "-t:01:30:00" });
            TimeSpan span = opts.AsType<TimeSpan>("t");
            Assert.AreEqual<int>(30, span.Minutes);
        }

        [TestMethod]
        public void Value_ParseNull()
        {
            var opts = new utility.Switch(null);
            TimeSpan span = opts.AsType<TimeSpan>(0);
            Assert.AreEqual<int>(0, span.Hours + span.Minutes + span.Seconds);
        }

        [TestMethod]
        public void Value_ParseNamedNull()
        {
            var opts = new utility.Switch(new string[] { "-t" });
            TimeSpan span = opts.AsType<TimeSpan>("t");
            Assert.AreEqual<int>(0, span.Hours + span.Minutes + span.Seconds);
        }

        [TestMethod, ExpectedException(typeof(System.FormatException), "Input string was not in a correct format.")]
        public void Value_ParseNamedEmpty()
        {
            var opts = new utility.Switch(new string[] { "-t=" });
            TimeSpan span = opts.AsType<TimeSpan>("t");
            Assert.Fail();
        }

        [TestMethod]
        public void Value_DateNone()
        {
            var opts = new utility.Switch(new string[] { "-start" });
            Assert.IsTrue(opts.AsType<DateTime>("start") == DateTime.MinValue);
            Assert.IsTrue(opts.AsType<DateTime>("end") == DateTime.MinValue);
            Assert.IsTrue(opts.AsType<DateTime>(0) == DateTime.MinValue);
        }

        [TestMethod, ExpectedException(typeof(FormatException), "String was not recognized as a valid DateTime.")]
        public void Value_DateNull()
        {
            var opts = new utility.Switch(new string[] { "/end=" });
            Assert.IsTrue(opts.AsType<DateTime>("end") == DateTime.MinValue);
            Assert.Fail();
        }

        [TestMethod]
        public void Value_StringNone()
        {
            var opts = new utility.Switch(new string[] { "/value=", "-Name" });
            Assert.IsTrue(opts.AsType<string>("value") == string.Empty);
            Assert.IsTrue(opts.AsType<string>("name") == null);
            Assert.IsTrue(opts.AsType<string>("nonexistent") == null);
        }

        [TestMethod]
        public void Value_IntNone()
        {
            var opts = new utility.Switch(new string[] { "/n" });
            Assert.IsTrue(opts.AsType<int>("n") == 0);
            Assert.IsTrue(opts.AsType<int>("x") == 0);
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Value_IntNull()
        {
            var opts = new utility.Switch(new string[] { "/m=" });
            Assert.IsTrue(opts.AsType<int>("m") == 0);
            Assert.Fail();
        }

        [TestMethod]
        public void Value_DoubleNone()
        {
            var opts = new utility.Switch(new string[] { "/n" });
            Assert.IsTrue(opts.AsType<double>("n") == 0D);
            Assert.IsTrue(opts.AsType<double>("x") == 0D);
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Value_DoubleNull()
        {
            var opts = new utility.Switch(new string[] { "/m=" });
            Assert.IsTrue(opts.AsType<double>("m") == 0D);
            Assert.Fail();
        }

        [TestMethod]
        public void Value_DecimalNone()
        {
            var opts = new utility.Switch(new string[] { "/n" });
            Assert.IsTrue(opts.AsType<decimal>("n") == 0M);
            Assert.IsTrue(opts.AsType<decimal>("x") == 0M);
            Assert.IsTrue(opts.AsType<decimal>(0) == 0M);
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void Value_DecimalNull()
        {
            var opts = new utility.Switch(new string[] { "/m=" });
            Assert.IsTrue(opts.AsType<decimal>("m") == 0M);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(FormatException), "Guid should contain 32 digits with 4 dashes (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).")]
        public void Value_GuidNull()
        {
            var opts = new utility.Switch(new string[] { "/id:" });
            Assert.IsTrue(opts.AsType<int>("ID") == 0);
            Assert.Fail();
        }

        [TestMethod]
        public void Value_EnumNone()
        {
            var opts = new utility.Switch(new string[] { "/form" });
            Assert.IsTrue(opts.AsType<NormalizationForm>("form") == default(NormalizationForm));
            Assert.IsTrue(opts.AsType<NormalizationForm>("x") == default(NormalizationForm));
        }

        [TestMethod, ExpectedException(typeof(ArgumentException), "Must specify valid information for parsing in the string.")]
        public void Value_EnumNull()
        {
            var opts = new utility.Switch(new string[] { "/form=" });
            Assert.IsTrue(opts.AsType<NormalizationForm>("form") == default(NormalizationForm));
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Value_EnumInvalid()
        {
            var opts = new utility.Switch(new string[] { "/form=Invalid" });
            Assert.AreEqual<NormalizationForm>(NormalizationForm.FormD, opts.AsType<NormalizationForm>("form"));
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null. Parameter name: uriString.")]
        public void Value_UriNone()
        {
            var opts = new utility.Switch(new string[] { "/url" });
            Assert.IsTrue(opts.AsType<Uri>("url") == null);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null. Parameter name: uriString.")]
        public void Value_UriNoneIndexed()
        {
            var opts = new utility.Switch(new string[] { "x" });
            Assert.IsTrue(opts.AsType<Uri>(1) == null);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null. Parameter name: uriString.")]
        public void Value_UriNull()
        {
            var opts = new utility.Switch(null);
            Assert.IsTrue(opts.AsType<Uri>("url") == null);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null. Parameter name: uriString.")]
        public void Value_UriNullIndexed()
        {
            var opts = new utility.Switch(null);
            Assert.IsTrue(opts.AsType<Uri>(0) == null);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(System.UriFormatException), "Invalid URI: The URI is empty.")]
        public void Value_UriEmpty()
        {
            var opts = new utility.Switch(new string[] { "-url=" });
            Assert.IsTrue(opts.AsType<Uri>("url") == null);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(System.UriFormatException), "Invalid URI: The URI is empty.")]
        public void Value_UriEmptyWithCreator()
        {
            var ctors = new Dictionary<Type, utility.Switch.InstanceCreator>();
            ctors.Add(typeof(Uri), (uri) => new Uri(uri));
            var opts = new utility.Switch(new string[] { "-url=" }, ctors);
            Assert.IsTrue(opts.AsType<Uri>("url") == null);
            Assert.Fail();
        }

        [TestMethod]
        public void NullInput()
        {
            var opts = new utility.Switch(null);
            Assert.IsFalse(opts.Is("x"));
            Assert.IsNull(opts["y"]);
            Assert.AreEqual<double>(new double(), opts.AsType<double>("non-existing"));
            Assert.AreEqual<int>(new int(), opts.AsType<int>("non-existing"));
            Assert.AreEqual<string>(null, opts.AsType<string>("non-existing"));
            Assert.AreEqual<Guid>(Guid.Empty, opts.AsType<Guid>("non-existing"));
            Assert.AreEqual<DateTime>(DateTime.MinValue, opts.AsType<DateTime>("non-existing"));
            Assert.AreEqual<NormalizationForm>(default(NormalizationForm), opts.AsType<NormalizationForm>("non-existing"));

            Assert.AreEqual<double>(new double(), opts.AsType<double>(0));
            Assert.AreEqual<int>(new int(), opts.AsType<int>(1));
            Assert.AreEqual<string>(null, opts.AsType<string>(2));
            Assert.AreEqual<Guid>(Guid.Empty, opts.AsType<Guid>(3));
            Assert.AreEqual<DateTime>(DateTime.MinValue, opts.AsType<DateTime>(3));
            Assert.AreEqual<NormalizationForm>(default(NormalizationForm), opts.AsType<NormalizationForm>(4));
        }

        [TestMethod]
        public void Value_basicIndexed()
        {
            string[] args =
      {
        "2", "True", "False", "name1", "uno dos", "3.2", "2008-10-10", "2008-10-10T23:45:45",
        "077e5f6f-49ba-4583-a383-64ec3156fe83", "http://hostname/path/", "FormC"
      };
            var opts = new utility.Switch(args);
            Assert.AreEqual<int>(2, opts.AsType<int>(0));
            Assert.AreEqual<double>(3.2, opts.AsType<double>(5));
            Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10), opts.AsType<DateTime>(6));
            Assert.AreEqual<DateTime>(new DateTime(2008, 10, 10, 23, 45, 45), opts.AsType<DateTime>(7));
            Assert.AreEqual<Guid>(new Guid("077e5f6f-49ba-4583-a383-64ec3156fe83"), opts.AsType<Guid>(8));
            Assert.AreEqual<Uri>(new Uri("http://hostname/path/"), opts.AsType<Uri>(9));
            Assert.AreEqual<NormalizationForm>(NormalizationForm.FormC, opts.AsType<NormalizationForm>(10));
        }

        [TestMethod]
        public void Value_object1()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/person: -name:name1 /last=last1 -age=24" });
            PersonType person = opts.AsType<PersonType>("person");
            Assert.AreEqual<string>("name1", person.Name);
            Assert.AreEqual<string>("last1", person.Last);
            Assert.AreEqual<int>(24, person.Age);
        }

        [TestMethod]
        public void Value_sql()
        {
            utility.Switch opts = new utility.Switch(new string[] { "-sql=server=.;database=db;trusted_connection=true" });
            System.Data.SqlClient.SqlConnection conn = opts.AsType<System.Data.SqlClient.SqlConnection>("sql");
            Assert.IsNotNull(conn);
        }

        [TestMethod]
        public void Value_arrayByName()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals", "val0", "val1", "-nums", "32.5", "12.75" });
            string[] vals = opts.AsType<string[]>("vals");
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<string>("val0", vals[0]);
            Assert.AreEqual<string>("val1", vals[1]);
            decimal[] nums = opts.AsType<decimal[]>("nums");
            Assert.AreEqual<int>(2, nums.Length);
            Assert.AreEqual<decimal>(32.5M, nums[0]);
            Assert.AreEqual<decimal>(12.75M, nums[1]);
        }

        [TestMethod]
        public void Value_arrayByIndex()
        {
            utility.Switch opts = new utility.Switch(new string[] { "val0 val1", "1 2 3", " -key:1 -value:11;-key=2 -value:12" });
            string[] vals = opts.AsType<string[]>(0);
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<string>("val0", vals[0]);
            Assert.AreEqual<string>("val1", vals[1]);
            int[] ints = opts.AsType<int[]>(1);
            Assert.AreEqual<int>(3, ints.Length);
            Assert.AreEqual<int>(1, ints[0]);
            Assert.AreEqual<int>(2, ints[1]);
            Assert.AreEqual<int>(3, ints[2]);
            DeviceValue[] values = opts.AsType<DeviceValue[]>(2);
            Assert.AreEqual<string>("1", values[0].Key);
            Assert.AreEqual<string>("11", values[0].Value);
            Assert.AreEqual<string>("2", values[1].Key);
            Assert.AreEqual<string>("12", values[1].Value);
        }

        [TestMethod]
        public void Value_objectarrayByIndex()
        {
            utility.Switch opts = new utility.Switch(new string[] { " -key:1 -value:11;-key=2 -value:12" });
            DeviceValue[] values = opts.AsType<DeviceValue[]>(0);
            Assert.AreEqual<string>("1", values[0].Key);
            Assert.AreEqual<string>("11", values[0].Value);
            Assert.AreEqual<string>("2", values[1].Key);
            Assert.AreEqual<string>("12", values[1].Value);
        }

        [TestMethod]
        public void Value_arrayInt()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals", "12", "21" });
            int[] vals = opts.AsType<int[]>("vals");
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<int>(12, vals[0]);
            Assert.AreEqual<int>(21, vals[1]);
        }

        [TestMethod]
        public void Value_arrayDate()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals", "2008-10-23", "2008-10-24" });
            DateTime[] vals = opts.AsType<DateTime[]>("vals");
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<DateTime>(DateTime.Parse("2008-10-23"), vals[0]);
            Assert.AreEqual<DateTime>(DateTime.Parse("2008-10-24"), vals[1]);
        }

        [TestMethod]
        public void Value_arrayUri()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals", "http://h1/a", "https://h2/b" });
            Uri[] vals = opts.AsType<Uri[]>("vals");
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<Uri>(new Uri("http://h1/a"), vals[0]);
            Assert.AreEqual<Uri>(new Uri("https://h2/b"), vals[1]);
        }

        [TestMethod]
        public void Value_arrayGuid()
        {
            string id1 = "7228ca40-3668-49e7-8ff3-1fcc1474960a";
            string id2 = "db2d826a-b365-4a9d-be6f-e7b82cace471";
            utility.Switch opts = new utility.Switch(new string[] { "/vals", id1, id2 });
            Guid[] vals = opts.AsType<Guid[]>("vals");
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<Guid>(new Guid(id1), vals[0]);
            Assert.AreEqual<Guid>(new Guid(id2), vals[1]);
        }

        [TestMethod]
        public void Value_objectarray1()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals=-key=k1 -value:v1;/key:k2 -value=v2" });
            DeviceValue[] vals = opts.AsType<DeviceValue[]>("vals");
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<string>("k1", vals[0].Key);
            Assert.AreEqual<string>("v1", vals[0].Value);
            Assert.AreEqual<string>("k2", vals[1].Key);
            Assert.AreEqual<string>("v2", vals[1].Value);
        }

        [TestMethod]
        public void Value_objectarray2()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals=-name=n1 -last:l1 -age:21;/name:n2 -last=l2 -age:31" });
            PersonType[] vals = opts.AsType<PersonType[]>("vals");
            Assert.AreEqual<int>(2, vals.Length);
            Assert.AreEqual<string>("n1", vals[0].Name);
            Assert.AreEqual<string>("l1", vals[0].Last);
            Assert.AreEqual<int>(21, vals[0].Age);
            Assert.AreEqual<string>("n2", vals[1].Name);
            Assert.AreEqual<string>("l2", vals[1].Last);
            Assert.AreEqual<int>(31, vals[1].Age);
        }

        bool IsDictionary(Type type)
        {
            Type[] roles = type.FindInterfaces((atype, gauge) => atype.ToString() == gauge.ToString(), "System.Collections.IDictionary");
            return roles.Length == 1;
        }

        [TestMethod]
        public void Value_maptype()
        {
            Assert.IsTrue(IsDictionary(typeof(System.Collections.Generic.Dictionary<string, int>)));
            Assert.IsTrue(IsDictionary(typeof(System.Collections.Generic.Dictionary<string, string>)));
            Assert.IsTrue(IsDictionary(typeof(System.Collections.DictionaryBase)));
            Assert.IsTrue(IsDictionary(typeof(System.Diagnostics.InstanceDataCollection)));
            Assert.IsFalse(IsDictionary(typeof(System.Collections.Generic.List<int>)));
            Assert.IsTrue(IsDictionary(typeof(System.Collections.Generic.SortedDictionary<int, bool>)));
            Assert.AreEqual<int>(2, typeof(System.Collections.Generic.Dictionary<string, int>).GetGenericArguments().Length);
        }

        [TestMethod]
        public void Value_map1()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals=-k1=1 -k2:2 /k3:3 -k4=4" });
            Dictionary<string, int> vals = opts.AsType<Dictionary<string, int>>("vals");
            Assert.AreEqual<int>(4, vals.Count);
            Assert.AreEqual<int>(1, vals["k1"]);
            Assert.AreEqual<int>(2, vals["k2"]);
            Assert.AreEqual<int>(3, vals["k3"]);
            Assert.AreEqual<int>(4, vals["k4"]);
        }

        [TestMethod]
        public void Value_map2()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/vals=-100=uno -200:\"dos two\" /300:tres" });
            Dictionary<int, string> vals = opts.AsType<Dictionary<int, string>>("vals");
            Assert.AreEqual<int>(3, vals.Count);
            Assert.AreEqual<string>("uno", vals[100]);
            Assert.AreEqual<string>("dos two", vals[200]);
            Assert.AreEqual<string>("tres", vals[300]);
        }

        [TestMethod]
        public void Value_map3()
        {
            utility.Switch opts = new utility.Switch(new string[] { " -100=uno -200:\"dos two\" /300:tres" });
            Dictionary<int, string> vals = opts.AsType<Dictionary<int, string>>(0);
            Assert.AreEqual<int>(3, vals.Count);
            Assert.AreEqual<string>("uno", vals[100]);
            Assert.AreEqual<string>("dos two", vals[200]);
            Assert.AreEqual<string>("tres", vals[300]);
        }

        [TestMethod, ExpectedException(typeof(ArgumentException), "Cannot ParseDictionary out of an empty or null value.")]
        public void Value_map4()
        {
            utility.Switch opts = new utility.Switch(new string[] { "-k1=1 -k2:2 /k3:3 -k4=4" });
            Dictionary<string, int> map = opts.AsType<Dictionary<string, int>>(0);
            Assert.AreEqual<int>(4, map.Count);
            Assert.AreEqual<int>(1, map["k1"]);
            Assert.AreEqual<int>(2, map["k2"]);
            Assert.AreEqual<int>(3, map["k3"]);
            Assert.AreEqual<int>(4, map["k4"]);
        }

        [TestMethod]
        public void bool_nullable_value()
        {
            string[] args = { "-Optional" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsTrue(WnObject.Optional.HasValue);
            Assert.IsTrue(WnObject.Optional.Value);
        }

        [TestMethod]
        public void bool_nullable_null()
        {
            string[] args = { "-unused" };
            WnClass WnObject = utility.Switch.AsType<WnClass>(args);
            Assert.IsFalse(WnObject.Optional.HasValue);
            Assert.IsNull(WnObject.Optional);
        }

        [TestMethod]
        public void Value_bool()
        {
            string[] args = { "-isactive", "/Log-", "-trace+", "-debug-", "/throw+" };
            var opts = new utility.Switch(args);
            Assert.IsTrue(opts.AsType<bool>("isactive"));
            Assert.IsFalse(opts.AsType<bool>("log"));
            Assert.IsTrue(opts.AsType<bool>("trace"));
            Assert.IsFalse(opts.AsType<bool>("Debug"));
            Assert.IsTrue(opts.AsType<bool>("throw"));

            Assert.AreEqual<bool?>(true, opts.AsType<bool?>("isactive"));
            Assert.AreEqual<bool?>(false, opts.AsType<bool?>("log"));
            Assert.AreEqual<bool?>(true, opts.AsType<bool?>("trace"));
            Assert.AreEqual<bool?>(false, opts.AsType<bool?>("Debug"));
            Assert.AreEqual<bool?>(true, opts.AsType<bool?>("throw"));
            Assert.IsNull(opts.AsType<bool?>("no_opt"));
        }

        [TestMethod]
        public void Value_int()
        {
            string[] args = { "-active:2", "/Log:3", "-trace=4", "-debug:5", "/throw:6" };
            var opts = new utility.Switch(args);
            Assert.AreEqual<int>(2,opts.AsType<int>("active"));
            Assert.AreEqual<int>(3,opts.AsType<int>("log"));
            Assert.AreEqual<int>(4,opts.AsType<int>("trace"));
            Assert.AreEqual<int>(5,opts.AsType<int>("Debug"));
            Assert.AreEqual<int>(6,opts.AsType<int>("throw"));

            Assert.AreEqual<int?>(2, opts.AsType<int?>("active"));
            Assert.AreEqual<int?>(3, opts.AsType<int?>("log"));
            Assert.AreEqual<int?>(4, opts.AsType<int?>("trace"));
            Assert.AreEqual<int?>(5, opts.AsType<int?>("Debug"));
            Assert.AreEqual<int?>(6, opts.AsType<int?>("throw"));
            Assert.IsNull(opts.AsType<int?>("unpresent"));
            int? v = opts.AsType<int?>("unpresent");
            Assert.IsFalse(v.HasValue);
            Assert.IsNull(v);
        }
        #endregion

        #region response file

        class InmemoryResponseProvider : utility.Switch.IResponseProvider, utility.Switch.IResponseEnumerable
        {
            List<string> lines;
            public InmemoryResponseProvider() { lines = new List<string>(new string[] { "-op:localtime" }); }
            utility.Switch.IResponseEnumerable utility.Switch.IResponseProvider.Open(string resource_name) { return this; }
            void IDisposable.Dispose() { }
            IEnumerator<string> IEnumerable<string>.GetEnumerator() { return lines.GetEnumerator(); }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return lines.GetEnumerator(); }
        }

        [TestMethod]
        public void response1()
        {
            var opts = new utility.Switch(new string[] { "@resp.txt" }, new InmemoryResponseProvider());
            Assert.AreEqual<int>(1, opts.Count);
            Assert.AreEqual<string>("localtime", opts["op"]);
        }

        #endregion

        #region Naming conflation (evaluation and reference with naming conflation)
        //See: http://wordreference.com/definition/conflation
        //By now, argument conflation does not work for members of object arguments (arguments parsed by variants of AsType<class type> method).

        [TestMethod]
        public void conflation_as_alias1()
        {
            var opts = new utility.Switch(new string[] { "/copy=file1.dat" });
            Assert.IsTrue(opts.Is("c|copy"));
            Assert.AreEqual<string>("file1.dat", opts["c|copy"]);
        }

        [TestMethod]
        public void conflation_as_alias2()
        {
            var opts = new utility.Switch(new string[] { "/c=file1.dat" });
            Assert.IsTrue(opts.Is("c|copy"));
            Assert.AreEqual<string>("file1.dat", opts["c|copy"]);
        }

        [TestMethod]
        public void conflation_as_alias_duplicate()
        {
            var opts = new utility.Switch(new string[] { "/c=file1.dat", "/copy=file2.dat" });
            Assert.IsTrue(opts.Is("c|copy"));
            Assert.AreEqual<string>("file1.dat", opts["c|copy"]);
            Assert.AreNotEqual<string>("file2.dat", opts["c|copy"]);
        }

        [TestMethod]
        public void conflation_and_switchs1()
        {
            var opts = new utility.Switch(new string[] { "/ex-" });
            Assert.IsFalse(opts.Is("e|ex|exclude"));
        }

        [TestMethod]
        public void conflation_and_switchs2()
        {
            var opts = new utility.Switch(new string[] { "/exclude-" });
            Assert.IsFalse(opts.Is("e|ex|exclude"));
        }

        [TestMethod]
        public void conflation_and_switchs_duplicate()
        {
            var opts = new utility.Switch(new string[] { "/exclude-", "-ex+" });
            Assert.IsTrue(opts.Is("e|ex|exclude"));
        }

        [TestMethod]
        public void conflation_with_typed_literals()
        {
            var opts = new utility.Switch(new string[] { "-amount=123" });
            Assert.AreEqual<int>(123, opts.AsType<int>("N|amount"));
        }

        [TestMethod]
        public void conflation_with_objects()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/agent: -name:name1 /last=last1 -age=24" });
            PersonType person = opts.AsType<PersonType>("person|agent");
            Assert.AreEqual<string>("name1", person.Name);
            Assert.AreEqual<string>("last1", person.Last);
            Assert.AreEqual<int>(24, person.Age);
        }

        [TestMethod]
        public void no_conflation_with_object_members() //TODO: By now, because CLR Attributes will help here in the next release of the Switch class.
        {
            utility.Switch opts = new utility.Switch(new string[] { "/agent: -n:name1 /last=last1 -age=24" });
            PersonType person = opts.AsType<PersonType>("person|agent");
            Assert.AreEqual<string>(null, person.Name);
            Assert.AreEqual<string>("last1", person.Last);
            Assert.AreEqual<int>(24, person.Age);
        }

        [TestMethod]
        public void suffix_of_named_with_conflation()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/favroot:path", "-exclude", "xd x", "asd", "dos", "/out:file.html", "four" });
            Assert.AreEqual<string>("path", opts["favs|favroot"]);
            Assert.IsTrue(opts.Is("exclude"));
            foreach (string arg in opts.SuffixOf("none|nonexist"))
                Assert.Fail("a non-existing argument 'none'");
            var args = new List<string>();
            foreach (string arg in opts.SuffixOf("favs|favroot"))
                args.Add(arg);
            Assert.AreEqual<int>(0, args.Count);
            foreach (string arg in opts.SuffixOf("ex|exclude"))
                args.Add(arg);
            var all = new StringBuilder();
            args.ForEach((x) => { all.Append(x); });
            Assert.AreEqual<int>(3, args.Count);
            Assert.AreEqual<string>("xd xasddos", all.ToString());

            var four = new StringBuilder();
            foreach (string arg in opts.SuffixOf("out|output"))
                four.Append(arg);
            Assert.AreEqual<string>("four", four.ToString());
        }

        [TestMethod]
        public void prefix_of_named_with_conflation()
        {
            utility.Switch opts = new utility.Switch(new string[] { "/favs:path", "-exclude", "xd x", "asd", "dos", "/out:file.html", "four" });
            Assert.AreEqual<string>("path", opts["favroot|favs"]);
            Assert.IsTrue(opts.Is("exclude"));
            foreach (string arg in opts.PrefixOf("none|nada"))
                Assert.Fail("a non-existing argument 'none'");
            var args = new List<string>();
            foreach (string arg in opts.PrefixOf("favs|favroot"))
                args.Add(arg);
            Assert.AreEqual<int>(0, args.Count);
            foreach (string arg in opts.PrefixOf("output|out"))
                args.Add(arg);
            var all = new StringBuilder();
            args.ForEach((x) => { all.Append(x); });
            Assert.AreEqual<int>(3, args.Count);
            Assert.AreEqual<string>("dosasdxd x", all.ToString());
        }

        #endregion

        #region namespaces

        [TestMethod]
        public void namespaces()
        {
            var opts = new utility.Switch(new string[] { "-a+", "-b-", "/c", "--a-", "-/b=y2", "//c", "--d=vx" });
            Assert.IsTrue(opts.Is("a"));
            Assert.IsFalse(opts.Is("b"));
            Assert.IsFalse(opts.Is("d"));
            Assert.IsNull(opts["c"]);

            Assert.AreEqual<string>("y2", opts["b", 2]);
            Assert.IsTrue(opts.Is("c", 2));
            Assert.AreEqual<string>("vx", opts["d", 2]);
        }

        [TestMethod]
        public void namespaces_AsType()
        {
            string[] args = { "/Log-", "--log+" };
            var opts = new utility.Switch(args);
            Assert.AreEqual<string>("-", opts["log"]);
            Assert.AreEqual<string>("+", opts["log", 2]);
            Assert.IsFalse(opts.AsType<bool>("log"));
            Assert.IsTrue(opts.AsType<bool>("log", 2));
        }

        [TestMethod]
        public void namespaces_with_inner_conflation()
        {
            var opts = new utility.Switch(new string[] { "-copy=file1.dat", "//copy=file2.dat", });
            Assert.AreEqual<string>("file1.dat", opts["c|copy"]);
            Assert.AreEqual<string>("file2.dat", opts["copy|c", 2]);
        }
    }

        #endregion

    [TestClass]
    public class SwitchFileInfoSpec
    {
        static readonly string FakeFile = System.IO.Path.GetTempFileName();

        [TestInitialize]
        public void setup()
        {
            using (var writer = System.IO.File.CreateText(FakeFile))
                writer.Write("fill");
        }

        [TestCleanup]
        public void cleanup()
        {
            System.IO.File.Delete(FakeFile);
        }

        [TestMethod]
        public void Value_file()
        {
            var opts = new utility.Switch(new string[] { "/file=" + FakeFile });
            System.IO.FileInfo file = opts.AsType<System.IO.FileInfo>("file");
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public void Value_fileIndexed()
        {
            var opts = new utility.Switch(new string[] { FakeFile });
            System.IO.FileInfo file = opts.AsType<System.IO.FileInfo>(0);
            Assert.IsNotNull(file);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null.")]
        public void Value_fileNull()
        {
            var opts = new utility.Switch(null);
            System.IO.FileInfo file = opts.AsType<System.IO.FileInfo>(0);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null.")]
        public void Value_fileNullNamed()
        {
            var opts = new utility.Switch(null);
            System.IO.FileInfo file = opts.AsType<System.IO.FileInfo>("file");
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null.")]
        public void Value_fileNone()
        {
            var opts = new utility.Switch(new string[] { "1" });
            System.IO.FileInfo file = opts.AsType<System.IO.FileInfo>(2);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Value_fileNoneNamed()
        {
            var mapping = new Dictionary<Type, utility.Switch.InstanceCreator>();
            mapping.Add(typeof(System.IO.FileInfo), (filename) => new System.IO.FileInfo(filename));
            var opts = new utility.Switch(new string[] { "/file=" });
            opts.ConstructorMap = mapping;
            System.IO.FileInfo file = opts.AsType<System.IO.FileInfo>("file");
            Assert.Fail();
        }
    }

    [TestClass]
    public class SwitchDirectoryInfoSpec
    {
        static readonly string DirectoryName = "switTch23";
        static readonly string FakeDirectory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), DirectoryName);

        [TestInitialize]
        public void setup()
        {
            System.IO.Directory.CreateDirectory(FakeDirectory);
        }

        [TestCleanup]
        public void cleanup()
        {
            System.IO.Directory.Delete(FakeDirectory, true);
        }

        [TestMethod]
        public void Value_dir()
        {
            var opts = new utility.Switch(new string[] { "/dir=" + FakeDirectory });
            System.IO.DirectoryInfo dir = opts.AsType<System.IO.DirectoryInfo>("dir");
            Assert.IsNotNull(dir);
            Assert.AreEqual<string>(FakeDirectory, dir.FullName);
            Assert.AreEqual<string>(DirectoryName, dir.Name);
        }

        [TestMethod]
        public void Value_dirIndexed()
        {
            var opts = new utility.Switch(new string[] { FakeDirectory });
            System.IO.DirectoryInfo dir = opts.AsType<System.IO.DirectoryInfo>(0);
            Assert.IsNotNull(dir);
            Assert.AreEqual<string>(FakeDirectory, dir.FullName);
            Assert.AreEqual<string>(DirectoryName, dir.Name);
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null.")]
        public void Value_dirNull()
        {
            var opts = new utility.Switch(null);
            System.IO.DirectoryInfo dir = opts.AsType<System.IO.DirectoryInfo>(0);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null.")]
        public void Value_dirNullNamed()
        {
            var opts = new utility.Switch(null);
            System.IO.DirectoryInfo file = opts.AsType<System.IO.DirectoryInfo>("dir");
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentNullException), "Value cannot be null.")]
        public void Value_dirNone()
        {
            var opts = new utility.Switch(new string[] { "1" });
            System.IO.DirectoryInfo file = opts.AsType<System.IO.DirectoryInfo>(2);
            Assert.Fail();
        }

        [TestMethod, ExpectedException(typeof(ArgumentException))]
        public void Value_dirNoneNamed()
        {
            var mapping = new Dictionary<Type, utility.Switch.InstanceCreator>();
            mapping.Add(typeof(System.IO.DirectoryInfo), (dirname) => new System.IO.FileInfo(dirname));
            var opts = new utility.Switch(new string[] { "/dir=" });
            opts.ConstructorMap = mapping;
            System.IO.DirectoryInfo file = opts.AsType<System.IO.DirectoryInfo>("dir");
            Assert.Fail();
        }
    }

    [TestClass]
    public class Win32ArgumentParser
    {
        /* C++\Win32

        ARGUMENT DELIMITER : whitespace (space or tab)
        ^ : ?
        "string" : Cancel whitespace - > " as STRING DELIMITER
        \" : LITERAL "
        2n\" : n\STRING DELIMITER
        [(2n)+1]\" : n\LITERAL "

        */

        /* PowerShell
       "uno "dos tres" cuatro"
        */

        [TestMethod]
        public void basic0()
        {
            string input = "uno";
            string[] args = utility.SystemArgumentParser.Parse(input);
            Assert.AreEqual<int>(1, args.Length);
            Assert.AreEqual<string>("uno", args[0]);
        }

        [TestMethod]
        public void basic1()
        {
            string input = "uno   -cuatro  ";
            string[] args = utility.SystemArgumentParser.Parse(input);
            Assert.AreEqual<int>(2, args.Length);
            Assert.AreEqual<string>("uno", args[0]);
            Assert.AreEqual<string>("-cuatro", args[1]);
        }

        [TestMethod]
        public void basic2()
        {
            string input = "uno   -cuatro  \"/dos tres\"";
            string[] args = utility.SystemArgumentParser.Parse(input);
            Assert.AreEqual<int>(3, args.Length);
            Assert.AreEqual<string>("uno", args[0]);
            Assert.AreEqual<string>("-cuatro", args[1]);
            Assert.AreEqual<string>("/dos tres", args[2]);
        }

        [TestMethod, Ignore]
        public void basic3()
        {
            string input = " uno   -cuatro  \"/dos tres\"";
            string[] args = utility.SystemArgumentParser.Parse(input);
            Assert.AreEqual<int>(3, args.Length);
            Assert.AreEqual<string>("uno", args[0]);
            Assert.AreEqual<string>("-cuatro", args[1]);
            Assert.AreEqual<string>("/dos tres", args[2]);
        }

        [TestMethod]
        public void basic()
        {
            string input = "uno \"-dos tres\" -cuatro -cinco=\"-seis=12 -siete:34\" /ocho:2";
            string[] args = utility.SystemArgumentParser.Parse(input);
            Assert.AreEqual<int>(5, args.Length);
            Assert.AreEqual<string>("uno", args[0]);
            Assert.AreEqual<string>("-dos tres", args[1]);
            Assert.AreEqual<string>("-cuatro", args[2]);
            Assert.AreEqual<string>("-cinco=-seis=12 -siete:34", args[3]);
            Assert.AreEqual<string>("/ocho:2", args[4]);
        }

        [TestMethod, Ignore]
        public void NameAndSpace()
        {
            string[] args = utility.SystemArgumentParser.Parse("/name=\"\"\"John Doe\"\"\" /age=21");
            //Assert.AreEqual<int>(1, args.Length);
            Assert.AreEqual<string>("/name=\"John", args[0]);
            //Assert.AreEqual<string>("Doe /age=21", args[1]);
        }

        [TestMethod, Ignore]
        public void quote0()
        {
            string[] args = utility.SystemArgumentParser.Parse("\x22\x22\x22uno\x22\x22\x22");
            Assert.AreEqual<string>("\x22uno\x22", args[0]);
        }

        [TestMethod, Ignore]
        public void quote1()
        {
            char[] t = { 'A', '\x22', 'u', 'n', 'o', '\x20', '\x22', 'd', 'o', 's' };
            string s = new string(t);
            string[] args = utility.SystemArgumentParser.Parse(s);
            Assert.AreEqual<int>(1, args.Length);

            //string[] args = utility.SystemArgumentParser.Parse("\x22uno \x22"+"dos tres\x22 cuatro\x22");
            //Assert.AreEqual<string>("uno dos", args[0]);
            //Assert.AreEqual<string>("tres cuatro", args[1]);
        }

        [TestMethod, Ignore]
        public void quote2()
        {
            string[] args = utility.SystemArgumentParser.Parse("\x5C\x22");
            Assert.AreEqual<int>(1, args.Length);
            Assert.AreEqual<string>("\x5C\x22", args[0]);
        }

        [TestMethod, Ignore]
        public void quote2_1()
        {
            string[] args = utility.SystemArgumentParser.Parse("\x5C\x22\x22");
            Assert.AreEqual<int>(1, args.Length);
            Assert.AreEqual<string>("\x5C\x22\x22", args[0]);
        }

        [TestMethod, Ignore]
        public void quote2_2()
        {
            string[] args = utility.SystemArgumentParser.Parse("/\x22\x22");
            Assert.AreEqual<int>(1, args.Length);
            Assert.AreEqual<string>("/\x22\x22", args[0]);
        }

        [TestMethod]
        public void quote3()
        {
            string[] args = utility.SystemArgumentParser.Parse("\x22\x22");
            Assert.AreEqual<int>(1, args.Length);
            Assert.AreEqual<string>("", args[0]);
        }
    }
}