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

namespace CKFinder
{
	public enum AccessControlRules
	{
		FolderView = 1,
		FolderCreate = 2,
		FolderRename = 4,
		FolderDelete = 8,

		FileView = 16,
		FileUpload = 32,
		FileRename = 64,
		FileDelete = 128
	}
}
