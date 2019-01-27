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
    class Stazione//classe che mi serve per il json ritornato dalle query fatte dalla main activity
    {
        public string nomeLungo { get; set; }
        public string nomeBreve { get; set; }
        public string label { get; set; }
        public string id { get; set; }
    }
}