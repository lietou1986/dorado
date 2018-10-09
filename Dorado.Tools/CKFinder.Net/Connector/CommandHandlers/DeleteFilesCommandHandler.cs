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
using System.Collections;
using System.Text.RegularExpressions;

namespace CKFinder.Connector.CommandHandlers
{
	internal class DeleteFilesCommandHandler : XmlCommandHandlerBase
	{
		private XmlNode ErrorsNode;

		public DeleteFilesCommandHandler()
			: base()
		{
		}

		private void appendErrorNode( int errorCode, string name, string type, string path )
		{
			if ( this.ErrorsNode == null )
				this.ErrorsNode = XmlUtil.AppendElement( this.ConnectorNode, "Errors" );

			XmlNode Error = XmlUtil.AppendElement( this.ErrorsNode, "Error" );
			XmlUtil.SetAttribute( Error, "code", errorCode.ToString() );
			XmlUtil.SetAttribute( Error, "name", name );
			XmlUtil.SetAttribute(Error, "type", type);
			XmlUtil.SetAttribute(Error, "folder", path);
		}

		protected override void BuildXml()
		{
			if ( Request.Form["CKFinderCommand"] != "true" )
			{
				ConnectorException.Throw( Errors.InvalidRequest );
			}

			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FileDelete ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}

			Settings.ResourceType resourceType;
			Hashtable resourceTypeConfig = new Hashtable();
			Hashtable checkedPaths = new Hashtable();
			XmlNode oDeletedFileNode = null;
			int iFileNum = 0;
			int deletedNum = 0;

			while ( Request.Form["files[" + iFileNum.ToString() + "][type]"] != null && Request.Form["files[" + iFileNum.ToString() + "][type]"].Length > 0 )
			{
				string name = Request.Form["files[" + iFileNum.ToString() + "][name]"];
				string type = Request.Form["files[" + iFileNum.ToString() + "][type]"];
				string path = Request.Form["files[" + iFileNum.ToString() + "][folder]"];

				if ( name == null || name.Length < 1 || type == null || type.Length < 1 || path == null || path.Length < 1 )
				{
					ConnectorException.Throw( Errors.InvalidRequest );
					return;
				}
				iFileNum++;

				// check #1 (path)
				if ( !Connector.CheckFileName( name ) || Regex.IsMatch( path, @"(/\.)|(\.\.)|(//)|([\\:\*\?""\<\>\|])" ) )
				{
					ConnectorException.Throw( Errors.InvalidRequest );
					return;
				}

				// get resource type config for current file
				if ( !resourceTypeConfig.ContainsKey( type ) )
				{
					resourceTypeConfig[type] = Config.Current.GetResourceTypeConfig( type );
				}

				// check #2 (resource type)
				if ( resourceTypeConfig[type] == null )
				{
					ConnectorException.Throw( Errors.InvalidRequest );
					return;
				}

				resourceType = (Settings.ResourceType)resourceTypeConfig[type];
				FolderHandler folder = new FolderHandler( type, path );
				string filePath = System.IO.Path.Combine( folder.ServerPath, name );

				// check #3 (extension)
				if ( !resourceType.CheckExtension( System.IO.Path.GetExtension( name ) ) )
				{
					ConnectorException.Throw( Errors.InvalidRequest );
					return;
				}

				// check #5 (hidden folders)
				if ( !checkedPaths.ContainsKey( path ) )
				{
					checkedPaths[path] = true;
					if ( Config.Current.CheckIsHidenPath( path ) )
					{
						ConnectorException.Throw( Errors.InvalidRequest );
						return;
					}
				}

				// check #6 (hidden file name)
				if ( Config.Current.CheckIsHiddenFile( name ) )
				{
					ConnectorException.Throw( Errors.InvalidRequest );
					return;
				}

				// check #7 (Access Control, need file view permission to source files)
				if ( !folder.CheckAcl( AccessControlRules.FileDelete ) )
				{
					ConnectorException.Throw( Errors.Unauthorized );
					return;
				}

				// check #8 (invalid file name)
				if ( !System.IO.File.Exists( filePath ) || System.IO.Directory.Exists( filePath ) )
				{
					this.appendErrorNode( Errors.FileNotFound, name, type, path );
					continue;
				}

				bool bDeleted = false;

				try
				{
					System.IO.File.Delete( filePath );
					bDeleted = true;
					deletedNum++;
				}
				catch ( System.UnauthorizedAccessException )
				{
					this.appendErrorNode( Errors.AccessDenied, name, type, path );
				}
				catch ( System.Security.SecurityException )
				{
					this.appendErrorNode( Errors.AccessDenied, name, type, path );
				}
				catch ( System.ArgumentException )
				{
					this.appendErrorNode( Errors.FileNotFound, name, type, path );
				}
				catch ( System.IO.PathTooLongException )
				{
					this.appendErrorNode( Errors.FileNotFound, name, type, path );
				}
				catch
				{
#if DEBUG
				throw;
#else
					this.appendErrorNode( Errors.Unknown, name, type, path );
#endif
				}

				if ( bDeleted )
				{
					try
					{
						string thumbPath = System.IO.Path.Combine( folder.ThumbsServerPath, name );
						System.IO.File.Delete( thumbPath );
					}
					catch { /* No errors if we are not able to delete the thumb. */ }
				}
			}
			oDeletedFileNode = XmlUtil.AppendElement( this.ConnectorNode, "DeleteFiles" );
			XmlUtil.SetAttribute( oDeletedFileNode, "deleted", deletedNum.ToString() );
			if ( this.ErrorsNode != null )
			{
				ConnectorException.Throw( Errors.DeleteFailed );
				return;
			}
		}
	}
}
