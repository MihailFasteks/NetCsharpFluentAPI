using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetCsharpFluentAPI2
{
   
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        [Required]
        [ForeignKey("JobIdent")]
        public Job Job { get; set; }

    }
    public class Job
    {
        public Job()
        {
            Employee = new HashSet<Employee>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Employee> Employee { get; set; }
    }
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Login {  get; set; }
        public string Password {  get; set; }
    }
    class FluentContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<User> Users { get; set; }

        public FluentContext()
        {
           
            Database.EnsureCreated();


        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=FluentApi;Integrated Security=SSPI;TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Установим связь Один ко Многим между объектом AcademyGroup и объектами Student 

            modelBuilder.Entity<Employee>().HasOne(p => p.Job).WithMany(t => t.Employee).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.Login).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Password).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.Password).HasMaxLength(20);
            base.OnModelCreating(modelBuilder);
        }
       
    }
}
