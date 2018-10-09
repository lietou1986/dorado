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

namespace CKFinder.Settings
{
	public class AccessControl
	{
		public string Role;
		public string ResourceType;
		private string _Folder;

		public TriStateBool FolderView;
		public TriStateBool FolderCreate;
		public TriStateBool FolderRename;
		public TriStateBool FolderDelete;

		public TriStateBool FileView;
		public TriStateBool FileUpload;
		public TriStateBool FileRename;
		public TriStateBool FileDelete;

		internal AccessControl()
		{
			Role = "*";
			ResourceType = "*";
			Folder = "/";

			FolderView = TriStateBool.Undefined;
			FolderCreate = TriStateBool.Undefined;
			FolderRename = TriStateBool.Undefined;
			FolderDelete = TriStateBool.Undefined;

			FileView = TriStateBool.Undefined;
			FileUpload = TriStateBool.Undefined;
			FileRename = TriStateBool.Undefined;
			FileDelete = TriStateBool.Undefined;
		}

		public string Folder
		{
			get
			{
				return _Folder;
			}

			set
			{
				_Folder = value;

				if ( !_Folder.StartsWith( "/" ) )
					_Folder = "/" + _Folder;

				if ( !_Folder.EndsWith( "/" ) )
					_Folder += "/";
			}
		}

		public string GetInternalKey()
		{
			return Role + "#@#" + ResourceType;
		}

		public int GetAllowedMask()
		{
			return ( FolderView == TriStateBool.True ? (int)AccessControlRules.FolderView : 0 )
					| ( FolderCreate == TriStateBool.True ? (int)AccessControlRules.FolderCreate : 0 )
					| ( FolderRename == TriStateBool.True ? (int)AccessControlRules.FolderRename : 0 )
					| ( FolderDelete == TriStateBool.True ? (int)AccessControlRules.FolderDelete : 0 )
					| ( FileView == TriStateBool.True ? (int)AccessControlRules.FileView : 0 )
					| ( FileUpload == TriStateBool.True ? (int)AccessControlRules.FileUpload : 0 )
					| ( FileRename == TriStateBool.True ? (int)AccessControlRules.FileRename : 0 )
					| ( FileDelete == TriStateBool.True ? (int)AccessControlRules.FileDelete : 0 );
		}

		public int GetDeniedMask()
		{
			return ( FolderView == TriStateBool.False ? (int)AccessControlRules.FolderView : 0 )
					| ( FolderCreate == TriStateBool.False ? (int)AccessControlRules.FolderCreate : 0 )
					| ( FolderRename == TriStateBool.False ? (int)AccessControlRules.FolderRename : 0 )
					| ( FolderDelete == TriStateBool.False ? (int)AccessControlRules.FolderDelete : 0 )
					| ( FileView == TriStateBool.False ? (int)AccessControlRules.FileView : 0 )
					| ( FileUpload == TriStateBool.False ? (int)AccessControlRules.FileUpload : 0 )
					| ( FileRename == TriStateBool.False ? (int)AccessControlRules.FileRename : 0 )
					| ( FileDelete == TriStateBool.False ? (int)AccessControlRules.FileDelete : 0 );
		}
	}
}
