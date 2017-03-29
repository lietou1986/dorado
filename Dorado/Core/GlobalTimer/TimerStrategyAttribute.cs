using System;

namespace Dorado.Core.GlobalTimer
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class TimerStrategyAttribute : Attribute
    {
        public TimerStrategyAttribute(string name, Type creatorType)
        {
            Name = name;
            CreatorType = creatorType;
        }

        public string Name { get; private set; }

        public Type CreatorType { get; private set; }
    }
}