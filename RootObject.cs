using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace InTreno
{


    public class Vehicle
    {
        public string origine { get; set; }
        public string destinazione { get; set; }
        public DateTime orarioPartenza { get; set; }
        public DateTime orarioArrivo { get; set; }
        public string categoria { get; set; }
        public string categoriaDescrizione { get; set; }
        public string numeroTreno { get; set; }
    }

    public class Soluzioni
    {
        public string durata { get; set; }
        public List<Vehicle> vehicles { get; set; }
    }

    public class RootObject
    {
        public List<Soluzioni> soluzioni { get; set; }
        public string origine { get; set; }
        public string destinazione { get; set; }
        public object errore { get; set; }

        public List<string> get_nome_treno()//questa funzione controlla se le soluzioni vengono proposte con scambio oppure sono dirette
        {
            List<string> c = new List<string>();
            foreach (var soluzione in soluzioni)
            {
                if (soluzione.vehicles.Count > 1)
                {
                    foreach (var d in soluzione.vehicles)
                    {
                        c.Add("Treno " + d.numeroTreno + " da " + d.origine + " a " + d.destinazione + ", Categoria = " + d.categoriaDescrizione + " \nOrario partenza " + d.orarioPartenza + "\nOrario arrivo " + d.orarioArrivo);
                    }
                    c.Add("**************************************************");
                }
                else
                {
                    foreach (var d in soluzione.vehicles)
                    {
                        c.Add("Treno " + d.numeroTreno + " da " + d.origine + " a " + d.destinazione + ", Categoria = " + d.categoriaDescrizione + " \nOrario partenza " + d.orarioPartenza + "\nOrario arrivo " + d.orarioArrivo);
                    }
                    c.Add("**************************************************");
                }
            }
            return c;
        }

        public List<Parziale> get_parziale()
        {
            List<Parziale> par = new List<Parziale>();
            foreach (var soluzione in soluzioni)
            {
                if (soluzione.vehicles.Count > 1)
                {
                    foreach (var d in soluzione.vehicles)
                    {
                        Parziale p = new Parziale(d.origine, d.destinazione,""+d.orarioPartenza,""+d.orarioArrivo, d.categoriaDescrizione, d.numeroTreno);
                        par.Add(p);
                    }
                    Parziale c = new Parziale("","","","","","");//metto vuoto cosi posso distinguere quando devo separare le soluzioni
                    par.Add(c);
                }
                else
                {
                    foreach (var d in soluzione.vehicles)
                    {
                        Parziale c = new Parziale(d.origine, d.destinazione,""+d.orarioPartenza,""+d.orarioArrivo, d.categoriaDescrizione, d.numeroTreno);
                        par.Add(c);
                    }
                    Parziale p = new Parziale("", "", "", "", "", "");
                    par.Add(p);
                }



            }
            return par;
        }
    }
}