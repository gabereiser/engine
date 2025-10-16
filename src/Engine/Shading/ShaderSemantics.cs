using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Red.Shading
{
    internal class ShaderSemantics : Dictionary<ShaderSemanticDefinition, ShaderSemantic>
    {
        private const string PARSER_REGEX = @"uniform\s(?<type>.\w*)\s(?<name>.\w*)\s[:]\s(?<macro>.\w*);";

        internal ShaderSemantics(ref string source)
        {
            source = Regex.Replace(source, PARSER_REGEX, delegate (Match match)
            {
                var semantic = new ShaderSemantic
                {
                    type = match.Groups["type"].Value.ToLower(),
                    name = match.Groups["name"].Value.ToLower()
                };
                Add(GetSemanticDefinition(match.Groups["macro"].Value.ToUpper()), semantic);
                var returnValue = string.Format("uniform {0} {1};", match.Groups["type"].Value,
                    match.Groups["name"].Value);
                return returnValue;
            });
        }

        private ShaderSemanticDefinition GetSemanticDefinition(string semantic)
        {
            return (ShaderSemanticDefinition)Enum.Parse(typeof(ShaderSemanticDefinition), semantic, true);
        }
    }

    internal struct ShaderSemantic
    {
        public string name;
        public string type;
    }

    public enum ShaderSemanticDefinition
    {
        WORLD,
        MODEL,
        VIEW,
        PROJECTION,
        INVERSE_WORLD,
        INVERSE_MODEL,
        INVERSE_VIEW,
        INVERSE_PROJECTION,
        MODELVIEW,
        WORLDVIEW,
        MODELVIEWPROJECTION,
        WORLDVIEWPROJECTION,
        VIEWPROJECTION,
        FAR_PLANE,
        NEAR_PLANE,
        TIME
    }
}