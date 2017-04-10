using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticProgramming.Computer
{
    public class StackComputer
    {
        //Codes
        public const int JUMP = 0;
        public const int PUSH = 1;
        public const int POP = 2;
        public const int ADD = 3;
        public const int SUB = 4;
        public const int INC = 5;
        public const int DEC = 6;
        public const int READPUSH = 7;
        public const int WRITEPOP = 8;
        public const int STOP = 9;

        public const int tapeSize = 40;
        public int offset { get; private set; }
        int instructionPointer;
        bool allowEmptyStackErrors = false;
        public bool trace = false;

        public int[] tape { get; private set; }
        public int[] startTape { get; private set; }
        public int breakpoint = 0;
        public string errorMessage { get; private set; }
        Stack<int> stack;

        public StackComputer()
        {
            tape = new int[tapeSize];
            startTape = new int[tapeSize];
            stack = new Stack<int>();
        }

        public StackComputer(int offset, int[] tape) : this()
        {
            instructionPointer = offset;
            this.offset = offset;
            this.tape = tape;
            Array.Copy(tape, startTape, tapeSize);
        }
        
        public void run(int cycles)
        {
            

            instructionPointer = offset;
            bool stackError = false;
            while(cycles > 0 && !stackError)
            {
                if(instructionPointer >= tapeSize || instructionPointer < 0) { stackError = true; errorMessage = "instruction pointer ran off the tape."; break; }
                else
                {
                    if (trace) { Console.Write(instructionPointer + ": "); }
                    switch(tape[instructionPointer])
                    {
                        case JUMP:
                            instructionPointer++;
                            if (instructionPointer >= tapeSize || instructionPointer < 0) { stackError = true; errorMessage = "instruction pointer ran off the tape.";}
                            else
                            {
                                if (trace){ Console.WriteLine("Jumped to " + tape[instructionPointer]); }
                                instructionPointer = tape[instructionPointer];
                            }
                            break;

                        case PUSH:
                            instructionPointer++;
                            if (instructionPointer >= tapeSize || instructionPointer < 0) { stackError = true; errorMessage = "instruction pointer ran off the tape."; }
                            else
                            {
                                stack.Push(tape[instructionPointer]);
                                if (trace) { Console.WriteLine("Pushed " + tape[instructionPointer] + "."); printStack(); }
                            }
                            break;

                        case POP:
                            if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                            else
                            {
                                stack.Pop();
                                if (trace) { Console.WriteLine("Popped "); printStack(); }
                            }
                            break;

                        case ADD:
                            int vala;
                            int vals;
                            if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                            else
                            {
                                vala = stack.Pop();
                                if (trace) { Console.Write("Add, popped " + vala + " "); }

                                if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                                else
                                {
                                    vals = stack.Pop();
                                    if (trace) { Console.Write(" and " + vals + " "); }
                                    stack.Push(vala + vals);
                                }
                                if (trace) { Console.WriteLine("For stack "); printStack(); }
                            }
                            break;

                        case SUB:
                            int vald;
                            int valf;
                            if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                            else
                            {
                                vald = stack.Pop();
                                if (trace) { Console.Write("Sub, popped " + vald + " "); }
                                if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                                else
                                {
                                    valf = stack.Pop();
                                    if (trace) { Console.Write(" and " + valf + " "); }
                                    stack.Push(vald - valf);
                                }
                                if (trace) { Console.WriteLine("For stack "); printStack(); }
                            }
                            break;

                        case INC:
                            if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                            else
                            {
                                stack.Push(stack.Pop() + 1);
                                if (trace) { Console.WriteLine("Inc "); printStack(); }
                            }
                            break;

                        case DEC:
                            if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                            else
                            {
                                stack.Push(stack.Pop() - 1);
                                if (trace) { Console.WriteLine("Dec "); printStack(); }
                            }
                            break;

                        case READPUSH:
                            instructionPointer++;
                            if (instructionPointer >= tapeSize || instructionPointer < 0) { stackError = true; errorMessage = "instruction pointer ran off the tape."; }
                            else
                            {
                                if (tape[instructionPointer] >= tapeSize || tape[instructionPointer] < 0) { stackError = true; errorMessage = "instruction pointer ran off the tape."; }
                                else
                                {
                                    stack.Push(tape[tape[instructionPointer]]);
                                    if (trace) { Console.WriteLine("Inc "); printStack(); }
                                }
                            }
                            break;

                        case WRITEPOP:
                            instructionPointer++;
                            if (instructionPointer >= tapeSize || instructionPointer < 0) { stackError = true; errorMessage = "instruction pointer ran off the tape."; }
                            else
                            {
                                int writeLoc = tape[instructionPointer];
                                if (stack.Count <= 0) { stackError = allowEmptyStackErrors; errorMessage = "popped from an empty stack"; }
                                else
                                {
                                    tape[instructionPointer] = stack.Pop();
                                    if (trace) { Console.WriteLine("Wrote " + tape[instructionPointer] + " to " + instructionPointer + ", performing a pop in the process."); printStack(); }
                                }
                            }
                            break;
                        case STOP:
                            stackError = true;
                            errorMessage = "STOP called.";
                            if (trace) { Console.WriteLine("STOP ");}
                            break;
                    }
                }
                if(instructionPointer >= tapeSize) { breakpoint = -1; }
                else{breakpoint = instructionPointer;}
                
                cycles--;
                instructionPointer++;
            }
            
        }

        public void input(int memLoc, int value)
        {
            if (memLoc >= tapeSize || memLoc < 0) { Console.WriteLine("bad input memloc: " + memLoc); }
            else
            {
                tape[memLoc] = value;
            }
        }

        public void printStack()
        {
            Console.Write("STACK: ");
            foreach (int i in stack)
            {
                Console.Write(i + " | ");
            }
            Console.WriteLine();
        }

    }
}
