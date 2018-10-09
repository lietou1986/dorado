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
using System.Xml;
using System.Globalization;

namespace CKFinder.Connector.CommandHandlers
{
    internal class LoadCookiesCommandHandler : XmlCommandHandlerBase
    {
        public LoadCookiesCommandHandler()
            : base()
        {
        }

        protected override void BuildXml()
        {
            if (Request.Form["CKFinderCommand"] != "true")
            {
                ConnectorException.Throw(Errors.InvalidRequest);
            }
            if (!this.CurrentFolder.CheckAcl(AccessControlRules.FileView))
            {
                ConnectorException.Throw(Errors.Unauthorized);
            }

            // Create the "Cookies" node.
            XmlNode oCookiesNode = XmlUtil.AppendElement(this.ConnectorNode, "Cookies");

            System.Web.HttpCookieCollection CookieColl;
            System.Web.HttpCookie Cookie;

            CookieColl = Request.Cookies;

            // Capture all cookie names into a string array.
            String[] arr1 = CookieColl.AllKeys;

            // Grab individual cookie objects by cookie name.
            for ( int i = 0; i < arr1.Length; i++ ) 
            {
                Cookie = CookieColl[arr1[i]];
                if ( !Cookie.Name.StartsWith("CKFinder_") )
                {
                    XmlNode oCookieNode = XmlUtil.AppendElement(oCookiesNode, "Cookie");
                    XmlUtil.SetAttribute(oCookieNode, "name", Cookie.Name);
                    XmlUtil.SetAttribute(oCookieNode, "value", Cookie.Value);
                }
            }
        }
    }
}
