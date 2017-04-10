using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticProgramming.Environment
{
    public abstract class Entity
    {
        public Texture2D texture { get; set; }
        public Color color { get; set; }

        public float width { get; set; }
        public Vector2 location { get; set; }
        public Vector2 velocity { get; set; }
        public Vector2 impulse { get; set; }

        public Entity()
        {
            width = 100;
            location = new Vector2();
            velocity = new Vector2();
            impulse = new Vector2();
            color = Color.White;
        }
        
        public virtual void update(World world)
        {
            velocity *= .9f;
            velocity += impulse;
            location += velocity;
            impulse = new Vector2();
        }

        public virtual void draw(SpriteBatch batch)
        {
            if(texture != null)
            {
                Rectangle drawRect = new Rectangle((int)location.X, (int)location.Y, (int)width, (int)width);

                batch.Draw(texture, drawRect, color);
            }
        }
    }
}
