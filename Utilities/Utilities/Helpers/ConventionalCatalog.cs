using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;

namespace Utilities.Helpers
{
    public class ConventionalCatalog : ComposablePartCatalog
    {
        private ConcurrentBag<ComposablePartDefinition> _parts = new ConcurrentBag<ComposablePartDefinition>();

        public void RegisterType<TImplementation, TContract>()
        {
            ComposablePartDefinition part = ReflectionModelServices.CreatePartDefinition(new Lazy<Type>(() => typeof(TImplementation)), false, new Lazy<IEnumerable<ImportDefinition>>(() => GetImportDefinitions(typeof(TImplementation))), new Lazy<IEnumerable<ExportDefinition>>(() => GetExportDefinitions(typeof(TImplementation), typeof(TContract))), new Lazy<IDictionary<string, object>>(() => new Dictionary<string, object>()), null);

            _parts.Add(part);
        }

        public void RegisterType(Type concreteType, Type interfaceType)
        {
            ComposablePartDefinition part = ReflectionModelServices.CreatePartDefinition(new Lazy<Type>(() => concreteType), false, new Lazy<IEnumerable<ImportDefinition>>(() => GetImportDefinitions(concreteType)), new Lazy<IEnumerable<ExportDefinition>>(() => GetExportDefinitions(concreteType, interfaceType)), new Lazy<IDictionary<string, object>>(() => new Dictionary<string, object>()), null);

            _parts.Add(part);
        }

        private ImportDefinition[] GetImportDefinitions(Type implementationType)
        {
            ConstructorInfo constructors = implementationType.GetConstructors().First();
            List<ImportDefinition> imports = new List<ImportDefinition>();

            foreach (ParameterInfo param in constructors.GetParameters())
            {
                ImportCardinality cardinality = GetCardinality(param);
                Type importType = cardinality == ImportCardinality.ZeroOrMore ? GetCollectionContractType(param.ParameterType) : param.ParameterType;

                imports.Add(ReflectionModelServices.CreateImportDefinition(new Lazy<ParameterInfo>(() => param), AttributedModelServices.GetContractName(importType), AttributedModelServices.GetTypeIdentity(importType), Enumerable.Empty<KeyValuePair<string, Type>>(), cardinality, CreationPolicy.Any, null));
            }

            return imports.ToArray();
        }

        private ImportCardinality GetCardinality(ParameterInfo param)
        {
            return typeof(IEnumerable).IsAssignableFrom(param.ParameterType) ? ImportCardinality.ZeroOrMore : ImportCardinality.ExactlyOne;
        }

        private Type GetCollectionContractType(Type collectionType)
        {
            Type itemType = collectionType.GetGenericArguments().First();
            Type contractType = itemType.GetGenericArguments().First();

            return contractType;
        }

        private ExportDefinition[] GetExportDefinitions(Type implementationType, Type contractType)
        {
            LazyMemberInfo lazyMember = new LazyMemberInfo(implementationType);
            string contracName = AttributedModelServices.GetContractName(contractType);
            Lazy<IDictionary<string, object>> metadata = new Lazy<IDictionary<string, object>>(() =>
                new Dictionary<string, object> { { CompositionConstants.ExportTypeIdentityMetadataName, AttributedModelServices.GetTypeIdentity(contractType) } });

            return new ExportDefinition[] { ReflectionModelServices.CreateExportDefinition(lazyMember, contracName, metadata, null) };
        }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return _parts.AsQueryable(); }
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return base.GetExports(definition);
        }
    }
}