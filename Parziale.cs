using System;
namespace InTreno
{
    public class Parziale
    {
         string origine ;
         string destinazione ;
         string orarioPartenza ;
         string orarioArrivo ;
         string categoriaDescrizione ;
         string numeroTreno ;

        public Parziale(string origine, string destinazione, string orarioPartenza, string orarioArrivo, string categoriaDescrizione, string numeroTreno)
        {
            this.origine = origine;
            this.destinazione = destinazione;
            this.orarioPartenza = orarioPartenza;
            this.orarioArrivo = orarioArrivo;
            this.categoriaDescrizione = categoriaDescrizione;
            this.numeroTreno = numeroTreno;
        }

        public string get_numeroTreno()
        {
            return numeroTreno;
        }

        public string get_origine()
        {
            return origine;
        }

        public string get_destinazione()
        {
            return destinazione;
        }

        public string get_orarioPartenza()
        {
            return orarioPartenza;
        }
        public string get_orarioArrivo()
        {
            return orarioArrivo;
        }

        public string get_categoriaDescrizione()
        {
            return categoriaDescrizione;
        }
    }
}
