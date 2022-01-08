using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kursova.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kursova.ViewModels
{
    public class ResultDashboardViewModel
    {
        public IEnumerable<GameResult> Results { get; set; }
        public SelectList Games { get; set; }
        public string Name { get; set; }
    }
}
