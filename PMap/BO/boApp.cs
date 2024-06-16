using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PMapCore.BO
{
    [Serializable]
    public class boApp
    {
        public int ID { get; set; }
        public string APP_NAME { get; set; }
        public int APP_VERSION { get; set; }
        public int APP_MAJOR { get; set; }
        public int APP_MINOR { get; set; }
        public string APP_SERIAL { get; set; }

    }
}
