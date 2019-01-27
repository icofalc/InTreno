using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace InTreno
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {

        private Button bottone_ora;
        private Button bottone_data;
        private Button bottone_cerca;

        private AutoCompleteTextView cerca_da;
        private AutoCompleteTextView cerca_a;

        String ora_scelta_def;//qui dopo aver selezionato l'ora, avrò proprio l'ora per la query.
        String data_scelta_def;//qui dopo aver selezionato la data, avrò proprio la data per la query.

        string codice_da_def="";//qui dopo le query, avrò il codice per l'activity successiva
        string codice_a_def="";//qui dopo le query, avrò il codice per l'activity successiva

        private WebClient mClient;
        private Uri mUrl;

        private List<Stazione> stazioni;//lista con oggetti di stazioni che poi vengono aggiunti i nomi a lista_risultato_stazioni
        private List<string> lista_risultato_stazioni;

        private ArrayAdapter<string> ada;

        private int editscelto=0;// 0 se prima edittext, 1 se secondaedittext
       

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            stazioni = new List<Stazione>();
            
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);

            bottone_ora = FindViewById<Button>(Resource.Id.buttonora);
            bottone_data= FindViewById<Button>(Resource.Id.buttondata);
            bottone_cerca = FindViewById<Button>(Resource.Id.buttoncerca);

            cerca_da = FindViewById<AutoCompleteTextView>(Resource.Id.autocompletamentopt);
            cerca_da.TextChanged += Cerca_da_TextChanged;//appena cambia il testo, allora inizio a lavorare

            cerca_a = FindViewById<AutoCompleteTextView>(Resource.Id.autocompletamentoar);
            cerca_a.TextChanged += Cerca_a_TextChanged;//appena cambia il testo, allora inizio a lavorare

            cerca_da.ItemClick += prova;//clicco sempre un elemento alla volta, e quando lo clicco, seleziono un elemento della lista
            cerca_a.ItemClick += prova;

            
            stazioni = new List<Stazione>();
            lista_risultato_stazioni = new List<string>();

            bottone_ora.Click += Bottone_ora_Click;
            bottone_data.Click += Bottone_data_Click;
            bottone_cerca.Click += Bottone_cerca_Click;

        }

        protected override void OnRestart()//se ritorno indietro pulisco per evitare errori, senno accadevà di ricercare la stessa soluzione anche con nomi sbagliati
        {
            base.OnRestart();
            codice_a_def = "";
            codice_da_def = "";
            cerca_da.Text = "";
            cerca_a.Text = "";
        }

        private void prova(object sender, AdapterView.ItemClickEventArgs e)//metodo per selezionare un elemento della lista
        {
            if (editscelto == 0)//verifico se aveva toccato prima o seconda edit
            {
                codice_da_def = stazioni[e.Position].id;//in base alla posizione toccata dall'utente prendo i dati
            }
            else
            {
                codice_a_def = stazioni[e.Position].id;
            }
            
        }


        private void Bottone_cerca_Click(object sender, EventArgs e)
        {
            //da qui faccio partire la nuova activity con i parametri settati, adesso devo trovare il modo di prendere le stazioni, perche passo i nomi originali
            //ma non vanno bene per la query sul server.

            if (controlla_connession())//controllo lo stato della connessione prima di far partire l'altra activity
            {
                //prendi_valori();
                if (cerca_da.Text.Equals("") || cerca_a.Text.Equals("") || bottone_ora.Text.Equals("Scegli Ora") || bottone_data.Text.Equals("Scegli Data"))
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Dati incompleti");
                    alert.SetMessage("Immetti tutti i dati necessari.");
                    alert.SetPositiveButton("Ok", (senderAlert, args) => {
                        //qua non faccio niente perchè ho gia preso i codici
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                else if (cerca_da.Text.Equals(cerca_a.Text)) {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Attenzione");
                    alert.SetMessage("Le stazioni di partenza e arrivo non possono coincidere.");
                    alert.SetPositiveButton("Ok", (senderAlert, args) => {
                        //qua non faccio niente perchè ho gia preso i codici
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }else//se ho tutti i valori impostati, allora faccio le query
                {
                    Intent intent = new Intent(this, typeof(Visualizza_sol));
                    intent.PutExtra("da", codice_da_def);
                    intent.PutExtra("a", codice_a_def);
                    intent.PutExtra("ora", ora_scelta_def + ":00");//aggiungo 00 perchè lo vuole viaggiatreno
                    intent.PutExtra("data", data_scelta_def);
                    StartActivity(intent);
                }


            }
            else
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("Connessione non disponibile");
                alert.SetMessage("Connessione dati non disponibile, riprova più tardi");
                alert.SetPositiveButton("Ok", (senderAlert, args) => {
                    //qua non faccio niente perchè ho gia preso i codici
                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }

            
        }

        private void Cerca_a_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            
            editscelto = 1;
            string stringa = cerca_a.Text.ToString();
            int conto = stringa.Trim().Length;
            if (conto != 0)//ho digitato qualcosa
            {
                //stazioni.Clear();
                mClient = new WebClient();
                mUrl = new Uri("http://www.viaggiatreno.it/viaggiatrenonew/resteasy/viaggiatreno/cercaStazione/" + stringa);
                if (controlla_connession())//prima di eseguire il task, mi accerto che la connessione sia disponibile
                {
                    prendi_valori();
                }
                else
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Connessione non disponibile");
                    alert.SetMessage("Connessione dati non disponibile, riprova più tardi");
                    alert.SetPositiveButton("Ok", (senderAlert, args) => {
                        cerca_a.Text = "";
                        cerca_da.Text = "";
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                
            }
        }

        private void Cerca_da_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            // throw new NotImplementedException();
            editscelto = 0;
            string stringa = cerca_da.Text.ToString();
            int conto = stringa.Trim().Length;
            if (conto != 0)//ho digitato qualcosa
            {
                //stazioni.Clear();
                mClient = new WebClient();
               // string q = "http://www.viaggiatreno.it/viaggiatrenonew/resteasy/viaggiatreno/cercaStazione/" + stringa;
               // System.Diagnostics.Debug.Write(q);
                mUrl = new Uri("http://www.viaggiatreno.it/viaggiatrenonew/resteasy/viaggiatreno/cercaStazione/" + stringa);
                if (controlla_connession())//prima di eseguire il task, mi accerto che la connessione sia disponibile
                {
                    prendi_valori();
                }
                else//provare a generare un alert dialog invece di un toast.
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Connessione non disponibile");
                    alert.SetMessage("Connessione dati non disponibile, riprova più tardi");
                    alert.SetPositiveButton("Ok", (senderAlert, args) => {
                        cerca_a.Text = "";
                        cerca_da.Text = "";
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
            }
        }

        private async void prendi_valori()
        {
            await Task.Run(() => {

                mClient.DownloadDataAsync(mUrl);
                mClient.DownloadDataCompleted += MClient_DownloadDataCompleted;

            });

        }

        private void MClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                try {
                    string json = Encoding.UTF8.GetString(e.Result);

                    stazioni = JsonConvert.DeserializeObject<List<Stazione>>(json);
                    lista_risultato_stazioni.Clear();
                } catch (Exception ec) {
                    System.Diagnostics.Debug.WriteLine(ec);

                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Soluzioni non disponibili");
                    alert.SetMessage("Il server è temporaneamente in aggiornamento, riprova più tardi");
                    alert.SetPositiveButton("Ok", (senderAlert, args) => {
                        
                    });
                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                
                foreach(var a in stazioni)
                {
                    lista_risultato_stazioni.Add(a.nomeLungo);
                }

                

                if (editscelto == 0)//controllo su quale edittext aveva scritto l'utente
                {
                    ada = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, lista_risultato_stazioni);
                    cerca_da.Adapter = ada;
                   // string r =lista_risultato_stazioni;
                }
                else
                {
                    ada = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, lista_risultato_stazioni);
                    cerca_a.Adapter = ada;
                }
                
                


            });
        }
        /////////////////////////////////////////////////////////////////////da qui stanno data ora e barra sotto
        private void Bottone_data_Click(object sender, EventArgs e)//metodo per chiamare la selezione della data
        {
            //throw new NotImplementedException();
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
               data_scelta_def = time.ToLongDateString();
               bottone_data.Text = data_scelta_def;
               data_scelta_def = time.Year + "-" + time.Month + "-" + time.Day;
               Toast.MakeText(this, data_scelta_def, ToastLength.Short).Show();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void Bottone_ora_Click(object sender, System.EventArgs e)//metodo per chiamare la selezione dell'ora
        {
            //throw new System.NotImplementedException();
            TimePickerFragment frag = TimePickerFragment.NewInstance(
               delegate (DateTime time)
               {
                   ora_scelta_def = time.ToShortTimeString();
                   bottone_ora.Text = "ORA " + ora_scelta_def;
                   //ora_scelta_def = ora_scelta_def + ":00";
                   //Toast.MakeText(this,ora_scelta_def,ToastLength.Short).Show(); //debug...
                   
               });

            frag.Show(FragmentManager, TimePickerFragment.TAG);

        }

        public bool OnNavigationItemSelected(IMenuItem item)//barra di navigazione posizionata in basso
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                   
                    Toast.MakeText(this,"Sei già sulla home", ToastLength.Long).Show();
                    return true;
                
               
            }
            return false;
        }


        private bool controlla_connession()//metodo creato per verificare la disponibilità di una connessione ad internet
        {
            var current = Connectivity.NetworkAccess;

            if (current == Xamarin.Essentials.NetworkAccess.Internet)
            {
                // Connessione disponibile
                return true;
            }
            else
            {
                // Connessione non disponibile
                return false;
            }
        }
    }
}

