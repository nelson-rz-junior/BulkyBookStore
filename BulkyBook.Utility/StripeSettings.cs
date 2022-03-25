﻿#nullable disable

namespace BulkyBook.Utility
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }

        public string PublishableKey { get; set; }

        public string SuccessUrl { get; set; }

        public string CancelUrl { get; set; }
    }
}
