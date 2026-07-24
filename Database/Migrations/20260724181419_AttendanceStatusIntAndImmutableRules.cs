using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace asistenciaBack.Database.Migrations
{
    /// <inheritdoc />
    public partial class AttendanceStatusIntAndImmutableRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Elimina asistencias históricas que no son sábado (DOW: 0=domingo ... 6=sábado).
            migrationBuilder.Sql("""
                DELETE FROM "AttendanceRecords"
                WHERE EXTRACT(DOW FROM "Date") <> 6;
                """);

            migrationBuilder.Sql("""
                ALTER TABLE "AttendanceRecords" ADD COLUMN "StatusInt" integer NULL;

                UPDATE "AttendanceRecords"
                SET "StatusInt" = CASE
                    WHEN "Status" IN ('Present', 'Presente', '1') THEN 1
                    WHEN "Status" IN ('Late', 'Tarde', '2') THEN 2
                    WHEN "Status" IN ('Absent', 'Ausente', '3') THEN 3
                    ELSE 1
                END;

                ALTER TABLE "AttendanceRecords" DROP COLUMN "Status";
                ALTER TABLE "AttendanceRecords" RENAME COLUMN "StatusInt" TO "Status";
                ALTER TABLE "AttendanceRecords" ALTER COLUMN "Status" SET NOT NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "AttendanceRecords" ADD COLUMN "StatusText" character varying(20) NULL;

                UPDATE "AttendanceRecords"
                SET "StatusText" = CASE "Status"
                    WHEN 1 THEN 'Present'
                    WHEN 2 THEN 'Late'
                    WHEN 3 THEN 'Absent'
                    ELSE 'Present'
                END;

                ALTER TABLE "AttendanceRecords" DROP COLUMN "Status";
                ALTER TABLE "AttendanceRecords" RENAME COLUMN "StatusText" TO "Status";
                ALTER TABLE "AttendanceRecords" ALTER COLUMN "Status" SET NOT NULL;
                """);
        }
    }
}
