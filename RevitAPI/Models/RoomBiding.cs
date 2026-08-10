using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIProject.Models
{
    public class RoomBiding : ObservableObject
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public Double Area { get; set; }
        public Double Volume { get; set; }
    }
}
