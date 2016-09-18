using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tanks._0
{


    public partial class Form1 : Form
    {
        private int counter;

        private List<Tanks> t;

        private List<Walls> w;

        //Инициализация таймера
        private void InitializeTimer()
        {
            counter = 0;
            timer1.Interval = -1 * trackBar1.Value;
            timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

        }

        #region Game logic

        #region Encounter logic

        //Проверяем, не столкнулись ли с какой-либо из границ
        private bool isEncounterWithBorder(Tanks T)
        {
            if (T.coordX - T.boundaries.Width / 2 <= 0)
            {
                T.destX = Math.Abs(T.destX);
                return true;
            }
            if (T.coordY - T.boundaries.Height / 2 <= 0)
            {
                T.destY = Math.Abs(T.destY);
                return true;
            }
            if (T.coordX + T.boundaries.Size.Width / 2 >= this.Controls.Owner.Bounds.Width)
            {
                T.destX = -Math.Abs(T.destX);
                return true;
            }
            if (T.coordY + T.boundaries.Size.Height / 2 >= trackBar1.Bounds.Top)
            {
                T.destY = -Math.Abs(T.destY);
                return true;
            }
            return false;
        }

        private bool isEncounterWithBorder(Walls W)
        {
                if (W.coordX - W.boundaries.Width / 2 <= 0)
                {
                    return true;
                }
                if (W.coordY - W.boundaries.Height / 2 <= 0)
                {
                    return true;
                }
                if (W.coordX + W.boundaries.Size.Width / 2 >= this.Controls.Owner.Bounds.Width)
                {
                    return true;
                }
                if (W.coordY + W.boundaries.Size.Height / 2 >= trackBar1.Bounds.Top)
                {
                    return true;
                }
            return false;
        }

        //Проверяем, не столкнулись ли с объектом того же типа
        private bool isEncounterSameType(Tanks T)
        {
            for (int k = 0; k < t.Count; k++)
            {
                //Исключаем столкновение с самим собой
                if (T.coordX != t[k].coordX && T.coordY != t[k].coordY)
                {
                    if (Math.Abs(T.coordX - t[k].coordX) < ((T.boundaries.Width + t[k].boundaries.Width) / 2) && (Math.Abs(T.coordY - t[k].coordY) < ((T.boundaries.Height + t[k].boundaries.Height) / 2)))
                    {
                        if (Math.Abs(T.coordX - t[k].coordX) > Math.Abs(T.coordY - t[k].coordY))
                        {
                            if (T.coordX < t[k].coordX)
                            {
                                T.destX = -Math.Abs(T.destX);
                                t[k].destX = Math.Abs(t[k].destX);
                            }
                            else
                            {
                                T.destX = Math.Abs(T.destX);
                                t[k].destX = -Math.Abs(t[k].destX);
                            }
                        }
                        else
                        {
                            if (T.coordY < t[k].coordY)
                            {
                                T.destY = -Math.Abs(T.destY);
                                t[k].destY = Math.Abs(t[k].destY);
                            }
                            else
                            {
                                T.destY = Math.Abs(T.destY);
                                t[k].destY = -Math.Abs(t[k].destY);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //Проверяем, не столкнулись ли с объектом того же типа
        private bool isEncounterSameType(Walls W)
        {
            for (int k = 0; k < w.Count; k++)
            {
                if (W.coordX != w[k].coordX && W.coordY != w[k].coordY)
                {
                    if (Math.Abs(W.coordX - w[k].coordX) < ((W.boundaries.Width + w[k].boundaries.Width) / 2) && (Math.Abs(W.coordY - w[k].coordY) < ((W.boundaries.Height + w[k].boundaries.Height) / 2)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Проверяем, не столкнулись ли с объектом другого типа
        private bool isEncounterOtherType(Tanks T, Walls W)
        {
            if (W.coordX != T.coordX && W.coordY != T.coordY)
            {
                if (Math.Abs(W.coordX - T.coordX) < ((W.boundaries.Width + T.boundaries.Width) / 2) && (Math.Abs(W.coordY - T.coordY) < ((W.boundaries.Height + T.boundaries.Height) / 2)))
                {
                    if (Math.Abs(T.coordX - W.coordX) > Math.Abs(T.coordY - W.coordY))
                    {
                        if (T.coordX < W.coordX)
                        {
                            T.destX = -Math.Abs(T.destX);
                        }
                        else
                        {
                            T.destX = Math.Abs(T.destX);
                        }
                    }
                    else
                    {
                        if (T.coordY < W.coordY)
                        {
                            T.destY = -Math.Abs(T.destY);
                        }
                        else
                        {
                            T.destY = Math.Abs(T.destY);
                        }
                    }

                    return true;
                }
            }
            return false;
        }

        #endregion

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            //При первом тике таймера собираем информацию о танках и стенах, т.к. их расставили независимо от игровой логики. 
            if (counter == 0)
            {
                Random rand = new Random();
                foreach(Label label in this.Controls.OfType<Label>())
                {
                    t.Add(new Tanks(label.Left, label.Top, label.Bounds, rand));
                }
                foreach(Button button in this.Controls.OfType<Button>())
                {
                    w.Add(new Walls(button.Left, button.Top, button.Bounds));
                }
                //Проверка на правильность расположения.
                #region StartTry
                foreach (Tanks T in t)
                {
                    if (isEncounterWithBorder(T))
                    {
                        MessageBox.Show("One or many tanks out of border", "Error");
                        this.Close();
                    }
                    foreach (Walls W in w)
                    {
                        if (isEncounterOtherType(T,W))
                        {
                            MessageBox.Show("One or many tanks on walls", "Error");
                            this.Close();
                        }
                    }
                    if(isEncounterSameType(T))
                    {
                        MessageBox.Show("One or many tanks on other tank", "Error");
                        this.Close();
                    }
                }
                foreach (Walls W in w)
                {
                    if (isEncounterWithBorder(W))
                    {
                        MessageBox.Show("One or many walls out of border", "Error");
                        this.Close();
                    }
/*                    foreach (Tanks T in t)
                    {
                        if (isEncounterOtherType(T,W))
                        {
                            MessageBox.Show("One or many walls on tank", "Error");
                            this.Close();
                        }
                    }*/
                    if (isEncounterSameType(W))
                    {
                        MessageBox.Show("One or many walls on other walls", "Error");
                        this.Close();
                    }
                }
                #endregion
            }

            //переменная требуется для удобной реализации передачи текущего игрового значения на форму
            int countForLable = 0;
            foreach(Tanks T in t)
            {
                countForLable++;
                //Пробуем просто двинуться
                T.Move();
                //Проверяем, не столкнулись ли с какой-либо из границ
                if (isEncounterWithBorder(T))
                {
                }
                //Проверяем, не столкнулись ли с какой-либо из стен
                foreach (Walls W in w)
                {
                    if(isEncounterOtherType(T, W))
                    {
                    }
                }
                //Проверяем, не столкнулись ли с другим танком
                if (isEncounterSameType(T))
                {
                }
                int i = 0;
                //Передаём текущее состояние танка на форму.
                foreach (Label label in this.Controls.OfType<Label>())
                {
                    i++;
                    if(i==countForLable)
                    {
                        label.Location = new Point(T.coordX - T.boundaries.Width / 2, T.coordY - T.boundaries.Height / 2);
                        label.Update();
                    }
                }
            }
            counter++;
        }

        #endregion

        public Form1()
        {
            InitializeComponent();
            //устанавливаем начальную скорость 5 пикселов/сек.
            trackBar1.Value = -250;
            //Инициализируем таймер и списки танков и стен.
            InitializeTimer();
            t = new List<Tanks>();
            w = new List<Walls>();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        //Изменяя положение трекбара изменяем скорость. Реализовано для удобства пользователя, чтобы двигая скролл вправо он увеличивал скорость.
        //21 отметка слева - 0 пикселов/cек.
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if(!timer1.Enabled)
            {
                timer1.Start();
            }
            timer1.Interval = -1 * trackBar1.Value;
            if(trackBar1.Value == trackBar1.Minimum)
            {
                timer1.Stop();
            }
        }
    }
}
