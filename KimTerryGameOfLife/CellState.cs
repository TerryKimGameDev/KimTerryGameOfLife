using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cells
{
    //A class for the state of a cell(alive/dead)
    public class CellState
    {
        //bool for alive cell state
        public bool Lcells = false;
        //generation counter for alive cells
        public int GenCount { get; private set; }

        public CellState()
        {
            GenCount = 0;
        }

        public void SetLcells()
        {
            //toggle state
            Lcells = !Lcells;
        }
        public bool GetCellState()
        {
            return Lcells;
        }
        
        public void GenerationCounter()
        {
            GenCount++;
        }
    }

}
