using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer.Models
{
    public class ListBoxLastItemModel : INotifyPropertyChanged
    {
        private string _message;
        private bool _isLast;

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsLast
        {
            get => _isLast;
            set
            {
                _isLast = value;
                OnPropertyChanged(nameof(_isLast));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
