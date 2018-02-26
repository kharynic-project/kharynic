using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace org.kharynic.Scripting
{
    internal abstract class GeneratorBase
    {
        protected const string ThisPtrVar = "thisPtr";
        protected const string PtrSuffix = "Ptr";
        protected virtual bool ProtectEditor => true;
        protected readonly Type TargetType;
        protected readonly string RootNamespace;
        protected readonly IEnumerable<MethodInfo> Methods;

        public void Run()
        {
            var code = GenerateCode();
            var path = GetPath();
            GeneratorUtils.WriteFile(code, path, ProtectEditor);
        }

        protected GeneratorBase(Type targetType, string rootNamespace)
        {
            TargetType = targetType;
            RootNamespace = rootNamespace;
            Methods = targetType
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ScriptableAttribute>() != null);
        }

        protected abstract string GenerateCode();
        protected abstract string GetPath();
    }
}