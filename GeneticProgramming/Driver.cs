using GeneticProgramming.Computer;
using GeneticProgramming.Environment;
using GeneticProgramming.ExampleProjects.Fruit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GeneticProgramming
{
    /**
     * 
     * Drives the simulation, while handling rendering and input.
     * 
     * */
    public class Driver : Game
    {
        //This defines the number of threads that try to run, as well as the population size.
        public const int critterThreads = 50;

        //Algorithm properties for easy change
        public const float mutationRate = .25f;

        static Random rand = new Random();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        bool hasLoadedContent = false;
        bool hasSetUp = false;
        bool displaying = true;

        //The program gives us acess to one texture for drawing: a circle. Use it wisely!
        public static Texture2D blobTex;

        //One world at a time can be displayed to the user, enabling him to check the progress on his problem.
        private World displayWorld;
        
        int generationCount = 0;
        public static List<ComputerScore> currentGeneration;
        public static List<int[]> nextGeneration;
        public static List<int[]> hallOfFame; //Keeps a list of the hightest-performing computers for user examination.
        
        //Tracks data so that even when not displaying a world, the player can see how, in general, the algorithm is performing.
        public static List<int> averageScores = new List<int>();
        public static List<int> maxScores = new List<int>();
        public static List<int> topContendersBreakpointAvg;
        public static int hallOfFameScore = 1;

        /**
         * 
         * Called when a world is done executing. (WARNING: DON'T CALL THIS YOURSELF!)
         * Adds a critter's score to the processing list and checks it against the hall
         * of fame.
         * 
         * */
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public static void scoreCritter(int score, int[] computer, int breakpoint)
        {
            ComputerScore entry = new ComputerScore(score, computer, breakpoint);
            currentGeneration.Add(entry);

            if(entry.score > hallOfFameScore)
            {
                hallOfFameScore = entry.score;
                hallOfFame.Add(entry.computer);
                Console.WriteLine("hall of fame entry added: " + score);
            }
        }

        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public static bool areCrittersDone()
        {
            return currentGeneration.Count == critterThreads;
        }

        /**
         * 
         * Workhorse method that performs a random selection of computers driven by
         * their likelyhood to survive (...in turn defined by their score), pairs them up,
         * produces offspring, and performs mutation.
         * 
         * */
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public static void naturalSelection()
        {
            nextGeneration.Clear();
            int totalScore = 0;
            currentGeneration.Sort((a, b) => b.score.CompareTo(a.score));

            //collect score data for display
            maxScores.Add(currentGeneration[0].score);
            foreach (ComputerScore score in currentGeneration)
            {
                totalScore += score.score;
            }
            averageScores.Add(totalScore / currentGeneration.Count);

            //automatically include the top contenders in the next generation.
            //If the top contenders are mutated, their breakpoints will be used.
            int topContendersAmt = critterThreads / 6;
            int[] breakpoints = new int[topContendersAmt];
            int breakpointCounter = 0;
            for (int i = 0; i < topContendersAmt; i++)
            {
                nextGeneration.Add(currentGeneration[i].computer);
                breakpoints[i] = currentGeneration[i].breakpoint;
                breakpointCounter += currentGeneration[i].breakpoint;
            }
            topContendersBreakpointAvg.Add(breakpointCounter / topContendersAmt);

            int p = critterThreads / 5;
            while(nextGeneration.Count <= critterThreads)
            {
                //take several random selections, and grab the one with the highest score.
                ComputerScore mother = currentGeneration[rand.Next(currentGeneration.Count)];
                ComputerScore father = currentGeneration[rand.Next(currentGeneration.Count)];
                for (int i = 0; i < 5; i++)
                {
                    ComputerScore altMother = currentGeneration[rand.Next(currentGeneration.Count)];
                    if(altMother.score > mother.score) { mother = altMother; }
                    ComputerScore altFather = currentGeneration[rand.Next(currentGeneration.Count)];
                    if (altFather.score > father.score) { father = altFather; }
                }

                int[] fatherMachine = father.computer;
                int[] motherMachine = mother.computer;
                int combiningPoint = rand.Next(StackComputer.tapeSize);

                int[] childMachine = new int[StackComputer.tapeSize];
                for(int k = 0; k < combiningPoint; k++)
                {
                    childMachine[k] = fatherMachine[k];
                }
                for (int k = combiningPoint; k < childMachine.Length; k++)
                {
                    childMachine[k] = motherMachine[k];
                }
                nextGeneration.Add(childMachine);
                p++;
            }

            //mutate 25% of contestants
            for (int i = 0; i < critterThreads * mutationRate; i++)
            {
                int accessed = rand.Next(nextGeneration.Count);
                if (accessed >= topContendersAmt)
                {
                    nextGeneration[accessed][rand.Next(StackComputer.tapeSize)] = rand.Next(StackComputer.tapeSize);
                }else
                {
                    if(breakpoints[accessed] < 0)
                    {
                        nextGeneration[accessed][rand.Next(StackComputer.tapeSize)] = rand.Next(StackComputer.tapeSize);
                    }
                    else
                    {
                        nextGeneration[accessed][breakpoints[accessed]] = rand.Next(9);
                    }
                    
                }
                
            }
        }

        public static void printComputer(int[] computer)
        {
            Console.Write("computer: ");
            foreach (int instr in computer)
            {
                switch (instr)
                {
                    case 0:
                        Console.Write("JUMP(" + instr + ")");
                        break;
                    case 1:
                        Console.Write("PUSH(" + instr + ")");
                        break;
                    case 2:
                        Console.Write("POP(" + instr + ")");
                        break;
                    case 3:
                        Console.Write("ADD(" + instr + ")");
                        break;
                    case 4:
                        Console.Write("SUB(" + instr + ")");
                        break;
                    case 5:
                        Console.Write("INC(" + instr + ")");
                        break;
                    case 6:
                        Console.Write("DEC(" + instr + ")");
                        break;
                    case 7:
                        Console.Write("READPUSH(" + instr + ")");
                        break;
                    case 8:
                        Console.Write("WRITEPOP(" + instr + ")");
                        break;
                    default:
                        Console.Write(instr + " ");
                        break;

                }

            }
            Console.WriteLine();
        }


        public Driver()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 700;
            currentGeneration = new List<ComputerScore>();
            nextGeneration = new List<int[]>();
            hallOfFame = new List<int[]>();
            topContendersBreakpointAvg = new List<int>();
            rand = new Random();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            prevKeyboardState = Keyboard.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            blobTex = Content.Load<Texture2D>("Graphics/Blob");
            // TODO: use this.Content to load your game content here
            hasLoadedContent = true;
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        bool wantsToDisplay = false;
        KeyboardState prevKeyboardState;

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            //Enables the player to print out a list of the highest-performing computers
            if(Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                foreach (int[] computer in hallOfFame)
                {
                    printComputer(computer);
                }
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            //Enables the player to toggle the display on and off.
            //(If the display is on, it significantly reduces the
            //program's speed.)
            if (Keyboard.GetState().IsKeyDown(Keys.W) && prevKeyboardState.IsKeyUp(Keys.W))
            {
                wantsToDisplay = !wantsToDisplay;
            }

            if (hasLoadedContent)
            {
                if (!hasSetUp)
                {
                    for(int i = 0; i < critterThreads + 1; i++)
                    {
                        nextGeneration.Add(new StackCritter(rand).stackComputer.startTape);
                        //nextGeneration.Add(new int[] { 7, 15, 29, 31, 8, 21, 7, 1, 27, 7, 7, 33, 6, 6, 7, 14, 8, 10, 13, 8, 21, 32, 20, 20, 24, 7, 3, 1, 1, 7, 31, 5, 8, 3, 3, 31, 21, 39, 22, 4 });
                        //nextGeneration.Add(new int[] { 7, 15, 29, 31, 8, 21, 7, 15, 29, 31, 8, 21, 7, 1, 27, 7, 7, 33, 6, 6, 7, 14, 8, 10, 13, 8, 21, 32, 20, 20, 24, 7, 3, 1, 1, 7, 31, 5, 8, 3, 3, 31, 21, 39, 22, 4 });
                    }
                    createNewWorlds();
                }
                if(displaying)
                {
                    if (!displayWorld.Update())
                    {
                        if (areCrittersDone())
                        {
                            displaying = wantsToDisplay;
                            createNewWorlds();
                        }
                    }
                }
                else
                {
                    while(displayWorld.Update())
                    {

                    }
                    if (areCrittersDone())
                    {
                        displaying = wantsToDisplay;
                        createNewWorlds();
                        
                    }
                }
                



            }

            prevKeyboardState = Keyboard.GetState();
            base.Update(gameTime);
        }

        public void createNewWorlds()
        {
            //don't run natural selection the first time
            if(currentGeneration.Count > 0)
            {
                naturalSelection();
            }
            
            currentGeneration.Clear();
            Thread[] workers = new Thread[critterThreads];
            for (int i = 0; i < critterThreads; i++)
            {
                Thread childThread = new Thread(createAndRunWorld);
                workers[i] = childThread;
                childThread.Start(nextGeneration[i]);
                
            }

            for (int i = 0; i < critterThreads; i++)
            {
                workers[i].Join();
            }

            displayWorld = new FruitWorld();
            displayWorld.setMachine(nextGeneration[critterThreads]);
            
            hasSetUp = true;
            generationCount++;
            Console.WriteLine("generation " + generationCount + " taking off.");
            Console.WriteLine(hallOfFameScore);
        }

        public void createAndRunWorld(object machine)
        {
            World world = new FruitWorld();
            world.setMachine((int[])machine);
            
            //Run a problem world to its conclusion.
            while(world.Update()){}

            scoreCritter((int)world.getScore(), world.getMachine().startTape, world.getMachine().breakpoint);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            if (hasLoadedContent && hasSetUp && displaying)
            {
                displayWorld.draw(spriteBatch);
            }

            for(int i = 0; i < hallOfFame.Count; i++)
            {
                spriteBatch.Draw(blobTex, new Rectangle(20 * i, 20, 20, 20), Color.Black);
            }


            for (int i = 0; i < averageScores.Count; i++)
            {
                spriteBatch.Draw(blobTex, new Rectangle((int)(((float)i / averageScores.Count) * graphics.PreferredBackBufferWidth), graphics.PreferredBackBufferHeight - (int)(((float)averageScores[i] / hallOfFameScore) * graphics.PreferredBackBufferHeight), 3, 3), Color.Red);
                spriteBatch.Draw(blobTex, new Rectangle((int)(((float)i / averageScores.Count) * graphics.PreferredBackBufferWidth), graphics.PreferredBackBufferHeight - (int)(((float)maxScores[i] / hallOfFameScore) * graphics.PreferredBackBufferHeight), 3, 3), Color.White);
                spriteBatch.Draw(blobTex, new Rectangle((int)(((float)i / averageScores.Count) * graphics.PreferredBackBufferWidth), graphics.PreferredBackBufferHeight - (int)(((float)topContendersBreakpointAvg[i] / StackComputer.tapeSize) * graphics.PreferredBackBufferHeight), 3, 3), Color.Green);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
