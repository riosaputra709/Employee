using System.ComponentModel.DataAnnotations;

namespace APIPEGAWAI.Models
{
    public class Pegawai
    {
        [Key]
        public string KodePegawai { get; set; }

        public string NamaPegawai { get; set; }

        public DateTime TanggalMulaiKontrak { get; set; }
        public DateTime TanggalHabisKontrak { get; set; }

        public string KodeCabang { get; set; }
        public string KodeJabatan { get; set; }
    }
}
