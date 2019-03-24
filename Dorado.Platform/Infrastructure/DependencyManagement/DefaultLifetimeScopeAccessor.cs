﻿using Autofac;
using Autofac.Core.Lifetime;
using Dorado.Platform.Utils;
using System;
using System.Diagnostics;
using System.Web;

namespace Dorado.Platform.Infrastructure.DependencyManagement
{
    public class DefaultLifetimeScopeAccessor : ILifetimeScopeAccessor
    {
        private ContextState<ILifetimeScope> _state;
        private readonly ILifetimeScope _rootContainer;
        internal static readonly object ScopeTag = "AutofacWebRequest";

        public DefaultLifetimeScopeAccessor(ILifetimeScope rootContainer)
        {
            Guard.ArgumentNotNull(() => rootContainer);

            //rootContainer.ChildLifetimeScopeBeginning += OnScopeBeginning;

            this._rootContainer = rootContainer;
            this._state = new ContextState<ILifetimeScope>("CustomLifetimeScopeProvider.WorkScope");
        }

        public ILifetimeScope ApplicationContainer
        {
            get { return _rootContainer; }
        }

        public IDisposable BeginContextAwareScope()
        {
            var disposer = HttpContext.Current != null
                ? ActionDisposable.Empty
                : new ActionDisposable(() => this.EndLifetimeScope());

            return disposer;
        }

        public void EndLifetimeScope()
        {
            try
            {
                var scope = _state.GetState();
                if (scope != null)
                {
                    try
                    {
                    }
                    catch { }

                    scope.Dispose();
                    _state.RemoveState();
                }
            }
            catch { }
        }

        public ILifetimeScope GetLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            var scope = _state.GetState();
            if (scope == null)
            {
                _state.SetState((scope = BeginLifetimeScope(configurationAction)));
                //scope.CurrentScopeEnding += OnScopeEnding;
            }
            return scope;
        }

        private void OnScopeBeginning(object sender, LifetimeScopeBeginningEventArgs args)
        {
            bool isWeb = HttpContext.Current != null;
            Debug.WriteLine("Scope Begin, Web: " + isWeb);
        }

        private void OnScopeEnding(object sender, LifetimeScopeEndingEventArgs args)
        {
            bool isWeb = HttpContext.Current != null;
            Debug.WriteLine("Scope END, Web: " + isWeb);
        }

        private ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return (configurationAction == null)
                ? _rootContainer.BeginLifetimeScope(ScopeTag)
                : _rootContainer.BeginLifetimeScope(ScopeTag, configurationAction);
        }
    }
}