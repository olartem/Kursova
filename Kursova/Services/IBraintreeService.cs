using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Braintree;

namespace Kursova.Services
{
    public interface IBraintreeService
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
