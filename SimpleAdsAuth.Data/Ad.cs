using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleAdsAuth.Data
{
    public class Ad
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string PhoneNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

    }
}
