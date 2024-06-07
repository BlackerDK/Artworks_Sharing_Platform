using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ProcessStatus
    {
        Success = 1,
        Fail = 2,
        Duplicate = 3,
        NotExisted = 4,
        NotPermission = 5,
        RelateEntity = 6,
        NotEnough = 7,
        NotFound = 8,
    }

}
