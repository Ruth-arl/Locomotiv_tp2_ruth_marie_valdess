using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locomotiv.Utils.Services.Interfaces
{
    public interface IMessageService
    {
        void Show(string message);
        void ShowError(string message);
        void ShowMessage(string message);
    }
}
