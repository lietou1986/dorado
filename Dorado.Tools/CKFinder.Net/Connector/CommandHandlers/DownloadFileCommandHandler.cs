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
	internal class DownloadFileCommandHandler : CommandHandlerBase
	{
		public DownloadFileCommandHandler()
			: base()
		{
		}

		public override void SendResponse( HttpResponse response )
		{
			this.CheckConnector();

			response.ClearHeaders();
			response.Clear();

			response.ContentType = "application/octet-stream";

			try
			{
				this.CheckRequest();
			}
			catch ( ConnectorException connectorException )
			{
				response.AddHeader( "X-CKFinder-Error", ( connectorException.Number ).ToString() );

				if ( connectorException.Number == Errors.FolderNotFound )
					response.StatusCode = 404;
				else
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

			if ( !this.CurrentFolder.CheckAcl( AccessControlRules.FileView ) )
			{
				response.StatusCode = 403;
				response.End();
				return;
			}

			string fileName = HttpContext.Current.Request[ "FileName" ];
			string filePath = System.IO.Path.Combine( this.CurrentFolder.ServerPath, fileName );

			if ( !this.CurrentFolder.ResourceTypeInfo.CheckExtension( System.IO.Path.GetExtension( fileName ) ) )
			{
				response.StatusCode = 403;
				response.End();
				return;
			}

			if ( !Connector.CheckFileName( fileName ) )
			{
				response.StatusCode = 403;
				response.End();
				return;
			}

			if ( !System.IO.File.Exists( filePath ) || Config.Current.CheckIsHiddenFile( fileName ) )
			{
				response.AddHeader( "Content-Disposition", "attachment; filename=\"File not found - Please refresh and try again\"" );
				response.StatusCode = 404;
				response.End();
				return;
			}

			if ( Request["format"] == "text" )
			{
				response.AddHeader( "Content-Type", "text/plain; charset=utf-8" );
			}
			else
			{
				fileName = fileName.Replace("\"", "\\\"");
				if (Request.Browser.Browser == "IE")
					fileName = HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8).Replace("+"," ");
				response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
			}

			// Buffer to read 10K bytes in chunk:
			byte[] aBuffer = new Byte[10000];
			System.IO.Stream oStream = new System.IO.FileStream( filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read );

			// Total bytes to read:
			long iDataToRead = oStream.Length;

			// Read the bytes.
			while ( iDataToRead > 0 )
			{
				// Verify that the client is connected.
				if ( response.IsClientConnected )
				{
					// Read the data in buffer.
					int iLength = oStream.Read( aBuffer, 0, 10000 );

					// Write the data to the current output stream.
					response.OutputStream.Write( aBuffer, 0, iLength );

					// Flush the data to the HTML output.
					response.Flush();

					aBuffer = new Byte[10000];
					iDataToRead = iDataToRead - iLength;
				}
				else
				{
					//prevent infinite loop if user disconnects
					iDataToRead = -1;
				}
			}

			oStream.Close();
			response.End();
		}
	}
}
