USE [DJValeting]
GO

ALTER TABLE [dbo].[Booking] DROP CONSTRAINT [FK_Booking_VehicleSize]
GO

ALTER TABLE [dbo].[Booking] DROP CONSTRAINT [FK_Booking_Flexibility]
GO

/****** Object:  Table [dbo].[Booking]    Script Date: 21/04/2022 19:30:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Booking]') AND type in (N'U'))
DROP TABLE [dbo].[Booking]
GO

/****** Object:  Table [dbo].[Booking]    Script Date: 21/04/2022 19:30:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Booking](
	[ID] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[BookingDate] [datetime] NOT NULL,
	[Flexibility_ID] [uniqueidentifier] NOT NULL,
	[VehicleSize_ID] [uniqueidentifier] NOT NULL,
	[ContactNumber] [int] NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Approved] [bit]
 CONSTRAINT [PK_Booking] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_Flexibility] FOREIGN KEY([Flexibility_ID])
REFERENCES [dbo].[RD_Flexibility] ([ID])
GO

ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_Flexibility]
GO

ALTER TABLE [dbo].[Booking]  WITH CHECK ADD  CONSTRAINT [FK_Booking_VehicleSize] FOREIGN KEY([VehicleSize_ID])
REFERENCES [dbo].[RD_VehicleSize] ([ID])
GO

ALTER TABLE [dbo].[Booking] CHECK CONSTRAINT [FK_Booking_VehicleSize]
GO


