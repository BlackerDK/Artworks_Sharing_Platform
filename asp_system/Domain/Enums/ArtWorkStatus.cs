using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ArtWorkStatus
    {
		PendingConfirmation = 1,
		InProgress = 2,
		Sold = 3,
		SoldPPendingConfirm = 4
	}
}
