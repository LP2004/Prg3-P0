namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Libros
    {
        public long Id { get; set; }

        public long? IdAutor { get; set; }

        [Required]
        [StringLength(256)]
        public string Titulo { get; set; }

        public int AnioPublicacion { get; set; }

        public int CantidadDeVentas { get; set; }

        public virtual Autores Autores { get; set; }
    }
}
