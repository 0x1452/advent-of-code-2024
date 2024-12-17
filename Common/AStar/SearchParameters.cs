using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.AStar;
public class SearchParameters
{
    public required Point Start { get; set; }
    public required Point End { get; set; }
    public required bool[,] Grid { get; set; }
}
