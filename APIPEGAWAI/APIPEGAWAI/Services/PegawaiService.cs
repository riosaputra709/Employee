using APIPEGAWAI.Data;
using APIPEGAWAI.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace APIPEGAWAI.Services
{
    public class PegawaiService : IPegawaiService
    {
        private readonly AppDbContext _context;
        public PegawaiService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PegawaiDetailDto>> GetAllPegawaiEF()
        {
            var result = await (from p in _context.Pegawais
                                join c in _context.Cabangs on p.KodeCabang equals c.KodeCabang
                                join j in _context.Jabatans on p.KodeJabatan equals j.KodeJabatan
                                select new PegawaiDetailDto
                                {
                                    KodePegawai = p.KodePegawai,
                                    NamaPegawai = p.NamaPegawai,
                                    KodeCabang = c.KodeCabang,
                                    NamaCabang = c.NamaCabang,
                                    KodeJabatan = j.KodeJabatan,
                                    NamaJabatan = j.NamaJabatan,
                                    TanggalMulaiKontrak = p.TanggalMulaiKontrak,
                                    TanggalHabisKontrak = p.TanggalHabisKontrak
                                }).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<PegawaiDetailDto>> GetAllPegawaiSP(string? nama, DateTime? tanggalAwal, DateTime? tanggalAkhir)
        {
            // Menggunakan FromSqlRaw dengan parameter untuk menjalankan stored procedure
            var result = await _context.PegawaiDetailDtos
                .FromSqlRaw("EXEC GetPegawaiDetails @nama = {0}, @tanggalAwal = {1}, @tanggalAkhir = {2}",
                    nama ?? (object)DBNull.Value,
                    tanggalAwal ?? (object)DBNull.Value,
                    tanggalAkhir ?? (object)DBNull.Value)
                .ToListAsync();

            return result;
        }


        public async Task<string> UploadDataPegawai(IFormFile file)
        {
            var pegawaiList = new List<Pegawai>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new OfficeOpenXml.ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var kodePegawai = worksheet.Cells[row, 1].Text;
                        var kodeCabang = worksheet.Cells[row, 3].Text;
                        var kodeJabatan = worksheet.Cells[row, 4].Text;
                        var tanggalAwal = DateTime.Parse(worksheet.Cells[row, 5].Text);
                        var tanggalAkhir = DateTime.Parse(worksheet.Cells[row, 6].Text);

                        if (tanggalAwal > tanggalAkhir)
                        {
                            throw new ArgumentException("Tanggal awal kontrak baris " + row + " tidak boleh lebih besar dari tanggal habis kontrak.");
                        }

                        var cabang = await _context.Cabangs.FirstOrDefaultAsync(c => c.KodeCabang == kodeCabang);
                        var jabatan = await _context.Jabatans.FirstOrDefaultAsync(j => j.KodeJabatan == kodeJabatan);

                        // Cek apakah Cabang dan Jabatan ditemukan, jika tidak throw error atau buat entitas baru
                        if (cabang == null || jabatan == null)
                        {
                            throw new ArgumentException("Kode Cabang atau Jabatan pada baris ke " + row + " tidak ditemukan untuk kode yang diberikan.");
                        }

                        var pegawai = await _context.Pegawais.FirstOrDefaultAsync(p => p.KodePegawai == kodePegawai);
                        if (pegawai != null)
                        {
                            throw new ArgumentException("Kode Pegawai yang diinput pada baris " + row + " sudah tersedia.");
                        }

                        var kontrak = new Pegawai
                        {
                            KodePegawai = kodePegawai,
                            NamaPegawai = worksheet.Cells[row, 2].Text,
                            KodeCabang = kodeCabang,
                            KodeJabatan = kodeJabatan,
                            TanggalMulaiKontrak = tanggalAwal,
                            TanggalHabisKontrak = tanggalAkhir
                        };

                        pegawaiList.Add(kontrak);
                    }

                    await _context.Pegawais.AddRangeAsync(pegawaiList);
                    await _context.SaveChangesAsync();
                }
            }

            return $"{pegawaiList.Count} data berhasil diupload.";
        }

        public async Task<List<Pegawai>?> DeletePegawai(string kdPegawai)
        {
            var pegawai = await _context.Pegawais.FindAsync(kdPegawai);
            if (pegawai == null)
                return null;

            _context.Pegawais.Remove(pegawai);
            await _context.SaveChangesAsync();
            return await _context.Pegawais.ToListAsync();
        }

        public async Task<Pegawai> UpdatePegawai(string id, Pegawai request)
        {
            var result = await _context.Pegawais.FindAsync(id);
            if (result == null)
                throw new ArgumentException("Id tidak ditemukan"); 

            var tanggalAwal = DateTime.Parse(request.TanggalMulaiKontrak.ToString());
            var tanggalAkhir = DateTime.Parse(request.TanggalHabisKontrak.ToString());
            if (tanggalAwal > tanggalAkhir)
            {
                throw new ArgumentException("Tanggal awal kontrak tidak boleh lebih besar dari tanggal habis kontrak.");
            }

            var cabang = await _context.Cabangs.FirstOrDefaultAsync(c => c.KodeCabang == request.KodeCabang);
            var jabatan = await _context.Jabatans.FirstOrDefaultAsync(j => j.KodeJabatan == request.KodeJabatan);

            // Cek apakah Cabang dan Jabatan ditemukan, jika tidak throw error atau buat entitas baru
            if (cabang == null || jabatan == null)
            {
                throw new ArgumentException("Kode Cabang atau Jabatan tidak ditemukan untuk kode yang diberikan.");
            }


            result.NamaPegawai = request.NamaPegawai;
            result.KodeJabatan = request.KodeJabatan;
            result.KodeCabang = request.KodeCabang;
            result.TanggalMulaiKontrak = request.TanggalMulaiKontrak;
            result.TanggalHabisKontrak = request.TanggalHabisKontrak;

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<Pegawai?> GetSinglePegawai(string id)
        {
            var result = await _context.Pegawais.FindAsync(id);
            if (result == null)
                return null;
            return result;
        }
    }
}
