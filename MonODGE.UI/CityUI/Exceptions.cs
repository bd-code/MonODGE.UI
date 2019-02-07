using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CityUI {
    internal class CityComponentUsedException : Exception {
        public CityComponentUsedException(string message) : base(message) { }
    }
}
