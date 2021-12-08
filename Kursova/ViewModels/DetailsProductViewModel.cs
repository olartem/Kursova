using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Kursova.Models;

namespace Kursova.ViewModels
{
    public class DetailsProductViewModel : Product
    {
        public string Nonce { get; set; }
    }
}
