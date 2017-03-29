namespace Dorado.Web.HtmlBuilder
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Web;

    public class HtmlString
    {
        private delegate HtmlString HtmlStringCreator(string value);

        private static readonly HtmlStringCreator Creator = GetCreator();

        // imporant: this declaration must occur after the _creator declaration
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "HtmlString is immutable")]
        public static readonly HtmlString Empty = Create(String.Empty);

        private readonly string _value;

        // This constructor is only protected so that we can subclass it in a dynamic module. In practice,
        // nobody should ever call this constructor, and it is likely to be removed in a future version
        // of the framework. Use the static Create() method instead.
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("The recommended alternative is the static HtmlString.Create(String value) method.")]
        protected HtmlString(string value)
        {
            _value = value ?? String.Empty;
        }

        public static HtmlString Create(string value)
        {
            return Creator(value);
        }

        // in .NET 4, we dynamically create a type that subclasses HtmlString and implements IHtmlString
        private static HtmlStringCreator GetCreator()
        {
            Type iHtmlStringType = typeof(HttpContext).Assembly.GetType("System.Web.IHtmlString");
            if (iHtmlStringType != null)
            {
                // first, create the dynamic type
                Type dynamicType = DynamicTypeGenerator.GenerateType("DynamicHtmlString", typeof(HtmlString), new Type[] { iHtmlStringType });

                // then, create the delegate to instantiate the dynamic type
                ParameterExpression valueParamExpr = Expression.Parameter(typeof(string), "value");
                NewExpression newObjExpr = Expression.New(dynamicType.GetConstructor(new Type[] { typeof(string) }), valueParamExpr);
                Expression<HtmlStringCreator> lambdaExpr = Expression.Lambda<HtmlStringCreator>(newObjExpr, valueParamExpr);
                return lambdaExpr.Compile();
            }
            else
            {
                // disabling 0618 allows us to call the HtmlString() constructor
#pragma warning disable 0618
                return value => new HtmlString(value);
#pragma warning restore 0618
            }
        }

        public static bool IsNullOrEmpty(HtmlString value)
        {
            return (value == null || value._value.Length == 0);
        }

        // IHtmlString.ToHtmlString()
        public string ToHtmlString()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}