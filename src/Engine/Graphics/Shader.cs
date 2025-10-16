using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Red.Common;
using Red.Platform;
using Red.Platform.WGPU;
using Red.Platform.WGPU.Wrappers;
using Red.Shading;
using Red.Systems;

namespace Red.Graphics
{
    public class Shader : IDisposable
    {
        public string Source { get; internal set; }
        public ShaderEffectType ShaderType { get; set; }

        public Shader(string source, int type)
        {
            ShaderType = (ShaderEffectType)type;
            Source = source;


        }

        #region IDisposable implementation
        public void Dispose()
        {

        }
        #endregion
    }

    public enum ShaderEffectType
    {
        VERTEX = 0,
        FRAGMENT = 1,
        GEOMETRY = 2,
        TESS_CONTROL = 3,
        TESS_EVAL = 4,
        COMPUTE = 5
    }
}