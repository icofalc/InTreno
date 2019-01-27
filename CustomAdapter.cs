using System;
using Android.App;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;

namespace InTreno
{
    public class CustomAdapter :BaseAdapter<Parziale>
    {
        public List<Parziale> lista;
        public Activity context;


        public CustomAdapter(Activity context, List<Parziale> lista)
        {
            this.context = context;
            this.lista = lista;
        }//costruttore del customadapter


        public override int Count
        {
            get
            {
                return lista.Count;
                
            }
        }//metodo per conteggio degli elementi

        public override long GetItemId(int position)
        {
            return position;
        }//prendi l'id dell'elemento corrente

        public override Parziale this[int position]
        {
            get
            {
                return lista[position];
            }
        }//iteratore

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Riga, parent, false);


            view.FindViewById<TextView>(Resource.Id.textview_tipotreno).Text = lista[position].get_categoriaDescrizione();
            view.FindViewById<TextView>(Resource.Id.textview_numerotreno).Text = lista[position].get_numeroTreno();
            view.FindViewById<TextView>(Resource.Id.textview_oraandata).Text = ""+lista[position].get_orarioPartenza();
            view.FindViewById<TextView>(Resource.Id.textview_oraarrivo).Text = "" + lista[position].get_orarioArrivo(); ;
            view.FindViewById<TextView>(Resource.Id.textview_stazionepartenza).Text = lista[position].get_origine();
            view.FindViewById<TextView>(Resource.Id.textview_stazionearrivo).Text = lista[position].get_destinazione();

            if (lista[position].get_numeroTreno().Equals(""))//se il numero treno viene vuoto, allora ho visto i cambi, pertanto metto una riga vuota
            {
                var view1 = LayoutInflater.From(parent.Context).Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
                return view1;
            }
            else
            {
                return view;
            }

           
        }
    }
}
