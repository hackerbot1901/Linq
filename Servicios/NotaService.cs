using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;


public class NotaService

{
    const string CADENA_CONEXION = "Data Source=DESKTOP-11O6E15\\MYSQLSERVER;Initial Catalog=categoria;Integrated Security=True";

    private List<Nota> notasCollection = new List<Nota>();
    public NotaService(DataSet notasData)
    {
        //LINQ
        notasCollection = notasData.Tables["nota"].AsEnumerable().Select(nota => new Nota
        (
            Convert.ToInt64(nota["id_nota"]),
            Convert.ToString(nota["descripcion_nota"]),
            Convert.ToDateTime(nota["fecha_creacion"]),
            Convert.ToInt64(nota["id_categoria"]),
            Convert.ToInt64(nota["id_cliente"])
        )).ToList(); ;
    }
    internal void mostrarNotas()
    {
        //LINQ
        var query = from nota in notasCollection select nota;

        foreach (var nota in query)
        {
            Console.WriteLine(nota.ToString());
        }
    }
    public Nota crearNota()
    {
       
        Console.WriteLine("\n***  NUEVA NOTA  ***\n");
        Console.Write("\nINGRESA UNA DESCRIPCION: ");
        string descripcion_nota = Convert.ToString(Console.ReadLine());
        var fecha_creacion = DateTime.Now;
        long fkCategoriaValido = verificarExistenciaCategoria();
        long fkClienteValidado = verificarExistenciaCliente();
        Nota nuevaNota = new Nota()
        {
            descripcionNota = descripcion_nota,
            fechaCreacion = fecha_creacion,
            fkCategoria = fkCategoriaValido,
            fkCliente = fkClienteValidado
        };
        crearEnBaseDatos(nuevaNota);
        Console.WriteLine("NOTA CREADA CON EXITO");
        return nuevaNota;

    }
    internal void mostrarFiltrosLinq()
    {
        var salir = false;

        do
        {
            Console.WriteLine("---- MENÚ ----");
            Console.WriteLine("1. Filtrar por categoria");
            Console.WriteLine("2. Ordenar por Fecha de Creación");
            Console.WriteLine("3. Contar Notas por Categoría");
            Console.WriteLine("4. Obtener la Última Nota Creada");
            Console.WriteLine("5. Salir");
            Console.Write("Ingrese la opción deseada: ");

            if (int.TryParse(Console.ReadLine(), out int opcion))
            {
                switch (opcion)
                {
                    case 1:
                        filtrarPorCategoria();
                        break;
                    case 2:
                        ordenarPorFechaCreacion();
                        break;
                    case 3:
                        contarNotasPorCategoria();                        
                        break;
                    case 4:
                        obtenerUltimaNota();
                        break;
                    case 5:
                        salir = true;
                        Console.WriteLine("Saliendo del programa...");
                        break;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Ingrese un número válido.");
            }

            Console.WriteLine("\nPresione una tecla para continuar...");
            Console.ReadKey();
            Console.Clear(); // Limpiar la consola antes de mostrar el menú nuevamente

        } while (!salir);

    }
    private void obtenerUltimaNota()
    {
        var ultimaNota = notasCollection.OrderByDescending(nota => nota.fechaCreacion).FirstOrDefault();
        if (ultimaNota != null)
        {
            Console.WriteLine($"Última nota creada - ID: {ultimaNota.id}, Descripción: {ultimaNota.descripcionNota}, Fecha: {ultimaNota.fechaCreacion}");
        }
        else
        {
            Console.WriteLine("No hay notas disponibles.");
        }
    }
    private void contarNotasPorCategoria()
    {
        var idCategoria = verificarExistenciaCategoria();
        {
            var cantidadNotas = notasCollection.Count(nota => nota.fkCategoria == idCategoria);
            Console.WriteLine($"Cantidad de notas para la categoría {idCategoria}: {cantidadNotas}");
        }
    }
    private void ordenarPorFechaCreacion()
    {
        var notasOrdenadas = notasCollection.OrderBy(nota => nota.fechaCreacion);
        foreach (var nota in notasOrdenadas)
        {
            Console.WriteLine($"ID: {nota.id}, Descripción: {nota.descripcionNota}, Fecha: {nota.fechaCreacion}");
        }
    }
    private void filtrarPorCategoria()
    {
        var idCategoria = verificarExistenciaCategoria();
        var notasFiltradas = notasCollection.Where(nota => nota.fkCategoria == idCategoria);
        foreach (var nota in notasFiltradas)
        {
            Console.WriteLine($"ID: {nota.id}, Descripción: {nota.descripcionNota}, Fecha: {nota.fechaCreacion}");
        }
    }
    private static void crearEnBaseDatos(Nota nota)
    {
        using (SqlConnection connection = new SqlConnection(CADENA_CONEXION))
        {
            connection.Open();
            // Aquí puedes realizar operaciones en la base de datos
            string insertQuery = "INSERT INTO Nota (" +
            "descripcion_nota, " +
            "fecha_creacion, " +
            "id_categoria, " +
            "id_cliente) VALUES (@valor1, @valor2, @valor3, @valor4)";

            using (SqlCommand command = new SqlCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@valor1", nota.descripcionNota);
                command.Parameters.AddWithValue("@valor2", nota.fechaCreacion);
                command.Parameters.AddWithValue("@valor3", nota.fkCategoria);
                command.Parameters.AddWithValue("@valor4", nota.fkCliente);
                command.ExecuteNonQuery();
            }
        }
    }
    internal void eliminarNota()
    {
        mostrarNotas();
        Console.WriteLine("\n***  ELIMINAR NOTA  ***\n");
        Nota notaAEliminar = obtenerNota();
        eliminarNotaEnBaseDatos(notaAEliminar);
        notasCollection.Remove(notaAEliminar);
        mostrarNotas();

    }
    private static void eliminarNotaEnBaseDatos(Nota notaAEliminar)
    {
        using (SqlConnection connection = new SqlConnection(CADENA_CONEXION))
        {
            connection.Open();
            string deleteQuery = "DELETE FROM Nota WHERE id_nota = @id";
            using (SqlCommand command = new SqlCommand(deleteQuery, connection))
            {
                command.Parameters.AddWithValue("@id", notaAEliminar.id);
                command.ExecuteNonQuery();
            }
        }
    }
    internal void modificarNotas()
    {
        mostrarNotas();
        Console.WriteLine("\n*** MODIFICAR NOTA ***\n");

        Nota notaModificada = obtenerNota();

        bool entradaValida = false;
        while (!entradaValida)
        {
            try
            {
                Console.Write("\nINGRESA UNA DESCRIPCION: ");
                string descripcion_nota = Convert.ToString(Console.ReadLine());

                Console.Write("\nINGRESA UNA NUEVA FECHA (yyyy-MM-dd HH:mm:ss): ");
                var fecha_creacion = Convert.ToDateTime(Console.ReadLine());
                notaModificada.descripcionNota = descripcion_nota;
                notaModificada.fechaCreacion = fecha_creacion;
                modificarNotaEnBaseDatos(notaModificada);
                mostrarNotas();
                entradaValida = true;
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Formato de entrada incorrecto.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Error: El valor ingresado es demasiado grande o demasiado pequeño.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

    }
    private static void modificarNotaEnBaseDatos(Nota notaModificada)
    {
        string updateQuery = "UPDATE Nota SET descripcion_nota = @descripcion_nota," +
                    "fecha_creacion = @fecha_creacion " +
                    "WHERE id_nota = @id";

        using (SqlConnection connection = new SqlConnection(CADENA_CONEXION))
        {
            try
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@descripcion_nota", notaModificada.descripcionNota);

                    DateTime fechaCreacion;
                    if (DateTime.TryParse(notaModificada.fechaCreacion.ToString(), out fechaCreacion))
                    {
                        command.Parameters.AddWithValue("@fecha_creacion", fechaCreacion);
                    }
                    else
                    {
                        throw new ArgumentException("Formato de fecha inválido.");
                    }

                    command.Parameters.AddWithValue("@id", notaModificada.id);

                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al actualizar la base de datos: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
            }
        }
    }
    private long verificarExistenciaCategoria()
    {
        bool entradaValida = false;
        long fkCategoriaValido = -1;

        while (!entradaValida)
        {
            try
            {
                Console.Write("\nINGRESA UNA CATEGORIA EXISTENTE: ");
                fkCategoriaValido = Convert.ToInt64(Console.ReadLine());
                var existeCategoria = notasCollection.Any(nota => nota.fkCategoria.Equals(fkCategoriaValido));
                if (existeCategoria)
                {
                    entradaValida = true;
                }
                else
                {
                    Console.WriteLine("NO EXISTE ESA CATEGORIA!!!\n");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Debes ingresar un número válido.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Error: El número ingresado es demasiado grande o demasiado pequeño.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
        return fkCategoriaValido;
    }
    private long verificarExistenciaCliente()
    {
        bool entradaValida = false;
        long fkClienteValidado = -1;

        while (!entradaValida)
        {
            try
            {
                Console.Write("\nINGRESA EL ID DE UN CLIENTE EXISTENTE: ");
                fkClienteValidado = Convert.ToInt64(Console.ReadLine());

                var existeIdClient = notasCollection.Any(nota => nota.fkCliente.Equals(fkClienteValidado));
                if (existeIdClient)
                {
                    entradaValida = true;
                }
                else
                {
                    Console.WriteLine("NO EXISTE ESE ID DEL CLIENTE!!!\n");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Debes ingresar un número válido.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Error: El número ingresado es demasiado grande o demasiado pequeño.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        return fkClienteValidado;
    }
    private Nota obtenerNota()
    {
        Nota notaEncontrada = null;

        bool entradaValida = false;
        while (!entradaValida)
        {
            try
            {
                Console.Write("ESCOGER LA NOTA: ");
                long id_nota = Convert.ToInt64(Console.ReadLine());
                var existeId = notasCollection.Any(nota => nota.id.Equals(id_nota));
                if (existeId)
                {
                    notaEncontrada = notasCollection.FirstOrDefault(nota => nota.id == id_nota);
                    if (notaEncontrada != null)
                    {
                        entradaValida = true;
                    }
                    else
                    {
                        Console.WriteLine("NO EXISTE ESE ID!!!\n");
                    }
                }
                else
                {
                    Console.WriteLine("NO EXISTE ESE ID!!!\n");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Error: Debes ingresar un número válido.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("Error: El número ingresado es demasiado grande o demasiado pequeño.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        return notaEncontrada;
    }
}