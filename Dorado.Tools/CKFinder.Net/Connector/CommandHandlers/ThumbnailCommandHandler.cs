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

namespace CKFinder.Connector.CommandHandlers
{
	internal class ThumbnailCommandHandler : CommandHandlerBase
	{
		public ThumbnailCommandHandler()
			: base()
		{
		}

		public override void SendResponse( System.Web.HttpResponse response )
		{
			this.CheckConnector();

			try
			{
				this.CheckRequest();
			}
			catch ( ConnectorException connectorException )
			{
				response.AddHeader( "X-CKFinder-Error", ( connectorException.Number ).ToString() );
				response.StatusCode = 403;
				response.End();
				return;
			}
			catch
			{
				response.AddHeader( "X-CKFinder-Error", ( (int)Errors.Unknown ).ToString() );
				response.StatusCode = 403;
				response.End();
				return;
			}

			if ( !Config.Current.Thumbnails.Enabled )
			{
				response.AddHeader( "X-CKFinder-Error", ((int)Errors.ThumbnailsDisabled).ToString() );
				response.StatusCode = 403;
				response.End();
				return;
			}

			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FileView ) )
			{
				response.AddHeader( "X-CKFinder-Error", ( (int)Errors.Unauthorized ).ToString() );
				response.StatusCode = 403;
				response.End();
				return;
			}

			bool is304 = false;

			string fileName = HttpContext.Current.Request[ "FileName" ];

			string thumbFilePath = System.IO.Path.Combine( this.CurrentFolder.ThumbsServerPath, fileName );

			if ( !Connector.CheckFileName( fileName ) )
			{
				response.AddHeader( "X-CKFinder-Error", ( (int)Errors.InvalidRequest ).ToString() );
				response.StatusCode = 403;
				response.End();
				return;
			}

			if ( Config.Current.CheckIsHiddenFile( fileName ) )
			{
				response.AddHeader( "X-CKFinder-Error", ( (int)Errors.FileNotFound ).ToString() + " - Hidden folder" );
				response.StatusCode = 404;
				response.End();
				return;
			}

			// If the thumbnail file doesn't exists, create it now.
			if ( !System.IO.File.Exists( thumbFilePath ) )
			{
				string sourceFilePath = System.IO.Path.Combine( this.CurrentFolder.ServerPath, fileName );

				if ( !System.IO.File.Exists( sourceFilePath ) )
				{
					response.AddHeader( "X-CKFinder-Error", ( (int)Errors.FileNotFound ).ToString() );
					response.StatusCode = 404;
					response.End();
					return;
				}

				ImageTools.ResizeImage( sourceFilePath, thumbFilePath, Config.Current.Thumbnails.MaxWidth, Config.Current.Thumbnails.MaxHeight, true, Config.Current.Thumbnails.Quality );
			}

			System.IO.FileInfo thumbfile = new System.IO.FileInfo( thumbFilePath ) ;

			if ( !thumbfile.Exists )
			{
				response.AddHeader("X-CKFinder-Error", ((int)Errors.Unknown).ToString());
				response.StatusCode = 404;
				response.End();
				return;
			}
			string eTag = thumbfile.LastWriteTime.Ticks.ToString("X") + "-" + thumbfile.Length.ToString("X");

			string chachedETag = Request.ServerVariables[ "HTTP_IF_NONE_MATCH" ];
			if ( chachedETag != null && chachedETag.Length > 0 && eTag == chachedETag )
			{
				is304 = true ;
			}

			if ( !is304 )
			{
				string cachedTimeStr = Request.ServerVariables[ "HTTP_IF_MODIFIED_SINCE" ];
				if ( cachedTimeStr != null && cachedTimeStr.Length > 0 )
				{
					try
					{
						DateTime cachedTime = DateTime.Parse( cachedTimeStr );

						if ( cachedTime >= thumbfile.LastWriteTime )
							is304 = true;
					}
					catch
					{
						is304 = false;
					}
				}
			}

			if ( is304 )
			{
				response.StatusCode = 304;
				response.End();
				return;
			}

			string thumbFileExt = System.IO.Path.GetExtension( thumbFilePath ).TrimStart( '.' ).ToLower() ;

			if ( thumbFilePath == ".jpg" )
				response.ContentType = "image/jpeg";
			else
				response.ContentType = "image/" + thumbFileExt;

			response.Cache.SetETag( eTag );
			response.Cache.SetLastModified( thumbfile.LastWriteTime );
			response.Cache.SetCacheability( HttpCacheability.Private );

			response.WriteFile( thumbFilePath );
		}
	}
}