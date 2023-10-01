namespace BlazorServer.Shared
{
    public class PaginasAux
    {
        public string Literal { get; set; }

        public int Pagina { get; set; }

        public bool Enabled { get; set; } = true;

        public bool Activa { get; set; } = false;

        public PaginasAux(int pagina, bool enabled, string literal, bool activa = false)
        {
            this.Pagina = pagina;
            this.Enabled = enabled;
            this.Literal = literal;
            this.Activa = activa;
        }
    }
}
