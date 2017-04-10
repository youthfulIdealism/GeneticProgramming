using GeneticProgramming.Environment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticProgramming.Computer;

namespace GeneticProgramming.ExampleProjects.Fruit
{
    /**
     * 
     * Sample world.
     * 
     * Problem defenition: The computer is the 'brain' of some blob-shaped herbivore which
     * needs to find and eat fruit. The computer starts with 300000 points. It loses points
     * each update based on its distance from each fruit, and gains points whenever it eats
     * a fruit.
     * 
     * */
    public class FruitWorld : World
    {
        //The critter has 200 update cycles to solve the world.
        public const int totalWorldTime = 200;
        public int worldTime;
        public float score;
        
        public List<Entity> fruit;
        public StackCritter critter;

        public Random rand;

        public FruitWorld()
        {
            fruit = new List<Entity>();
            worldTime = totalWorldTime;

            Fruit newFruit1 = new Fruit();
            newFruit1.location = new Vector2(50, 500);
            fruit.Add(newFruit1);

            Fruit newFruit2 = new Fruit();
            newFruit2.location = new Vector2(400, 500);
            fruit.Add(newFruit2);

            Fruit newFruit3 = new Fruit();
            newFruit3.location = new Vector2(750, 500);
            fruit.Add(newFruit3);

            score = 300000;
        }

        public override bool Update()
        {
            worldTime--;
            if (worldTime > 0)
            {
                updateFruit();
                updateCritter();
            }
            
            return worldTime > 0;
        }

        private void updateFruit()
        {
            List<Entity> consumedFruit = new List<Entity>();
            foreach (Entity e in fruit)
            {
                e.update(this);

                float distanceFromFruit = Vector2.Distance(e.location, critter.location);
                if (distanceFromFruit < Math.Max(e.width, critter.width))
                {
                    consumedFruit.Add(e);
                }
                else
                {
                    score -= distanceFromFruit;
                }
            }

            foreach (Entity e in consumedFruit)
            {
                fruit.Remove(e);
                score += 50000;
            }

        }

        private void updateCritter()
        {
            float lastDist = float.MaxValue;
            Entity selectedFruit = null;
            foreach (Entity e in fruit)
            {
                float distance = Vector2.Distance(critter.location, e.location);
                if (distance < lastDist)
                {
                    lastDist = distance;
                    selectedFruit = e;
                }
            }

            if (selectedFruit != null)
            {
                critter.input(0, (int)critter.location.X);
                critter.input(1, (int)critter.location.Y);
                critter.input(2, (int)selectedFruit.location.X);
                critter.input(3, (int)selectedFruit.location.Y);
            }
            critter.runComputer();

            int moveX = critter.getOutput(4);
            int moveY = critter.getOutput(5);
            critter.impulse += Vector2.Normalize(new Vector2(moveX, moveY) * .25f);

            critter.update(this);
        }

        public override void draw(SpriteBatch batch)
        {
            critter.draw(batch);
            foreach (Entity e in fruit)
            {
                e.draw(batch);
            }
        }

        public override void setMachine(int[] machine)
        {
            critter = new StackCritter(machine);
        }

        public override StackComputer getMachine()
        {
            return ((StackCritter)critter).stackComputer;
        }

        public override float getScore()
        {
            return score;
        }
    }
}
