using System;

namespace Blazm.Hid
{
    [Serializable]
    public class HidDeviceFilter
    {
        public int? vendorId { get; set; } = null;
        public int? productId { get; set; } = null;
        public string? classCode { get; set; } = null;
        public string? subclassCode { get; set; } = null;
        public string? protocolCode { get; set; } = null;
        public string? serialNumber { get; set; } = null;
    }
}
