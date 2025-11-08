using System.ComponentModel;
using System.Runtime.CompilerServices;
using Locomotiv.Utils.Services.Interfaces;
using Locomotiv.Model;

namespace Locomotiv.Utils.Services
{
    public class Service : IUserSessionService
    {
        private User _connectedUser;
        public User ConnectedUser
        {
            get => _connectedUser;
            set
            {
                _connectedUser = value;
                OnPropertyChanged(nameof(ConnectedUser));
                OnPropertyChanged(nameof(IsUserConnected));
                OnPropertyChanged(nameof(IsUserDisconnected));
            }
        }

        public bool IsUserConnected
        {
            get => _connectedUser != null;
        }
        public bool IsUserDisconnected
        {
            get => _connectedUser == null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Connect(User user)
        {
            ConnectedUser = user;
        }

        public void Disconnect()
        {
            ConnectedUser = null;
        }
    }
}
