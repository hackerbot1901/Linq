public class Cliente
{
    public long id { get; set; }
    public string nombreCliente { get; set; }

    public Cliente(long id, string nombreCliente)
    {
        this.id = id;
        this.nombreCliente = nombreCliente;
    }
}