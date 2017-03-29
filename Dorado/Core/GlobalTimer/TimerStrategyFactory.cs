using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Dorado.Core.GlobalTimer
{
    public static class TimerStrategyFactory
    {
        static TimerStrategyFactory()
        {
            var list = from t in typeof(TimerStrategyFactory).Assembly.GetTypes()
                       where t.IsDefined(typeof(TimerStrategyAttribute), false)
                       let attr = (TimerStrategyAttribute)t.GetCustomAttributes(typeof(TimerStrategyAttribute), false)[0]
                       let creator = Activator.CreateInstance(attr.CreatorType) as IObjectCreator
                       where creator != null
                       select new {attr.Name, Creator = creator };

            Creators = list.ToDictionary(v => v.Name, v => v.Creator);
        }

        private static readonly Dictionary<string, IObjectCreator> Creators;

        /// <summary>
        /// get a object creator by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IObjectCreator GetCreator(string name)
        {
            Contract.Ensures(name != null);

            IObjectCreator creator;
            Creators.TryGetValue(name, out creator);
            return creator;
        }

        private static IObjectCreator _GetCreatorWithThrowError(string name)
        {
            IObjectCreator creator = GetCreator(name);
            if (creator == null)
                throw new NotSupportedException("creation of type " + name + " is not supported!");

            return creator;
        }

        public static ITimerStrategy Create(string name, string argument)
        {
            IObjectCreator creator = _GetCreatorWithThrowError(name);
            return creator.CreateObject(argument) as ITimerStrategy;
        }

        public static ITimerStrategy Create(string info)
        {
            Contract.Requires(info != null);

            int k = info.IndexOf(':');
            if (k < 0)
                throw new FormatException("format of argument 'info' is not correct!");

            string type = info.Substring(0, k), arg = info.Substring(k + 1);
            return Create(type, arg);
        }

        /// <summary>
        /// create objects of ITimerStrategy type by infos with format likes: everyDay:10:32:00; staticInterval:00:00:50
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public static ITimerStrategy[] CreateRange(string infos)
        {
            Contract.Ensures(infos != null);

            var list = from info in infos.Split(';')
                       let k = info.IndexOf(':')
                       where k >= 0
                       let name = info.Substring(0, k).Trim()
                       let arg = info.Substring(k + 1).Trim()
                       select Create(name, arg);

            return list.ToArray();
        }
    }
}