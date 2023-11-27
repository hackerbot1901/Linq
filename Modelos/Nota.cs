using System;

public class Nota
{
    public long id { get; set; }
    public string descripcionNota { get; set; }
    public DateTime fechaCreacion { get; set; }
    public long fkCategoria { get; set; }
    public long fkCliente { get; set; }

    public Nota(long id, string descripcionNota, DateTime fechaCreacion, long fkCategoria, long fkCliente)
    {
        this.id = id;
        this.descripcionNota = descripcionNota;
        this.fechaCreacion = fechaCreacion;
        this.fkCategoria = fkCategoria;
        this.fkCliente = fkCliente;
    }

    public Nota()
    {
    }

    public string ToString()
    {
        return $"ID: {this.id}, Descripción: {this.descripcionNota}, Fecha: {this.fechaCreacion}, Categoría: {this.fkCategoria}, Cliente: {this.fkCliente}";
    }


}