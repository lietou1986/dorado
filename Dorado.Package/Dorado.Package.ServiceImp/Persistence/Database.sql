USE [Message]
GO
/****** Object:  Table [dbo].[Package]    Script Date: 02/25/2011 10:09:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Package](
	[PackageNo] [uniqueidentifier] NOT NULL,
	[ProductType] [int] NULL,
	[PackageType] [nvarchar](50) NULL,
	[PackageStruct] [xml] NULL,
	[PackageNotice] [xml] NULL,
	[PackageStatus] [int] NULL,
	[PackagePriority] [int] NULL,
	[PackageAddress] [nvarchar](255) NULL,
	[PackageAddTime] [datetime] NULL,
	[PackageCompletedTime] [datetime] NULL,
 CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED 
(
	[PackageNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Package] ON [dbo].[Package] 
(
	[PackageStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePackageStatus]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[UpdatePackageStatus]
(
	@PackageNo uniqueidentifier ,
	@PackageStatus int
)
AS
BEGIN

	SET NOCOUNT OFF;

    UPDATE dbo.Package SET PackageStatus=@PackageStatus where PackageNo=@PackageNo

END
GO
/****** Object:  StoredProcedure [dbo].[UpdatePackageDataByNo]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[UpdatePackageDataByNo]
(
	@PackageNo uniqueidentifier,
	@PackageAddress nvarchar(255)
)
AS
BEGIN
DECLARE @COUNT int;

	SET NOCOUNT OFF;

	UPDATE dbo.Package SET 
	[PackageAddress] = @PackageAddress ,
	--[PackageStatus] = 2, 
	[PackageCompletedTime]= GETDATE()  WHERE [PackageNo]=@PackageNo
    

END
GO
/****** Object:  StoredProcedure [dbo].[GetPackageTask]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		flystudio
-- Create date: 2010/2/23
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetPackageTask]
(
	@PackagePriority int
)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
       PackageNo,PackageNotice,PackagePriority,PackageStruct,ProductType,PackageType
  FROM [dbo].[Package] where [PackagePriority]=@PackagePriority and [PackageStatus] = 0 
  order by [PackageAddTime]
END
GO
/****** Object:  StoredProcedure [dbo].[GetPackageStatusByNo]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		flystudio
-- Create date: 2010/2/23
-- Description:	
-- =============================================
create PROCEDURE [dbo].[GetPackageStatusByNo]
(
	@PackageNo uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
		PackageStatus
  FROM  dbo.Package where PackageNo=@PackageNo
END
GO
/****** Object:  StoredProcedure [dbo].[GetPackageDataByNo]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		flystudio
-- Create date: 2010/2/23
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetPackageDataByNo]
(
	@PackageNo uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
        PackageNo,PackageNotice,PackagePriority,PackageStruct,ProductType,PackageType
  FROM [dbo].[Package] where [PackageNo]=@PackageNo
END
GO
/****** Object:  StoredProcedure [dbo].[GetPackageAddressByNo]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		flystudio
-- Create date: 2010/2/23
-- Description:	
-- =============================================
create PROCEDURE [dbo].[GetPackageAddressByNo]
(
	@PackageNo uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT ON;
	SELECT
       PackageAddress
  FROM [dbo].[Package] where [PackageNo]=@PackageNo
END
GO
/****** Object:  StoredProcedure [dbo].[DeletePackageByNo]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[DeletePackageByNo]
(
		@PackageNo uniqueidentifier
)
AS
BEGIN
	SET NOCOUNT OFF;

	DELETE FROM [dbo].[Package]
	where [PackageNo]=@PackageNo
 
END
GO
/****** Object:  StoredProcedure [dbo].[AddPackage]    Script Date: 02/25/2011 10:09:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[AddPackage]
(

	@PackageNo uniqueidentifier,
	@ProductType int,
	@PackageType nvarchar(50),
	@PackageStruct xml,
	@PackageNotice xml,
	@PackageStatus int,
	@PackagePriority int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO [Package](PackageNo,ProductType,PackageType,PackageStruct,PackageNotice,PackageStatus,PackagePriority) 
	VALUES(@PackageNo,@ProductType,@PackageType,@PackageStruct,@PackageNotice,@PackageStatus,@PackagePriority);

if @@Error <> 0
		return -1;
	else
		return 1;

END
GO
/****** Object:  Default [DF_Package_PackageAddTime]    Script Date: 02/25/2011 10:09:46 ******/
ALTER TABLE [dbo].[Package] ADD  CONSTRAINT [DF_Package_PackageAddTime]  DEFAULT (getdate()) FOR [PackageAddTime]
GO
