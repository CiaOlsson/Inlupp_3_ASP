﻿using Inlämning_Bank.Data.Interfaces;
using Inlämning_Bank.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inlämning_Bank.Core.Interfaces
{
    public interface IDispositionService
    {
        
        Task<List<Disposition>> GetDispositionsByCustomerId(int id);
        Task AddDisposition(int customerId, int accountId);
    }
}
