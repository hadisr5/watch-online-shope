
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/24/2019 15:54:23
-- Generated from EDMX file: D:\Projects\desktop\Royan\royan\Royan\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Royan];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AdminUser]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AdminUser];
GO
IF OBJECT_ID(N'[dbo].[ContactInformation]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContactInformation];
GO
IF OBJECT_ID(N'[dbo].[Menus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Menus];
GO
IF OBJECT_ID(N'[dbo].[Sliders]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sliders];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AdminUsers'
CREATE TABLE [dbo].[AdminUsers] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] nvarchar(500)  NULL,
    [password] nvarchar(500)  NULL,
    [token] nvarchar(500)  NULL
);
GO

-- Creating table 'ContactInformations'
CREATE TABLE [dbo].[ContactInformations] (
    [id] int IDENTITY(1,1) NOT NULL,
    [phone] nvarchar(500)  NULL,
    [email] nvarchar(500)  NULL,
    [address] nvarchar(500)  NULL,
    [facebook] nvarchar(max)  NULL,
    [twitter] nvarchar(max)  NULL,
    [instagram] nvarchar(max)  NULL,
    [pinterest] nvarchar(max)  NULL,
    [skype] nvarchar(max)  NULL,
    [lat] nvarchar(max)  NULL,
    [lon] nvarchar(max)  NULL
);
GO

-- Creating table 'Menus'
CREATE TABLE [dbo].[Menus] (
    [id] int IDENTITY(1,1) NOT NULL,
    [title] nvarchar(500)  NULL,
    [link] nvarchar(max)  NULL,
    [priority] int  NULL,
    [Parent] int  NULL
);
GO

-- Creating table 'Sliders'
CREATE TABLE [dbo].[Sliders] (
    [id] int IDENTITY(1,1) NOT NULL,
    [title] nvarchar(500)  NULL,
    [subTitle] nvarchar(500)  NULL,
    [img] nvarchar(max)  NULL,
    [btnText] nvarchar(100)  NULL,
    [btnLink] nvarchar(max)  NULL,
    [active] bit  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'AdminUsers'
ALTER TABLE [dbo].[AdminUsers]
ADD CONSTRAINT [PK_AdminUsers]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'ContactInformations'
ALTER TABLE [dbo].[ContactInformations]
ADD CONSTRAINT [PK_ContactInformations]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Menus'
ALTER TABLE [dbo].[Menus]
ADD CONSTRAINT [PK_Menus]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'Sliders'
ALTER TABLE [dbo].[Sliders]
ADD CONSTRAINT [PK_Sliders]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------