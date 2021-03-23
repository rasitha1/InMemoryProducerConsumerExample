using System;

namespace WebProducerConsumerExample.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class FileDownloadAction
    {
        public int MessageCount { get; set; }
    }
}
