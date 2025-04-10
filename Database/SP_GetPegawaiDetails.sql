USE [mytestdb]
GO

/****** Object:  StoredProcedure [dbo].[GetPegawaiDetails]    Script Date: 2025-04-10 10:51:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPegawaiDetails]
    @nama NVARCHAR(100) = NULL,   -- Menambahkan parameter untuk nama
    @tanggalAwal DATE = NULL,      -- Menambahkan parameter untuk tanggal awal
    @tanggalAkhir DATE = NULL      -- Menambahkan parameter untuk tanggal akhir
AS
BEGIN
    DECLARE @sqlQuery NVARCHAR(MAX)

    SET @sqlQuery = '
        SELECT 
            p.KodePegawai,
            p.NamaPegawai,
            c.KodeCabang,
            c.NamaCabang,
            j.KodeJabatan,
            j.NamaJabatan,
            p.TanggalMulaiKontrak,
            p.TanggalHabisKontrak,
            -- Menambahkan kolom status berdasarkan TanggalHabisKontrak
            CASE
                WHEN CAST(p.TanggalHabisKontrak AS DATE) >= CAST(GETDATE() AS DATE) THEN ''Aktif''
                ELSE ''Non Aktif''
            END AS Status
        FROM 
            Pegawais p
        INNER JOIN 
            Cabangs c ON p.KodeCabang = c.KodeCabang
        INNER JOIN 
            Jabatans j ON p.KodeJabatan = j.KodeJabatan
        WHERE 1 = 1 ' -- Kondisi tambahan untuk fleksibilitas filter

    -- Jika parameter nama diberikan, tambahkan filter berdasarkan nama
    IF @nama IS NOT NULL
    BEGIN
        SET @sqlQuery = @sqlQuery + ' AND p.NamaPegawai LIKE ''%' + @nama + '%'''
    END

    -- Jika parameter tanggalAwal diberikan, tambahkan filter berdasarkan tanggal mulai kontrak
    IF @tanggalAwal IS NOT NULL
    BEGIN
        SET @sqlQuery = @sqlQuery + ' AND CAST(p.TanggalMulaiKontrak AS DATE) >= @tanggalAwal'
    END

    -- Jika parameter tanggalAkhir diberikan, tambahkan filter berdasarkan tanggal habis kontrak
    IF @tanggalAkhir IS NOT NULL
    BEGIN
        SET @sqlQuery = @sqlQuery + ' AND CAST(p.TanggalHabisKontrak AS DATE) <= @tanggalAkhir'
    END

    -- Menjalankan query dinamis
    EXEC sp_executesql @sqlQuery, N'@tanggalAwal DATE, @tanggalAkhir DATE', @tanggalAwal, @tanggalAkhir
END
GO


