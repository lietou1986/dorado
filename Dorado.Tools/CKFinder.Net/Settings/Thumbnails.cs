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
	public class Thumbnails
	{
		public string Url;
		public string Dir;
		public bool Enabled;
		public bool DirectAccess;
		public int MaxWidth;
		public int MaxHeight;
		public int Quality;

		public Thumbnails()
		{
			Url = "";
			Dir = "";
			Enabled = true;
			DirectAccess = false;
			MaxWidth = 100;
			MaxHeight = 100;
			Quality = 80;
		}

		public string GetTargetDirectory()
		{
			if ( Dir.Length == 0 )
				return System.Web.HttpContext.Current.Server.MapPath( Url );
			else
				return Dir;
		}
	}
}
