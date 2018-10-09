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
using System.Text.RegularExpressions;

namespace CKFinder.Connector.CommandHandlers
{
	public abstract class CommandHandlerBase
	{
		private Connector _Connector;
		private FolderHandler _CurrentFolder;
		private HttpRequest _Request;

		public CommandHandlerBase()
		{
			_CurrentFolder = FolderHandler.GetCurrent();
			_Request = HttpContext.Current.Request;
		}

		protected HttpRequest Request
		{
			get { return _Request; }
		}

		protected Connector Connector
		{
			get
			{
				if ( _Connector == null )
					_Connector = (Connector)HttpContext.Current.Handler;
				return _Connector;
			}
		}

		public FolderHandler CurrentFolder
		{
			get
			{
				if ( _CurrentFolder == null )
					_CurrentFolder = FolderHandler.GetCurrent();
				return _CurrentFolder;
			}
		}

		protected void CheckConnector()
		{
			if ( !Config.Current.CheckAuthentication() )
				ConnectorException.Throw( Errors.ConnectorDisabled );
		}

		protected void CheckRequest()
		{
			// Check if the current folder is a valid path.
			if ( Regex.IsMatch( this.CurrentFolder.ClientPath, @"(/\.)|(\.\.)|(//)|([\\:\*\?""\<\>\|])" ) )
				ConnectorException.Throw( Errors.InvalidName );

			// Check all parts of "CurrentFolder".
			string[] dirs = this.CurrentFolder.ClientPath.Split( '/' ) ;
			foreach ( string dir in dirs )
			{
				if ( Config.Current.CheckIsHiddenFolder( dir ) )
					ConnectorException.Throw( Errors.InvalidRequest );
			}

			if ( this.CurrentFolder.ResourceTypeInfo == null )
				ConnectorException.Throw( Errors.InvalidType );

			if ( !this.CurrentFolder.FolderInfo.Exists )
			{
				if ( this.CurrentFolder.ClientPath == "/" )
				{
					try
					{
						this.CurrentFolder.FolderInfo.Create();
					}
					catch ( System.UnauthorizedAccessException )
					{
#if DEBUG
						throw;
#else
						ConnectorException.Throw( Errors.AccessDenied );
#endif
					}
					catch ( System.Security.SecurityException )
					{
#if DEBUG
						throw;
#else
						ConnectorException.Throw( Errors.AccessDenied );
#endif
					}
					catch
					{
#if DEBUG
						throw;
#else
						ConnectorException.Throw( Errors.Unknown );
#endif
					}
				}
				else
					ConnectorException.Throw( Errors.FolderNotFound );
			}
		}

		public abstract void SendResponse( HttpResponse response );

		protected Boolean hasChildren( String ClientPath, System.IO.DirectoryInfo oDir )
		{
			return this.hasChildren(ClientPath, oDir, this.CurrentFolder.ResourceTypeName);
		}

		protected Boolean hasChildren( String ClientPath, System.IO.DirectoryInfo oDir, String resourceTypeName )
		{
			System.IO.DirectoryInfo[] aSubDirs = oDir.GetDirectories();

			for ( int i = 0; i < aSubDirs.Length; i++ )
			{
				string sSubDirName = aSubDirs[i].Name;

				if ( Config.Current.CheckIsHiddenFolder( sSubDirName ) )
					continue;

				int aclMask = Config.Current.AccessControl.GetComputedMask( resourceTypeName, ClientPath + sSubDirName + "/" );

				if ( ( aclMask & (int)AccessControlRules.FolderView ) != (int)AccessControlRules.FolderView )
					continue;

				return true;
			}
			return false;
		}
	}
}
