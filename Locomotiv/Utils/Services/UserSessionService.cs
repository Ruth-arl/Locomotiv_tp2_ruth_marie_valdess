using System.ComponentModel;
using System.Runtime.CompilerServices;
using Locomotiv.Model;
using Locomotiv.Utils.Services.Interfaces;

namespace Locomotiv.Utils.Services
{
    public class UserSessionService : IUserSessionService
    {
        private User _connectedUser;

        public User ConnectedUser
        {
            get => _connectedUser;
            set
            {
                if (_connectedUser != value)
                {
                    _connectedUser = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsUserConnected));
                    OnPropertyChanged(nameof(IsUserDisconnected));
                }
            }
        }

        public bool IsUserConnected => ConnectedUser != null;
        public bool IsUserDisconnected => ConnectedUser == null;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Connect(User user)
        {
            ConnectedUser = user;
        }

        public void Disconnect()
        {
            ConnectedUser = null;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
