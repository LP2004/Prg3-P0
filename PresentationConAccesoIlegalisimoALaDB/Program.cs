using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresentationConAccesoIlegalisimoALaDB
{
    class Program
    {
        static void Main(string[] args)
        {
            bool repetir = true;
            do {
                MostrarOpciones();

                int opcionElegida = PreguntarIntYExplotarSiNoEsInt("Elige la opcion");
                switch (opcionElegida) {
                    case 0:
                        AddAutor();
                        break;
                    case 1:
                        AddLibro();
                        break;
                    case 2:
                        ModificarVentas();
                        break;
                    case 3:
                        ConsultarLibrosDisponibles();
                        break;
                    case 4:
                        ConsultarAutores();
                        break;
                    case 5:
                        EliminarLibro();
                        break;
                }

                repetir = !PreguntarBool("\nQuieres salir?");
            } while (repetir) ;
        }

        //hardcode feo, pero como es cortito el programa lo dejo así
        public static void MostrarOpciones()
        {
            Console.WriteLine(
                "0 - Insertar un nuevo autor" +
                "\n1 - Insertar un nuevo libro" +
                "\n2 - Modificar ventas de libro desde identificador" +
                "\n3 - Obtener libros disponibles" +
                "\n4 - Obtener autores en orden alfabetico" +
                "\n5 - Borrar libro desde identificador\n"
            );
        }

        public static bool PreguntarBool(String pregunta)
        {
            Console.WriteLine(pregunta + "\nS - N");
            String respuesta = Console.ReadLine();
            return respuesta.ToUpper().Equals("S");
        }

        public static int PreguntarIntYExplotarSiNoEsInt(String pregunta)
        {
            Console.WriteLine(pregunta + "\n");
            String respuesta = Console.ReadLine();
            return int.Parse(respuesta);
        }

        public static void AddAutor()
        {
            Console.WriteLine("Ingresa el nombre del autor");
            string nombre = Console.ReadLine();

            using (var context = new LibrosDB())
            {
                using (var transfer = context.Database.BeginTransaction())
                {
                    Autores autor = new Autores()
                    {
                        Nombre = nombre
                    };

                    context.Autores.Add(autor);

                    context.SaveChanges();

                    transfer.Commit();
                }
            }
        }

        public static void AddLibro()
        {
            int idAutor = PreguntarIntYExplotarSiNoEsInt("Ingresa el id del autor");
            using (var context = new LibrosDB())
            {
                if (!context.Autores.Any(autor => autor.Id == idAutor))
                {
                    Console.WriteLine("El id del autor es inválido");
                    return;
                }
            }
            Console.WriteLine("Ingresa el título del libro");
            string titulo = Console.ReadLine(); // nota: no estoy verificando que tenga un máximo de 256 caracteres
            int anioPublicacion = PreguntarIntYExplotarSiNoEsInt("Ingresa el año de publicación");
            int cantidadVentas = PreguntarIntYExplotarSiNoEsInt("Ingresa la cantidad de ventas");

            using (var context = new LibrosDB())
            {
                using (var transfer = context.Database.BeginTransaction())
                {
                    Libros libro = new Libros()
                    {
                        IdAutor = idAutor,
                        Titulo = titulo,
                        AnioPublicacion = anioPublicacion,
                        CantidadDeVentas = cantidadVentas
                    };

                    context.Libros.Add(libro);

                    context.SaveChanges();

                    transfer.Commit();
                }
            }
        }

        public static void ModificarVentas()
        {
            int idLibro = PreguntarIntYExplotarSiNoEsInt("Ingresa el id del libro");
            if (!ExisteLibro(idLibro))
            {
                Console.WriteLine("El id del libro es inválido");
                return;
            }
            int cantidadVentas = PreguntarIntYExplotarSiNoEsInt("Ingresa la cantidad de ventas");

            using (var context = new LibrosDB())
            {
                using (var transfer = context.Database.BeginTransaction())
                {

                    Libros libro = context.Libros.FirstOrDefault(element => element.Id == idLibro);

                    libro.CantidadDeVentas = cantidadVentas;

                    context.SaveChanges();

                    transfer.Commit();
                }
            }
        }

        public static void ConsultarLibrosDisponibles()
        {
            List<Libros> libros = null;

            using (var context = new LibrosDB())
            {
                libros = context.Libros
                    .Include("Autores")
                    .OrderByDescending(libro => libro.CantidadDeVentas)
                    .ToList();
            }

            foreach (var libro in libros)
                Console.WriteLine($"\n{libro.Titulo} - Año de publicación: {libro.AnioPublicacion} - Cantidad de ventas: {libro.CantidadDeVentas} - Por: {libro.Autores.Nombre}");
        }

        public static void ConsultarAutores()
        {
            List<Autores> autores = null;

            using (var context = new LibrosDB())
            {
                autores = context.Autores
                    .OrderBy(autor => autor.Nombre)
                    .ToList();
            }

            foreach (var autor in autores)
                Console.WriteLine($"\nNombre: {autor.Nombre} - Id: {autor.Id}");
        }

        public static void EliminarLibro()
        {
            int idLibro = PreguntarIntYExplotarSiNoEsInt("Ingresa el id del libro");
            if (!ExisteLibro(idLibro))
            {
                Console.WriteLine("El id del libro es inválido");
                return;
            }

            using (var context = new LibrosDB())
            {
                using (var transfer = context.Database.BeginTransaction())
                {

                    Libros libro = context.Libros.FirstOrDefault(element => element.Id == idLibro);

                    context.Libros.Remove(libro);

                    context.SaveChanges();

                    transfer.Commit();
                }
            }
        }

        public static bool ExisteLibro(int id)
        {
            using (var context = new LibrosDB())
            {
                return context.Autores.Any(libro => libro.Id == id);
            }
        }
    }
}
