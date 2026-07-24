using asistenciaBack.Attendance.Domain.Entities;
using asistenciaBack.Identity.Domain.Entities;
using asistenciaBack.Membership.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace asistenciaBack.Database.Configurations;

public class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.MemberId)
            .IsRequired();

        builder.Property(x => x.Date)
            .IsRequired();

        // Persistido como entero: Present=1, Late=2, Absent=3
        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(100);

        builder.Property(x => x.RegisteredByUserId)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => new { x.MemberId, x.Date })
            .IsUnique();

        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.RegisteredByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
