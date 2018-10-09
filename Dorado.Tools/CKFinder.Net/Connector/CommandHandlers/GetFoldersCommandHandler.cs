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
	internal class GetFoldersCommandHandler : XmlCommandHandlerBase
	{
		public GetFoldersCommandHandler()
			: base()
		{
		}

		protected override void BuildXml()
		{
			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FolderView ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}
			
			// Map the virtual path to the local server path.
			string sServerDir = this.CurrentFolder.ServerPath;

			System.IO.DirectoryInfo oDir = new System.IO.DirectoryInfo( sServerDir );
			if ( !oDir.Exists )
			{
				ConnectorException.Throw( Errors.FolderNotFound );
				return;
			}

			// Create the "Folders" node.
			XmlNode oFoldersNode = XmlUtil.AppendElement( this.ConnectorNode, "Folders" );

			System.IO.DirectoryInfo[] aSubDirs = oDir.GetDirectories();

			for ( int i = 0 ; i < aSubDirs.Length ; i++ )
			{
				string sSubDirName = aSubDirs[ i ].Name;

				if ( Config.Current.CheckIsHiddenFolder( sSubDirName ) )
					continue;

				int aclMask = Config.Current.AccessControl.GetComputedMask( this.CurrentFolder.ResourceTypeName, this.CurrentFolder.ClientPath + sSubDirName + "/" );

				if ( ( aclMask & (int)AccessControlRules.FolderView ) != (int)AccessControlRules.FolderView )
					continue;

				// Create the "Folders" node.
				XmlNode oFolderNode = XmlUtil.AppendElement( oFoldersNode, "Folder" );
				XmlUtil.SetAttribute( oFolderNode, "name", sSubDirName );
				try
				{
					XmlUtil.SetAttribute(oFolderNode, "hasChildren", this.hasChildren( this.CurrentFolder.ClientPath + sSubDirName + "/", aSubDirs[i] ) ? "true" : "false");
				}
				catch
				{
					// It was not possible to verify if it has children. Assume "yes".
					XmlUtil.SetAttribute( oFolderNode, "hasChildren", "true" );
				}
				XmlUtil.SetAttribute( oFolderNode, "acl", aclMask.ToString() );
			}
		}
	}
}
