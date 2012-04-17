using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Caches;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace XunitContrib.Runner.ReSharper.UnitTestProvider
{
    [SolutionComponent]
    public class UnitTestElementFactory
    {
        private readonly XunitTestProvider provider;
        private readonly UnitTestElementManager unitTestManager;
        private readonly CacheManager cacheManager;
        private readonly PsiModuleManager psiModuleManager;

        public UnitTestElementFactory(XunitTestProvider provider, UnitTestElementManager unitTestManager, CacheManager cacheManager, PsiModuleManager psiModuleManager)
        {
            this.provider = provider;
            this.unitTestManager = unitTestManager;
            this.cacheManager = cacheManager;
            this.psiModuleManager = psiModuleManager;
        }

        public XunitTestClassElement GetOrCreateTestClass(IProject project, IClrTypeName typeName, string assemblyLocation)
        {
            var id = "xunit:" + typeName.FullName;
            var element = unitTestManager.GetElementById(project, id);
            if (element != null)
            {
                element.State = UnitTestElementState.Valid;
                return element as XunitTestClassElement;
            }

            return new XunitTestClassElement(provider, new ProjectModelElementEnvoy(project), cacheManager, psiModuleManager, id, typeName.GetPersistent(), assemblyLocation);
        }

        public XunitTestMethodElement GetOrCreateTestMethod(IProject project, XunitTestClassElement testClassElement, IClrTypeName typeName, string methodName, string skipReason)
        {
            var baseTypeName = string.Empty;
            if (!testClassElement.TypeName.Equals(typeName))
                baseTypeName = typeName.ShortName + ".";

            var id = string.Format("xunit:{0}.{1}{2}", testClassElement.TypeName.FullName, baseTypeName, methodName);
            var element = unitTestManager.GetElementById(project, id);
            if (element != null)
            {
                element.State = UnitTestElementState.Valid;
                return element as XunitTestMethodElement;
            }

            return new XunitTestMethodElement(provider, testClassElement, new ProjectModelElementEnvoy(project), cacheManager, psiModuleManager, id, typeName.GetPersistent(), methodName, skipReason);
        }

        public XunitInheritedTestMethodContainerElement GetOrCreateInheritedTestMethodContainer(IProject project, IClrTypeName typeName, string methodName)
        {
            var id = string.Format("xunit:{0}.{1}", typeName.FullName, methodName);
            var element = unitTestManager.GetElementById(project, id);
            if (element != null)
                return element as XunitInheritedTestMethodContainerElement;

            return new XunitInheritedTestMethodContainerElement(provider, project, id, typeName.GetPersistent(), methodName);
        }
    }
}