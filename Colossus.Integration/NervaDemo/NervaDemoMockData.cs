using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colossus.Integration.NervaDemo
{
    public class NervaDemoMockData
    {
        public Guid? ChannelId { get; set; }
        public Guid? SubChannelId { get; set; }
        public Guid? ChannelTypeId { get; set; }

        public string DeviceType { get; set; }
    }
}
