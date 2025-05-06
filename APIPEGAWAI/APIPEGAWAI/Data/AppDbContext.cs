using APIPEGAWAI.Models;
using Microsoft.EntityFrameworkCore;

namespace APIPEGAWAI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Cabang> Cabangs { get; set; }
        public DbSet<Jabatan> Jabatans { get; set; }
        public DbSet<Pegawai> Pegawais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cabang>().HasKey(c => c.KodeCabang);

            modelBuilder.Entity<Jabatan>().HasKey(j => j.KodeJabatan);

            modelBuilder.Entity<Pegawai>().HasKey(p => p.KodePegawai);


            modelBuilder.Entity<Pegawai>()
        .HasOne(p => p.Cabang)
        .WithMany()
        .HasForeignKey(p => p.KodeCabang)
        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pegawai>()
                .HasOne(p => p.Jabatan)
                .WithMany()
                .HasForeignKey(p => p.KodeJabatan)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
