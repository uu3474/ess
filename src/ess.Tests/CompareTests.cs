using NUnit.Framework;
using System.Collections.Generic;

namespace ess.Tests
{
    public class CompareTests
    {
        [Test]
        public void CompareTest1()
        {
            var string1 = new ProblemString("2. AAA");
            var string2 = new ProblemString("9. B");
            var string3 = new ProblemString("1. AAA");

            var strings = new List<ProblemString> { string1, string2, string3 };
            strings.Sort(ProblemStringComparer.Default);

            Assert.AreEqual(string3, strings[0]);
            Assert.AreEqual(string1, strings[1]);
            Assert.AreEqual(string2, strings[2]);
        }

    }

}