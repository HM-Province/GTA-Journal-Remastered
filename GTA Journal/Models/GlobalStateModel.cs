using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GTA_Journal.Models
{
    public class GlobalStateModel : INotifyPropertyChanged
    {
        public enum UserStatus
        {
            Online, Offline, AFK
        }

        private UserStatus _currentUserStatus = UserStatus.Offline;
        public UserStatus CurrentUserStatus
        {
            get => _currentUserStatus;
            set {  _currentUserStatus = value; OnPropertyChanged(); }
        }

        private bool _useMicaTheme = false;
        public bool UseMicaTheme
        {
            get => _useMicaTheme;
            set { _useMicaTheme = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
