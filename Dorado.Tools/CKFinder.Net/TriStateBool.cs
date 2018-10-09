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

namespace CKFinder
{
	public class TriStateBool
	{
		public const int True = 1;
		public const int False = 0;
		public const int Undefined = 2;

		private int _Value = 0;

		public static implicit operator TriStateBool( bool value )
		{
			TriStateBool triStateBool = new TriStateBool();
			triStateBool._Value = value ? True : False;
			return triStateBool;
		}

		public static implicit operator TriStateBool( int value )
		{
			TriStateBool triStateBool = new TriStateBool();
			triStateBool._Value = value;
			return triStateBool;
		}

		public static implicit operator int( TriStateBool value )
		{
			return value._Value ;
		}
	}
}
