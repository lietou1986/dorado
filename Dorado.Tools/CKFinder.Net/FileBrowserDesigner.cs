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
	public class FileBrowserDesigner : System.Web.UI.Design.ControlDesigner
	{
		public FileBrowserDesigner()
		{ }

		public override string GetDesignTimeHtml()
		{
			FileBrowser _FileBrowser = (FileBrowser)this.Component;

			return "<table width=\"" + _FileBrowser.Width + 
				"\" height=\"" + _FileBrowser.Height + 
				"\" bgcolor=\"#f5f5f5\" bordercolor=\"#c7c7c7\" cellpadding=\"0\" cellspacing=\"0\" border=\"1\"><tr><td valign=\"middle\" align=\"center\">CKFinder - <b>" + _FileBrowser.ID + "</b></td></tr></table>";
		}
	}
}
