using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp.Choices;

namespace ProiectPSSC.Domain.Models
{
    [AsChoice]
    public static partial class TakingTheOrderEvent
    {
        public interface ITakingTheOrderEvent { }
        public record TakingTheOrderSuccededEvent : ITakingTheOrderEvent
        { }
        public record TakingTheOrderFailedEvent : ITakingTheOrderEvent
        { }
    }
}
