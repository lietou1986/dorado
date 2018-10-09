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
using System.Web;
using System.Xml;
using System.Globalization;

namespace CKFinder.Connector.CommandHandlers
{
	internal class DeleteFolderCommandHandler : XmlCommandHandlerBase
	{
		public DeleteFolderCommandHandler()
			: base()
		{
		}

		protected override void BuildXml()
		{
			if ( Request.Form["CKFinderCommand"] != "true" )
			{
				ConnectorException.Throw( Errors.InvalidRequest );
			}

			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FolderDelete ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}

			// The root folder cannot be deleted.
			if ( this.CurrentFolder.ClientPath == "/" )
			{
				ConnectorException.Throw( Errors.InvalidRequest );
				return;
			}

			if ( !System.IO.Directory.Exists( this.CurrentFolder.ServerPath ) )
				ConnectorException.Throw( Errors.FolderNotFound );

			try
			{
				System.IO.Directory.Delete( this.CurrentFolder.ServerPath, true );
			}
			catch ( System.UnauthorizedAccessException )
			{
				ConnectorException.Throw( Errors.AccessDenied );
			}
			catch ( System.Security.SecurityException )
			{
				ConnectorException.Throw( Errors.AccessDenied );
			}
			catch ( System.ArgumentException )
			{
				ConnectorException.Throw( Errors.InvalidName );
			}
			catch ( System.IO.PathTooLongException )
			{
				ConnectorException.Throw( Errors.InvalidName );
			}
			catch ( Exception )
			{
#if DEBUG
				throw;
#else
				ConnectorException.Throw( Errors.Unknown );
#endif
			}

			try
			{
				System.IO.Directory.Delete( this.CurrentFolder.ThumbsServerPath, true );
			}
			catch { /* No errors if we are not able to delete the thumbs directory. */ }
		}
	}
}
