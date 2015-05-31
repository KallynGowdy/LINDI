using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LINDI.Tests
{
    public interface IBinding<out T>
    {
        T Resolve();
    }
}
