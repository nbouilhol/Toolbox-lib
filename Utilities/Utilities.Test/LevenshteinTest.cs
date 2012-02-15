using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Utilities.Test
{
    [TestClass()]
    public class LevenshteinTest
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        // 
        //Vous pouvez utiliser les attributs supplémentaires suivants lorsque vous écrivez vos tests :
        //
        //Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test dans la classe
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Utilisez ClassCleanup pour exécuter du code après que tous les tests ont été exécutés dans une classe
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void Test_CalculateDistance_With_Two_Empty_String()
        {
            Assert.AreEqual(0, Levenshtein.CalculateDistance(string.Empty, string.Empty));
        }

        [TestMethod()]
        public void Test_CalculateDistance_With_Empty_First_String()
        {
            Assert.AreEqual(6, Levenshtein.CalculateDistance(string.Empty, "kitten"));
        }

        [TestMethod()]
        public void Test_CalculateDistance_With_Empty_Second_String()
        {
            Assert.AreEqual(6, Levenshtein.CalculateDistance("kitten", string.Empty));
        }

        [TestMethod()]
        public void Test_CalculateDistance_With_Missing_Characters()
        {
            Assert.AreEqual(2, Levenshtein.CalculateDistance("kitten", "kitt"));
        }

        [TestMethod()]
        public void Test_CalculateDistance_With_Wrong_Characters()
        {
            Assert.AreEqual(1, Levenshtein.CalculateDistance("kitten", "kittyn"));
        }

        [TestMethod()]
        public void Test_CalculateDistance_With_Too_Much_Characters()
        {
            Assert.AreEqual(5, Levenshtein.CalculateDistance("kitten", "kittenkitty"));
        }

        [TestMethod()]
        public void Test_CalculateDistance_With_Equal_Strings()
        {
            Assert.AreEqual(0, Levenshtein.CalculateDistance("kitten", "kitten"));
        }

        //[TestMethod()]
        //public void Bench_CalculateDistance()
        //{
        //    Stopwatch s = new Stopwatch();
        //    s.Start();
        //    //for (int i = 0; i < 1000000; i++) Levenshtein.CalculateDistance("kitten", "kittyn");
        //    for (int i = 0; i < 1000000; i++) Levenshtein.CalculateDistance("kitten", "kittyn");
        //    s.Stop();
        //    Assert.IsFalse(false);
        //}
    }
}
