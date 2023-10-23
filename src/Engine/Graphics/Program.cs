using System;
using System.Collections.Generic;
using Reactor.Geometry;
using Reactor.Math3D;
using Reactor.Platform;
using Reactor.Platform.WGPU;
using Reactor.Platform.WGPU.Wrappers;
using Reactor.Shading;
using Reactor.Types;


namespace Reactor.Graphics
{
    public class Program : IDisposable
    {
        private static string r_position = "r_Position";
        private static string r_color = "r_Color";
        private static string r_normal = "r_Normal";
        private static string r_bitangent = "r_Bitangent";
        private static string r_tangent = "r_Tangent";
        private static string r_texcoord = "r_TexCoord";
        private static string r_blendindices = "r_BlendIndices";
        private static string r_blendweight = "r_BlendWeight";
        private static string r_tessellatefactor = "r_TessellateFactor";

        internal static Program basicShader;

        private readonly Dictionary<ShaderSemanticDefinition, ShaderSemantic> _semantics =
            new Dictionary<ShaderSemanticDefinition, ShaderSemantic>();
        
        public uint Id { get; internal set; }
        internal ShaderModule shader;
        
        public Shader Vertex { get; internal set; }
        public Shader Fragment { get; internal set; }
        
        public void Dispose()
        {
            if (shader != null)
            {
                shader.Dispose();
            }
        }

        public void Load(string vertSource, string fragSource)
        {
            Load(vertSource, fragSource, null, null);
        }

        public void Load(string vertSource, string fragSource, string geomSource)
        {
            Load(vertSource, fragSource, geomSource, null);
        }

        public void Load(string vertSource, string fragSource, string geomSource, string[] defines)
        {
            Vertex = new Shader(vertSource, (int)ShaderEffectType.VERTEX);
            Fragment = new Shader(fragSource, (int)ShaderEffectType.FRAGMENT);
            
        }

        public void SetUniformValue(string name, bool value)
        {
            
        }

        public void SetUniformValue(string name, int value)
        {
            
        }

        public void SetUniformValue(string name, double value)
        {
            
        }

        public void SetUniformValue(string name, float value)
        {
            
        }

        public void SetUniformValue(string name, Vector2 value)
        {
            
        }

        public void SetUniformValue(string name, Vector3 value)
        {
            
        }

        public void SetUniformValue(string name, Vector4 value)
        {
            
        }

        public void SetUniformValue(string name, RColor value)
        {
            SetUniformValue(name, value.ToVector4());
        }

        public void SetUniformValue(string name, Matrix value)
        {
            
        }

        public void SetSamplerValue(TextureLayer layer, Texture texture)
        {
            
        }

        public int GetUniformBySemantic(ShaderSemanticDefinition semantic)
        {
            return GetUniformLocation(_semantics[semantic].name);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, bool value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "bool")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, int value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "int")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, double value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "double")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, float value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "float")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, Vector2 value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "vec2")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, Vector3 value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "vec3")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, Vector4 value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "vec4")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, Matrix value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "mat4")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        public void SetUniformBySemantic(ShaderSemanticDefinition semantic, RColor value)
        {
            if (_semantics.ContainsKey(semantic))
                if (_semantics[semantic].type == "vec4")
                    SetUniformValue(_semantics[semantic].name, value);
        }

        internal int GetTexUniformLocation(TextureLayer layer)
        {
            var v = (int)layer - (int)TextureLayer.TEXTURE0;
            var name = $"texture{v}";
            return GetUniformLocation(name);
        }

        internal int GetUniformLocation(string name)
        {
            return -1;
        }

        internal int GetAttribLocation(string name)
        {
            return -1;
        }

        internal int GetAttribLocation(VertexElementUsage rVertexElementUsage)
        {
            var name = "";
            switch (rVertexElementUsage)
            {
                case VertexElementUsage.Position:
                    name = r_position;
                    break;
                case VertexElementUsage.Color:
                    name = r_color;
                    break;
                case VertexElementUsage.Normal:
                    name = r_normal;
                    break;
                case VertexElementUsage.Bitangent:
                    name = r_bitangent;
                    break;
                case VertexElementUsage.Tangent:
                    name = r_tangent;
                    break;
                case VertexElementUsage.TextureCoordinate:
                    name = r_texcoord;
                    break;
                case VertexElementUsage.BlendIndices:
                    name = r_blendindices;
                    break;
                case VertexElementUsage.BlendWeight:
                    name = r_blendweight;
                    break;
                case VertexElementUsage.TessellateFactor:
                    name = r_tessellatefactor;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return GetAttribLocation(name);
        }

        internal int GetAttribLocation(VertexElementUsage rVertexElementUsage, uint usageIndex)
        {
            //var name = GPU.GetActiveAttrib(Id, usageIndex, out var size, out var type);
            //return GPU.GetAttribLocation(Id, name.ToString());
            return 0;
        }

        public void Bind()
        {
            if (Id != 0)
            {
                //GPU.UseProgram(Id);
                GPU.CheckError();
            }
            else
            {
                throw new Exception("You must first compile a shader program before you can bind it");
            }
        }

        public void Unbind()
        {
            //GPU.UseProgram(0);
            GPU.CheckError();
        }

        internal void BindSemantics(Matrix Model, Matrix View, Matrix Projection, float near, float far)
        {
            foreach (var keyPair in _semantics)
                switch (keyPair.Key)
                {
                    case ShaderSemanticDefinition.VIEW:
                        SetUniformBySemantic(ShaderSemanticDefinition.VIEW, View);
                        break;
                    case ShaderSemanticDefinition.PROJECTION:
                        SetUniformBySemantic(ShaderSemanticDefinition.PROJECTION, Projection);
                        break;
                    case ShaderSemanticDefinition.VIEWPROJECTION:
                        SetUniformBySemantic(ShaderSemanticDefinition.VIEWPROJECTION, View * Projection);
                        break;
                    case ShaderSemanticDefinition.MODEL:
                        SetUniformBySemantic(ShaderSemanticDefinition.MODEL, Model);
                        break;
                    case ShaderSemanticDefinition.WORLD:
                        SetUniformBySemantic(ShaderSemanticDefinition.WORLD, Model);
                        break;
                    case ShaderSemanticDefinition.MODELVIEW:
                        SetUniformBySemantic(ShaderSemanticDefinition.MODELVIEW, Model * View);
                        break;
                    case ShaderSemanticDefinition.MODELVIEWPROJECTION:
                        SetUniformBySemantic(ShaderSemanticDefinition.MODELVIEWPROJECTION, Model * View * Projection);
                        break;
                    case ShaderSemanticDefinition.INVERSE_VIEW:
                        Matrix.Invert(ref View, out var view);
                        SetUniformBySemantic(ShaderSemanticDefinition.INVERSE_VIEW, view);
                        break;
                    case ShaderSemanticDefinition.INVERSE_PROJECTION:
                        Matrix.Invert(ref Projection, out var proj);
                        SetUniformBySemantic(ShaderSemanticDefinition.INVERSE_PROJECTION, proj);
                        break;
                    case ShaderSemanticDefinition.FAR_PLANE:
                        SetUniformBySemantic(ShaderSemanticDefinition.FAR_PLANE, far);
                        break;
                    case ShaderSemanticDefinition.NEAR_PLANE:
                        SetUniformBySemantic(ShaderSemanticDefinition.NEAR_PLANE, near);
                        break;
                }
        }

        public static Program GetBasicShader()
        {
            return basicShader;
        }

        internal static void InitShaders()
        {
            
        }
        
    }
    
}