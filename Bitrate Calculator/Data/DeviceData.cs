using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitrate_Calculator.Data
{
    public class DeviceData
    {
        public string Device { get; set; }
        public string Model { get; set; }
        public List<NetworkInterface> NIC { get; set; }
    }

    public class NetworkInterface
    {
        public string Description { get; set; }
        public string MAC { get; set; }
        public DateTime Timestamp { get; set; }
        public long Rx { get; set; }
        public long Tx { get; set; }
    }

    
}
