using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace org.kharynic.Scripting
{
    internal abstract class GeneratorBase
    {
        protected const string ThisPtrVar = "thisPtr";
        protected const string PtrSuffix = "Ptr";
        protected virtual bool ProtectEditor => true;
        protected readonly Type TargetType;
        protected readonly IEnumerable<MethodInfo> Methods;
        private readonly string _rootNamespace;
        
        public void Run()
        {
            var code = GenerateCode();
            GeneratorUtils.WriteFile(code, Path, ProtectEditor);
        }

        protected GeneratorBase(Type targetType, string rootNamespace)
        {
            TargetType = targetType;
            _rootNamespace = rootNamespace;
            Methods = targetType
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ScriptableAttribute>() != null);
        }

        public virtual string Path =>
            new Regex($"^{_rootNamespace}")
                .Replace(TargetType.Namespace ?? "", "")
                .Replace(".", "/") + $"/{TargetType.Name}.generated";

        protected abstract string GenerateCode();
    }
}