using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Red.Platform.WGPU.Helpers;
using static Red.Platform.WGPU.Wgpu;

namespace Red.Platform.WGPU.Wrappers
{
    public struct RequiredLimits
    {
        public Limits Limits;
    }

    public partial struct RequiredLimitsExtras
    {
        public uint MaxPushConstantSize;
    }

    public struct DeviceExtras
    {
        public string TracePath;
    }

    public class Adapter : IDisposable
    {
        internal AdapterImpl Impl;

        internal Adapter(AdapterImpl impl)
        {
            if (impl.Handle == IntPtr.Zero)
                throw new ResourceCreationError(nameof(Adapter));

            Impl = impl;
        }


        public FeatureName[] EnumerateFeatures()
        {
            unsafe
            {
                FeatureName features = default;

                ulong size = AdapterEnumerateFeatures(Impl, ref features);

                var featuresSpan = new Span<FeatureName>(Unsafe.AsPointer(ref features), (int)size);

                FeatureName[] result = new FeatureName[size];

                featuresSpan.CopyTo(result);

                return result;
            }
        }

        public bool GetLimits(out SupportedLimits limits)
        {
            limits = new SupportedLimits();

            return AdapterGetLimits(Impl, ref limits);
        }

        public void GetProperties(out AdapterProperties properties)
        {
            properties = new AdapterProperties();

            AdapterGetProperties(Impl, ref properties);
        }

        public bool HasFeature(FeatureName feature) => AdapterHasFeature(Impl, feature);

        public void RequestDevice(RequestDeviceCallback callback, string label, NativeFeature[] nativeFeatures, QueueDescriptor defaultQueue = default,
            Limits? limits = null, RequiredLimitsExtras? limitsExtras = null, DeviceExtras? deviceExtras = null, DeviceLostCallback deviceLostCallback = null)
        {
            Wgpu.RequiredLimits requiredLimits = default;
            WgpuStructChain limitsExtrasChain = null;
            WgpuStructChain deviceExtrasChain = null;

            if (limitsExtras != null)
            {
                limitsExtrasChain = new WgpuStructChain()
                    .AddRequiredLimitsExtras(
                        limitsExtras.Value.MaxPushConstantSize);
            }

            if (limits != null)
            {
                unsafe
                {
                    requiredLimits = new Wgpu.RequiredLimits
                    {

                        nextInChain = limitsExtras == null
                            ? (Wgpu.ChainedStruct*)IntPtr.Zero
                            : (Wgpu.ChainedStruct*)limitsExtrasChain.GetPointer(),
                        limits = limits.Value
                    };
                }
            }

            if (deviceExtras != null)
                deviceExtrasChain = new WgpuStructChain().AddDeviceExtras(deviceExtras.Value.TracePath);

            GCHandle losthandle = default;
            var lostcontext = new Callback<Wgpu.DeviceLostReason>
            {
                Delegate = (s, m, userData) =>
                {
                    if (losthandle.IsAllocated) losthandle.Free();
                    deviceLostCallback.Invoke(s, m);
                },
                UserData = IntPtr.Zero,
            };

            GCHandle handle = default;
            var context = new CallbackContext<RequestDeviceStatus, DeviceImpl>
            {
                Delegate = (s, d, m, userData) =>
                {
                    if (handle.IsAllocated) handle.Free();
                    callback.Invoke(s, new Device(d), m);
                },
                UserData = IntPtr.Zero
            };
            handle = GCHandle.Alloc(context);
            losthandle = GCHandle.Alloc(lostcontext);
            try
            {
                unsafe
                {
                    fixed (NativeFeature* requiredFeatures = nativeFeatures)
                    {
                        AdapterRequestDevice(Impl, new DeviceDescriptor()
                        {
                            defaultQueue = defaultQueue,
                            requiredLimits = limits != null ? &requiredLimits : null,
                            requiredFeaturesCount = (uint)nativeFeatures.Length,
                            requiredFeatures = (FeatureName*)requiredFeatures,
                            label = label,
                            deviceLostCallback = &DeviceLostCallback,
                            nextInChain = deviceExtras == null ? null : (ChainedStruct*)deviceExtrasChain.GetPointer()
                        },
                            &RequestDeviceCallback, (void*)GCHandle.ToIntPtr(handle));
                    }
                }
            }
            catch
            {
                throw;
            }

            limitsExtrasChain?.Dispose();
            deviceExtrasChain?.Dispose();
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void DeviceLostCallback(DeviceLostReason reason, byte* message, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (Callback<DeviceLostReason>)handle.Target;
                string messageStr = (message == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(message, int.MaxValue)[..(int)Util.StrLen(message)]);
                context.Delegate?.Invoke(reason, messageStr, context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        internal static unsafe void RequestDeviceCallback(RequestDeviceStatus status, DeviceImpl device, byte* message, void* userData)
        {
            GCHandle handle = GCHandle.FromIntPtr((IntPtr)userData);
            try
            {
                var context = (CallbackContext<RequestDeviceStatus, DeviceImpl>)handle.Target;
                string messageStr = (message == null) ? null : Encoding.UTF8.GetString(new ReadOnlySpan<byte>(message, int.MaxValue).Slice(0, (int)Util.StrLen(message)));
                context.Delegate?.Invoke(status, device, messageStr, context.UserData);
            }
            finally
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
            }
        }
        public void Dispose() => AdapterRelease(Impl);
    };
    public delegate void RequestDeviceCallback(RequestDeviceStatus status, Device device, string message);
}
