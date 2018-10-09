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
using System.Collections;

namespace CKFinder.Settings
{
	public class AccessControlManager
	{
		private ArrayList _AclEntries;

		private bool _IsAclEntriesComputed;
		private Hashtable _ComputedAclEntries;

		public AccessControlManager()
		{
			_AclEntries = new ArrayList();
			_ComputedAclEntries = new Hashtable();
		}

		public AccessControl Add()
		{
			AccessControl acl = new AccessControl();
			_AclEntries.Add( acl );
			return acl;
		}

		private void ComputeAclEntries()
		{
			for ( int i = 0 ; i < _AclEntries.Count ; i++ )
			{
				AccessControl acl = (AccessControl)_AclEntries[ i ];

				string folderPath = acl.Folder;
				string entryKey = acl.GetInternalKey();
				int allowRulesMask = acl.GetAllowedMask();
				int denyRulesMask = acl.GetDeniedMask();

				if ( _ComputedAclEntries.ContainsKey( folderPath ) )
				{
					if ( ( (Hashtable)_ComputedAclEntries[ folderPath ] ).ContainsKey( entryKey ) )
					{
						int[] rulesMasks = (int[])( (Hashtable)_ComputedAclEntries[ folderPath ] )[ entryKey ];
						allowRulesMask |= rulesMasks[ 0 ];
						denyRulesMask |= rulesMasks[ 1 ];
					}
				}
				else
					_ComputedAclEntries[ folderPath ] = new Hashtable( 1 );

				( (Hashtable)_ComputedAclEntries[ folderPath ] )[ entryKey ] = new int[] { allowRulesMask, denyRulesMask };
			}

			_IsAclEntriesComputed = true;
		}

		public int GetComputedMask( string resourceType, string folderPath )
		{
			if ( !_IsAclEntriesComputed )
				ComputeAclEntries();

			int computedMask = 0;

			// Get the user role from the session.
			string userRole = null;
			try
			{
				if (ConfigFile.Current.RoleSessionVar.Length > 0 && System.Web.HttpContext.Current.Session != null)
					userRole = System.Web.HttpContext.Current.Session[ConfigFile.Current.RoleSessionVar] as string;
				if (userRole != null && userRole.Length == 0)
					userRole = null;
			}
			catch { userRole = null; }

			// Take the folder parts.
			folderPath = folderPath.Trim( '/' );
			string[] pathParts = folderPath.Split( '/' );

			string currentPath = "/";

			for ( int i = -1 ; i < pathParts.Length ; i++ )
			{
				if ( i >= 0 )
				{
					if ( pathParts[ i ].Length == 0 )
						continue;

					if ( _ComputedAclEntries.ContainsKey( currentPath + "*/" ) )
						computedMask = this.MergePathComputedMask( computedMask, resourceType, userRole, currentPath + "*/" );

					currentPath += pathParts[ i ] + "/";
				}

				if ( _ComputedAclEntries.ContainsKey( currentPath ) )
					computedMask = this.MergePathComputedMask( computedMask, resourceType, userRole, currentPath );
			}

			return computedMask;
		}

		private int MergePathComputedMask( int currentMask, string resourceType, string userRole, string path )
		{
			Hashtable folderEntries = (Hashtable)_ComputedAclEntries[ path ];

			string[] possibleEntries = new string[ userRole != null ? 4 : 2 ];

			possibleEntries[ 0 ] = "*#@#*";
			possibleEntries[ 1 ] = "*#@#" + resourceType;

			if ( userRole != null )
			{
				possibleEntries[ 2 ] = userRole + "#@#*";
				possibleEntries[ 3 ] = userRole + "#@#" + resourceType;
			}

			for ( int r = 0 ; r < possibleEntries.Length ; r++ )
			{
				string possibleKey = possibleEntries[ r ];
				if ( folderEntries.ContainsKey( possibleKey ) )
				{
					int[] rulesMasks = (int[])folderEntries[ possibleKey ];

					currentMask |= rulesMasks[ 0 ];
					currentMask ^= ( currentMask & rulesMasks[ 1 ] );
				}
			}

			return currentMask;
		}
	}
}
