using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.DynamicXml;
using Utilities.Test.Properties;

namespace Utilities.Test
{
    /// <summary>
    ///Classe de test pour DynamicXmlExtensionsTest, destinée à contenir tous
    ///les tests unitaires DynamicXmlExtensionsTest
    ///</summary>
    [TestClass()]
    public class DynamicXmlExtensionsTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active ainsi que ses fonctionnalités.
        ///</summary>
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
        public void ParseTest()
        {
            Stream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(Resources.hypotheses));
            IList<HypotheseDTO> actual = Map(DynamicXmlExtensions.Parse(stream));
            Assert.IsTrue(actual.Count == 4);
        }

        private static IList<HypotheseDTO> Map(dynamic config)
        {
            IEnumerable<dynamic> associations = config.associations;
            return associations.SelectMany(association => BuildHypotheses((int)association.CibleCodeFunc, (string)association.NoGaiAnnonceur.ToString(), ((IEnumerable<dynamic>)config.hypotheses))).ToList();
        }

        private static IList<HypotheseDTO> BuildHypotheses(int noGaiAnnonceur, string cible, IEnumerable<dynamic> hypotheses)
        {
            return hypotheses.Select(hypothese => new HypotheseDTO
            {
                Name = hypothese.Name,
                NoGaiAnnonceur = noGaiAnnonceur,
                CibleCodeFunc = cible,
                AchatCL2 = hypothese.AchatCL2,
                AchatOpportuniteHorsAO15 = hypothese.AchatOpportuniteHorsAO15,
                AchatOpportunite = hypothese.AchatOpportunite,
                Gratuits = hypothese.Gratuits.Cast<string>(),
                CodesEcransExclus = hypothese.CodesEcransExclus.Cast<short>(),
                TauxPassage = hypothese.TauxPassage,
                RentaPreviousYear = hypothese.RentaPreviousYear,
                Year = 2012
            }).ToList();
        }
    }
}
