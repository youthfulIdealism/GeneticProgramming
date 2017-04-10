using GeneticProgramming.Computer;
using Microsoft.Xna.Framework.Graphics;


namespace GeneticProgramming.Environment
{
    /**
     * 
     * In this context, a "World" is a problem space. If you want to define a problem space and
     * set machines to the task, this is the class to extend.
     * 
     * It is the world's responsibility to define how a machine
     * interfaces with it.
     * 
     * */
    public abstract class World
    {
        /**
         * Updates the problem space. Returns whether or not the world should keep executing.
         * */
        public abstract bool Update();

        /**
         * Draws the world if it's a display world. Don't do any computation in this method
         * unless it's tied to drawing and nothing else.
         * */
        public abstract void draw(SpriteBatch batch);

        /**
         * Retrieves the score associated with the world's computer.
         * */
        public abstract float getScore();

        /**
         * Assigns the world the instructions for a computer. It's the world's responsibility
         * to convert the instructions into a working critter and interface with that critter.
         * */
        public abstract void setMachine(int[] machine);

        /**
         * Retrieves the world's computer.
         * */
        public abstract StackComputer getMachine();
    }
}
