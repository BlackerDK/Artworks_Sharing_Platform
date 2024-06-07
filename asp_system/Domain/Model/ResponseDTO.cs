using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Model
{
    public class ResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public object Data { get; set; } = null;

    }
}
