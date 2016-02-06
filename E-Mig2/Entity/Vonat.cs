using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Mig2.Entity
{
    public class Vonat
    {
        private string _vonatszam;
        private string _uic;
        private int _index;
    
        public string Vonatszam
        {
            get { return _vonatszam; }
            set { _vonatszam = value; }
        }      
        public string UIC
        {
            get { return _uic; }
            set { _uic = value; }
        }
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

    }
}
