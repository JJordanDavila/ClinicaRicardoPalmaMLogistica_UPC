USE [master]
GO

CREATE DATABASE [BDRicardoPalma]

GO

USE [BDRicardoPalma]
GO
/****** Object:  StoredProcedure [dbo].[Spl_GetIdLic]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Spl_GetIdTra]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  StoredProcedure [dbo].[Spl_GetNumero]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  StoredProcedure [dbo].[sql2pdf]    Script Date: 11/7/2017 2:09:12 AM ******/
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
  INSERT INTO #pdf (code) VALUES ('%сссс')
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
/****** Object:  Table [dbo].[Destino]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Almacen]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Area]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Articulo]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_ConsultaLicitacion]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Contrato]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Convocatoria]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Cotizacion]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_DetalleConvocatoria]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_DetallePostulante]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_DetalleTransaccionCompra]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_DetSolicitudActProveedor]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Empleado]    Script Date: 11/7/2017 2:09:12 AM ******/
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
	[Correo] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[EmpleadoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_FormaPago]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Licitacion]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Moneda]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_OrdenCompra]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Postulante]    Script Date: 11/7/2017 2:09:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Postulante](
	[PostulanteId] [int] NOT NULL,
	[RazonSocial] [varchar](50) NOT NULL,
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
/****** Object:  Table [dbo].[GL_Presupuesto]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Proveedor]    Script Date: 11/7/2017 2:09:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_Proveedor](
	[ProveedorID] [int] IDENTITY(1,1) NOT NULL,
	[RubroID] [int] NOT NULL,
	[NombreComercial] [varchar](50) NOT NULL,
	[RazonSocial] [varchar](50) NOT NULL,
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
/****** Object:  Table [dbo].[GL_RegistroProveedorParticipante]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_ReqMovAlmacen]    Script Date: 11/7/2017 2:09:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_ReqMovAlmacen](
	[ReqMovAlmacenID] [int] NOT NULL,
	[TipoMovimientoID] [int] NOT NULL,
	[Descripcion] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ReqMovAlmacenID] ASC,
	[TipoMovimientoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_RequerimientoCompra]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_Rubro]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_SolicitudActProveedor]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_TipoMovimiento]    Script Date: 11/7/2017 2:09:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GL_TipoMovimiento](
	[TipoMovimientoID] [int] IDENTITY(1,1) NOT NULL,
	[Descripcion] [varchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TipoMovimientoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GL_TipoSolicitud]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_TransaccionCompra]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[GL_UnidadMedida]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[Paciente]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[Prioridad]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[Protocolo]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[psopdf]    Script Date: 11/7/2017 2:09:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[psopdf](
	[code] [nvarchar](200) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sintoma]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[TicketEmergencia]    Script Date: 11/7/2017 2:09:12 AM ******/
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
	[Ingreso] [datetime] NULL,
	[Egreso] [datetime] NULL,
 CONSTRAINT [PK_TicketEmergencia] PRIMARY KEY CLUSTERED 
(
	[IdTicketEmergencia] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TicketSalaObservacion]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[TicketTraumaShockTopico]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[TipoPaciente]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[Triaje]    Script Date: 11/7/2017 2:09:12 AM ******/
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
/****** Object:  Table [dbo].[Usuario]    Script Date: 11/7/2017 2:09:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Usuario](
	[IdUsuario] [int] IDENTITY(1,1) NOT NULL,
	[CodigoUsuario] [varchar](20) NOT NULL,
	[Nombres] [varchar](100) NULL,
	[Clave] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET IDENTITY_INSERT [dbo].[Destino] ON 

GO
INSERT [dbo].[Destino] ([IdDestino], [Descripcion]) VALUES (1, N'Local Ricardo Palma')
GO
INSERT [dbo].[Destino] ([IdDestino], [Descripcion]) VALUES (2, N'Local Surco')
GO
SET IDENTITY_INSERT [dbo].[Destino] OFF
GO
SET IDENTITY_INSERT [dbo].[Paciente] ON 

GO
INSERT [dbo].[Paciente] ([IdPaciente], [IdTipoPaciente], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [dni]) VALUES (1, 1, N'Pablo', N'Flores', N'Flores', 12345678)
GO
INSERT [dbo].[Paciente] ([IdPaciente], [IdTipoPaciente], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [dni]) VALUES (2, 2, N'Jose', N'Gomez', N'Gonzales', 65214589)
GO
INSERT [dbo].[Paciente] ([IdPaciente], [IdTipoPaciente], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [dni]) VALUES (3, 3, N'Manuel', N'Gomez', N'Nuñez', 78952365)
GO
INSERT [dbo].[Paciente] ([IdPaciente], [IdTipoPaciente], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [dni]) VALUES (4, 4, N'Pierre', N'Santos', N'Perez', 652365412)
GO
INSERT [dbo].[Paciente] ([IdPaciente], [IdTipoPaciente], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [dni]) VALUES (9, 5, N'jose', N'caceda', N'lopez', 52354252)
GO
INSERT [dbo].[Paciente] ([IdPaciente], [IdTipoPaciente], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [dni]) VALUES (10, 5, N'miguel', N'perez', N'lopez', 73463453)
GO
SET IDENTITY_INSERT [dbo].[Paciente] OFF
GO
SET IDENTITY_INSERT [dbo].[Prioridad] ON 

GO
INSERT [dbo].[Prioridad] ([IdPrioridad], [Descripcion]) VALUES (1, N'Prioridad I - Gravedad súbita extrema')
GO
INSERT [dbo].[Prioridad] ([IdPrioridad], [Descripcion]) VALUES (2, N'Prioridad II - Urgencia mayor')
GO
INSERT [dbo].[Prioridad] ([IdPrioridad], [Descripcion]) VALUES (3, N'Prioridad III - Urgencia menor')
GO
INSERT [dbo].[Prioridad] ([IdPrioridad], [Descripcion]) VALUES (4, N'Prioridad IV - Patología aguda común')
GO
SET IDENTITY_INSERT [dbo].[Prioridad] OFF
GO
SET IDENTITY_INSERT [dbo].[Protocolo] ON 

GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (1, 1, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 10)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (2, 2, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 6)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (3, 3, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (4, 4, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (5, 5, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (6, 6, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 10)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (7, 7, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (8, 8, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (9, 9, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (10, 10, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (11, 11, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (12, 12, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 6)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (13, 13, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 7)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (14, 14, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (15, 15, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 6)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (16, 16, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (17, 17, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (18, 18, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 7)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (19, 19, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (20, 20, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (21, 21, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 6)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (22, 22, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (23, 23, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (24, 24, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 10)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (25, 25, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 7)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (26, 26, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (27, 27, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (28, 28, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 7)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (29, 29, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (30, 30, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (31, 31, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 10)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (32, 32, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (33, 33, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (34, 34, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 6)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (35, 35, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (36, 36, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (37, 37, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 10)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (38, 38, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 8)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (39, 39, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 5)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (40, 40, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 6)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (41, 41, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 6)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (42, 42, 1, 1, N'Trauma', N'Cuidado intensivos', N'Llevar con ambulancia caso si es necesario', N'Dar aprobación para dar de alta', 9)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (43, 43, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (44, 44, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (45, 45, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (46, 46, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (47, 47, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (48, 48, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (49, 49, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (50, 50, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (51, 51, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (52, 52, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (53, 53, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (54, 54, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (55, 55, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (56, 56, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (57, 57, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (58, 58, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (59, 59, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (60, 60, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (61, 61, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (62, 62, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (63, 63, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (64, 64, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (65, 65, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (66, 66, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (67, 67, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (68, 68, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (69, 69, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (70, 70, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (71, 71, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (72, 72, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (73, 73, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (74, 74, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (75, 75, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (76, 76, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 4)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (77, 77, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (78, 78, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (79, 79, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (80, 80, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (81, 81, 2, 1, N'Trauma', N'Medicación bajo control', N'Internado si es necesario', N'Dar aprobación para dar de alta', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (82, 82, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (83, 83, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (84, 84, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (85, 85, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (86, 86, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (87, 87, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (88, 88, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (89, 89, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (90, 90, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (91, 91, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (92, 92, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (93, 93, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (94, 94, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (95, 95, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (96, 96, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (97, 97, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (98, 98, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 3)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (99, 99, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (100, 100, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (101, 101, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (102, 102, 3, 1, N'Topico', N'Tratamiento bajo control', N'Varios exámenes médicos', N'Tratamiento a seguir', 2)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (103, 103, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 0)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (104, 104, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 0)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (105, 105, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 0)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (106, 106, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 0)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (107, 107, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (108, 108, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (109, 109, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 1)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (110, 110, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 0)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (111, 111, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 0)
GO
INSERT [dbo].[Protocolo] ([IdProtocolo], [IdSintoma], [IdPrioridad], [IdDestino], [Sala], [Diagnostico], [CondicionIngreso], [CondicionEgreso], [DiasAtencion]) VALUES (112, 112, 4, 1, N'Topico', N'Receta médica', N'Consulta general', N'Tratamiento a seguir', 1)
GO
SET IDENTITY_INSERT [dbo].[Protocolo] OFF
GO
SET IDENTITY_INSERT [dbo].[Sintoma] ON 

GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (1, N'Paro Cardio Respiratorio')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (2, N' Dolor Torácico Precordial de posible origen cardiogénico con o sin hipotensión')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (3, N' Dificultad respiratoria (evidenciada por polípnea, taquípnea, tiraje, sibilantes, estridor,cianosis)')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (4, N' Shock (Hemorrágico, cardiogénico, distributivo, obstructivo)')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (5, N' Arritmia con compromiso hemodinámico de posible origen cardiogénico con o sin hipotensión')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (6, N' Hemorragia profusa')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (7, N' Obstrucción de vía respiratoria alta')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (8, N' Inestabilidad Hemodinámica (hipotensión 1 shock 1 crisis hipertensiva)')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (9, N' Paciente inconsciente que no responde a estímulos')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (10, N' Víctima de accidente de tránsito')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (11, N' Quemaduras con extensión mayor del 20% de superficie corporal')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (12, N' Caída o precipitación del altura')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (13, N' Dos o más fracturas de huesos largos proximales')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (14, N' Injurias en extremidades con compromiso neurovascular')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (15, N' Herida de bala o arma blanca')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (16, N' Sospecha de traumatismo vertebro medular')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (17, N' Evisceración')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (18, N' Amputación con sangrado no controlado')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (19, N' Traumatismo encéfalo craneano')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (20, N' Status Convulsivo')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (21, N' Sobredosis de drogas o alcohol más depresión respiratoria')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (22, N' lngesta de órgano fosforados, ácidos, álcalis, otras intoxicaciones o envenenamientos')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (23, N' Signos y síntomas de abdomen agudo con descompensación hemodinámica')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (24, N' Signos y síntomas de embarazo ectópico roto')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (25, N'Suicidio frustro')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (26, N' Intento suicida')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (27, N' Crisis de agitación psicomotora con conducta heteroagresiva')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (28, N' Problemas específicos en pacientes pediátricos')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (29, N' Intoxicaciones por ingesta o contacto')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (30, N' Períodos de apnea')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (31, N' Cambios en el estado mental: letargia, delirio, alucinaciones, llanto débil')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (32, N' Deshidratación con Shock: Llenado capilar mayor de tres segundos')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (33, N' Sangrado: Hematemesis, sangrado rectal, vaginal, epistaxis severa')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (34, N' Quemaduras en cara o más del 10% de área corporal')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (35, N' Quemaduras por fuego en ambiente cerrado')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (36, N' Acontecimiento de aspiración u obstrucción con cuerpo extraño')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (37, N' Status convulsivo')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (38, N' Status asmático')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (39, N' Hipertermia maligna')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (40, N' Trastornos de sensorio')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (41, N' Politraumatismo')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (42, N' Herida por arma de fuego ')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (43, N'Frecuencia respiratoria > de 24 por minuto')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (44, N' Crisis asmática con broncoespasmo moderado')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (45, N' Diabetes Mellitus Descompensada')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (46, N' Hemoptisis')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (47, N' Signos y síntomas de Abdomen Agudo')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (48, N' Convulsión reciente en paciente consciente')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (49, N' Dolor torácico no cardiogénico sin compromiso hemodinámico')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (50, N' Arritmias sin compromiso hemodinámico')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (51, N' Sangrado gastrointestinal, con signos vitales estables')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (52, N' Paciente con trastornos en el sensorio')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (53, N' Hipotonía, flacidez muscular aguda y de evolución progresiva')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (54, N' Descompensación Hepática')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (55, N' Hernia umbilical o inguinal incarcerada')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (56, N' Signos y síntomas de descompensación tiroidea')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (57, N' Contusiones o traumatismos con sospecha de fractura o luxación')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (58, N' Herida cortante que requiere sutura')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (59, N' Injuria en ojos: perforación, laceración, avulsión')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (60, N' Desprendimiento de retina')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (61, N' Fiebre y signos inflamatorios en articulaciones')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (62, N' Síntomas y signos de cólera')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (63, N' Deshidratación Aguda sin descompensación hemodinámica')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (64, N' Hematuria macroscópica')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (65, N'Reacción alérgica, sin compromiso respiratorio')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (66, N' Síndrome febril o Infección en paciente inmunosuprimido ')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (67, N'Hemodiálísis, con síntomas y signos agudos')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (68, N' Coagulopatía sin descompensación hemodinámica')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (69, N' Sobredosis de drogas y alcohol sin depresión respiratoria')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (70, N' Cefalea con antecedentes de trauma craneal')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (71, N' Síndrome Meníngeo')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (72, N' Síntomas y signos de enfermedades vasculares agudas')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (73, N' Cólico renal sin respuesta a la analgesia, mayor de 06 horas')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (74, N' Retención urinaria')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (75, N' Síndrome de abstinencia de drogas y alcohol')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (76, N' Cuerpos extraños en orificios corporales')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (77, N' Cuerpos extraños en esófago y estómago')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (78, N' Pacientes con ideación suicida')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (79, N' Pacientes con crisis de ansiedad')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (80, N'Cuadro de demencia con conducta psicótica')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (81, N' Esguinces')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (82, N'Dolor abdominal leve con nauseas, vómitos, diarrea, signos vitales estables')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (83, N'Herida que no requiere sutura')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (84, N'Intoxicación alimentaría')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (85, N' Trastornos de músculos y ligamentos')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (86, N' Otitis Media Aguda')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (87, N'Deshidratación hídroelectrólitíca leve')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (88, N'Osteocondropatía aguda ')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (89, N'Sinusitis aguda')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (90, N' Hiperémesis gravídica sin compromiso metabólico')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (91, N' Fiebre> de 39° sin síntomas asociados')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (92, N' Síndrome vertiginoso y trastorno vascular')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (93, N' Celulitis o absceso con fiebre')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (94, N' Funcionamiento defectuoso de colostomía, ureterostomía, talla vesical u otros similares')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (95, N' Lumbalgia aguda')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (96, N' Broncoespasmo leve')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (97, N' Hipertensión arterial leve no controlada')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (98, N' Signos y síntomas de depresión')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (99, N' Crisis de ansiedad o disociativas')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (100, N' Signos y síntomas de infección urinaria alta')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (101, N' Pacientes con neurosis de ansiedad')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (102, N' Pacientes sicóticos con reagudización de sus síntomas pero aún sin conducta psicótica')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (103, N'Faringitis aguda')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (104, N' Amigdalitis aguda')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (105, N' Enfermedad diarreica aguda sin deshidratación o vómitos')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (106, N' Absceso sin fiebre')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (107, N' Sangrado vaginal leve en no gestante, con funciones vitales estables')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (108, N' Fiebre sin síntomas asociados')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (109, N' Resfrío común')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (110, N' Dolor de oído leve')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (111, N' Dolor de garganta sin disfagia')
GO
INSERT [dbo].[Sintoma] ([IdSintoma], [Descripcion]) VALUES (112, N'Enfermedades crónicas no descompensadas')
GO
SET IDENTITY_INSERT [dbo].[Sintoma] OFF
GO
SET IDENTITY_INSERT [dbo].[TicketEmergencia] ON 

GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (7, 1, 1, NULL, 1, NULL, CAST(0x0000A81D00C3FE61 AS DateTime), CAST(0x0000A82500C3FF7C AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (11, 1, 1, NULL, NULL, 4, CAST(0x0000A81D00C7604B AS DateTime), CAST(0x0000A81E00C76043 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (12, 1, 1, NULL, NULL, 5, CAST(0x0000A81D00C7DBD8 AS DateTime), CAST(0x0000A81E00C7DBD4 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (13, 1, 1, NULL, NULL, 6, CAST(0x0000A81D00CA575C AS DateTime), CAST(0x0000A81E00CA5755 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (14, 1, 1, NULL, NULL, 7, CAST(0x0000A81D00CAEBEC AS DateTime), CAST(0x0000A81E00CAEBE4 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (17, 1, 1, NULL, NULL, 8, CAST(0x0000A81D00CB8D6F AS DateTime), CAST(0x0000A81E00CB8D68 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (18, 1, 1, NULL, 2, NULL, CAST(0x0000A81D00CBCADE AS DateTime), CAST(0x0000A82300CBCADE AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (19, 1, 1, NULL, 3, NULL, CAST(0x0000A81D00CD3BCF AS DateTime), CAST(0x0000A82600CD3BCF AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (20, 1, 1, 1, 4, NULL, CAST(0x0000A81D00F91F99 AS DateTime), CAST(0x0000A82400F91F99 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (21, 1, 1, 1, 5, NULL, CAST(0x0000A81D00F942B9 AS DateTime), CAST(0x0000A82400F942B9 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (22, 2, 1, 0, 6, NULL, CAST(0x0000A81D00F9CAA3 AS DateTime), CAST(0x0000A82300F9CAA3 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (23, 1, 1, 0, NULL, 9, CAST(0x0000A81D00FDFC8C AS DateTime), CAST(0x0000A81E00FDFC84 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (25, 1, 1, 0, 7, NULL, CAST(0x0000A81D00FED293 AS DateTime), CAST(0x0000A82600FED293 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (26, 1, 1, 0, NULL, 10, CAST(0x0000A81D00FEEB34 AS DateTime), CAST(0x0000A81E00FEEB2D AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (27, 1, 1, 1, 8, NULL, CAST(0x0000A81F00B2AA91 AS DateTime), CAST(0x0000A82600B2AA91 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (28, 1, 1, 1, 9, NULL, CAST(0x0000A81F00B2CC97 AS DateTime), CAST(0x0000A82600B2CC97 AS DateTime))
GO
INSERT [dbo].[TicketEmergencia] ([IdTicketEmergencia], [IdPaciente], [IdDestino], [EsViolencia], [IdTicketTrauma], [IdTicketSala], [Ingreso], [Egreso]) VALUES (29, 1, 1, 1, 10, NULL, CAST(0x0000A81F00B3B71A AS DateTime), CAST(0x0000A82500B3B71B AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[TicketEmergencia] OFF
GO
SET IDENTITY_INSERT [dbo].[TicketSalaObservacion] ON 

GO
INSERT [dbo].[TicketSalaObservacion] ([IdTicketSala], [Ingreso], [Egreso], [Diagnostico], [CondicionIngreso], [CondicionEgreso]) VALUES (4, CAST(0x0000A81D00C7604B AS DateTime), CAST(0x0000A81E00C76043 AS DateTime), N'Receta médica', N'Consulta general', N'Tratamiento a seguir')
GO
INSERT [dbo].[TicketSalaObservacion] ([IdTicketSala], [Ingreso], [Egreso], [Diagnostico], [CondicionIngreso], [CondicionEgreso]) VALUES (5, CAST(0x0000A81D00C7DBD8 AS DateTime), CAST(0x0000A81E00C7DBD4 AS DateTime), N'Receta médica', N'Consulta general', N'Tratamiento a seguir')
GO
INSERT [dbo].[TicketSalaObservacion] ([IdTicketSala], [Ingreso], [Egreso], [Diagnostico], [CondicionIngreso], [CondicionEgreso]) VALUES (6, CAST(0x0000A81D00CA575C AS DateTime), CAST(0x0000A81E00CA5755 AS DateTime), N'Receta médica', N'Consulta general', N'Tratamiento a seguir')
GO
INSERT [dbo].[TicketSalaObservacion] ([IdTicketSala], [Ingreso], [Egreso], [Diagnostico], [CondicionIngreso], [CondicionEgreso]) VALUES (7, CAST(0x0000A81D00CAEBEC AS DateTime), CAST(0x0000A81E00CAEBE4 AS DateTime), N'Receta médica', N'Consulta general', N'Tratamiento a seguir')
GO
INSERT [dbo].[TicketSalaObservacion] ([IdTicketSala], [Ingreso], [Egreso], [Diagnostico], [CondicionIngreso], [CondicionEgreso]) VALUES (8, CAST(0x0000A81D00CB8D6F AS DateTime), CAST(0x0000A81E00CB8D68 AS DateTime), N'Receta médica', N'Consulta general', N'Tratamiento a seguir')
GO
INSERT [dbo].[TicketSalaObservacion] ([IdTicketSala], [Ingreso], [Egreso], [Diagnostico], [CondicionIngreso], [CondicionEgreso]) VALUES (9, CAST(0x0000A81D00FDFC8C AS DateTime), CAST(0x0000A81E00FDFC84 AS DateTime), N'Receta médica', N'Consulta general', N'Tratamiento a seguir')
GO
INSERT [dbo].[TicketSalaObservacion] ([IdTicketSala], [Ingreso], [Egreso], [Diagnostico], [CondicionIngreso], [CondicionEgreso]) VALUES (10, CAST(0x0000A81D00FEEB34 AS DateTime), CAST(0x0000A81E00FEEB2D AS DateTime), N'Receta médica', N'Consulta general', N'Tratamiento a seguir')
GO
SET IDENTITY_INSERT [dbo].[TicketSalaObservacion] OFF
GO
SET IDENTITY_INSERT [dbo].[TicketTraumaShockTopico] ON 

GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (1, CAST(0x0000A81D00C3FE61 AS DateTime), CAST(0x0000A82500C3FF7C AS DateTime), N'Cuidado intensivos', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (2, CAST(0x0000A81D00CBCADE AS DateTime), CAST(0x0000A82300CBCADE AS DateTime), N'Cuidado intensivos', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (3, CAST(0x0000A81D00CD3BCF AS DateTime), CAST(0x0000A82600CD3BCF AS DateTime), N'Cuidado intensivos', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (4, CAST(0x0000A81D00F91F99 AS DateTime), CAST(0x0000A82400F91F99 AS DateTime), N'Cuidado intensivos', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (5, CAST(0x0000A81D00F942B9 AS DateTime), CAST(0x0000A82400F942B9 AS DateTime), N'Medicación bajo control', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (6, CAST(0x0000A81D00F9CAA3 AS DateTime), CAST(0x0000A82300F9CAA3 AS DateTime), N'Cuidado intensivos', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (7, CAST(0x0000A81D00FED293 AS DateTime), CAST(0x0000A82600FED293 AS DateTime), N'Cuidado intensivos', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (8, CAST(0x0000A81F00B2AA91 AS DateTime), CAST(0x0000A82600B2AA91 AS DateTime), N'Medicación bajo control', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (9, CAST(0x0000A81F00B2CC97 AS DateTime), CAST(0x0000A82600B2CC97 AS DateTime), N'Cuidado intensivos', 1)
GO
INSERT [dbo].[TicketTraumaShockTopico] ([IdTicketTrauma], [Ingreso], [Egreso], [Diagnostico], [EsTraumaShock]) VALUES (10, CAST(0x0000A81F00B3B71A AS DateTime), CAST(0x0000A82500B3B71B AS DateTime), N'Medicación bajo control', 1)
GO
SET IDENTITY_INSERT [dbo].[TicketTraumaShockTopico] OFF
GO
SET IDENTITY_INSERT [dbo].[TipoPaciente] ON 

GO
INSERT [dbo].[TipoPaciente] ([IdTipoPaciente], [Descripcion]) VALUES (1, N'Pediátrico lactante')
GO
INSERT [dbo].[TipoPaciente] ([IdTipoPaciente], [Descripcion]) VALUES (2, N'Pediátrico pre-escolar')
GO
INSERT [dbo].[TipoPaciente] ([IdTipoPaciente], [Descripcion]) VALUES (3, N'Problemas sistema nervioso')
GO
INSERT [dbo].[TipoPaciente] ([IdTipoPaciente], [Descripcion]) VALUES (4, N'Obstétrica')
GO
INSERT [dbo].[TipoPaciente] ([IdTipoPaciente], [Descripcion]) VALUES (5, N'Adulto')
GO
SET IDENTITY_INSERT [dbo].[TipoPaciente] OFF
GO
SET IDENTITY_INSERT [dbo].[Triaje] ON 

GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (6, 1, 7, 3)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (7, 1, 7, 5)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (8, 1, 7, 8)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (9, 1, 7, 9)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (10, 1, 7, 10)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (11, 1, 7, 13)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (25, 2, 11, 66)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (26, 3, 11, 101)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (27, 4, 11, 105)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (28, 4, 11, 109)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (29, 4, 11, 112)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (30, 3, 12, 101)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (31, 4, 12, 104)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (32, 4, 12, 108)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (33, 4, 12, 111)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (34, 1, 13, 5)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (35, 3, 13, 102)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (36, 4, 13, 106)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (37, 4, 13, 111)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (38, 1, 14, 7)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (39, 3, 14, 102)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (40, 4, 14, 105)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (41, 4, 14, 112)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (52, 4, 17, 104)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (53, 4, 17, 105)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (54, 4, 17, 109)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (55, 4, 17, 112)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (56, 1, 18, 2)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (57, 3, 18, 101)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (58, 4, 18, 104)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (59, 4, 18, 108)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (60, 4, 18, 110)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (61, 1, 19, 4)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (62, 1, 20, 5)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (63, 1, 20, 7)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (64, 1, 20, 9)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (65, 1, 20, 13)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (66, 1, 21, 3)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (67, 2, 21, 70)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (68, 4, 21, 111)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (69, 4, 21, 112)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (70, 1, 22, 25)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (71, 1, 22, 30)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (72, 3, 22, 89)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (73, 3, 22, 93)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (74, 3, 22, 100)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (75, 3, 22, 102)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (76, 4, 22, 107)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (77, 4, 22, 112)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (78, 1, 23, 5)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (79, 2, 23, 51)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (80, 2, 23, 58)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (81, 3, 23, 96)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (82, 4, 23, 103)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (88, 1, 25, 5)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (89, 1, 25, 26)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (90, 3, 25, 101)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (91, 4, 25, 108)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (92, 3, 26, 100)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (93, 3, 26, 101)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (94, 3, 26, 102)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (95, 4, 26, 103)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (96, 4, 26, 104)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (97, 4, 26, 105)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (98, 4, 26, 106)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (99, 4, 26, 107)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (100, 4, 26, 108)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (101, 4, 26, 109)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (102, 4, 26, 110)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (103, 4, 26, 111)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (104, 4, 26, 112)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (105, 1, 27, 5)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (106, 1, 27, 2)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (107, 4, 27, 108)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (108, 4, 27, 109)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (109, 1, 27, 33)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (110, 2, 27, 56)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (111, 1, 28, 18)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (112, 1, 28, 12)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (113, 1, 28, 29)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (114, 1, 28, 7)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (115, 1, 29, 18)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (116, 1, 29, 12)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (117, 2, 29, 70)
GO
INSERT [dbo].[Triaje] ([IdTriaje], [IdPrioridad], [IdTicketEmergencia], [IdSintoma]) VALUES (118, 3, 29, 85)
GO
SET IDENTITY_INSERT [dbo].[Triaje] OFF
GO
SET IDENTITY_INSERT [dbo].[Usuario] ON 

GO
INSERT [dbo].[Usuario] ([IdUsuario], [CodigoUsuario], [Nombres], [Clave]) VALUES (1, N'RP001', N'Pablo Flores', N'123')
GO
SET IDENTITY_INSERT [dbo].[Usuario] OFF
GO
ALTER TABLE [dbo].[GL_ConsultaLicitacion] ADD  CONSTRAINT [DF_GL_ConsultaLicitacion_revisado]  DEFAULT ((0)) FOR [revisado]
GO
ALTER TABLE [dbo].[GL_Licitacion] ADD  CONSTRAINT [DF_GL_Licitacion_Fecha]  DEFAULT (getdate()) FOR [Fecha]
GO
ALTER TABLE [dbo].[GL_Proveedor] ADD  CONSTRAINT [DF_GL_Proveedor_ConstanciaRNP]  DEFAULT ((0)) FOR [ConstanciaRNP]
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
ALTER TABLE [dbo].[GL_ReqMovAlmacen]  WITH CHECK ADD  CONSTRAINT [FK_GL_ReqMovAlmacen] FOREIGN KEY([TipoMovimientoID])
REFERENCES [dbo].[GL_TipoMovimiento] ([TipoMovimientoID])
GO
ALTER TABLE [dbo].[GL_ReqMovAlmacen] CHECK CONSTRAINT [FK_GL_ReqMovAlmacen]
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
