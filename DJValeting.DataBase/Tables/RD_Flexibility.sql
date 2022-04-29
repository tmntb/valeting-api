USE [DJValeting]
GO

/****** Object:  Table [dbo].[RD_Flexibility]    Script Date: 21/04/2022 19:28:37 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RD_Flexibility]') AND type in (N'U'))
DROP TABLE [dbo].[RD_Flexibility]
GO

/****** Object:  Table [dbo].[RD_Flexibility]    Script Date: 21/04/2022 19:28:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[RD_Flexibility](
	[ID] [uniqueidentifier] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_RD_Flexibility] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


