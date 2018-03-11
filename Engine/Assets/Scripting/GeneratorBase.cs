using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Kharynic.Engine.Scripting
{
    internal abstract class GeneratorBase
    {
        protected const string ThisPtrVar = "thisPtr";
        protected const string PtrSuffix = "Ptr";
        protected virtual bool ProtectEditor => true;
        protected readonly Type TargetType;
        protected readonly IEnumerable<MethodInfo> Methods;

        public void Run()
        {
            var code = GenerateCode();
            GeneratorUtils.WriteFile(code, Path, ProtectEditor);
        }

        protected GeneratorBase(Type targetType)
        {
            TargetType = targetType;
            Methods = targetType
                .GetMethods()
                .Where(m => m.GetCustomAttribute<ScriptableAttribute>() != null);
            Methods = Methods.Concat(targetType
                .GetProperties()
                .Where(m => m.GetCustomAttribute<ScriptableAttribute>() != null)
                .SelectMany(p => p.GetAccessors()));
        }

        public abstract string Path { get; }

        protected abstract string GenerateCode();
    }
}