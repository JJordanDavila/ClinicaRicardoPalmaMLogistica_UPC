USE [master]
GO
/****** Object:  Database [BDRicardoPalma]    Script Date: 11/21/2017 4:07:17 PM ******/
CREATE DATABASE [BDRicardoPalma]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BDRicardoPalma', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\BDRicardoPalma.mdf' , SIZE = 4160KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BDRicardoPalma_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\BDRicardoPalma_log.ldf' , SIZE = 1040KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [BDRicardoPalma] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BDRicardoPalma].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BDRicardoPalma] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET ARITHABORT OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [BDRicardoPalma] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [BDRicardoPalma] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BDRicardoPalma] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BDRicardoPalma] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET  ENABLE_BROKER 
GO
ALTER DATABASE [BDRicardoPalma] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BDRicardoPalma] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BDRicardoPalma] SET  MULTI_USER 
GO
ALTER DATABASE [BDRicardoPalma] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BDRicardoPalma] SET DB_CHAINING OFF 
GO
ALTER DATABASE [BDRicardoPalma] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [BDRicardoPalma] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [BDRicardoPalma]
GO
/****** Object:  StoredProcedure [dbo].[Spl_GetIdLic]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[Spl_GetIdLic]
AS
Begin
	Select (Year(GetDate())*1000000)+
					 (Convert(TinyInt,Right('00'+Convert(Varchar,Month(GetDate())),2))*10000)+
					 (Select IsNull(Max(Convert(Int,Right(LicitacionID,4)))+1,1) From GL_Licitacion) Id;
End



GO
/****** Object:  StoredProcedure [dbo].[Spl_GetIdTra]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[Spl_GetIdTra]
AS
Begin
	Select (Year(GetDate())*1000000)+
					 (Convert(TinyInt,Right('00'+Convert(Varchar,Month(GetDate())),2))*10000)+
					 (Select IsNull(Max(Convert(Int,Right([TransaccionCompraID],4)))+1,1) From GL_TransaccionCompra) Id;
End


GO
/****** Object:  StoredProcedure [dbo].[Spl_GetNumero]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[Spl_GetNumero]
@Doc Char(2)
AS
Begin
	Declare @Num Int;
	Declare @Txt Varchar(10)
	--Declare @Doc Char(2)
	--Set @Doc='OC'
	If(@Doc='OC')
		Begin
			Set @Num = (Select IsNull(Max(Convert(Int,SubString(Tc.Numero,6,5))),0)+1 From GL_OrdenCompra Oc Inner Join GL_TransaccionCompra Tc On Tc.TransaccionCompraID=Oc.OrdenCompraID Where IsNumeric(SubString(Tc.Numero,6,5))=1)
			Set @Txt = Convert(Varchar,Year(GetDate())) + '-' + Right('00000' + Convert(Varchar,@Num),5)
		End
	If(@Doc='LI')
		Begin
			Set @Num = (Select IsNull(Max(Convert(Int,SubString(Numero,7,4))),0)+1 From GL_Licitacion Oc  Where IsNumeric(SubString(Numero,7,4))=1)
			Set @Txt = Convert(Varchar,Year(GetDate())) + Right('00'+Convert(Varchar,Month(GetDate())),2) + Right('0000' + Convert(Varchar,@Num),4)
			--YYYYMM####
		End
	Select @Txt Id
End



GO
/****** Object:  StoredProcedure [dbo].[sql2pdf]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- DROP PROCEDURE sql2pdf
CREATE PROCEDURE [dbo].[sql2pdf]
   @filename VARCHAR(100) 
AS 
  CREATE TABLE #pdf (idnumber INT IDENTITY(1,1)
  		    ,code NVARCHAR(200))
  CREATE TABLE #xref (idnumber INT IDENTITY(1,1)
  		    ,code VARCHAR(30))
  CREATE TABLE #text (idnumber INT IDENTITY(1,1)
  		    ,code VARCHAR(200))

  DECLARE @end VARCHAR(7),
  	@beg   VARCHAR(7),
  	@a1    VARCHAR(3),
  	@a2    VARCHAR(3),
  	@ad    VARCHAR(5),
  	@cr    VARCHAR(8),
  	@pr    VARCHAR(9),
  	@ti    VARCHAR(6),
  	@xstr  VARCHAR(10),
  	@page  VARCHAR(8000),
	@pdf   VARCHAR(100),
	@trenutniRed NVARCHAR(200),
  	@rows   INT,
  	@ofset  INT,
  	@len    INT,
  	@nopg   INT,
        @fs 	INT,
	@ole    INT,
	@x 	INT,
	@file   INT,
  	@object INT
  SELECT @pdf = 'C:\pdf\' + @filename + '.pdf'  
  SET @page = ''
  SET @nopg = 0
  SET @object = 6
  SET @end = 'endobj'
  SET @beg = ' 0 obj'
  SET @a1 = '<<'
  SET @a2 = '>>'
  SET @ad = ' 0 R'
  SET @cr = CHAR(67) + CHAR(114) + CHAR (101) + CHAR(97) + CHAR(116) + CHAR (111) + CHAR(114)
  SET @pr = CHAR(80) + CHAR(114) + CHAR (111) + CHAR(100) + CHAR(117) + CHAR (99 ) + CHAR(101) + CHAR(114)
  SET @ti = CHAR(84) + CHAR(105) + CHAR (116) + CHAR(108) + CHAR(101)
  SET @xstr = ' 00000 n'
  SET @ofset = 396  
  INSERT INTO #xref(code) VALUES ('xref')
  INSERT INTO #xref(code) VALUES ('0 10')
  INSERT INTO #xref(code) VALUES ('0000000000 65535 f')
  INSERT INTO #xref(code) VALUES ('0000000017' + @xstr)
  INSERT INTO #xref(code) VALUES ('0000000790' + @xstr)
  INSERT INTO #xref(code) VALUES ('0000000869' + @xstr)
  INSERT INTO #xref(code) VALUES ('0000000144' + @xstr)
  INSERT INTO #xref(code) VALUES ('0000000247' + @xstr)
  INSERT INTO #xref(code) VALUES ('0000000321' + @xstr)
  INSERT INTO #xref(code) VALUES ('0000000396' + @xstr)  
  INSERT INTO #pdf (code) VALUES ('%' + CHAR(80) + CHAR(68) + CHAR (70) + '-1.2')
  INSERT INTO #pdf (code) VALUES ('%????')
  INSERT INTO #pdf (code) VALUES ('1' + @beg)
  INSERT INTO #pdf (code) VALUES (@a1)
  INSERT INTO #pdf (code) VALUES ('/' + @cr + ' (Ivica Masar ' + CHAR(80) + CHAR(83) + CHAR (79) + CHAR(80) + CHAR(68) + CHAR (70) + ')')
  INSERT INTO #pdf (code) VALUES ('/' + @pr + ' (stored procedure for ms sql  pso@vip.hr)')
  INSERT INTO #pdf (code) VALUES ('/' + @ti + ' (SQL2' + CHAR(80) + CHAR(68) + CHAR (70) + ')')
  INSERT INTO #pdf (code) VALUES (@a2)
  INSERT INTO #pdf (code) VALUES (@end)
  INSERT INTO #pdf (code) VALUES ('4' + @beg)
  INSERT INTO #pdf (code) VALUES (@a1)
  INSERT INTO #pdf (code) VALUES ('/Type /Font')
  INSERT INTO #pdf (code) VALUES ('/Subtype /Type1')
  INSERT INTO #pdf (code) VALUES ('/Name /F1')
  INSERT INTO #pdf (code) VALUES ('/Encoding 5' + @ad)
  INSERT INTO #pdf (code) VALUES ('/BaseFont /Courier')
  INSERT INTO #pdf (code) VALUES (@a2)
  INSERT INTO #pdf (code) VALUES (@end)
  INSERT INTO #pdf (code) VALUES ('5' + @beg)
  INSERT INTO #pdf (code) VALUES (@a1)
  INSERT INTO #pdf (code) VALUES ('/Type /Encoding')
  INSERT INTO #pdf (code) VALUES ('/BaseEncoding /WinAnsiEncoding')
  INSERT INTO #pdf (code) VALUES (@a2)
  INSERT INTO #pdf (code) VALUES (@end)
  INSERT INTO #pdf (code) VALUES ('6' + @beg)
  INSERT INTO #pdf (code) VALUES (@a1)
  INSERT INTO #pdf (code) VALUES ('  /Font ' + @a1 + ' /F1 4' + @ad + ' ' + @a2 + '  /ProcSet [ /' + CHAR(80) + CHAR(68) + CHAR (70) + ' /Text ]')
  INSERT INTO #pdf (code) VALUES (@a2)
  INSERT INTO #pdf (code) VALUES (@end)
  INSERT INTO #text(code) (SELECT code FROM psopdf)
  SELECT @x = COUNT(*) FROM #text
  SELECT @x = (@x / 60) + 1
  WHILE  @nopg < @x
    BEGIN
      DECLARE SysKursor  INSENSITIVE SCROLL CURSOR 
      FOR SELECT SUBSTRING((code + SPACE(81)), 1, 80) FROM #text WHERE idnumber BETWEEN ((@nopg * 60) + 1) AND ((@nopg + 1) * 60 )
      FOR READ ONLY    
      OPEN SysKursor
      FETCH NEXT FROM SysKursor INTO @trenutniRed
      SELECT @object = @object + 1
      SELECT @page = @page +  ' ' + CAST(@object AS VARCHAR) + @ad
      SELECT @len = LEN(@object) + LEN(@object + 1)
      INSERT INTO #pdf (code) VALUES (CAST(@object AS VARCHAR)  + @beg)
      INSERT INTO #pdf (code) VALUES (@a1)
      INSERT INTO #pdf (code) VALUES ('/Type /Page')
      INSERT INTO #pdf (code) VALUES ('/Parent 3' + @ad)
      INSERT INTO #pdf (code) VALUES ('/Resources 6' + @ad)
      SELECT @object = @object + 1
      INSERT INTO #pdf (code) VALUES ('/Contents ' + CAST(@object AS VARCHAR) + @ad)
      INSERT INTO #pdf (code) VALUES (@a2)
      INSERT INTO #pdf (code) VALUES (@end)
      SELECT @ofset = @len + 86 + @ofset
      INSERT INTO #xref(code) (SELECT SUBSTRING('0000000000' + CAST(@ofset AS VARCHAR), 
    	LEN('0000000000' + CAST(@ofset AS VARCHAR)) - 9, 
    	LEN('0000000000' + CAST(@ofset AS VARCHAR))) + @xstr)  
      INSERT INTO #pdf (code) VALUES (CAST(@object AS VARCHAR)  + @beg)
      INSERT INTO #pdf (code) VALUES (@a1)
      SELECT @object = @object + 1
      INSERT INTO #pdf (code) VALUES ('/Length ' + CAST(@object AS VARCHAR) + @ad)
      INSERT INTO #pdf (code) VALUES (@a2)
      INSERT INTO #pdf (code) VALUES ('stream')
      INSERT INTO #pdf (code) VALUES ('BT')
      INSERT INTO #pdf (code) VALUES ('/F1 10 Tf')
      INSERT INTO #pdf (code) VALUES ('1 0 0 1 50 802 Tm')
      INSERT INTO #pdf (code) VALUES ('12 TL')
      WHILE @@Fetch_Status = 0
         BEGIN
             INSERT INTO #pdf (code) VALUES ('T* (' + @trenutniRed + ') Tj')
             FETCH NEXT FROM  SysKursor INTO @trenutniRed
          END
      INSERT INTO #pdf (code) VALUES ('ET')
      INSERT INTO #pdf (code) VALUES ('endstream')
      INSERT INTO #pdf (code) VALUES (@end)
      SELECT @rows = (SELECT COUNT(*) FROM #text WHERE idnumber BETWEEN ((@nopg * 60) + 1) AND ((@nopg + 1) * 60 ))* 90 + 45
      SELECT @nopg = @nopg + 1    
      SELECT @len = LEN(@object) + LEN(@object - 1)
      SELECT @ofset = @len + 57 + @ofset + @rows
      INSERT INTO #xref(code) (SELECT SUBSTRING('0000000000' + CAST(@ofset AS VARCHAR), 
     	LEN('0000000000' + CAST(@ofset AS VARCHAR)) - 9, 
   	LEN('0000000000' + CAST(@ofset AS VARCHAR))) + @xstr)   
      INSERT INTO #pdf (code) VALUES (CAST(@object AS VARCHAR)  + @beg)
      INSERT INTO #pdf (code) VALUES (@rows)
      INSERT INTO #pdf (code) VALUES (@end)
      SELECT @len = LEN(@object) + LEN(@rows)
      SELECT @ofset = @len + 18 + @ofset
      INSERT INTO #xref(code) (SELECT SUBSTRING('0000000000' + CAST(@ofset AS VARCHAR), 
    	LEN('0000000000' + CAST(@ofset AS VARCHAR)) - 9, 
    	LEN('0000000000' + CAST(@ofset AS VARCHAR))) + @xstr)  
      CLOSE SysKursor
      DEALLOCATE SysKursor
    END
    INSERT INTO #pdf (code) VALUES ('2' + @beg)
    INSERT INTO #pdf (code) VALUES (@a1)
    INSERT INTO #pdf (code) VALUES ('/Type /Catalog')
    INSERT INTO #pdf (code) VALUES ('/Pages 3' + @ad)
    INSERT INTO #pdf (code) VALUES ('/PageLayout /OneColumn')
    INSERT INTO #pdf (code) VALUES (@a2)
    INSERT INTO #pdf (code) VALUES (@end)
    UPDATE #xref SET code = (SELECT code FROM #xref WHERE idnumber = (SELECT MAX(idnumber) FROM #xref)) WHERE idnumber = 5
    DELETE FROM #xref WHERE idnumber = (SELECT MAX(idnumber) FROM #xref)
    INSERT INTO #pdf (code) VALUES ('3' + @beg)
    INSERT INTO #pdf (code) VALUES (@a1)
    INSERT INTO #pdf (code) VALUES ('/Type /Pages')
    INSERT INTO #pdf (code) VALUES ('/Count ' + CAST(@nopg AS VARCHAR))
    INSERT INTO #pdf (code) VALUES ('/MediaBox [ 0 0 595 842 ]')
    INSERT INTO #pdf (code) VALUES ('/Kids [' + @page + ' ]')
    INSERT INTO #pdf (code) VALUES (@a2)
    INSERT INTO #pdf (code) VALUES (@end)
    SELECT @ofset = @ofset + 79
    UPDATE #xref SET code =(SELECT SUBSTRING('0000000000' + CAST(@ofset AS VARCHAR), 
  	LEN('0000000000' + CAST(@ofset AS VARCHAR)) - 9, 
  	LEN('0000000000' + CAST(@ofset AS VARCHAR))) + @xstr) WHERE idnumber = 6
    INSERT INTO #xref(code) VALUES ('trailer')
    INSERT INTO #xref(code) VALUES (@a1)
    SELECT @object = @object + 1
    UPDATE #xref SET code = '0 ' + CAST(@object AS VARCHAR) WHERE idnumber = 2
    INSERT INTO #xref(code) VALUES ('/Size ' + CAST(@object AS VARCHAR))
    INSERT INTO #xref(code) VALUES ('/Root 2' + @ad)
    INSERT INTO #xref(code) VALUES ('/Info 1' + @ad)
    INSERT INTO #xref(code) VALUES (@a2)
    INSERT INTO #xref(code) VALUES ('startxref')
    SELECT @len = LEN(@nopg) + LEN(@page)
    SELECT @ofset = @len + 86 + @ofset
    INSERT INTO #xref(code) VALUES (@ofset)
    INSERT INTO #xref(code) VALUES ('%%' + CHAR(69) + CHAR (79) + CHAR(70))
    INSERT INTO #pdf (code) (SELECT code FROM #xref) 
    --SELECT code FROM #pdf
    SELECT @trenutniRed = 'del '+ @pdf
    EXECUTE @ole = sp_OACreate 'Scripting.FileSystemObject', @fs OUT
    EXEC master..xp_cmdshell @trenutniRed, NO_OUTPUT
    EXECUTE @ole = sp_OAMethod @fs, 'OpenTextFile', @file OUT, @pdf, 8, 1
    DECLARE SysKursor  INSENSITIVE SCROLL CURSOR 
    FOR SELECT code FROM #pdf ORDER BY idnumber
    FOR READ ONLY    
    OPEN SysKursor
    FETCH NEXT FROM SysKursor INTO @trenutniRed
    WHILE @@Fetch_Status = 0
	BEGIN
	  EXECUTE @ole = sp_OAMethod @file, 'WriteLine', Null, @trenutniRed
	  FETCH NEXT FROM  SysKursor INTO @trenutniRed 
        END
    CLOSE SysKursor
    DEALLOCATE SysKursor
    DELETE FROM psopdf
    EXECUTE @ole = sp_OADestroy @file
    EXECUTE @ole = sp_OADestroy @fs

GO
/****** Object:  Table [dbo].[Destino]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Destino](
	[IdDestino] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](100) NULL,
 CONSTRAINT [PK_Destino] PRIMARY KEY CLUSTERED 
(
	[IdDestino] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DetalleTurno]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DetalleTurno](
	[IdDetalleTurno] [int] IDENTITY(1,1) NOT NULL,
	[IdTurno] [int] NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[IdPersonalEmergencia] [int] NULL,
	[IdEstado] [int] NOT NULL,
	[Comentario] [varchar](200) NULL,
 CONSTRAINT [PK_DetalleTurno] PRIMARY KEY CLUSTERED 
(
	[IdDetalleTurno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Estados]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Estados](
	[IdEstado] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Estados] PRIMARY KEY CLUSTERED 
(
	[IdEstado] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Almacen]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Almacen](
	[AlmacenID] [int] IDENTITY(1,1) NOT NULL,
	[Direccion] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AlmacenID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Area]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Area](
	[AreaID] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Articulo]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Articulo](
	[ArticuloID] [int] IDENTITY(1,1) NOT NULL,
	[UnidadMedidaID] [int] NOT NULL,
	[Codigo] [varchar](10) NOT NULL,
	[Descripcion] [varchar](50) NOT NULL,
	[PrecioReferencial] [money] NOT NULL,
	[StockMinimo] [int] NOT NULL,
	[StockMaximo] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ArticuloID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_ConsultaLicitacion]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_ConsultaLicitacion](
	[ProveedorID] [int] NOT NULL,
	[LicitacionID] [int] NOT NULL,
	[ConsultaID] [tinyint] IDENTITY(1,1) NOT NULL,
	[consulta] [varchar](100) NOT NULL,
	[observacion] [varchar](100) NOT NULL,
	[fechaConsulta] [datetime] NOT NULL,
	[revisado] [bit] NOT NULL,
	[respuesta] [varchar](100) NULL,
	[fechaRespuesta] [nchar](10) NULL,
 CONSTRAINT [PK_GL_ConsultaLicitacion] PRIMARY KEY CLUSTERED 
(
	[ProveedorID] ASC,
	[LicitacionID] ASC,
	[ConsultaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Contrato]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Contrato](
	[ContratoID] [int] IDENTITY(1,1) NOT NULL,
	[LicitacionID] [int] NOT NULL,
	[MonedaID] [int] NOT NULL,
	[FormaPagoID] [int] NOT NULL,
	[Numero] [varchar](10) NOT NULL,
	[FechaInicio] [date] NOT NULL,
	[FechaFin] [date] NOT NULL,
	[Monto] [money] NOT NULL,
	[Penalidades] [varbinary](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ContratoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Convocatoria]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Convocatoria](
	[ConvocatoriaId] [int] IDENTITY(1,1) NOT NULL,
	[Numero] [varchar](12) NULL,
	[FechaInicio] [datetime] NULL,
	[FechaFin] [datetime] NULL,
	[Requisito] [image] NULL,
	[Estado] [char](2) NULL,
	[RubroID] [int] NOT NULL,
	[EmpleadoID] [int] NOT NULL,
	[ObservacionSuspension] [nvarchar](max) NULL,
	[FechaSuspension] [datetime] NULL,
 CONSTRAINT [PK_GL_Convocatoria] PRIMARY KEY CLUSTERED 
(
	[ConvocatoriaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Cotizacion]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Cotizacion](
	[CotizacionID] [int] NOT NULL,
	[RequerimientoCompraID] [int] NOT NULL,
	[ProveedorID] [int] NOT NULL,
	[FormaPagoID] [int] NOT NULL,
	[PlazoEntrega] [int] NOT NULL,
	[TiempoValidez] [int] NOT NULL,
	[NroReferencia] [varchar](10) NOT NULL,
 CONSTRAINT [PK__GL_Cotiz__30443A59136E90E1] PRIMARY KEY CLUSTERED 
(
	[CotizacionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_DetalleConvocatoria]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GL_DetalleConvocatoria](
	[ConvocatoriaId] [int] NOT NULL,
	[PostulanteId] [int] NOT NULL,
	[Fecha_Registro] [date] NOT NULL,
 CONSTRAINT [PK_GL_DetalleConvocatoria] PRIMARY KEY CLUSTERED 
(
	[ConvocatoriaId] ASC,
	[PostulanteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GL_DetallePostulante]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_DetallePostulante](
	[DetalleId] [int] NOT NULL,
	[PostulanteId] [int] NOT NULL,
	[NombreArchivo] [varchar](100) NULL,
	[Archivo] [image] NULL,
 CONSTRAINT [PK_GL_DetallePostulante] PRIMARY KEY CLUSTERED 
(
	[DetalleId] ASC,
	[PostulanteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_DetalleTransaccionCompra]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GL_DetalleTransaccionCompra](
	[TransaccionCompraID] [int] NOT NULL,
	[DetalleTransaccionCompraID] [int] NOT NULL,
	[ArticuloID] [int] NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Precio] [money] NOT NULL,
 CONSTRAINT [PK_GL_DetalleTransaccionCompra] PRIMARY KEY CLUSTERED 
(
	[DetalleTransaccionCompraID] ASC,
	[TransaccionCompraID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GL_DetSolicitudActProveedor]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_DetSolicitudActProveedor](
	[SolicitudActProveedorID] [int] NOT NULL,
	[ProveedorID] [int] NOT NULL,
	[Adjunto] [varbinary](max) NOT NULL,
	[Resultado] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SolicitudActProveedorID] ASC,
	[ProveedorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Empleado]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Empleado](
	[EmpleadoID] [int] IDENTITY(1,1) NOT NULL,
	[AreaID] [int] NOT NULL,
	[DNI] [varchar](10) NOT NULL,
	[ApellidoPaterno] [varchar](20) NOT NULL,
	[ApellidoMaterno] [varchar](20) NOT NULL,
	[Nombres] [varchar](20) NOT NULL,
	[Correo] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmpleadoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_FormaPago]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_FormaPago](
	[FormaPagoID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[NroDiasPago] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[FormaPagoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Licitacion]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Licitacion](
	[LicitacionID] [int] IDENTITY(1,1) NOT NULL,
	[RequerimientoCompraID] [int] NOT NULL,
	[Numero] [varchar](10) NOT NULL,
	[Fecha] [date] NOT NULL,
	[TerminoRefInicial] [image] NULL,
	[TerminoRefFinal] [image] NULL,
	[FechaConvocatoria] [date] NULL,
	[FecRecepcionConsultas] [date] NULL,
	[FecAbsolucionConsultas] [date] NULL,
	[FecRecepcionExpediente] [date] NULL,
	[FecEvaluacionIni] [date] NULL,
	[FecEvaluacionFin] [date] NULL,
	[FecAdjudicacion] [date] NULL,
	[ObservacionAnulacion] [varchar](100) NULL,
	[Estado] [char](2) NULL,
	[FileNameTDR1] [nvarchar](500) NULL,
	[ContentTypeTDR1] [nvarchar](500) NULL,
	[FileNameTDR2] [nvarchar](500) NULL,
	[ContentTypeTDR2] [nvarchar](500) NULL,
 CONSTRAINT [PK__GL_Licit__415A04ACC13CDF9E] PRIMARY KEY CLUSTERED 
(
	[LicitacionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Moneda]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Moneda](
	[MonedaID] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](50) NOT NULL,
	[Abreviatura] [varchar](5) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MonedaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_OrdenCompra]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_OrdenCompra](
	[OrdenCompraID] [int] NOT NULL,
	[CotizacionID] [int] NOT NULL,
	[AlmacenID] [int] NOT NULL,
	[Observacion] [varchar](50) NOT NULL,
	[FechaEntrega] [datetime] NOT NULL,
 CONSTRAINT [PK__GL_Orden__0B556E16AE5DA502] PRIMARY KEY CLUSTERED 
(
	[OrdenCompraID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Postulante]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Postulante](
	[PostulanteId] [int] IDENTITY(1,1) NOT NULL,
	[RazonSocial] [varchar](300) NOT NULL,
	[Direccion] [varchar](50) NOT NULL,
	[Correo] [varchar](50) NOT NULL,
	[RUC] [varchar](11) NOT NULL,
	[ConstanciaRNP] [bit] NOT NULL,
	[FechaRegistro] [datetime] NOT NULL,
 CONSTRAINT [PK_GL_Postulante] PRIMARY KEY CLUSTERED 
(
	[PostulanteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Presupuesto]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Presupuesto](
	[PresupuestoID] [int] IDENTITY(1,1) NOT NULL,
	[AreaID] [int] NOT NULL,
	[Periodo] [varchar](10) NOT NULL,
	[Monto] [money] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PresupuestoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_Proveedor]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Proveedor](
	[ProveedorID] [int] IDENTITY(1,1) NOT NULL,
	[RubroID] [int] NOT NULL,
	[NombreComercial] [varchar](300) NOT NULL,
	[RazonSocial] [varchar](300) NOT NULL,
	[Direccion] [varchar](50) NOT NULL,
	[Correo] [varchar](50) NOT NULL,
	[RUC] [varchar](11) NOT NULL,
	[CertificadoISO] [bit] NOT NULL,
	[ConstanciaRNP] [bit] NOT NULL,
	[PostulanteId] [int] NULL,
	[ObservacionesSuspension] [varchar](100) NULL,
	[FechaSuspension] [datetime] NULL,
	[Estado] [char](2) NULL,
 CONSTRAINT [PK__GL_Prove__61266BB907EBCC72] PRIMARY KEY CLUSTERED 
(
	[ProveedorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_RegistroProveedorParticipante]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_RegistroProveedorParticipante](
	[ProveedorID] [int] NOT NULL,
	[LicitacionID] [int] NOT NULL,
	[envioExpediente] [bit] NULL,
	[fechaColExpediente] [datetime] NULL,
	[expediente] [varbinary](max) NULL,
	[adjudicado] [bit] NULL,
	[monto] [money] NULL,
 CONSTRAINT [PK_GL_RegistroProveedorParticipante] PRIMARY KEY CLUSTERED 
(
	[ProveedorID] ASC,
	[LicitacionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_RequerimientoCompra]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GL_RequerimientoCompra](
	[RequerimientoCompraID] [int] NOT NULL,
	[SolicitanteID] [int] NOT NULL,
	[EncargadoID] [int] NOT NULL,
	[FechaEstimada] [datetime] NOT NULL,
 CONSTRAINT [PK_GL_RequerimientoCompra] PRIMARY KEY CLUSTERED 
(
	[RequerimientoCompraID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GL_Rubro]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Rubro](
	[RubroID] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RubroID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_SolicitudActProveedor]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_SolicitudActProveedor](
	[SolicitudActProveedorID] [int] NOT NULL,
	[TipoSolicitudID] [int] NOT NULL,
	[fecha] [date] NOT NULL,
	[Observaciones] [varchar](50) NOT NULL,
	[fechaPublicacion] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SolicitudActProveedorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_TipoSolicitud]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_TipoSolicitud](
	[TipoSolicitudID] [int] NOT NULL,
	[Descripcion] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TipoSolicitudID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_TransaccionCompra]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_TransaccionCompra](
	[TransaccionCompraID] [int] NOT NULL,
	[MonedaID] [int] NOT NULL,
	[Numero] [varchar](20) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Estado] [char](2) NOT NULL,
	[ObservacionAnulacion] [varchar](50) NULL,
 CONSTRAINT [PK__GL_Trans__72A28926D7A2870F] PRIMARY KEY CLUSTERED 
(
	[TransaccionCompraID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_UnidadMedida]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_UnidadMedida](
	[UnidadMedidaID] [int] IDENTITY(1,1) NOT NULL,
	[Abreviatura] [varchar](10) NOT NULL,
	[Descripcion] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UnidadMedidaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Insumo]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Insumo](
	[IdInsumo] [int] IDENTITY(1,1) NOT NULL,
	[NombreInsumo] [varchar](100) NOT NULL,
	[Marca] [varchar](50) NULL,
	[Linea] [varchar](50) NULL,
 CONSTRAINT [PK_Insumo] PRIMARY KEY CLUSTERED 
(
	[IdInsumo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Paciente]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Paciente](
	[IdPaciente] [int] IDENTITY(1,1) NOT NULL,
	[IdTipoPaciente] [int] NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[ApellidoPaterno] [varchar](100) NOT NULL,
	[ApellidoMaterno] [varchar](100) NOT NULL,
	[dni] [int] NOT NULL,
 CONSTRAINT [PK_Paciente] PRIMARY KEY CLUSTERED 
(
	[IdPaciente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PersonalEmergencia]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PersonalEmergencia](
	[IdPersonalEmergencia] [int] IDENTITY(1,1) NOT NULL,
	[Nombres] [varchar](50) NOT NULL,
	[ApellidoPaterno] [varchar](100) NOT NULL,
	[ApellidoMaterno] [varchar](100) NOT NULL,
	[DNI] [int] NOT NULL,
	[Rol] [varchar](10) NOT NULL,
 CONSTRAINT [PK_PersonalEmergencia] PRIMARY KEY CLUSTERED 
(
	[IdPersonalEmergencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Prioridad]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Prioridad](
	[IdPrioridad] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](100) NULL,
 CONSTRAINT [PK_Prioridad] PRIMARY KEY CLUSTERED 
(
	[IdPrioridad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Protocolo]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Protocolo](
	[IdProtocolo] [int] IDENTITY(1,1) NOT NULL,
	[IdSintoma] [int] NOT NULL,
	[IdPrioridad] [int] NOT NULL,
	[IdDestino] [int] NOT NULL,
	[Sala] [varchar](20) NOT NULL,
	[Diagnostico] [varchar](200) NOT NULL,
	[CondicionIngreso] [varchar](200) NOT NULL,
	[CondicionEgreso] [varchar](200) NOT NULL,
	[DiasAtencion] [int] NOT NULL,
 CONSTRAINT [PK_Protocolo] PRIMARY KEY CLUSTERED 
(
	[IdProtocolo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[psopdf]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[psopdf](
	[code] [nvarchar](200) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Requerimiento_Insumo]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Requerimiento_Insumo](
	[IdRequerimiento_Insumo] [int] IDENTITY(1,1) NOT NULL,
	[IdRequerimientoInsumo] [int] NOT NULL,
	[IdInsumo] [int] NOT NULL,
	[IdSala] [int] NOT NULL,
	[Sala] [varchar](50) NOT NULL,
	[Cantidad] [int] NOT NULL,
	[Motivo] [varchar](300) NULL,
	[IdEstado] [int] NOT NULL,
	[EsAutorizado] [bit] NOT NULL,
	[FechaAutorizacion] [datetime] NULL,
 CONSTRAINT [PK_Requerimiento_Insumo] PRIMARY KEY CLUSTERED 
(
	[IdRequerimiento_Insumo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RequerimientoInsumo]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequerimientoInsumo](
	[IdRequerimientoInsumo] [int] IDENTITY(1,1) NOT NULL,
	[IdPersonalEmergencia] [int] NOT NULL,
	[FechaSolicitud] [datetime] NOT NULL,
 CONSTRAINT [PK_RequerimientoInsumo] PRIMARY KEY CLUSTERED 
(
	[IdRequerimientoInsumo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RequerimientoTurno]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RequerimientoTurno](
	[IdRequerimientoTurno] [int] IDENTITY(1,1) NOT NULL,
	[IdPersonalEmergencia] [int] NOT NULL,
	[FechaSolicitud] [datetime] NOT NULL,
	[HoraInicio] [varchar](8) NOT NULL,
	[HoraFin] [varchar](8) NOT NULL,
	[Motivo] [varchar](100) NULL,
	[EsAprobado] [bit] NOT NULL,
 CONSTRAINT [PK_RequerimientoTurno] PRIMARY KEY CLUSTERED 
(
	[IdRequerimientoTurno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Sintoma]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Sintoma](
	[IdSintoma] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](200) NULL,
 CONSTRAINT [PK_Sintoma] PRIMARY KEY CLUSTERED 
(
	[IdSintoma] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TicketEmergencia]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TicketEmergencia](
	[IdTicketEmergencia] [int] IDENTITY(1,1) NOT NULL,
	[IdPaciente] [int] NOT NULL,
	[IdDestino] [int] NULL,
	[EsViolencia] [bit] NULL,
	[IdTicketTrauma] [int] NULL,
	[IdTicketSala] [int] NULL,
	[IdTratamiento] [int] NULL,
	[IdPersonalEmergencia] [int] NULL,
	[Ingreso] [datetime] NULL,
	[Egreso] [datetime] NULL,
 CONSTRAINT [PK_TicketEmergencia] PRIMARY KEY CLUSTERED 
(
	[IdTicketEmergencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TicketSalaObservacion]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TicketSalaObservacion](
	[IdTicketSala] [int] IDENTITY(1,1) NOT NULL,
	[Ingreso] [datetime] NULL,
	[Egreso] [datetime] NULL,
	[Diagnostico] [varchar](200) NULL,
	[CondicionIngreso] [varchar](200) NULL,
	[CondicionEgreso] [varchar](200) NULL,
 CONSTRAINT [PK_TicketSalaObservacion] PRIMARY KEY CLUSTERED 
(
	[IdTicketSala] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TicketTraumaShockTopico]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TicketTraumaShockTopico](
	[IdTicketTrauma] [int] IDENTITY(1,1) NOT NULL,
	[Ingreso] [datetime] NULL,
	[Egreso] [datetime] NULL,
	[Diagnostico] [varchar](200) NULL,
	[EsTraumaShock] [bit] NULL,
 CONSTRAINT [PK_TicketTraumaShock] PRIMARY KEY CLUSTERED 
(
	[IdTicketTrauma] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TipoPaciente]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TipoPaciente](
	[IdTipoPaciente] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](200) NULL,
 CONSTRAINT [PK_TipoPaciente] PRIMARY KEY CLUSTERED 
(
	[IdTipoPaciente] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Tratamiento]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tratamiento](
	[IdTratamiento] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](500) NOT NULL,
 CONSTRAINT [PK_Tratamiento] PRIMARY KEY CLUSTERED 
(
	[IdTratamiento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Triaje]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Triaje](
	[IdTriaje] [int] IDENTITY(1,1) NOT NULL,
	[IdPrioridad] [int] NOT NULL,
	[IdTicketEmergencia] [int] NOT NULL,
	[IdSintoma] [int] NOT NULL,
 CONSTRAINT [PK_Triaje] PRIMARY KEY CLUSTERED 
(
	[IdTriaje] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Turno]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Turno](
	[IdTurno] [int] IDENTITY(1,1) NOT NULL,
	[NombreTurno] [varchar](20) NOT NULL,
	[Rango1] [varchar](8) NOT NULL,
	[Rango2] [varchar](8) NOT NULL,
 CONSTRAINT [PK_Turno] PRIMARY KEY CLUSTERED 
(
	[IdTurno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 11/21/2017 4:07:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Usuario](
	[IdUsuario] [int] IDENTITY(1,1) NOT NULL,
	[CodigoUsuario] [varchar](20) NOT NULL,
	[Nombres] [varchar](100) NOT NULL,
	[Clave] [varchar](50) NOT NULL,
	[EsAutorizador] [bit] NOT NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[GL_ConsultaLicitacion] ADD  CONSTRAINT [DF_GL_ConsultaLicitacion_revisado]  DEFAULT ((0)) FOR [revisado]
GO
ALTER TABLE [dbo].[GL_Licitacion] ADD  CONSTRAINT [DF_GL_Licitacion_Fecha]  DEFAULT (getdate()) FOR [Fecha]
GO
ALTER TABLE [dbo].[GL_Proveedor] ADD  CONSTRAINT [DF_GL_Proveedor_ConstanciaRNP]  DEFAULT ((0)) FOR [ConstanciaRNP]
GO
ALTER TABLE [dbo].[DetalleTurno]  WITH CHECK ADD  CONSTRAINT [FK_DetalleTurno_PersonalEmergencia] FOREIGN KEY([IdPersonalEmergencia])
REFERENCES [dbo].[PersonalEmergencia] ([IdPersonalEmergencia])
GO
ALTER TABLE [dbo].[DetalleTurno] CHECK CONSTRAINT [FK_DetalleTurno_PersonalEmergencia]
GO
ALTER TABLE [dbo].[DetalleTurno]  WITH CHECK ADD  CONSTRAINT [FK_DetalleTurno_Turno1] FOREIGN KEY([IdTurno])
REFERENCES [dbo].[Turno] ([IdTurno])
GO
ALTER TABLE [dbo].[DetalleTurno] CHECK CONSTRAINT [FK_DetalleTurno_Turno1]
GO
ALTER TABLE [dbo].[GL_Articulo]  WITH CHECK ADD  CONSTRAINT [FK_UnidadMedArticulo] FOREIGN KEY([UnidadMedidaID])
REFERENCES [dbo].[GL_UnidadMedida] ([UnidadMedidaID])
GO
ALTER TABLE [dbo].[GL_Articulo] CHECK CONSTRAINT [FK_UnidadMedArticulo]
GO
ALTER TABLE [dbo].[GL_ConsultaLicitacion]  WITH CHECK ADD  CONSTRAINT [FK_GL_ConsultaLicitacion_GL_RegistroProveedorParticipante] FOREIGN KEY([ProveedorID], [LicitacionID])
REFERENCES [dbo].[GL_RegistroProveedorParticipante] ([ProveedorID], [LicitacionID])
GO
ALTER TABLE [dbo].[GL_ConsultaLicitacion] CHECK CONSTRAINT [FK_GL_ConsultaLicitacion_GL_RegistroProveedorParticipante]
GO
ALTER TABLE [dbo].[GL_Contrato]  WITH CHECK ADD  CONSTRAINT [FK_FormaPagoContrato] FOREIGN KEY([FormaPagoID])
REFERENCES [dbo].[GL_FormaPago] ([FormaPagoID])
GO
ALTER TABLE [dbo].[GL_Contrato] CHECK CONSTRAINT [FK_FormaPagoContrato]
GO
ALTER TABLE [dbo].[GL_Contrato]  WITH CHECK ADD  CONSTRAINT [FK_LicitacionContrato] FOREIGN KEY([LicitacionID])
REFERENCES [dbo].[GL_Licitacion] ([LicitacionID])
GO
ALTER TABLE [dbo].[GL_Contrato] CHECK CONSTRAINT [FK_LicitacionContrato]
GO
ALTER TABLE [dbo].[GL_Contrato]  WITH CHECK ADD  CONSTRAINT [FK_MonedaContrato] FOREIGN KEY([MonedaID])
REFERENCES [dbo].[GL_Moneda] ([MonedaID])
GO
ALTER TABLE [dbo].[GL_Contrato] CHECK CONSTRAINT [FK_MonedaContrato]
GO
ALTER TABLE [dbo].[GL_Convocatoria]  WITH CHECK ADD  CONSTRAINT [FK_Convocatoria_Empleado] FOREIGN KEY([EmpleadoID])
REFERENCES [dbo].[GL_Empleado] ([EmpleadoID])
GO
ALTER TABLE [dbo].[GL_Convocatoria] CHECK CONSTRAINT [FK_Convocatoria_Empleado]
GO
ALTER TABLE [dbo].[GL_Convocatoria]  WITH CHECK ADD  CONSTRAINT [FK_Convocatoria_Rubro] FOREIGN KEY([RubroID])
REFERENCES [dbo].[GL_Rubro] ([RubroID])
GO
ALTER TABLE [dbo].[GL_Convocatoria] CHECK CONSTRAINT [FK_Convocatoria_Rubro]
GO
ALTER TABLE [dbo].[GL_Cotizacion]  WITH CHECK ADD  CONSTRAINT [FK_FormaPagoCotizacion] FOREIGN KEY([FormaPagoID])
REFERENCES [dbo].[GL_FormaPago] ([FormaPagoID])
GO
ALTER TABLE [dbo].[GL_Cotizacion] CHECK CONSTRAINT [FK_FormaPagoCotizacion]
GO
ALTER TABLE [dbo].[GL_Cotizacion]  WITH CHECK ADD  CONSTRAINT [FK_ProveedorCotizacion] FOREIGN KEY([ProveedorID])
REFERENCES [dbo].[GL_Proveedor] ([ProveedorID])
GO
ALTER TABLE [dbo].[GL_Cotizacion] CHECK CONSTRAINT [FK_ProveedorCotizacion]
GO
ALTER TABLE [dbo].[GL_Cotizacion]  WITH CHECK ADD  CONSTRAINT [FK_ReqCompraCotizacion] FOREIGN KEY([RequerimientoCompraID])
REFERENCES [dbo].[GL_RequerimientoCompra] ([RequerimientoCompraID])
GO
ALTER TABLE [dbo].[GL_Cotizacion] CHECK CONSTRAINT [FK_ReqCompraCotizacion]
GO
ALTER TABLE [dbo].[GL_Cotizacion]  WITH CHECK ADD  CONSTRAINT [FK_TransaccionCompra_Cotizacion] FOREIGN KEY([CotizacionID])
REFERENCES [dbo].[GL_TransaccionCompra] ([TransaccionCompraID])
GO
ALTER TABLE [dbo].[GL_Cotizacion] CHECK CONSTRAINT [FK_TransaccionCompra_Cotizacion]
GO
ALTER TABLE [dbo].[GL_DetalleConvocatoria]  WITH CHECK ADD  CONSTRAINT [FK_DetalleConvocatoria_Convocatoria] FOREIGN KEY([ConvocatoriaId])
REFERENCES [dbo].[GL_Convocatoria] ([ConvocatoriaId])
GO
ALTER TABLE [dbo].[GL_DetalleConvocatoria] CHECK CONSTRAINT [FK_DetalleConvocatoria_Convocatoria]
GO
ALTER TABLE [dbo].[GL_DetalleConvocatoria]  WITH CHECK ADD  CONSTRAINT [FK_DetalleConvocatoria_Postulante] FOREIGN KEY([PostulanteId])
REFERENCES [dbo].[GL_Postulante] ([PostulanteId])
GO
ALTER TABLE [dbo].[GL_DetalleConvocatoria] CHECK CONSTRAINT [FK_DetalleConvocatoria_Postulante]
GO
ALTER TABLE [dbo].[GL_DetallePostulante]  WITH CHECK ADD  CONSTRAINT [FK_DetallePostulante_Postulante] FOREIGN KEY([PostulanteId])
REFERENCES [dbo].[GL_Postulante] ([PostulanteId])
GO
ALTER TABLE [dbo].[GL_DetallePostulante] CHECK CONSTRAINT [FK_DetallePostulante_Postulante]
GO
ALTER TABLE [dbo].[GL_DetalleTransaccionCompra]  WITH CHECK ADD  CONSTRAINT [FK_Articulodetalle] FOREIGN KEY([ArticuloID])
REFERENCES [dbo].[GL_Articulo] ([ArticuloID])
GO
ALTER TABLE [dbo].[GL_DetalleTransaccionCompra] CHECK CONSTRAINT [FK_Articulodetalle]
GO
ALTER TABLE [dbo].[GL_DetalleTransaccionCompra]  WITH CHECK ADD  CONSTRAINT [FK_TransaccionDetalle] FOREIGN KEY([TransaccionCompraID])
REFERENCES [dbo].[GL_TransaccionCompra] ([TransaccionCompraID])
GO
ALTER TABLE [dbo].[GL_DetalleTransaccionCompra] CHECK CONSTRAINT [FK_TransaccionDetalle]
GO
ALTER TABLE [dbo].[GL_DetSolicitudActProveedor]  WITH CHECK ADD  CONSTRAINT [FK_DetSolicitudActProveedor] FOREIGN KEY([SolicitudActProveedorID])
REFERENCES [dbo].[GL_SolicitudActProveedor] ([SolicitudActProveedorID])
GO
ALTER TABLE [dbo].[GL_DetSolicitudActProveedor] CHECK CONSTRAINT [FK_DetSolicitudActProveedor]
GO
ALTER TABLE [dbo].[GL_DetSolicitudActProveedor]  WITH CHECK ADD  CONSTRAINT [FK_DetSolicitudActProveedor_ProveedorID] FOREIGN KEY([ProveedorID])
REFERENCES [dbo].[GL_Proveedor] ([ProveedorID])
GO
ALTER TABLE [dbo].[GL_DetSolicitudActProveedor] CHECK CONSTRAINT [FK_DetSolicitudActProveedor_ProveedorID]
GO
ALTER TABLE [dbo].[GL_Empleado]  WITH CHECK ADD  CONSTRAINT [FK_AreaEmpleado] FOREIGN KEY([AreaID])
REFERENCES [dbo].[GL_Area] ([AreaID])
GO
ALTER TABLE [dbo].[GL_Empleado] CHECK CONSTRAINT [FK_AreaEmpleado]
GO
ALTER TABLE [dbo].[GL_Licitacion]  WITH CHECK ADD  CONSTRAINT [FK_ReqCompraLicitacion] FOREIGN KEY([RequerimientoCompraID])
REFERENCES [dbo].[GL_RequerimientoCompra] ([RequerimientoCompraID])
GO
ALTER TABLE [dbo].[GL_Licitacion] CHECK CONSTRAINT [FK_ReqCompraLicitacion]
GO
ALTER TABLE [dbo].[GL_OrdenCompra]  WITH CHECK ADD  CONSTRAINT [FK_AlmacenOrdenCompra] FOREIGN KEY([AlmacenID])
REFERENCES [dbo].[GL_Almacen] ([AlmacenID])
GO
ALTER TABLE [dbo].[GL_OrdenCompra] CHECK CONSTRAINT [FK_AlmacenOrdenCompra]
GO
ALTER TABLE [dbo].[GL_OrdenCompra]  WITH CHECK ADD  CONSTRAINT [FK_CotizacionOrdenCompra] FOREIGN KEY([CotizacionID])
REFERENCES [dbo].[GL_Cotizacion] ([CotizacionID])
GO
ALTER TABLE [dbo].[GL_OrdenCompra] CHECK CONSTRAINT [FK_CotizacionOrdenCompra]
GO
ALTER TABLE [dbo].[GL_OrdenCompra]  WITH CHECK ADD  CONSTRAINT [FK_TransaccionCompra_OrdenCompra] FOREIGN KEY([OrdenCompraID])
REFERENCES [dbo].[GL_TransaccionCompra] ([TransaccionCompraID])
GO
ALTER TABLE [dbo].[GL_OrdenCompra] CHECK CONSTRAINT [FK_TransaccionCompra_OrdenCompra]
GO
ALTER TABLE [dbo].[GL_Presupuesto]  WITH CHECK ADD  CONSTRAINT [FK_AreaPresupuesto] FOREIGN KEY([AreaID])
REFERENCES [dbo].[GL_Area] ([AreaID])
GO
ALTER TABLE [dbo].[GL_Presupuesto] CHECK CONSTRAINT [FK_AreaPresupuesto]
GO
ALTER TABLE [dbo].[GL_Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_Proveedor_Postulante] FOREIGN KEY([PostulanteId])
REFERENCES [dbo].[GL_Postulante] ([PostulanteId])
GO
ALTER TABLE [dbo].[GL_Proveedor] CHECK CONSTRAINT [FK_Proveedor_Postulante]
GO
ALTER TABLE [dbo].[GL_Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_RubroProveedor] FOREIGN KEY([RubroID])
REFERENCES [dbo].[GL_Rubro] ([RubroID])
GO
ALTER TABLE [dbo].[GL_Proveedor] CHECK CONSTRAINT [FK_RubroProveedor]
GO
ALTER TABLE [dbo].[GL_RegistroProveedorParticipante]  WITH CHECK ADD  CONSTRAINT [FK_GL_RegistroProveedorParticipante_GL_Licitacion] FOREIGN KEY([LicitacionID])
REFERENCES [dbo].[GL_Licitacion] ([LicitacionID])
GO
ALTER TABLE [dbo].[GL_RegistroProveedorParticipante] CHECK CONSTRAINT [FK_GL_RegistroProveedorParticipante_GL_Licitacion]
GO
ALTER TABLE [dbo].[GL_RegistroProveedorParticipante]  WITH CHECK ADD  CONSTRAINT [FK_GL_RegistroProveedorParticipante_GL_Proveedor] FOREIGN KEY([ProveedorID])
REFERENCES [dbo].[GL_Proveedor] ([ProveedorID])
GO
ALTER TABLE [dbo].[GL_RegistroProveedorParticipante] CHECK CONSTRAINT [FK_GL_RegistroProveedorParticipante_GL_Proveedor]
GO
ALTER TABLE [dbo].[GL_RequerimientoCompra]  WITH CHECK ADD  CONSTRAINT [FK_EmpleadoReq] FOREIGN KEY([SolicitanteID])
REFERENCES [dbo].[GL_Empleado] ([EmpleadoID])
GO
ALTER TABLE [dbo].[GL_RequerimientoCompra] CHECK CONSTRAINT [FK_EmpleadoReq]
GO
ALTER TABLE [dbo].[GL_RequerimientoCompra]  WITH CHECK ADD  CONSTRAINT [FK_GL_RequerimientoCompra_GL_Empleado] FOREIGN KEY([EncargadoID])
REFERENCES [dbo].[GL_Empleado] ([EmpleadoID])
GO
ALTER TABLE [dbo].[GL_RequerimientoCompra] CHECK CONSTRAINT [FK_GL_RequerimientoCompra_GL_Empleado]
GO
ALTER TABLE [dbo].[GL_RequerimientoCompra]  WITH CHECK ADD  CONSTRAINT [FK_TransaccionCompra_RequerimientoCompra] FOREIGN KEY([RequerimientoCompraID])
REFERENCES [dbo].[GL_TransaccionCompra] ([TransaccionCompraID])
GO
ALTER TABLE [dbo].[GL_RequerimientoCompra] CHECK CONSTRAINT [FK_TransaccionCompra_RequerimientoCompra]
GO
ALTER TABLE [dbo].[GL_SolicitudActProveedor]  WITH CHECK ADD  CONSTRAINT [FK_SolicitudActProveedor] FOREIGN KEY([TipoSolicitudID])
REFERENCES [dbo].[GL_TipoSolicitud] ([TipoSolicitudID])
GO
ALTER TABLE [dbo].[GL_SolicitudActProveedor] CHECK CONSTRAINT [FK_SolicitudActProveedor]
GO
ALTER TABLE [dbo].[GL_TransaccionCompra]  WITH CHECK ADD  CONSTRAINT [FK_MonedaTransaccion] FOREIGN KEY([MonedaID])
REFERENCES [dbo].[GL_Moneda] ([MonedaID])
GO
ALTER TABLE [dbo].[GL_TransaccionCompra] CHECK CONSTRAINT [FK_MonedaTransaccion]
GO
ALTER TABLE [dbo].[Paciente]  WITH CHECK ADD  CONSTRAINT [FK_Paciente_TipoPaciente] FOREIGN KEY([IdTipoPaciente])
REFERENCES [dbo].[TipoPaciente] ([IdTipoPaciente])
GO
ALTER TABLE [dbo].[Paciente] CHECK CONSTRAINT [FK_Paciente_TipoPaciente]
GO
ALTER TABLE [dbo].[Protocolo]  WITH CHECK ADD  CONSTRAINT [FK_Protocolo_Destino] FOREIGN KEY([IdDestino])
REFERENCES [dbo].[Destino] ([IdDestino])
GO
ALTER TABLE [dbo].[Protocolo] CHECK CONSTRAINT [FK_Protocolo_Destino]
GO
ALTER TABLE [dbo].[Protocolo]  WITH CHECK ADD  CONSTRAINT [FK_Protocolo_Prioridad] FOREIGN KEY([IdPrioridad])
REFERENCES [dbo].[Prioridad] ([IdPrioridad])
GO
ALTER TABLE [dbo].[Protocolo] CHECK CONSTRAINT [FK_Protocolo_Prioridad]
GO
ALTER TABLE [dbo].[Protocolo]  WITH CHECK ADD  CONSTRAINT [FK_Protocolo_Sintoma] FOREIGN KEY([IdSintoma])
REFERENCES [dbo].[Sintoma] ([IdSintoma])
GO
ALTER TABLE [dbo].[Protocolo] CHECK CONSTRAINT [FK_Protocolo_Sintoma]
GO
ALTER TABLE [dbo].[Requerimiento_Insumo]  WITH CHECK ADD  CONSTRAINT [FK_Requerimiento_Insumo_Estados] FOREIGN KEY([IdEstado])
REFERENCES [dbo].[Estados] ([IdEstado])
GO
ALTER TABLE [dbo].[Requerimiento_Insumo] CHECK CONSTRAINT [FK_Requerimiento_Insumo_Estados]
GO
ALTER TABLE [dbo].[Requerimiento_Insumo]  WITH CHECK ADD  CONSTRAINT [FK_Requerimiento_Insumo_Insumo] FOREIGN KEY([IdInsumo])
REFERENCES [dbo].[Insumo] ([IdInsumo])
GO
ALTER TABLE [dbo].[Requerimiento_Insumo] CHECK CONSTRAINT [FK_Requerimiento_Insumo_Insumo]
GO
ALTER TABLE [dbo].[Requerimiento_Insumo]  WITH CHECK ADD  CONSTRAINT [FK_Requerimiento_Insumo_RequerimientoInsumo] FOREIGN KEY([IdRequerimientoInsumo])
REFERENCES [dbo].[RequerimientoInsumo] ([IdRequerimientoInsumo])
GO
ALTER TABLE [dbo].[Requerimiento_Insumo] CHECK CONSTRAINT [FK_Requerimiento_Insumo_RequerimientoInsumo]
GO
ALTER TABLE [dbo].[RequerimientoInsumo]  WITH CHECK ADD  CONSTRAINT [FK_RequerimientoInsumo_PersonalEmergencia] FOREIGN KEY([IdPersonalEmergencia])
REFERENCES [dbo].[PersonalEmergencia] ([IdPersonalEmergencia])
GO
ALTER TABLE [dbo].[RequerimientoInsumo] CHECK CONSTRAINT [FK_RequerimientoInsumo_PersonalEmergencia]
GO
ALTER TABLE [dbo].[RequerimientoTurno]  WITH CHECK ADD  CONSTRAINT [FK_RequerimientoTurno_PersonalEmergencia] FOREIGN KEY([IdPersonalEmergencia])
REFERENCES [dbo].[PersonalEmergencia] ([IdPersonalEmergencia])
GO
ALTER TABLE [dbo].[RequerimientoTurno] CHECK CONSTRAINT [FK_RequerimientoTurno_PersonalEmergencia]
GO
ALTER TABLE [dbo].[TicketEmergencia]  WITH CHECK ADD  CONSTRAINT [FK_TicketEmergencia_Destino] FOREIGN KEY([IdDestino])
REFERENCES [dbo].[Destino] ([IdDestino])
GO
ALTER TABLE [dbo].[TicketEmergencia] CHECK CONSTRAINT [FK_TicketEmergencia_Destino]
GO
ALTER TABLE [dbo].[TicketEmergencia]  WITH CHECK ADD  CONSTRAINT [FK_TicketEmergencia_Paciente] FOREIGN KEY([IdPaciente])
REFERENCES [dbo].[Paciente] ([IdPaciente])
GO
ALTER TABLE [dbo].[TicketEmergencia] CHECK CONSTRAINT [FK_TicketEmergencia_Paciente]
GO
ALTER TABLE [dbo].[TicketEmergencia]  WITH CHECK ADD  CONSTRAINT [FK_TicketEmergencia_PersonalEmergencia] FOREIGN KEY([IdPersonalEmergencia])
REFERENCES [dbo].[PersonalEmergencia] ([IdPersonalEmergencia])
GO
ALTER TABLE [dbo].[TicketEmergencia] CHECK CONSTRAINT [FK_TicketEmergencia_PersonalEmergencia]
GO
ALTER TABLE [dbo].[TicketEmergencia]  WITH CHECK ADD  CONSTRAINT [FK_TicketEmergencia_TicketSalaObservacion] FOREIGN KEY([IdTicketSala])
REFERENCES [dbo].[TicketSalaObservacion] ([IdTicketSala])
GO
ALTER TABLE [dbo].[TicketEmergencia] CHECK CONSTRAINT [FK_TicketEmergencia_TicketSalaObservacion]
GO
ALTER TABLE [dbo].[TicketEmergencia]  WITH CHECK ADD  CONSTRAINT [FK_TicketEmergencia_TicketTraumaShockTopico] FOREIGN KEY([IdTicketTrauma])
REFERENCES [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma])
GO
ALTER TABLE [dbo].[TicketEmergencia] CHECK CONSTRAINT [FK_TicketEmergencia_TicketTraumaShockTopico]
GO
ALTER TABLE [dbo].[TicketEmergencia]  WITH CHECK ADD  CONSTRAINT [FK_TicketEmergencia_Tratamiento] FOREIGN KEY([IdTratamiento])
REFERENCES [dbo].[Tratamiento] ([IdTratamiento])
GO
ALTER TABLE [dbo].[TicketEmergencia] CHECK CONSTRAINT [FK_TicketEmergencia_Tratamiento]
GO
ALTER TABLE [dbo].[Triaje]  WITH CHECK ADD  CONSTRAINT [FK_Triaje_Prioridad] FOREIGN KEY([IdPrioridad])
REFERENCES [dbo].[Prioridad] ([IdPrioridad])
GO
ALTER TABLE [dbo].[Triaje] CHECK CONSTRAINT [FK_Triaje_Prioridad]
GO
ALTER TABLE [dbo].[Triaje]  WITH CHECK ADD  CONSTRAINT [FK_Triaje_Sintoma] FOREIGN KEY([IdSintoma])
REFERENCES [dbo].[Sintoma] ([IdSintoma])
GO
ALTER TABLE [dbo].[Triaje] CHECK CONSTRAINT [FK_Triaje_Sintoma]
GO
ALTER TABLE [dbo].[Triaje]  WITH CHECK ADD  CONSTRAINT [FK_Triaje_TicketEmergencia] FOREIGN KEY([IdTicketEmergencia])
REFERENCES [dbo].[TicketEmergencia] ([IdTicketEmergencia])
GO
ALTER TABLE [dbo].[Triaje] CHECK CONSTRAINT [FK_Triaje_TicketEmergencia]
GO
USE [master]
GO
ALTER DATABASE [BDRicardoPalma] SET  READ_WRITE 
GO
