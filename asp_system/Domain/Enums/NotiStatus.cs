using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum NotiStatus
    {
        Normal = 1, 
        Order = 2,
        AcceptOrder = 3,
        DenyOrder = 4,
		ConfirmPost = 5,
        AcceptPost = 6,
        DenyPost = 7,
	}
}
