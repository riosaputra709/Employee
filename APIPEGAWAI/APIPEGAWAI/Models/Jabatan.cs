using System.ComponentModel.DataAnnotations;

namespace APIPEGAWAI.Models
{
    public class Jabatan
    {
        [Key]
        public string KodeJabatan { get; set; }
        public string NamaJabatan { get; set; }

        public ICollection<Pegawai> Pegawais { get; set; }
    }
}
