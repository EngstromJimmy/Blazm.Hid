namespace Blazm.Hid
{
        public class OnInputReportArgs
        {
            public int ReportId { get; set; }
            public byte[] Data { get; set; }
        }
}