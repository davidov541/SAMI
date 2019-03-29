using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Debug;
using SAMI.Test.Utilities;

namespace SAMI.Test.Apps
{
    [DeploymentItem("DebugGrammar.grxml")]
    [TestClass]
    public class NumberGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void NumberGrammar0()
        {
            TestNumber("zero", "0");
        }

        [TestMethod]
        public void NumberGrammar1()
        {
            TestNumber("one", "1");
        }

        [TestMethod]
        public void NumberGrammar2()
        {
            TestNumber("two", "2");
        }

        [TestMethod]
        public void NumberGrammar3()
        {
            TestNumber("three", "3");
        }

        [TestMethod]
        public void NumberGrammar4()
        {
            TestNumber("four", "4");
        }

        [TestMethod]
        public void NumberGrammar5()
        {
            TestNumber("five", "5");
        }

        [TestMethod]
        public void NumberGrammar6()
        {
            TestNumber("six", "6");
        }

        [TestMethod]
        public void NumberGrammar7()
        {
            TestNumber("seven", "7");
        }

        [TestMethod]
        public void NumberGrammar8()
        {
            TestNumber("eight", "8");
        }

        [TestMethod]
        public void NumberGrammar9()
        {
            TestNumber("nine", "9");
        }

        [TestMethod]
        public void NumberGrammar10()
        {
            TestNumber("ten", "10");
        }

        [TestMethod]
        public void NumberGrammar11()
        {
            TestNumber("eleven", "11");
        }

        [TestMethod]
        public void NumberGrammar12()
        {
            TestNumber("twelve", "12");
        }

        [TestMethod]
        public void NumberGrammar13()
        {
            TestNumber("thirteen", "13");
        }

        [TestMethod]
        public void NumberGrammar14()
        {
            TestNumber("fourteen", "14");
        }

        [TestMethod]
        public void NumberGrammar15()
        {
            TestNumber("fifteen", "15");
        }

        [TestMethod]
        public void NumberGrammar16()
        {
            TestNumber("sixteen", "16");
        }

        [TestMethod]
        public void NumberGrammar17()
        {
            TestNumber("seventeen", "17");
        }

        [TestMethod]
        public void NumberGrammar18()
        {
            TestNumber("eighteen", "18");
        }

        [TestMethod]
        public void NumberGrammar19()
        {
            TestNumber("nineteen", "19");
        }

        [TestMethod]
        public void NumberGrammar20()
        {
            TestNumber("twenty", "20");
        }

        [TestMethod]
        public void NumberGrammar21()
        {
            TestNumber("twenty one", "21");
        }

        [TestMethod]
        public void NumberGrammar22()
        {
            TestNumber("twenty two", "22");
        }

        [TestMethod]
        public void NumberGrammar23()
        {
            TestNumber("twenty three", "23");
        }

        [TestMethod]
        public void NumberGrammar24()
        {
            TestNumber("twenty four", "24");
        }

        [TestMethod]
        public void NumberGrammar25()
        {
            TestNumber("twenty five", "25");
        }

        [TestMethod]
        public void NumberGrammar26()
        {
            TestNumber("twenty six", "26");
        }

        [TestMethod]
        public void NumberGrammar27()
        {
            TestNumber("twenty seven", "27");
        }

        [TestMethod]
        public void NumberGrammar28()
        {
            TestNumber("twenty eight", "28");
        }

        [TestMethod]
        public void NumberGrammar29()
        {
            TestNumber("twenty nine", "29");
        }

        [TestMethod]
        public void NumberGrammar30()
        {
            TestNumber("thirty", "30");
        }

        [TestMethod]
        public void NumberGrammar40()
        {
            TestNumber("forty", "40");
        }

        [TestMethod]
        public void NumberGrammar50()
        {
            TestNumber("fifty", "50");
        }

        [TestMethod]
        public void NumberGrammar60()
        {
            TestNumber("sixty", "60");
        }

        [TestMethod]
        public void NumberGrammar70()
        {
            TestNumber("seventy", "70");
        }

        [TestMethod]
        public void NumberGrammar80()
        {
            TestNumber("eighty", "80");
        }

        [TestMethod]
        public void NumberGrammar90()
        {
            TestNumber("ninety", "90");
        }

        [TestMethod]
        public void NumberGrammar100()
        {
            TestNumber("one hundred", "100");
        }

        [TestMethod]
        public void NumberGrammar501()
        {
            TestNumber("five hundred one", "501");
        }

        [TestMethod]
        public void NumberGrammar581()
        {
            TestNumber("five hundred eighty one", "581");
        }

        [TestMethod]
        public void NumberGrammar1000()
        {
            TestNumber("one thousand", "1000");
        }

        [TestMethod]
        public void NumberGrammar1005()
        {
            TestNumber("one thousand five", "1005");
        }

        [TestMethod]
        public void NumberGrammar1050()
        {
            TestNumber("one thousand fifty", "1050");
        }

        [TestMethod]
        public void NumberGrammar1500()
        {
            TestNumber("one thousand five hundred", "1500");
        }

        [TestMethod]
        public void NumberGrammar1506()
        {
            TestNumber("one thousand five hundred six", "1506");
        }

        [TestMethod]
        public void NumberGrammar1550()
        {
            TestNumber("one thousand five hundred fifty", "1550");
        }

        [TestMethod]
        public void NumberGrammar1556()
        {
            TestNumber("one thousand five hundred fifty six", "1556");
        }

        [TestMethod]
        public void NumberGrammar10006()
        {
            TestNumber("ten thousand six", "10006");
        }

        [TestMethod]
        public void NumberGrammar10456()
        {
            TestNumber("ten thousand four hundred fifty six", "10456");
        }

        [TestMethod]
        public void NumberGrammar100006()
        {
            TestNumber("one hundred thousand six", "100006");
        }

        [TestMethod]
        public void NumberGrammar123456()
        {
            TestNumber("one hundred twenty three thousand four hundred fifty six", "123456");
        }

        [TestMethod]
        public void NumberGrammar9876543210()
        {
            TestNumber("nine eight seven six five four three two one zero", "9876543210");
        }

        [TestMethod]
        public void PercentageGrammar0()
        {
            TestNumber("zero percent", "0");
        }

        [TestMethod]
        public void PercentageGrammar23()
        {
            TestNumber("twenty three percent", "23");
        }

        [TestMethod]
        public void PercentageGrammar100()
        {
            TestNumber("one hundred percent", "100");
        }

        private void TestNumber(String command, String number)
        {
            Dictionary<String, String> expectedOutput = new Dictionary<string, string>
            {
                { "Command", "debug" },
                { "Param", number }
            };
            TestGrammar<DebugApp>("echo " + command, expectedOutput);
        }
    }
}
