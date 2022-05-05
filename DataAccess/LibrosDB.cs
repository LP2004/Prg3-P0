using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DataAccess
{
    public partial class LibrosDB : DbContext
    {
        public LibrosDB()
            : base("name=LibrosDB")
        {
        }

        public virtual DbSet<Autores> Autores { get; set; }
        public virtual DbSet<Libros> Libros { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Autores>()
                .HasMany(e => e.Libros)
                .WithOptional(e => e.Autores)
                .HasForeignKey(e => e.IdAutor);
        }
    }
}
