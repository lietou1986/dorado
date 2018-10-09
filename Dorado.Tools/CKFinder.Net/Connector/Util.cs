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
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Text;
using System.Security.Cryptography;

namespace CKFinder.Connector
{
	public sealed class Util
	{
		// The "_wmkdir" function is used by the "CreateDirectory" method.
		[DllImport( "msvcrt.dll", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true )]
		private static extern int _mkdir( string path );

		private Util()
		{ }

		/// <summary>
		/// This method should provide safe substitude for Directory.CreateDirectory().
		/// </summary>
		/// <param name="path">The directory path to be created.</param>
		/// <returns>A <see cref="System.IO.DirectoryInfo"/> object for the created directory.</returns>
		/// <remarks>
		///		<para>
		///		This method creates all the directory structure if needed.
		///		</para>
		///		<para>
		///		The System.IO.Directory.CreateDirectory() method has a bug that gives an
		///		error when trying to create a directory and the user has no rights defined
		///		in one of its parent directories. The CreateDirectory() should be a good 
		///		replacement to solve this problem.
		///		</para>
		/// </remarks>
		public static DirectoryInfo CreateDirectory( string path )
		{
			// Create the directory info object for that dir (normalized to its absolute representation).
			DirectoryInfo oDir = new DirectoryInfo( Path.GetFullPath( path ) );

			try
			{
				// Try to create the directory by using standard .Net features. (#415)
				if ( !oDir.Exists )
					oDir.Create();

				return oDir;
			}
			catch
			{
				CreateDirectoryUsingDll( oDir );

				return new DirectoryInfo( path );
			}
		}

		private static void CreateDirectoryUsingDll( DirectoryInfo dir )
		{
			// On some occasion, the DirectoryInfo.Create() function will 
			// throw an error due to a bug in the .Net Framework design. For
			// example, it may happen that the user has no permissions to
			// list entries in a lower level in the directory path, and the
			// Create() call will simply fail.
			// To workaround it, we use mkdir directly.

			ArrayList oDirsToCreate = new ArrayList();

			// Check the entire path structure to find directories that must be created.
			while ( dir != null && !dir.Exists )
			{
				oDirsToCreate.Add( dir.FullName );
				dir = dir.Parent;
			}

			// "dir == null" means that the check arrives in the root and it doesn't exist too.
			if ( dir == null )
				throw ( new System.IO.DirectoryNotFoundException( "Directory \"" + oDirsToCreate[ oDirsToCreate.Count - 1 ] + "\" not found." ) );

			// Create all directories that must be created (from bottom to top).
			for ( int i = oDirsToCreate.Count - 1 ; i >= 0 ; i-- )
			{
				string sPath = (string)oDirsToCreate[ i ];
				int iReturn = _mkdir( sPath );

				if ( iReturn != 0 )
				{
#if DEBUG
					throw new ApplicationException( "Error calling [msvcrt.dll]:_wmkdir(" + sPath + "), error code: " + iReturn );
#else
					ConnectorException.Throw( Errors.AccessDenied );
#endif
				}
			}
		}

		public static string ReadTextFile( string filePath )
		{
			System.IO.StreamReader _Reader = new StreamReader( filePath );
			string data = _Reader.ReadToEnd();
			_Reader.Close();

			return data;
		}

		public static string GetFileNameWithoutExtension( string fileName )
		{
			int length = fileName.Length - 1, dotPos = fileName.IndexOf( "." );

			if ( dotPos == -1 )
				return fileName;

			return fileName.Substring( 0, dotPos );
		}

		public static string GetExtension( string fileName )
		{
			int length = fileName.Length - 1, dotPos = fileName.IndexOf( "." );

			if ( dotPos == -1 )
				return "";

			return fileName.Substring( dotPos );
		}

		public static string encodeURIComponent( string _string )
		{
			_string = System.Web.HttpUtility.UrlEncode( _string );
			return _string.Replace("+", "%20");
		}

		public static bool ArrayContains( Array array, object value, System.Collections.IComparer comparer )
		{
			foreach ( object item in array )
			{
				if ( comparer.Compare( item, value ) == 0 )
					return true;
			}
			return false;
		}

		///<summary>
		/// Hash an input string and return the hash as
		/// a 40 character hexadecimal string.
		/// </summary>
		public static string GetMACTripleDESHash(string input)
		{
			// Create a new instance of the MACTripleDESCryptoServiceProvider object.
			KeyedHashAlgorithm macTripleDESHasher = MACTripleDES.Create();

			// Convert the input string to a byte array and compute the hash.
			byte[] data = macTripleDESHasher.ComputeHash(Encoding.Default.GetBytes(input));

			// Create a new Stringbuilder to collect the bytes
			// and create a string.
			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for ( int i = 0; i < data.Length; i++ )
			{
				sBuilder.Append( data[i].ToString( "x2" ) );
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}
	}
}
