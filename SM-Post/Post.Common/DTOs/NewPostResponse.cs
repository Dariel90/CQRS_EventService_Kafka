using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Common.DTOs
{
    public class NewPostResponse : BaseResponse
    {
        public Guid Id { get; set; }
        public int test { get; set; }
    }
}