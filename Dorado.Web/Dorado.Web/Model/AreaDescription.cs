using System;
using System.Web.Mvc;

namespace Dorado.Web.Model
{
    public class AreaDescription
    {
        public AreaRegistration AreaInstance
        {
            get;
            set;
        }

        public Type AreaType
        {
            get;
            set;
        }
    }
}