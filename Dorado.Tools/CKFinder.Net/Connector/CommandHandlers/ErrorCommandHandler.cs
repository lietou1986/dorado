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

namespace CKFinder.Connector.CommandHandlers
{
	internal class ErrorCommandHandler : XmlCommandHandlerBase
	{
		private int _ErrorNumber;

		public ErrorCommandHandler()
			: base()
		{
			_ErrorNumber = Errors.Unknown;
		}

		public ErrorCommandHandler( ConnectorException connectorException )
			: base()
		{
			_ErrorNumber = connectorException.Number;
		}

		protected override bool MustCheckRequest()
		{
			return false;
		}

		protected override bool MustIncludeCurrentFolder()
		{
			return false;
		}

		protected override void BuildXml()
		{
			ConnectorException.Throw( _ErrorNumber );
		}
	}
}
