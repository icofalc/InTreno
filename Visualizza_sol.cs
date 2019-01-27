using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace InTreno
{
    [Activity(Label = "Visualizza_sol")]
    public class Visualizza_sol :  AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        //private List<string> listina;//questa lista conterrà effettivamente i dati che mi interessano

            //dichiaro un po' di variabili che serviranno a contenere i vari dati
        ListView listView;

        String codice_da_def;
        String codice_a_def;
        String data;
        String ora;

        private WebClient mClient;
        private Uri mUrl;
        RootObject risultato;
        private List<string> lista;
        private List<Parziale> listin;//conterrà gli oggetti che poi verranno messi dal customadapter nella listview
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.visualizza_layout);

            listView = FindViewById<ListView>(Resource.Id.listView1);

            //////prendo i dati passati dall'activity precedente
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation_visuale);
            navigation.SetOnNavigationItemSelectedListener(this);

            //codice_da_def = Intent.GetStringExtra("da").Substring(1);
            //codice_a_def = Intent.GetStringExtra("a").Substring(1);

            codice_da_def = Intent.GetStringExtra("da");
            codice_a_def = Intent.GetStringExtra("a");
            if (codice_da_def.Trim().Equals("") || codice_a_def.Trim().Equals(""))
            {

                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Attenzione");
                alert.SetMessage("Non hai selezionato una stazione disponibile, ritorna alla schermata principale e seleziona le stazioni corrette");
                alert.SetPositiveButton("Ok", (senderAlert, args) =>
                {
                    Finish();
                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
            else
            {
                codice_da_def = Intent.GetStringExtra("da").Substring(1);
                codice_a_def = Intent.GetStringExtra("a").Substring(1);
                data = Intent.GetStringExtra("data");
                ora = Intent.GetStringExtra("ora");

                //////prendo i dati passati dall'activity precedente
                mClient = new WebClient();
                string a = "http://www.viaggiatreno.it/viaggiatrenonew/resteasy/viaggiatreno/soluzioniViaggioNew/" + codice_da_def + "/" + codice_a_def + "/" + data + "T" + ora + "";
                System.Diagnostics.Debug.WriteLine(a);

                mUrl = new Uri(a);

                lista = new List<string>();//lista per debug

                trova_stazioni();//una volta che ho predisposto tutto, chiamo il task async trova_stazioni
            }

        }


        private async void trova_stazioni()//task asincrono per scaricare la ricerca
        {
            
            await Task.Run(() => {
                mClient.DownloadDataAsync(mUrl);//scarica i dati dall'url
                mClient.DownloadDataCompleted += MClient_DownloadDataCompleted;//una volta completato, richiama l'altro metodo che penserà all'interfaccia

            });

        }

        private void MClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
               
                string json = Encoding.UTF8.GetString(e.Result);
                risultato = new RootObject();
                risultato = JsonConvert.DeserializeObject<RootObject>(json);//deserializzo il json nel nuovo oggetto risultato
                if (risultato.soluzioni != null)//questi due controlli li devo inspiegabilmente fare a causa del ritorno
                {
                    if(risultato.soluzioni.Count != 0)//se lo metto in && con l'if sopra, solleva un'eccezione
                    {
                        listin = risultato.get_parziale();
                        //lista = risultato.get_nome_treno();
                        //ArrayAdapter<string> ada = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, lista);
                        CustomAdapter ada = new CustomAdapter(this, listin);
                        listView.Adapter = ada;
                    }
                    else
                    {
                        Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                        alert.SetTitle("Soluzioni non disponibili");
                        alert.SetMessage("Non sono disponibili soluzioni per la tratta selezionata, torna alla schermata di ricerca");
                        alert.SetPositiveButton("Ok", (senderAlert, args) => {
                            Finish();
                        });
                        Dialog dialog = alert.Create();
                        dialog.Show();
                    }

                }
                else
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Soluzioni non disponibili");
                    alert.SetMessage("Non sono disponibili soluzioni per la tratta selezionata, torna alla schermata di ricerca");
                    alert.SetPositiveButton("Ok", (senderAlert, args) => {
                        Finish();
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                
            });
        }

        public bool OnNavigationItemSelected(IMenuItem item)//metoto che mi riporta alla schermata precedente , premendo l'unico pulsante sulla barra di navigazione in basso
        {//la barra lho messa per gli schermi full screen
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home_sol:

                    Finish();
                    return true;


            }
            return false;
        }
    }
}
