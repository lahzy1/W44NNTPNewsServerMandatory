using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W44NntpNewsServerMandatory.Model
{
    internal class NewsGroup : INotifyPropertyChanged
    {
        private string _newsGroupHeader;

        public string NewsGroupHeader
        {
            get { return _newsGroupHeader; }
            set
            {
                if (_newsGroupHeader != value)
                {
                    _newsGroupHeader = value;
                    //OnPropertyChanged(nameof(NewsGroupHeader));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
