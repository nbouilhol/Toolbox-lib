using System;
using System.Diagnostics;

namespace Utilities.AOP
{
    public class ProfilerWrapper<T> : BaseWrapper<T> where T : class
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public Stopwatch Stopwatch { get { return _stopwatch; } }

        public ProfilerWrapper(T source)
            : base(source)
        {
        }

        public override void OnEntry(T source, string methodName, object[] args)
        {
            _stopwatch.Restart();
        }

        public override void OnExit(T source, string methodName, object[] args, object result)
        {
            _stopwatch.Stop();
            if (Console.Out != null) Console.Out.WriteLine("Method({0}.{1}) call in {2} ms", typeof(T).Name, methodName, _stopwatch.Elapsed.TotalMilliseconds);
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