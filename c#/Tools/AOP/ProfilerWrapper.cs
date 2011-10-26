using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Concurrent;
using System.Threading;

namespace BouilholLib.Helper
{
    public class ProfilerWrapper<T> : BaseWrapper<T> where T : class
    {
        private readonly Stopwatch stopwatch = new Stopwatch();

        public Stopwatch Stopwatch { get { return stopwatch; } }

        public ProfilerWrapper(T source)
            : base(source)
        {
        }

        public override void OnEntry(T source, string methodName, object[] args)
        {
            stopwatch.Restart();
        }

        public override void OnExit(T source, string methodName, object[] args, object result)
        {
            stopwatch.Stop();
            Console.Out.WriteLine("Method({0}) call in {1} ms", methodName, stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    public static class ProfilerWrapperExtension
    {
        public static dynamic ToProfiler<T>(this T source) where T : class
        {
            return new ProfilerWrapper<T>(source);
        }
    }
}
