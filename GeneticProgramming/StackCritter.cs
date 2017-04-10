using GeneticProgramming.Computer;
using GeneticProgramming.Environment;
using GeneticProgramming.ExampleProjects.Fruit;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GeneticProgramming
{
    public class StackCritter : Entity
    {
        public StackComputer stackComputer;
        public Random rand;

        
        public StackCritter() : base()
        {
            color = Color.Green;
            texture = Driver.blobTex;
        }

        public void addPush(List<int> tape)
        {
            int ix = rand.Next(tape.Count);
            if (rand.NextDouble() < .5)
            {
                tape.Insert(ix, StackComputer.PUSH);
            }
            else
            {
                tape.Insert(ix, StackComputer.READPUSH);
            }
            tape.Insert(ix + 1, rand.Next(StackComputer.tapeSize));
        }

        /**
         * 
         * builds a stack critter with a preset machine
         * 
         * */
        public StackCritter(int[] machine) : this()
        {
            color = Color.Green;
            texture = Driver.blobTex;

            stackComputer = new StackComputer(6, machine);
        }

        /**
         * 
         * builds a random stack critter
         * 
         * */
        public StackCritter(Random rand)
        {
            List<int> queuedInstructionRanges = new List<int>();
            List<int> randomTape = new List<int>();
            while (randomTape.Count < StackComputer.tapeSize)
            {
                if (queuedInstructionRanges.Count == 0)
                {
                    int randomInstruction = rand.Next(9);

                    switch (randomInstruction)
                    {
                        case StackComputer.JUMP:
                            queuedInstructionRanges.Add(StackComputer.tapeSize);
                            break;
                        case StackComputer.PUSH:
                            queuedInstructionRanges.Add(StackComputer.tapeSize);
                            break;
                        case StackComputer.POP:
                            addPush(randomTape);
                            break;
                        case StackComputer.ADD:
                            addPush(randomTape);
                            addPush(randomTape);
                            break;
                        case StackComputer.INC:
                            addPush(randomTape);
                            break;
                        case StackComputer.DEC:
                            addPush(randomTape);
                            break;
                        case StackComputer.READPUSH:
                            queuedInstructionRanges.Add(StackComputer.tapeSize);
                            break;
                        case StackComputer.STOP:
                            break;
                    }
                    randomTape.Add(randomInstruction);
                }
                else
                {
                    randomTape.Add(rand.Next(queuedInstructionRanges[0]));
                    queuedInstructionRanges.RemoveAt(0);
                }
            }
        }

        public void runComputer()
        {
            stackComputer.run(StackComputer.tapeSize * 10);
        }

        public void input(int memloc, int value)
        {
            if(memloc >= stackComputer.offset)
            {
                throw new ArgumentOutOfRangeException("You tried to input a value into a StackCritter that exceeded its input/output zone.");
            }
            stackComputer.input(memloc, value);
        }

        public int getOutput(int memloc)
        {
            if (memloc >= stackComputer.offset)
            {
                throw new ArgumentOutOfRangeException("You tried to request a value from a StackCritter that exceeded its input/output zone.");
            }
            return stackComputer.tape[memloc];
        }
    }
}
