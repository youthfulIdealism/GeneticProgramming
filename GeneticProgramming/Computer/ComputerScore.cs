using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticProgramming.Computer
{
    /**
     * 
     * In this project, we're applying genetic algorithms to stack computers.
     * 
     * This class stores a score, the computer that got the score, and the point
     * at which the computer "crashed", called the breakpoint. The Driver will
     * sometimes use the breakpoint as the point of mutation, because can open
     * up a larger portion of the computer for use.
     * 
     * */
    public class ComputerScore
    {
        public int score;
        public int[] computer;
        public int breakpoint;

        /**
         * 
         * 
         * 
         * */
        public ComputerScore(int score, int[] computer, int breakpoint)
        {
            this.score = score;
            this.computer = computer;
            this.breakpoint = breakpoint;
        }
    }
}
