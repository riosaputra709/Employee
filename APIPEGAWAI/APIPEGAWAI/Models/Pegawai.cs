using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIPEGAWAI.Models
{
    public class Pegawai
    {
        [Key]
        public string KodePegawai { get; set; }
        public string NamaPegawai { get; set; }

        public DateTime TanggalMulaiKontrak { get; set; }
        public DateTime TanggalHabisKontrak { get; set; }

        [ForeignKey("Cabang")]
        public string KodeCabang { get; set; }

        [ForeignKey("Jabatan")]
        public string KodeJabatan { get; set; }

        public Cabang Cabang { get; set; }
        public Jabatan Jabatan { get; set; }
    }
}
