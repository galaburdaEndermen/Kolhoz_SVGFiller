using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolhoz_SVGFiller.Models
{
    public class Field : INotifyPropertyChanged
    {
        private string name;
        public string Name 
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string value;
        public string Value 
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                RaisePropertyChanged(nameof(Value));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
