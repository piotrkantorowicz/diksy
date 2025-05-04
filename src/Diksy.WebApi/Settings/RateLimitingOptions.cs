using System.ComponentModel.DataAnnotations;
using System.Threading.RateLimiting;

namespace Diksy.WebApi.Settings
{
    public class RateLimitingOptions
    {
        public bool Enabled { get; set; } = true;

        [Range(1, 1440, ErrorMessage = "WindowInMinutes must be between 1 and 1440 (1 day).")]
        public int WindowInMinutes { get; set; } = 1;

        [Range(1, 10000, ErrorMessage = "PermitLimit must be between 1 and 10000.")]
        public int PermitLimit { get; set; } = 20;

        [Range(0, 10000, ErrorMessage = "QueueLimit must be between 0 and 10000.")]
        public int QueueLimit { get; set; } = 10;

        public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.NewestFirst;
    }
}