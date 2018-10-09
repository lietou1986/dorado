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
	public class ResourceTypeManager
	{
		System.Collections.ArrayList _ResourceTypesList;
		System.Collections.Hashtable _ResourceTypes;

		public ResourceTypeManager()
		{
			_ResourceTypesList = new System.Collections.ArrayList();
			_ResourceTypes = new System.Collections.Hashtable();
		}

		public ResourceType Add( string name )
		{
			ResourceType acl = new ResourceType( name );
			_ResourceTypesList.Add( acl );
			_ResourceTypes.Add( name, acl );
			return acl;
		}

		public ResourceType GetByName( string name )
		{
			return (ResourceType)_ResourceTypes[ name ];
		}

		public ResourceType GetByIndex( int index )
		{
			return (ResourceType)_ResourceTypesList[ index ];
		}

		public int Count
		{
			get 
			{
				return _ResourceTypesList.Count;
			}
		}
	}
}
