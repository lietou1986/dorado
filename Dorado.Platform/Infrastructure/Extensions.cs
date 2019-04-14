using Autofac.Integration.Mvc;
using System;

namespace Dorado.Platform.Infrastructure
{
    public static class Extensions
    {
        public static T GetService<T>(this object obj)
        {
            return DoradoContext.Instance.ContainerManager.GetService<T>();
        }
    }
}