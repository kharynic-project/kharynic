using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kharynic.Engine.Scripting.ScriptExports
{
    public class ScriptImportsGenerator
    {
        private readonly IEnumerable<Function> _functions;
        private static readonly Regex AttributesPattern = new Regex(@"/\*\*?@[Ee]xport\*/(?<isStatic>static )?");
        private static readonly Regex NamePattern = new Regex(@"(?<funcName>\w+)");
        private static readonly Regex ParameterListPattern = new Regex(@"\( (?<param>(?<paramName>\w+) /\* : (?<paramType>\w+) \*/ ,? )*\)");
        private static readonly Regex ReturnTypePattern = new Regex(@"/\* : (?<funcType>\w+) \*/");
        private static readonly Regex SignaturePattern = new Regex(
            string.Join(@" ", 
                AttributesPattern,
                NamePattern,
                ParameterListPattern,
                ReturnTypePattern)
                .Replace(" ", @"\s*"), 
            RegexOptions.Multiline);
        
        private class Function
        {
            public string Name;
            public string Type;
            public Dictionary<string, string> Params; // Name, Type
            public bool IsStatic;
        }
        
        public ScriptImportsGenerator(Type placeholder)
        {
            var definitionPath = GeneratorUtils.GetSourceFilePath
                (placeholder, "js", isUnityAsset: false, isGenerated: false);
            var definition = GeneratorUtils.ReadFile(definitionPath);
            _functions = Parse(definition);
        }

        private IEnumerable<Function> Parse(string definition)
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
                        Type = match.Groups["funcType"].Value,
                        Params = Enumerable.Range(0, paramNames.Count).ToDictionary(
                            i => paramNames[i].Value,
                            i => paramTypes[i].Value),
                        IsStatic = match.Groups["isStatic"].Success
                    };
                }).ToArray();
        }

        public void Generate()
        {
            foreach (var function in _functions)
            {
                Debug.Log("generating " + function.Name);
            }
        }
    }
}