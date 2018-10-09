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
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CKFinder.Connector.CommandHandlers
{
	internal class InitCommandHandler : XmlCommandHandlerBase
	{
		public const string K_CHARS = "123456789ABCDEFGHJKLMNPQRSTUVWXYZ";

		public InitCommandHandler()
			: base()
		{ }

		protected override bool MustCheckRequest()
		{
			return false;
		}

		protected override bool MustIncludeCurrentFolder()
		{
			return false;
		}

		protected override void BuildXml()
		{
			Config _Config = Config.Current;
			bool _IsEnabled = _Config.CheckAuthentication();

			// Create the "ConnectorInfo" node.
			XmlNode oConnInfo = XmlUtil.AppendElement( this.ConnectorNode, "ConnectorInfo" );
			XmlUtil.SetAttribute( oConnInfo, "enabled", _IsEnabled.ToString().ToLower() );

			if ( !_IsEnabled )
				ConnectorException.Throw( Errors.ConnectorDisabled );

			XmlUtil.SetAttribute(oConnInfo, "csrfProtection", _Config.EnableCsrfProtection ? "true" : "false");

			string ln = "" ;
			string lc = _Config.LicenseKey.ToUpper() ;
			int index = K_CHARS.IndexOf( lc[0] ) % 5;

			if ( 1 == index || 4 == index )
				ln = _Config.LicenseName.ToLower();

			XmlUtil.SetAttribute( oConnInfo, "imgWidth", _Config.Images.MaxWidth.ToString() );
			XmlUtil.SetAttribute( oConnInfo, "imgHeight", _Config.Images.MaxHeight.ToString() );
			XmlUtil.SetAttribute( oConnInfo, "s", ln );
			XmlUtil.SetAttribute( oConnInfo, "c", string.Concat( lc[11] , lc[0] , lc[8] , lc[12], lc[26], lc[2], lc[3], lc[25], lc[1] ).Trim() );

			XmlUtil.SetAttribute( oConnInfo, "thumbsEnabled", _Config.Thumbnails.Enabled.ToString().ToLower() );
			XmlUtil.SetAttribute( oConnInfo, "uploadCheckImages", _Config.CheckSizeAfterScaling ? "false" : "true");
			if ( _Config.Thumbnails.Enabled )
			{
				XmlUtil.SetAttribute( oConnInfo, "thumbsUrl", _Config.Thumbnails.Url );
				XmlUtil.SetAttribute( oConnInfo, "thumbsWidth", _Config.Thumbnails.MaxWidth.ToString() );
				XmlUtil.SetAttribute( oConnInfo, "thumbsHeight", _Config.Thumbnails.MaxHeight.ToString() );
				XmlUtil.SetAttribute( oConnInfo, "thumbsDirectAccess", _Config.Thumbnails.DirectAccess.ToString().ToLower() );
			}

			// Create the "ResourceTypes" node.
			XmlNode oResourceTypes = XmlUtil.AppendElement( this.ConnectorNode, "ResourceTypes" );
			XmlNode oPluginsInfo = XmlUtil.AppendElement( this.ConnectorNode, "PluginsInfo" );

			// Load the resource types in an array.
			string[] aTypes;

			if ( Request.QueryString[ "type" ] != null && Request.QueryString[ "type" ].Length > 0 )
			{
				aTypes = new string[] { Request.QueryString[ "type" ] };
			}
			else
			{
				string sDefaultTypes = Config.Current.DefaultResourceTypes ;

				if ( sDefaultTypes.Length == 0 )
				{
					aTypes = new string[ _Config.ResourceTypes.Count ];

					for ( int i = 0 ; i < _Config.ResourceTypes.Count ; i++ )
					{
						aTypes[ i ] = _Config.ResourceTypes.GetByIndex( i ).Name;
					}
				}
				else
					aTypes = sDefaultTypes.Split( ',' );
			}

			for ( int i = 0 ; i < aTypes.Length ; i++ )
			{
				string resourceTypeName = aTypes[ i ];

				int aclMask = Config.Current.AccessControl.GetComputedMask( resourceTypeName, "/" );

				if ( ( aclMask & (int)AccessControlRules.FolderView ) != (int)AccessControlRules.FolderView )
					continue;

				Settings.ResourceType oTypeInfo = _Config.GetResourceTypeConfig( resourceTypeName );
				string sTargetDirectory = oTypeInfo.GetTargetDirectory();

				bool bHasChildren = System.IO.Directory.Exists( sTargetDirectory ) && this.hasChildren( "/", new System.IO.DirectoryInfo( oTypeInfo.GetTargetDirectory() ), resourceTypeName );

				XmlNode oResourceType = XmlUtil.AppendElement( oResourceTypes, "ResourceType" );
				XmlUtil.SetAttribute( oResourceType, "name", resourceTypeName );
				XmlUtil.SetAttribute( oResourceType, "url", oTypeInfo.Url );
				XmlUtil.SetAttribute( oResourceType, "maxSize", oTypeInfo.MaxSize.ToString() );
				XmlUtil.SetAttribute( oResourceType, "allowedExtensions", string.Join( ",", oTypeInfo.AllowedExtensions ) );
				XmlUtil.SetAttribute( oResourceType, "deniedExtensions", string.Join( ",", oTypeInfo.DeniedExtensions ) );
				XmlUtil.SetAttribute( oResourceType, "hash", Util.GetMACTripleDESHash( sTargetDirectory ).Substring( 0, 16 ) );
				XmlUtil.SetAttribute( oResourceType, "hasChildren", ( bHasChildren ? "true" : "false" ) );
				XmlUtil.SetAttribute( oResourceType, "acl", aclMask.ToString() );
			}

			if ( Connector.JavascriptPlugins != null && Connector.JavascriptPlugins.Count > 0 )
			{
				string Plugins = "";
				foreach( string pluginName in Connector.JavascriptPlugins )
				{
					if ( Plugins.Length > 0 )
						Plugins += ",";

					Plugins += pluginName;
				}
				XmlUtil.SetAttribute( oConnInfo, "plugins", Plugins );
			}

			Connector.CKFinderEvent.ActivateEvent( CKFinderEvent.Hooks.InitCommand, this.ConnectorNode );
		}
	}
}
