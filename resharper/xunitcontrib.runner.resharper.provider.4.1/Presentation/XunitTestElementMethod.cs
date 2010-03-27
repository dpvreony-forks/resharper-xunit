using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.CamelTyping;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestExplorer;

namespace XunitContrib.Runner.ReSharper.UnitTestProvider
{
    internal class XunitTestElementMethod : XunitTestElement
    {
        readonly XunitTestElementClass @class;
        readonly string methodName;
        readonly int order;

        internal XunitTestElementMethod(IUnitTestProvider provider,
                                      XunitTestElementClass @class,
                                      IProjectModelElement project,
                                      string declaringTypeName,
                                      string methodName,
                                      int order)
            : base(provider, @class, project, declaringTypeName)
        {
            this.@class = @class;
            this.order = order;
            this.methodName = methodName;
        }

        internal XunitTestElementClass Class
        {
            get { return @class; }
        }

        internal string MethodName
        {
            get { return methodName; }
        }

        internal int Order
        {
            get { return order; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                var elementMethod = (XunitTestElementMethod)obj;

                if (Equals(@class, elementMethod.@class))
                    return (methodName == elementMethod.methodName);
            }

            return false;
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            var declaredType = GetDeclaredType();
            if (declaredType != null)
            {
                return (from member in MiscUtil.EnumerateMembers(declaredType, methodName, true)
                        let method = member as IMethod
                        where method != null && !method.IsAbstract && method.TypeParameters.Length <= 0 && method.AccessibilityDomain.DomainType == AccessibilityDomain.AccessibilityDomainType.PUBLIC
                        select member).FirstOrDefault();
            }

            return null;
        }

        public override string GetKind()
        {
            return "xUnit.net Test";
        }

        public override string GetTitle()
        {
            return string.Format("{0}.{1}", @class.GetTitle(), methodName);
        }

        public override bool Matches(string filter, PrefixMatcher matcher)
        {
            return GetCategories().Any(category => matcher.IsMatch(category.Name)) || @class.Matches(filter, matcher) || matcher.IsMatch(methodName);
        }
    }
}