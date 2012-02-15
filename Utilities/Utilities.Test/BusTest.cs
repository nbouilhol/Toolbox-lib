using Utilities.DomainCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Utilities.Test
{


    /// <summary>
    ///Classe de test pour BusTest, destinée à contenir tous
    ///les tests unitaires BusTest
    ///</summary>
    [TestClass()]
    public class BusTest
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


        ///// <summary>
        /////Test pour ValidateAsync
        /////</summary>
        //public void ValidateAsyncTestHelper<TCommand>()
        //    where TCommand : ICommand
        //{
        //    IRegistrationService registrationService = null; // TODO: initialisez à une valeur appropriée
        //    Bus target = new Bus(registrationService); // TODO: initialisez à une valeur appropriée
        //    TCommand command = default(TCommand); // TODO: initialisez à une valeur appropriée
        //    Task<IEnumerable<ValidationResult>> expected = null; // TODO: initialisez à une valeur appropriée
        //    Task<IEnumerable<ValidationResult>> actual;
        //    actual = target.ValidateAsync<TCommand>(command);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        //}

        //[TestMethod()]
        //public void ValidateAsyncTest()
        //{
        //    Assert.Inconclusive("Impossible de trouver un paramètre de type approprié qui satisfait aux contrainte" +
        //            "s de type de TCommand. Appelez ValidateAsyncTestHelper<TCommand>() avec les para" +
        //            "mètres de type appropriés.");
        //}

        ///// <summary>
        /////Test pour Validate
        /////</summary>
        //public void ValidateTestHelper<TCommand>()
        //    where TCommand : ICommand
        //{
        //    IRegistrationService registrationService = null; // TODO: initialisez à une valeur appropriée
        //    Bus target = new Bus(registrationService); // TODO: initialisez à une valeur appropriée
        //    TCommand command = default(TCommand); // TODO: initialisez à une valeur appropriée
        //    IEnumerable<ValidationResult> expected = null; // TODO: initialisez à une valeur appropriée
        //    IEnumerable<ValidationResult> actual;
        //    actual = target.Validate<TCommand>(command);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        //}

        //[TestMethod()]
        //public void ValidateTest()
        //{
        //    Assert.Inconclusive("Impossible de trouver un paramètre de type approprié qui satisfait aux contrainte" +
        //            "s de type de TCommand. Appelez ValidateTestHelper<TCommand>() avec les paramètre" +
        //            "s de type appropriés.");
        //}

        ///// <summary>
        /////Test pour ExecuteAsync
        /////</summary>
        //public void ExecuteAsyncTestHelper<TCommand>()
        //    where TCommand : ICommand
        //{
        //    IRegistrationService registrationService = null; // TODO: initialisez à une valeur appropriée
        //    Bus target = new Bus(registrationService); // TODO: initialisez à une valeur appropriée
        //    TCommand command = default(TCommand); // TODO: initialisez à une valeur appropriée
        //    Task<ICommandResult> expected = null; // TODO: initialisez à une valeur appropriée
        //    Task<ICommandResult> actual;
        //    actual = target.ExecuteAsync<TCommand>(command);
        //    Assert.AreEqual(expected, actual);
        //    Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        //}

        //[TestMethod()]
        //public void ExecuteAsyncTest()
        //{
        //    Assert.Inconclusive("Impossible de trouver un paramètre de type approprié qui satisfait aux contrainte" +
        //            "s de type de TCommand. Appelez ExecuteAsyncTestHelper<TCommand>() avec les param" +
        //            "ètres de type appropriés.");
        //}

        [TestMethod()]
        public void ExecuteTest()
        {
            IRegistrationService registrationService = new CommandRegistrations().Add(new HandlerBase()).Add(new Handler());
            Bus target = new Bus(registrationService);
            ICommand command = new FooCommand { Id = 3 };
            ICommandResult expected = null;
            ICommandResult actual = target.Execute(command);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Vérifiez l\'exactitude de cette méthode de test.");
        }

        public class BaseCommand : ICommand
        {
        }

        public class FooCommand : BaseCommand
        {
            public int Id { get; set; }
        }

        private class HandlerBase : ICommandHandler<FooCommand>
        {
            public ICommandResult Handle(FooCommand command)
            {
                return null;
            }
        }

        private class Handler : HandlerBase
        {
        }

        private class NonGenericHandler : ICommandHandler<FooCommand>
        {
            public ICommandResult Handle(FooCommand command)
            {
                throw new NotImplementedException();
            }
        }
    }
}
