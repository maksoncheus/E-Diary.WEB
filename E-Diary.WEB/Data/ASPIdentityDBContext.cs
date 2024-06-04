using E_Diary.WEB.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using E_Diary.WEB.Models;
using System.Reflection.Emit;

namespace E_Diary.WEB.Data
{
    public class ASPIdentityDBContext : IdentityDbContext
    {
        new public DbSet<User> Users { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<StudyYear> StudyYears { get; set; }
        public DbSet<PeriodGrade> PeriodGrades { get; set; }
        public DbSet<YearGrade> YearGrades { get; set; }
        public DbSet<CertificationPeriod> CertificationPeriods { get; set; }
        public DbSet<TeacherGroupSubject> TeacherGroupSubjects { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public ASPIdentityDBContext(DbContextOptions<ASPIdentityDBContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().Property(x => x.Gender).HasConversion<int>();
            builder.Entity<Lesson>().ToTable(t => 
                t.HasCheckConstraint(
                    "ValidLessonNumber",
                    "LessonOnDayNumber > 0 AND LessonOnDayNumber < 11")
                );
            builder.Entity<Parent>().HasMany(p => p.Children).WithMany(c => c.Parents);
        }
    }
}
