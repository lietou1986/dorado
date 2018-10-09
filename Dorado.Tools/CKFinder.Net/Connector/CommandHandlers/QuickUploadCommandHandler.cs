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

namespace CKFinder.Connector.CommandHandlers
{
	internal class QuickUploadCommandHandler : FileUploadCommandHandler
	{
		public QuickUploadCommandHandler()
			: base()
		{
		}

		protected override string GetJavaScriptCode( int errorNumber, string fileName, string folderUrl )
		{
			System.Web.HttpRequest _Request = System.Web.HttpContext.Current.Request;
			string _CKEditorName = _Request.QueryString["CKEditor"];
			string _funcNum = _Request.QueryString["CKEditorFuncNum"];
			string _errorMsg = "";
			string fileUrl = folderUrl + CKFinder.Connector.Util.encodeURIComponent(fileName);

			if ( _CKEditorName == null || _funcNum == null )
			{
				switch ( errorNumber )
				{
					case Errors.None:
					case Errors.UploadedFileRenamed:
					case Errors.UploadedInvalidNameRenamed:
						return "window.parent.OnUploadCompleted(" + errorNumber + ",'" + fileUrl.Replace( "'", "\\'" ) + "','" + fileName.Replace( "'", "\\'" ) + "') ;";
					default:
						return "window.parent.OnUploadCompleted(" + errorNumber + ") ;";
				}
			}
			else
			{
				_funcNum = Regex.Replace( _funcNum, @"[^0-9]", "", RegexOptions.None );
				if ( errorNumber > 0 )
				{
					_errorMsg = Lang.getErrorMessage( errorNumber ).Replace( "%1", fileName );
					if ( errorNumber != Errors.UploadedFileRenamed )
						fileUrl = "";
				}
				return "window.parent.CKEDITOR.tools.callFunction(" + _funcNum + ",'" + fileUrl.Replace( "'", "\\'" ) + "','" + _errorMsg.Replace( "'", "\\'" ) + "') ;";
			}
		}
	}
}
