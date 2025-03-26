using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO
{
    public class ReplyFeedBackDTO
    {
        public string Subject {  get; set; }
        public string Message {  get; set; }
        public Guid FeedBackID { get; set; }
    }
}
