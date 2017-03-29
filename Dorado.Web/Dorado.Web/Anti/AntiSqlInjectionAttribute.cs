using Dorado.Web.Exceptions;
using System;
using System.Web;
using System.Web.Mvc;

namespace Dorado.Web.Filter
{
    [AttributeUsage((AttributeTargets)68, AllowMultiple = false, Inherited = true)]
    public class AntiSqlInjectionAttribute : FilterAttribute, IAuthorizationFilter
    {
        public string[] QueryKeys
        {
            get;
            set;
        }

        public string[] FormKeys
        {
            get;
            set;
        }

        public bool ValidateQuerystring
        {
            get;
            set;
        }

        public bool ValidateForm
        {
            get;
            set;
        }

        public AntiSqlInjectionAttribute()
        {
            ValidateQuerystring = true;
            ValidateForm = true;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
            if (ValidateForm && request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
            {
                string[] formKeys = FormKeys ?? request.Form.AllKeys;
                string[] array = formKeys;
                for (int i = 0; i < array.Length; i++)
                {
                    string formKey = array[i];
                    if (!InputFilter.Validate(request.Form[formKey]))
                    {
                        throw new HttpInvalidReqeust(formKey, request.Form[formKey]);
                    }
                }
            }
            if (ValidateQuerystring)
            {
                string[] queryKeys = QueryKeys ?? request.QueryString.AllKeys;
                string[] array2 = queryKeys;
                for (int j = 0; j < array2.Length; j++)
                {
                    string queryKey = array2[j];
                    if (!InputFilter.Validate(request.QueryString[queryKey]))
                    {
                        throw new HttpInvalidReqeust(queryKey, request.QueryString[queryKey]);
                    }
                }
            }
        }
    }
}