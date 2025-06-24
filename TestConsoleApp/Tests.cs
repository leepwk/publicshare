using System;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Internal;
using TestConsoleApp.Interfaces;
using TestConsoleApp.Models;

namespace TestConsoleApp
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [TestCase(1, 1L)]
        [TestCase(2, 2L)]
        [TestCase(3, 6L)]
        [TestCase(4, 24L)]
        [TestCase(5, 120L)]
        [TestCase(6, 720L)]
        [TestCase(7, 5040L)]
        [TestCase(8, 40320L)]
        [TestCase(9, 362880L)]
        [TestCase(10, 3628800L)]
        [TestCase(11, 39916800L)]
        [TestCase(12, 479001600L)]
        [TestCase(13, 6227020800L)]
        [TestCase(14, 87178291200L)]
        [TestCase(15, 1307674368000L)]
        public void FactorialRecursiveTest(int n, long expectedResult)
        {
            var result = new FactorialRecursive().Calculate(n);
            result.ShouldBe(expectedResult);
        }

        [TestCase(1, 1L)]
        [TestCase(2, 2L)]
        [TestCase(3, 6L)]
        [TestCase(4, 24L)]
        [TestCase(5, 120L)]
        [TestCase(6, 720L)]
        [TestCase(7, 5040L)]
        [TestCase(8, 40320L)]
        [TestCase(9, 362880L)]
        [TestCase(10, 3628800L)]
        [TestCase(11, 39916800L)]
        [TestCase(12, 479001600L)]
        [TestCase(13, 6227020800L)]
        [TestCase(14, 87178291200L)]
        [TestCase(15, 1307674368000L)]
        public void FactorialIterativeTest(int n, long expectedResult)
        {
            var result = new FactorialIterative().Calculate(n);
            result.ShouldBe(expectedResult);
        }

        [TestCase("rotor", true)]
        [TestCase("roToR", true)]
        [TestCase("Racecar", true)]
        [TestCase("redivIder", true)]
        [TestCase("REDDER", true)]
        [TestCase("REDDEr", true)]
        [TestCase("R3DD3r", true)]
        [TestCase("example", false)]
        [TestCase("Elephant", false)]
        public void PalindromeIterativeTest(string word, bool expectedResult)
        {
            var result = new Palindrome().IsPalindrome(word);
            result.ShouldBe(expectedResult);
        }

        [TestCase("rotor", true)]
        [TestCase("roToR", true)]
        [TestCase("Racecar", true)]
        [TestCase("redivIder", true)]
        [TestCase("REDDER", true)]
        [TestCase("REDDEr", true)]
        [TestCase("R3DD3r", true)]
        [TestCase("example", false)]
        [TestCase("Elephant", false)]
        public void PalindromeEfficientTest(string word, bool expectedResult)
        {
            var result = new PalindromeEfficient().IsPalindrome(word);
            result.ShouldBe(expectedResult);
        }

        [TestCase("DCCLXXXIX", "789")]
        [TestCase("MCMXCIV", "1994")]
        [TestCase("MMXII", "2012")]
        [TestCase("MCDLXX", "1470")]
        [TestCase("MDCCLVI ", "1756")]
        [TestCase("IIV", "")]
        [TestCase("VVII", "")]
        [TestCase("XXXX", "")]
        [TestCase("II:MMXII", "022012")]
        [TestCase("IV:II:MMXII", "04022012")]
        [TestCase("XIV:VIII:MMXII", "14082012")]
        public void RomansTest(string romanText, string expectedResult)
        {
            var result = new Roman().Convert(romanText);
            result.ShouldBe(expectedResult);
        }


        public static IEnumerable<TestCaseData> TreeNodeTestCases()
        {
            yield return TestData.TreeData1();
            yield return TestData.TreeData2();
            yield return TestData.TreeData3();
            yield return TestData.TreeData4();
        }
        public static IEnumerable<TestCaseData> TreeNodeRecursiveTestCases()
        {
            var isRecursive = true;
            yield return TestData.TreeData1(isRecursive);
            yield return TestData.TreeData2(isRecursive);
            yield return TestData.TreeData3(isRecursive);
            yield return TestData.TreeData4(isRecursive);
        }

        [Test, TestCaseSource(nameof(TreeNodeTestCases))]
        public void TreeProcess(ITree tree, int expectedNodesCount, int expectedTotalValue)
        {
            tree.Process().NodesCount.ShouldBe(expectedNodesCount);
            tree.Process().TotalValues.ShouldBe(expectedTotalValue);
        }

        [Test, TestCaseSource(nameof(TreeNodeRecursiveTestCases))]
        public void TreeRecursiveProcess(ITree tree, int expectedNodesCount, int expectedTotalValue)
        {
            tree.Process().NodesCount.ShouldBe(expectedNodesCount);
            tree.Process().TotalValues.ShouldBe(expectedTotalValue);
        }

        [TestCase(5)]
        [TestCase(8)]
        public void PostcodesTest(int numOf)
        {
            var result = new Postcodes().Process(numOf);

            result.Count.ShouldBe(numOf);
            result[0].ShouldContain("EC4N");
            result[1].ShouldContain("W1D");
        }

        public static IEnumerable<TestCaseData> ScrabbleTestCases()
        {
            yield return TestData.ScrabbleData1();
        }
        
        [Test, TestCaseSource(nameof(ScrabbleTestCases))]
        public void ScrabbleTest(List<WordItem> wordItems, int expectedNumOf)
        {
            var result = new Scrabble().Process(wordItems.Select(s => s.Word).ToList(), expectedNumOf);

            result.Count.ShouldBe(expectedNumOf);
            result[0].Word.ShouldBe("Zipper");
            result[0].Points.ShouldBe(19);
            result[1].Word.ShouldBe("Delightful");
            result[1].Points.ShouldBe(18);
        }

        public static IEnumerable<TestCaseData> TemperatureTestCases()
        {
            yield return TestData.TemperatureData1();
            yield return TestData.TemperatureData2();
        }

        [Test, TestCaseSource(nameof(TemperatureTestCases))]
        public void TemperatureTest(string[] cityTemps, int targetMonth, string expectedResult)
        {
            var result = new Temperature().Process(cityTemps, targetMonth);

            result.ShouldNotBeEmpty();
            result.ShouldBe(expectedResult);
        }

        public static IEnumerable<TestCaseData> OrdersTestCases()
        {
            yield return TestData.OrdersData1();
        }

        [Test, TestCaseSource(nameof(OrdersTestCases))]
        public void OrdersTest(List<OrderEvent> events, List<string> expectedResult)
        {
            var result = new Orders().FindTraders(events);

            result.ShouldNotBeEmpty();
            result.ShouldBe(expectedResult);
        }

        public static IEnumerable<TestCaseData> GalaxyTestCases()
        {
            yield return TestData.GalaxyData1();
            yield return TestData.GalaxyData2();
        }

        [Test, TestCaseSource(nameof(GalaxyTestCases))]
        public void GalaxyTest(Tuple<List<List<int>>, int> levels, bool success, int expectedSum)
        {
            var result = new BreadthTraverse().Escape(levels.Item1, levels.Item2);

            result.Item1.ShouldBe(success);
            result.Item2.Sum().ShouldBe(expectedSum);
        }
    }
}
