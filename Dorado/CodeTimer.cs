using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Dorado
{
    public class CodeTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        private readonly long _check;
        private long _startTime, _stopTime;
        private readonly long _freq;
        private readonly Decimal _multiplier = new Decimal(1.0e9);

        // 构造函数
        public CodeTimer()
        {
            _startTime = 0;
            _stopTime = 0;

            if (QueryPerformanceFrequency(out _freq) == false)
            {
                // 不支持高性能计数器
                throw new Win32Exception();
            }
            _check = 0;

            QueryPerformanceCounter(out _startTime);
            QueryPerformanceCounter(out _stopTime);

            QueryPerformanceCounter(out _startTime);
            QueryPerformanceCounter(out _stopTime);

            _check += _stopTime - _startTime;
        }

        // 开始计时器
        public void Start()
        {
            QueryPerformanceCounter(out _startTime);
        }

        // 停止计时器
        public void Stop()
        {
            QueryPerformanceCounter(out _stopTime);
        }

        // 返回计时器经过时间(单位：秒)
        public double Duration(int iterations = 1)
        {
            return (_stopTime - _startTime - _check) * (double)_multiplier / _freq / iterations;
        }
    }
}