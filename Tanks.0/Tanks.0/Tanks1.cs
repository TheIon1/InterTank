using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Tanks
{
    class Tanks
    {
        // У танка есть координаты местоположения, направление движения выраженное целым числом, и размеры танка.
        public int coordX, coordY;
        public int destX, destY;
        //Размеры реализованы с помощью типа Rectangle, чтобы было удобно передавать значения от Lable.
        public Rectangle boundaries;

        #region LifeTime

        public Tanks(int x, int y, Rectangle bound, Random rand)
        {
            this.boundaries = bound;
            this.coordX = bound.X + bound.Width / 2;
            this.coordY = bound.Y + bound.Height / 2;

            //четыре направления движения
            this.destX = rand.Next(0, 2) * 2 - 1;
            this.destY = rand.Next(0, 2) * 2 - 1;
        }


        ~Tanks() { }

        #endregion

        #region Methods
        //Простое движение танка по траектории заданной destX и destY
        public void Move()
        {
            coordX += destX;
            coordY += destY;
        }

        //Изменение вектора направления движения танка в случае встречи препятствия
        public void Encounter()
        {
            //Проверяем направление и инвертируем в соответствии с ним.
            if (destX == -1 && destY == -1 || destX == 1 && destY == 1) 
                destX *= -1;
            else if (destX == 1 && destY == -1 || destX == -1 && destY == 1)
                destY *= -1;
        }

        #endregion
    }

    class Walls
    {
        public int coordX, coordY;
        public Rectangle boundaries;

        public Walls(int x, int y, Rectangle bound)
        {
            this.boundaries = bound;
            this.coordX = bound.X + bound.Width / 2;
            this.coordY = bound.Y + bound.Height / 2;

        }


        ~Walls() { }
    }
}
