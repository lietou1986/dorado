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

namespace CKFinder.Connector
{
	public static class Errors
	{
		public const int None = 0;
		public const int CustomError = 1;
		public const int InvalidCommand = 10;
		public const int TypeNotSpecified = 11;
		public const int InvalidType = 12;
		public const int InvalidName = 102;
		public const int Unauthorized = 103;
		public const int AccessDenied = 104;
		public const int InvalidExtension = 105;
		public const int InvalidRequest = 109;
		public const int Unknown = 110;
		public const int AlreadyExist = 115;
		public const int FolderNotFound = 116;
		public const int FileNotFound = 117;
		public const int SourceAndTargetPathEqual = 118;
		public const int UploadedFileRenamed = 201;
		public const int UploadedInvalid = 202;
		public const int UploadedTooBig = 203;
		public const int UploadedCorrupt = 204;
		public const int UploadedNoTmpDir = 205;
		public const int UploadedWrongHtmlFile = 206;
		public const int UploadedInvalidNameRenamed = 207;
		public const int MoveFailed = 300;
		public const int CopyFailed = 301;
		public const int DeleteFailed = 302;
		public const int ConnectorDisabled = 500;
		public const int ThumbnailsDisabled = 501;
	}
}
