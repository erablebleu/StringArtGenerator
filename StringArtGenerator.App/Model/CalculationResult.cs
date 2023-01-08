using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringArtGenerator.App.Model;
public class CalculationResult
{
    public ThreadInstruction[] Instructions { get; set; } = Array.Empty<ThreadInstruction>();
}
