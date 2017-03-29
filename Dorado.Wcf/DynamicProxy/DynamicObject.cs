#region Copyright Notice

// ----------------------------------------------------------------------------
// Copyright (C) 2006 Microsoft Corporation, All rights reserved.
// ----------------------------------------------------------------------------

// Author: Vipul Modi (vipul.modi@microsoft.com)

#endregion Copyright Notice

namespace Dorado.Wcf.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;

    public class DynamicObject
    {
        private Type objType;
        private object obj;

        private delegate object FastInvokeHandler(object target, object[] paramters);

        private Dictionary<string, FastInvokeHandler> fastInvokHandlerCache = new Dictionary<string, FastInvokeHandler>();
        private readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        private BindingFlags CommonBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public;

        public DynamicObject(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            obj = obj;
            objType = obj.GetType();
        }

        public DynamicObject(Type objType)
        {
            if (objType == null)
                throw new ArgumentNullException("objType");

            objType = objType;
        }

        public void CacheAllMethods()
        {
            MethodInfo[] mis = objType.GetMethods(BindingFlags.InvokeMethod | CommonBindingFlags);
            foreach (var mi in mis)
            {
                FastInvokeHandler handler = GetMethodInvoker(mi);
                fastInvokHandlerCache.Add(mi.Name, handler);
            }
        }

        public void CallConstructor()
        {
            CallConstructor(new Type[0], new object[0]);
        }

        public void CallConstructor(Type[] paramTypes, object[] paramValues)
        {
            ConstructorInfo ctor = objType.GetConstructor(paramTypes);
            if (ctor == null)
            {
                throw new DynamicProxyException(
                        Constants.ErrorMessages.ProxyCtorNotFound);
            }

            obj = ctor.Invoke(paramValues);
        }

        public object GetProperty(string property)
        {
            object retval = objType.InvokeMember(
                property,
                BindingFlags.GetProperty | CommonBindingFlags,
                null /* Binder */,
                obj,
                null /* args */);

            return retval;
        }

        public object SetProperty(string property, object value)
        {
            object retval = objType.InvokeMember(
                property,
                BindingFlags.SetProperty | CommonBindingFlags,
                null /* Binder */,
                obj,
                new object[] { value });

            return retval;
        }

        public object GetField(string field)
        {
            object retval = objType.InvokeMember(
                field,
                BindingFlags.GetField | CommonBindingFlags,
                null /* Binder */,
                obj,
                null /* args */);

            return retval;
        }

        public object SetField(string field, object value)
        {
            object retval = objType.InvokeMember(
                field,
                BindingFlags.SetField | CommonBindingFlags,
                null /* Binder */,
                obj,
                new object[] { value });

            return retval;
        }

        private static readonly object lockObj = new object();

        public object CallMethod(string method, params object[] parameters)
        {
            return objType.InvokeMember(
               method,
               BindingFlags.InvokeMethod | CommonBindingFlags,
               null /* Binder */,
               obj,
               parameters /* args */);
        }

        public object CallMethodFast(string method, params object[] parameters)
        {
            FastInvokeHandler handler = null;
            if (fastInvokHandlerCache.ContainsKey(method))
            {
                handler = fastInvokHandlerCache[method];
            }
            object retval = handler(obj, parameters);
            return retval;
        }

        private FastInvokeHandler GetMethodInvoker(MethodInfo methodInfo)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Empty, typeof(object), new Type[] { typeof(object), typeof(object[]) }, methodInfo.DeclaringType.Module);
            ILGenerator il = dynamicMethod.GetILGenerator();
            ParameterInfo[] ps = methodInfo.GetParameters();
            Type[] paramTypes = new Type[ps.Length];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    paramTypes[i] = ps[i].ParameterType.GetElementType();
                else
                    paramTypes[i] = ps[i].ParameterType;
            }
            LocalBuilder[] locals = new LocalBuilder[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; i++)
            {
                locals[i] = il.DeclareLocal(paramTypes[i], true);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                il.Emit(OpCodes.Ldarg_1);
                EmitFastInt(il, i);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastToReference(il, paramTypes[i]);
                il.Emit(OpCodes.Stloc, locals[i]);
            }
            if (!methodInfo.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
            }
            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                    il.Emit(OpCodes.Ldloca_S, locals[i]);
                else
                    il.Emit(OpCodes.Ldloc, locals[i]);
            }
            if (methodInfo.IsStatic)
                il.EmitCall(OpCodes.Call, methodInfo, null);
            else
                il.EmitCall(OpCodes.Callvirt, methodInfo, null);
            if (methodInfo.ReturnType == typeof(void))
                il.Emit(OpCodes.Ldnull);
            else
                EmitBoxIfNeeded(il, methodInfo.ReturnType);

            for (int i = 0; i < paramTypes.Length; i++)
            {
                if (ps[i].ParameterType.IsByRef)
                {
                    il.Emit(OpCodes.Ldarg_1);
                    EmitFastInt(il, i);
                    il.Emit(OpCodes.Ldloc, locals[i]);
                    if (locals[i].LocalType.IsValueType)
                        il.Emit(OpCodes.Box, locals[i].LocalType);
                    il.Emit(OpCodes.Stelem_Ref);
                }
            }

            il.Emit(OpCodes.Ret);
            FastInvokeHandler invoder = (FastInvokeHandler)dynamicMethod.CreateDelegate(typeof(FastInvokeHandler));
            return invoder;
        }

        private void EmitCastToReference(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, type);
            }
            else
            {
                il.Emit(OpCodes.Castclass, type);
            }
        }

        private void EmitBoxIfNeeded(ILGenerator il, System.Type type)
        {
            if (type.IsValueType)
            {
                il.Emit(OpCodes.Box, type);
            }
        }

        private void EmitFastInt(ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;

                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;

                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;

                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;

                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;

                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;

                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;

                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;

                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;

                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
            {
                il.Emit(OpCodes.Ldc_I4_S, (SByte)value);
            }
            else
            {
                il.Emit(OpCodes.Ldc_I4, value);
            }
        }

        public object CallMethod(string method, Type[] types,
            object[] parameters)
        {
            if (types.Length != parameters.Length)
                throw new ArgumentException(
                    Constants.ErrorMessages.ParameterValueMistmatch);

            MethodInfo mi = objType.GetMethod(method, types);
            if (mi == null)
                throw new ApplicationException(string.Format(
                    Constants.ErrorMessages.MethodNotFound, method));

            object retval = mi.Invoke(obj, CommonBindingFlags, null,
                parameters, null);

            return retval;
        }

        public Type ObjectType
        {
            get
            {
                return objType;
            }
        }

        public object ObjectInstance
        {
            get
            {
                return obj;
            }
        }

        public BindingFlags BindingFlags
        {
            get
            {
                return CommonBindingFlags;
            }

            set
            {
                CommonBindingFlags = value;
            }
        }

        public virtual Type GetObjectType(string typeName)
        {
            throw new NotSupportedException();
        }
    }
}