﻿using LanguageExt;
using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Repositories
{
    public interface IOrdersRepository
    {
        TryAsync<int> TryCreateNewOrder(double ? totalPrice);
    }
}
