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
using System.Web;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CKFinder.Connector.CommandHandlers
{
	internal class RenameFolderCommandHandler : XmlCommandHandlerBase
	{
		public RenameFolderCommandHandler()
			: base()
		{
		}

		protected override void BuildXml()
		{
			if ( Request.Form["CKFinderCommand"] != "true" )
			{
				ConnectorException.Throw( Errors.InvalidRequest );
			}

			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FolderRename ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}

			// The root folder cannot be deleted.
			if ( this.CurrentFolder.ClientPath == "/" )
			{
				ConnectorException.Throw( Errors.InvalidRequest );
				return;
			}

			string newFileName = Request[ "NewFolderName" ];

			if ( !Connector.CheckFolderName( newFileName ) || Config.Current.CheckIsHiddenFolder( newFileName ) )
			{
				ConnectorException.Throw( Errors.InvalidName );
				return;
			}

			// Get the current folder.
			System.IO.DirectoryInfo oDir = new System.IO.DirectoryInfo( this.CurrentFolder.ServerPath );

			bool bMoved = false;

			try
			{
				if ( !oDir.Exists )
					ConnectorException.Throw( Errors.InvalidRequest );
				else
				{
					// Build the new folder path.
					string newFolderPath = System.IO.Path.Combine( oDir.Parent.FullName, newFileName );

					if ( System.IO.Directory.Exists( newFolderPath ) || System.IO.File.Exists( newFolderPath ) )
						ConnectorException.Throw( Errors.AlreadyExist );

					oDir.MoveTo( newFolderPath );
					bMoved = true;
				}
			}
			catch ( System.UnauthorizedAccessException )
			{
				ConnectorException.Throw( Errors.AccessDenied );
			}
			catch ( System.Security.SecurityException )
			{
				ConnectorException.Throw( Errors.AccessDenied );
			}
			catch ( System.ArgumentException )
			{
				ConnectorException.Throw( Errors.InvalidName );
			}
			catch ( System.NotSupportedException )
			{
				ConnectorException.Throw( Errors.InvalidName );
			}
			catch ( System.IO.PathTooLongException )
			{
				ConnectorException.Throw( Errors.InvalidName );
			}
			catch ( System.IO.IOException )
			{
				ConnectorException.Throw( Errors.Unknown );
			}
			catch ( ConnectorException connectorException )
			{
				throw connectorException;
			}
			catch
			{
#if DEBUG
				throw;
#else
				ConnectorException.Throw( Errors.Unknown );
#endif
			}

			if ( bMoved )
			{
				try
				{
					// Get the thumbnails folder.
					System.IO.DirectoryInfo oThumbsDir = new System.IO.DirectoryInfo( this.CurrentFolder.ThumbsServerPath );

					// Build the new folder path.
					string newThumbsFolderPath = System.IO.Path.Combine( oThumbsDir.Parent.FullName, newFileName );

					if ( System.IO.Directory.Exists( newThumbsFolderPath ) )
					{
						System.IO.File.Delete( this.CurrentFolder.ThumbsServerPath );
					}
					else
					{
						try
						{
							oThumbsDir.MoveTo( newThumbsFolderPath );
						}
						catch
						{
							System.IO.File.Delete( this.CurrentFolder.ThumbsServerPath );
						}
					}
				}
				catch { /* No errors if we are not able to delete the thumb. */ }

				string newFolderPath = Regex.Replace( this.CurrentFolder.ClientPath, "[^/]+/?$", newFileName ) + "/";
				string newFolderUrl = this.CurrentFolder.ResourceTypeInfo.Url + newFolderPath.TrimStart( '/' );

				XmlNode oRenamedNode = XmlUtil.AppendElement( this.ConnectorNode, "RenamedFolder" );
				XmlUtil.SetAttribute( oRenamedNode, "newName", newFileName );
				XmlUtil.SetAttribute( oRenamedNode, "newPath", newFolderPath );
				XmlUtil.SetAttribute( oRenamedNode, "newUrl", newFolderUrl );
			}
		}
	}
}
