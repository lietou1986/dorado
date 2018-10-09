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
using System.Globalization ;
using System.Xml ;

namespace CKFinder.Connector
{
	public sealed class XmlUtil
	{
		private XmlUtil()
		{}

		public static XmlNode AppendElement( XmlNode node, string newElementName )
		{
			return AppendElement( node, newElementName, null ) ;
		}

		public static XmlNode AppendElement( XmlNode node, string newElementName, string innerValue )
		{
			XmlNode oNode ;

			if ( node is XmlDocument )
				oNode = node.AppendChild( ((XmlDocument)node).CreateElement( newElementName ) ) ;
			else
				oNode = node.AppendChild( node.OwnerDocument.CreateElement( newElementName ) ) ;

			if ( innerValue != null )
				oNode.AppendChild( node.OwnerDocument.CreateTextNode( innerValue ) ) ;

			return oNode ;
		}

		public static XmlAttribute CreateAttribute( XmlDocument xmlDocument, string name, string value )
		{
			XmlAttribute oAtt = xmlDocument.CreateAttribute( name ) ;
			oAtt.Value = value ;
			return oAtt ;
		}

		public static void SetAttribute( XmlNode node, string attributeName, string attributeValue )
		{
			if ( node.Attributes[ attributeName ] != null )
				node.Attributes[ attributeName ].Value = attributeValue ;
			else
				node.Attributes.Append( CreateAttribute( node.OwnerDocument, attributeName, attributeValue ) ) ;
		}

		public static string GetAttribute( XmlNode node, string attributeName, string defaultValue )
		{
			XmlAttribute att = node.Attributes[attributeName];
			if ( att != null )
				return att.Value;
			else
				return defaultValue;
		}

		public static string GetNodeValue( XmlNode parentNode, string nodeXPath, string defaultValue )
		{
			XmlNode node = parentNode.SelectSingleNode( nodeXPath );
			if ( node.FirstChild != null )
				return node.FirstChild.Value;
			else if ( node != null )
				return node.Value;
			else
				return defaultValue;
		}
	}
}
