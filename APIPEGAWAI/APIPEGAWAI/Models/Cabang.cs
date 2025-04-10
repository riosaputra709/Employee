using System.ComponentModel.DataAnnotations;

namespace APIPEGAWAI.Models
{
    public class Cabang
    {
        [Key]
        public string KodeCabang { get; set; }

        public string NamaCabang { get; set; }

    }
}
