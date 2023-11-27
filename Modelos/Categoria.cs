using System;

public class Categoria
{
    public long id { get; set; }
    public string descripcionCategoria { get; set; }

    public DateTime fechaCreacion { get; set; }

    public Categoria(long id, string descripcionCategoria, DateTime fechaCreacion)
    {
        this.id = id;
        this.descripcionCategoria = descripcionCategoria;
        this.fechaCreacion = fechaCreacion;
    }

    public string ToString()
    {
        return string.Format("[ id: {0}, Descripcion categoria: {1},  Fecha creacion: {2} ]", this.id, this.descripcionCategoria, this.fechaCreacion);
    }


}