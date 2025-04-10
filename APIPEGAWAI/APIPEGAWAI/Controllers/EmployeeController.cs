using APIPEGAWAI.Data;
using APIPEGAWAI.Models;
using APIPEGAWAI.Services;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace APIPEGAWAI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IPegawaiService _pegawaiService;
        private readonly AppDbContext _context;

        public EmployeeController(IPegawaiService pegawaiService, AppDbContext context)
        {
            _pegawaiService = pegawaiService;
            _context = context;
        }

        [Route("GetPegawaiDetailsEF")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PegawaiDetailDto>>> GetPegawaiDetailsEF()
        {
            var result = await _pegawaiService.GetAllPegawaiEF();
            return Ok(result);
        }

        [Route("GetPegawaiDetailsSP")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PegawaiDetailDto>>> GetPegawaiDetailsSP(
            [FromQuery] string? name,
            [FromQuery] DateTime? tanggalAwal,
            [FromQuery] DateTime? tanggalAkhir)
        {
            if (tanggalAwal != null && tanggalAkhir != null)
            {
                if(tanggalAwal > tanggalAkhir)
                {
                    return BadRequest("Tanggal awal kontrak tidak boleh lebih besar dari tanggal habis kontrak.");
                }
            }

            var result = await _pegawaiService.GetAllPegawaiSP(name, tanggalAwal, tanggalAkhir);
            return Ok(result);
        }



        [HttpPost("UploadDataPegawai")]
        public async Task<IActionResult> UploadDataPegawai(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File tidak ditemukan");

                var result = await _pegawaiService.UploadDataPegawai(file);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Menangani error lain (misalnya masalah koneksi database)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeletePegawai/{id}")]
        public async Task<ActionResult> DeletePegawai(string id)
        {
            var result = await _pegawaiService.DeletePegawai(id);

            if (result == null)
                return NotFound("Pegawai not found.");
            return Ok("Delete Successfully");
        }

        [HttpPut("UpdatePegawai/{id}")]
        public async Task<ActionResult<List<Pegawai>>> UpdatePegawai(string id, Pegawai request)
        {
            try
            {
                var result = await _pegawaiService.UpdatePegawai(id, request);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetSinglePegawai/{id}")]
        public async Task<ActionResult<Pegawai>> GetSingleProduct(string id)
        {
            var result = await _pegawaiService.GetSinglePegawai(id);
            if (result == null)
                return NotFound("Product Not Found.");
            return Ok(result);
        }

    }
}
