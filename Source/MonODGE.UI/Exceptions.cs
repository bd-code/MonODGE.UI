using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonODGE.UI {
    internal class CityComponentUsedException : Exception {
        public CityComponentUsedException(string message) : base(message) { }
    }
}
