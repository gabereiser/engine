using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Reactor.Common;
using Reactor.Common.Collections;

namespace Reactor.Platform
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Metric
    {
        public DateTime DateTime;
        public string Label;
        public ulong Value;

        public Metric(DateTime dateTime, string label, ulong value)
        {
            DateTime = dateTime;
            Label = label;
            Value = value;
        }
    }
    public static class Metrics
    {
        private static Dictionary<string, RingList<Metric>> _history = new Dictionary<string, RingList<Metric>>();

        static Metrics()
        {
            CreateMetric("GCMemoryAllocated");
            CreateMetric("VirtualMemory");
            CreateMetric("TotalMemory");
            CreateMetric("Allocated Fences");
            CreateMetric("Allocations");
        }

        internal static void ProcessMetrics()
        {
            LogMetric("GCMemoryAllocated", GCMemoryAllocated);
            LogMetric("VirtualMemory", MemoryAllocated);
            LogMetric("TotalMemory", TotalMemoryAllocated);
            LogMetric("Allocated Fences", Allocator.MemoryAllocated);
            LogMetric("Allocations", (ulong)Allocator.Allocations);
        }

        public static ulong GCMemoryAllocated => (ulong)GC.GetTotalMemory(false);

        public static ulong MemoryAllocated
        {
            get
            {
                using (var p = Process.GetCurrentProcess())
                {
                    p.Refresh();
                    return (ulong)p.VirtualMemorySize64;
                }
            }
        } 
        public static ulong TotalMemoryAllocated
        {
            get
            {
                using (var p = Process.GetCurrentProcess())
                {
                    p.Refresh();
                    return (ulong)p.PeakVirtualMemorySize64;
                }
            }
        }
        
        public static void CreateMetric(string label)
        {
            _history[label] = new RingList<Metric>(3600);
        }
        public static void LogMetric(string label, ulong value)
        {
            var now = DateTime.Now;
            var metric = new Metric(now, label, value);
            
            if (!_history.ContainsKey(label))
                CreateMetric(label);
            
            _history[label].Add(metric);
        }
    }
}