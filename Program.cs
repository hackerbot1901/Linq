using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System;


const string CADENA_CONEXION = "Data Source=DESKTOP-11O6E15\\MYSQLSERVER;Initial Catalog=categoria;Integrated Security=True";
List<DataSet> GetData()
{
    SqlConnection connection = new SqlConnection(CADENA_CONEXION);
    List<DataSet> datasets = new List<DataSet>();
    string query = "select * from";
    string[] tb = { "nota", "categoria", "cliente" };

    for (int i = 0; i < tb.Length; i++)
    {
        SqlDataAdapter adapter = new SqlDataAdapter(query + " " + tb[i], connection);
        DataSet dataset = new DataSet();
        adapter.Fill(dataset, tb[i]);
        datasets.Add(dataset);
    }
    return datasets;
}

var data = GetData();
NotaService servicioNotas = new NotaService(data[0]);
int opcion;
do
{
    Console.WriteLine("------ MENÚ DE OPCIONES ------");
    Console.WriteLine("1. Mostrar notas");
    Console.WriteLine("2. Crear nota");
    Console.WriteLine("3. Modificar nota");
    Console.WriteLine("4. Eliminar nota");
    Console.WriteLine("5. Filtros utilizando LINQ");
    Console.WriteLine("6. Salir");

    Console.Write("Seleccione una opción: ");
    if (int.TryParse(Console.ReadLine(), out opcion))
    {
        switch (opcion)
        {
            case 1:
                servicioNotas.mostrarNotas();
                break;
            case 2:
                servicioNotas.crearNota();
                break;
            case 3:
                servicioNotas.modificarNotas();
                break;
            case 4:
                servicioNotas.eliminarNota();
                break;
            case 5:
                servicioNotas.mostrarFiltrosLinq();
                break;
            case 6:
                Console.WriteLine("Saliendo de la aplicación...");
                break;
            default:
                Console.WriteLine("Opción no válida. Por favor, seleccione una opción válida.");
                break;
        }
    }
    else
    {
        Console.WriteLine("Entrada no válida. Ingrese un número correspondiente a una opción del menú.");
    }
    Console.WriteLine("\nPresione cualquier tecla para continuar...");
    Console.ReadKey();
    Console.Clear();
} while (opcion != 6);
