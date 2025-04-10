using APIPEGAWAI.Models;
using Microsoft.EntityFrameworkCore;

namespace APIPEGAWAI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<PegawaiDetailDto> PegawaiDetailDtos { get; set; }

        public DbSet<Cabang> Cabangs { get; set; }
        public DbSet<Jabatan> Jabatans { get; set; }
        public DbSet<Pegawai> Pegawais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cabang>().HasKey(c => c.KodeCabang);

            modelBuilder.Entity<Jabatan>().HasKey(j => j.KodeJabatan);

            modelBuilder.Entity<Pegawai>().HasKey(p => p.KodePegawai);

            modelBuilder.Entity<PegawaiDetailDto>().HasNoKey();
        }
    }
}
