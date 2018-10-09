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
using System.Text.RegularExpressions;

namespace CKFinder.Connector
{
	public class FolderHandler
	{
		private Settings.ResourceType _ResourceTypeInfo;

		private string _ResourceTypeName;
		private string _ClientPath;
		private string _Url;
		private string _ServerPath;
		private string _ThumbsServerPath;
		private int _AclMask;

		private System.IO.DirectoryInfo _FolderInfo;
		private System.IO.DirectoryInfo _ThumbsFolderInfo;

		public FolderHandler( string resourceTypeName, string clientFolderPath )
		{
			_ResourceTypeName = resourceTypeName;

			// ## ClientPath
			_ClientPath = clientFolderPath;

			// Check the current folder syntax (must begin and start with a slash).
			if ( !_ClientPath.EndsWith( "/" ) )
				_ClientPath += "/";
			if ( !_ClientPath.StartsWith( "/" ) )
				_ClientPath = "/" + _ClientPath;

			_AclMask = -1;
		}

		public static FolderHandler GetCurrent()
		{
			System.Web.HttpRequest _Request = System.Web.HttpContext.Current.Request;

			string _ResourceType = _Request.QueryString[ "type" ];
			if ( _ResourceType == null )
				_ResourceType = "";
			//				ConnectorException.Throw( Errors.TypeNotSpecified );

			string _CurrentFolder = _Request.QueryString[ "currentFolder" ];
			if ( _CurrentFolder == null )
				_CurrentFolder = "/";

			return new FolderHandler( _ResourceType, _CurrentFolder );
		}

		public Settings.ResourceType ResourceTypeInfo
		{
			get
			{
				if ( _ResourceTypeInfo == null )
				{
					_ResourceTypeInfo = Config.Current.GetResourceTypeConfig( this.ResourceTypeName );
					if ( _ResourceTypeInfo == null )
						ConnectorException.Throw( Errors.InvalidType );
				}
				return _ResourceTypeInfo;
			}
		}

		public string ResourceTypeName
		{
			get { return _ResourceTypeName; }
		}

		public string ClientPath
		{
			get { return _ClientPath; }
		}

		public string Url
		{
			get
			{
				if ( _Url == null )
					_Url = this.ResourceTypeInfo.Url + this.ClientPath.TrimStart( '/' );
				return _Url;
			}
		}

		public string ServerPath
		{
			get
			{
				if ( _ServerPath == null )
					_ServerPath = System.IO.Path.Combine( this.ResourceTypeInfo.GetTargetDirectory(), this.ClientPath.TrimStart( '/' ) );
				return _ServerPath;
			}
		}

		public string ThumbsServerPath
		{
			get
			{
				if ( _ThumbsServerPath == null )
				{
					// Get the resource type directory.
					_ThumbsServerPath = System.IO.Path.Combine( Config.Current.Thumbnails.GetTargetDirectory(), this.ResourceTypeInfo.Name );

					// Return the resource type directory combined with the required path.
					_ThumbsServerPath = System.IO.Path.Combine( _ThumbsServerPath, this.ClientPath.TrimStart( '/' ) );

					// Ensure that the directory exists.
					System.IO.Directory.CreateDirectory( _ThumbsServerPath );
				}
				return _ThumbsServerPath;
			}
		}

		public System.IO.DirectoryInfo FolderInfo
		{
			get
			{
				if ( _FolderInfo == null )
					_FolderInfo = new System.IO.DirectoryInfo( this.ServerPath );
				return _FolderInfo;
			}
		}

		public System.IO.DirectoryInfo ThumbsFolderInfo
		{
			get
			{
				if ( _ThumbsFolderInfo == null )
					_ThumbsFolderInfo = new System.IO.DirectoryInfo( this.ThumbsServerPath );
				return _ThumbsFolderInfo;
			}
		}

		public int AclMask
		{
			get
			{
				if ( _AclMask == -1 )
					_AclMask = Config.Current.AccessControl.GetComputedMask( this.ResourceTypeName, this.ClientPath );
				return _AclMask;
			}
		}

		public bool CheckAcl( int aclToCheck )
		{
			return ( ( this.AclMask & aclToCheck ) == aclToCheck );
		}

		public bool CheckAcl( AccessControlRules aclToCheck )
		{
			return this.CheckAcl( (int)aclToCheck );
		}
	}
}
