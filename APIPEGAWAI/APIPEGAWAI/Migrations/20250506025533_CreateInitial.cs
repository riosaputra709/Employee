using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIPEGAWAI.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cabangs",
                columns: table => new
                {
                    KodeCabang = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NamaCabang = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cabangs", x => x.KodeCabang);
                });

            migrationBuilder.CreateTable(
                name: "Jabatans",
                columns: table => new
                {
                    KodeJabatan = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NamaJabatan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jabatans", x => x.KodeJabatan);
                });

            migrationBuilder.CreateTable(
                name: "Pegawais",
                columns: table => new
                {
                    KodePegawai = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NamaPegawai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TanggalMulaiKontrak = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TanggalHabisKontrak = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KodeCabang = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    KodeJabatan = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pegawais", x => x.KodePegawai);
                    table.ForeignKey(
                        name: "FK_Pegawais_Cabangs_KodeCabang",
                        column: x => x.KodeCabang,
                        principalTable: "Cabangs",
                        principalColumn: "KodeCabang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pegawais_Jabatans_KodeJabatan",
                        column: x => x.KodeJabatan,
                        principalTable: "Jabatans",
                        principalColumn: "KodeJabatan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pegawais_KodeCabang",
                table: "Pegawais",
                column: "KodeCabang");

            migrationBuilder.CreateIndex(
                name: "IX_Pegawais_KodeJabatan",
                table: "Pegawais",
                column: "KodeJabatan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pegawais");

            migrationBuilder.DropTable(
                name: "Cabangs");

            migrationBuilder.DropTable(
                name: "Jabatans");
        }
    }
}
