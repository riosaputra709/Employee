using APIPEGAWAI.Models;
using APIPEGAWAI.Models.Request;
using APIPEGAWAI.Models.Response;

namespace APIPEGAWAI.Services
{
    public interface IPegawaiService
    {
        Task<IEnumerable<PegawaiDetailDto>> GetAllPegawaiEF();
        Task<IEnumerable<PegawaiDetailDto>> GetAllPegawaiSP(string? nama, DateTime? tanggalAwal, DateTime? tanggalAkhir);
        Task<string> UploadDataPegawai(IFormFile file);
        Task<List<Pegawai>?> DeletePegawai(string kodePegawai);
        Task<Pegawai?> UpdatePegawai(string id, PegawaiRequest request);
        Task<Pegawai?> GetSinglePegawai(string id);
    }
}
