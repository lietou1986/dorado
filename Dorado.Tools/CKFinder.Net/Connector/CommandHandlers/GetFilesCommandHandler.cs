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
using System.Xml ;
using System.Globalization ;

namespace CKFinder.Connector.CommandHandlers
{
	internal class GetFilesCommandHandler : XmlCommandHandlerBase
	{
		public GetFilesCommandHandler() : base()
		{
		}

		protected override void BuildXml()
		{
			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FileView ) )
			{
				ConnectorException.Throw( Errors.Unauthorized );
			}

			// Map the virtual path to the local server path.
			string sServerDir = this.CurrentFolder.ServerPath ;
			bool bShowThumbs = Request.QueryString["showThumbs"] != null && Request.QueryString["showThumbs"].ToString().Equals( "1" ) ;

			// Create the "Files" node.
			XmlNode oFilesNode = XmlUtil.AppendElement( this.ConnectorNode, "Files" ) ;

			System.IO.DirectoryInfo oDir = new System.IO.DirectoryInfo( sServerDir ) ;
			System.IO.FileInfo[] aFiles = oDir.GetFiles() ;

			for ( int i = 0 ; i < aFiles.Length ; i++ )
			{
				System.IO.FileInfo oFile = aFiles[ i ];
				string sExtension = System.IO.Path.GetExtension( oFile.Name ) ;

				if ( Config.Current.CheckIsHiddenFile( oFile.Name ) )
					continue;

				if ( !this.CurrentFolder.ResourceTypeInfo.CheckExtension( sExtension ) )
					continue;

				Decimal iFileSize = Math.Round( (Decimal)oFile.Length / 1024 ) ;
				if ( iFileSize < 1 && oFile.Length != 0 ) iFileSize = 1 ;

				// Create the "File" node.
				XmlNode oFileNode = XmlUtil.AppendElement( oFilesNode, "File" ) ;
				XmlUtil.SetAttribute( oFileNode, "name", oFile.Name );
				XmlUtil.SetAttribute( oFileNode, "date", oFile.LastWriteTime.ToString( "yyyyMMddHHmm" ) );
				if ( Config.Current.Thumbnails.Enabled 
					&& ( Config.Current.Thumbnails.DirectAccess || bShowThumbs )
					&& ImageTools.IsImageExtension( sExtension.TrimStart( '.' ) ) )
				{
					bool bFileExists = false;
					try
					{
						bFileExists = System.IO.File.Exists(System.IO.Path.Combine(this.CurrentFolder.ThumbsServerPath, oFile.Name));
					}
					catch {}

					if ( bFileExists )
						XmlUtil.SetAttribute( oFileNode, "thumb", oFile.Name ) ;
					else if ( bShowThumbs )
						XmlUtil.SetAttribute( oFileNode, "thumb", "?" + oFile.Name ) ;
				}
				XmlUtil.SetAttribute( oFileNode, "size", iFileSize.ToString( CultureInfo.InvariantCulture ) );
			}
		}
	}
}
