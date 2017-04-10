using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeneticProgramming.Computer;

namespace UnitTests
{
    [TestClass]
    public class StackComputerTest
    {


        [TestMethod]
        public void ComputerTest()
        {
            int[] instructions = {
                5,
                StackComputer.READPUSH,
                0,
                StackComputer.PUSH,
                5,
                StackComputer.ADD,
                StackComputer.WRITEPOP,
                0
            };

            StackComputer computer = new StackComputer(1, instructions);
            computer.run(instructions.Length);
            if(computer.tape[0] != 10)
            {
                throw new Exception("The computer failed the test.");
            }

        }
    }
}
