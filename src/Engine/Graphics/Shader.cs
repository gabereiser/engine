using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Reactor.Common;
using Reactor.Platform;
using Reactor.Platform.WGPU;
using Reactor.Platform.WGPU.Wrappers;
using Reactor.Shading;
using Reactor.Systems;

namespace Reactor.Graphics
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