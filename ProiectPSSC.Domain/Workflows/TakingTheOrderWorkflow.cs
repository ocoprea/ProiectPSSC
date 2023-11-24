using ProiectPSSC.Domain.Commands;
using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Models.TakingTheOrderEvent;

namespace ProiectPSSC.Domain.Workflows
{
    public class TakingTheOrderWorkflow
    {
        public async Task <ITakingTheOrderEvent> ExecuteAsync (TakingTheOrderCommand command)
        {
            return new TakingTheOrderSuccededEvent();
        }
    }
}
