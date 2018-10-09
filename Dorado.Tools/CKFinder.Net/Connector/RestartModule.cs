/*
 * CKFinder
 * ========
 * http://cksource.com/ckfinder
 * Copyright (C) 2007-2015, CKSource - Frederico Knabben. All rights reserved.
 *
 * The software, this file and its contents are subject to the CKFinder
 * License. Please read the license.txt file before using, installing, copying,
 * modifying or distribute this file or part of its contents. The contents of
 * this file is part of the Source Code of CKFinder.
 */

using System;
using System.Reflection;
using System.Web;

namespace CKFinder.Utils
{
    /// <summary> 
    ///     Disable AppDomain restarts when folder is deleted.
    ///     https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=240686
    /// </summary> 
    public class StopAppDomainRestartModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            PropertyInfo p = typeof(HttpRuntime).GetProperty("FileChangesMonitor",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            object o = p.GetValue(null, null);

            FieldInfo f = o.GetType().GetField("_dirMonSubdirs",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

            object monitor = f.GetValue(o);

            MethodInfo m = monitor.GetType().GetMethod("StopMonitoring",
                BindingFlags.Instance | BindingFlags.NonPublic);

            m.Invoke(monitor, new object[] { });
        }

        public void Dispose() { }
    }
}
