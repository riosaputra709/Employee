namespace APIPEGAWAI.Models
{
    public class PegawaiDetailDto
    {
        public string KodePegawai { get; set; }
        public string NamaPegawai { get; set; }
        public string KodeCabang { get; set; }
        public string NamaCabang { get; set; }
        public string KodeJabatan { get; set; }
        public string NamaJabatan { get; set; }
        public string Status { get; set; }
        public DateTime TanggalMulaiKontrak { get; set; }
        public DateTime TanggalHabisKontrak { get; set; }
    }

}
