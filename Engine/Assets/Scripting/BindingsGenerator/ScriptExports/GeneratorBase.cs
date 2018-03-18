using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kharynic.Engine.Scripting.BindingsGenerator.ScriptExports
{
    internal abstract class GeneratorBase
    {
        protected readonly Type TargetType;
        protected readonly IReadOnlyList<Function> Functions;
        private static readonly Regex AttributesPattern = new Regex(@"/\*\*?@[Ee]xport\*/(?<isStatic>static )?");
        private static readonly Regex NamePattern = new Regex(@"(?<funcName>\w+)");
        private static readonly Regex ParameterListPattern = new Regex(@"\( (?<param>(?<paramName>\w+) /\* : (?<paramType>[\w.]+) \*/ ,? )*\)");
        private static readonly Regex ReturnTypePattern = new Regex(@"/\* : (?<funcType>[\w.]+) \*/");
        private static readonly Regex SignaturePattern = new Regex(
            string.Join(@" ", 
                AttributesPattern,
                NamePattern,
                ParameterListPattern,
                ReturnTypePattern)
                .Replace(" ", @"\s*"), 
            RegexOptions.Multiline);
        protected virtual bool ProtectEditor => true;
        
        protected class Function
        {
            public string Name;
            public Type Type;
            public Dictionary<string, Type> Params;
            public bool IsStatic;
        }
        
        public void Run()
        {
            var code = GenerateCode();
            GeneratorUtils.WriteFile(code, Path, ProtectEditor);
        }
        
        protected GeneratorBase(Type targetType)
        {
            TargetType = targetType;
            var definitionPath = GeneratorUtils.GetSourceFilePath
                (targetType, "js", isGenerated: false);
            var definition = GeneratorUtils.ReadFile(definitionPath);
            Functions = Parse(definition);
            Debug.Log($"Loaded {Functions.Count()} functions from {definitionPath}: {string.Join(", ", Functions.Select(f => f.Name))}");
        }

        private IReadOnlyList<Function> Parse(string definition)
        {
            return definition
                .Split('\n')
                .Select(s => s.Trim())
                .Where(l => SignaturePattern.IsMatch(l))
                .Select(l =>
                {
                    var match = SignaturePattern.Match(l);
                    var paramNames = match.Groups["paramName"].Captures;
                    var paramTypes = match.Groups["paramType"].Captures;
                    return new Function
                    {
                        Name = match.Groups["funcName"].Value,
                        Type = Type.GetType(match.Groups["funcType"].Value),
                        Params = Enumerable.Range(0, paramNames.Count).ToDictionary(
                            i => paramNames[i].Value,
                            i => Type.GetType(paramTypes[i].Value)),
                        IsStatic = match.Groups["isStatic"].Success
                    };
                }).ToArray();
        }
        
        public abstract string Path { get; }

        protected abstract string GenerateCode();
    }
}