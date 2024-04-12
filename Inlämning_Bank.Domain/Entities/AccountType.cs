using System;
using System.Collections.Generic;

namespace Inlämning_Bank.Domain.Entities;

public partial class AccountType
{
    public int AccountTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
