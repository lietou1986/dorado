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
using System.Web ;
using System.Xml ;

namespace CKFinder.Connector.CommandHandlers
{
	public abstract class XmlCommandHandlerBase : CommandHandlerBase
	{
		private XmlDocument _Xml ;
		private XmlNode _ConnectorNode ;

		public XmlCommandHandlerBase() : base()
		{}

		public override void SendResponse( HttpResponse response )
		{
			// Cleans the response buffer.
			response.ClearHeaders();
			response.Clear();

			// Prevent the browser from caching the result.
			response.CacheControl = "no-cache";

			// Set the response format.
			response.ContentEncoding = System.Text.UTF8Encoding.UTF8;
			response.ContentType = "text/xml";

			_Xml = new XmlDocument();

			// Create the XML document header.
			_Xml.AppendChild( _Xml.CreateXmlDeclaration( "1.0", "utf-8", null ) ) ;

			// Create the main "Connector" node.
			_ConnectorNode = XmlUtil.AppendElement( _Xml, "Connector" ) ;

			XmlNode oErrorNode = XmlUtil.AppendElement( this.ConnectorNode, "Error" ) ;

			try
			{
				this.CheckConnector();

				if ( this.MustCheckRequest() )
					this.CheckRequest();

				if ( this.CurrentFolder.ResourceTypeName.Length > 0 )
					XmlUtil.SetAttribute( _ConnectorNode, "resourceType", this.CurrentFolder.ResourceTypeName );

				if ( this.MustIncludeCurrentFolder() )
				{
					// Add the current folder node.
					XmlNode currentFolder = XmlUtil.AppendElement( _ConnectorNode, "CurrentFolder" );
					XmlUtil.SetAttribute( currentFolder, "path", this.CurrentFolder.ClientPath );
					try
					{
						XmlUtil.SetAttribute( currentFolder, "url", this.CurrentFolder.Url );
					}
					catch
					{
						XmlUtil.SetAttribute( currentFolder, "url", "" );
					}

					XmlUtil.SetAttribute( currentFolder, "acl", this.CurrentFolder.AclMask.ToString() );
				}

				this.BuildXml();

				XmlUtil.SetAttribute( oErrorNode, "number", "0" );
			}
			catch ( ConnectorException connectorException )
			{
			    XmlUtil.SetAttribute( oErrorNode, "number", connectorException.Number.ToString() );
			}
			catch
			{
#if DEBUG
				throw;
#else
			    XmlUtil.SetAttribute( oErrorNode, "number", Errors.Unknown.ToString() );
#endif
			}

			// Output the resulting XML.
			response.Write( _Xml.OuterXml ) ;

			response.End() ;
		}

		protected XmlDocument Xml
		{
			get { return _Xml ; }
		}

		protected XmlNode ConnectorNode
		{
			get { return _ConnectorNode ; }
		}

		protected virtual bool MustCheckRequest()
		{
			return true;
		}

		protected virtual bool MustIncludeCurrentFolder()
		{
			return true;
		}

		protected abstract void BuildXml();
	}
}
