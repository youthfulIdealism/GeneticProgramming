using GeneticProgramming.Environment;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticProgramming.ExampleProjects.Fruit
{
    public class Fruit : Entity
    {
        public Fruit() : base()
        {
            color = Color.Red;
            texture = Driver.blobTex;
        }

    }
}
