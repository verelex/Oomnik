using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oomnik
{
    public partial class Form1 : Form
    {
        private static int rnd1, rnd2, sum, t3iter;

        private const String rules1 = "На экран последовательно выводятся ряды однозначных\n чисел по 6 в каждом, которые изображены в квадратах.\n Время экспозиции ряда - 3 секунды.\n Необходимо запомнить числа в том порядке, в котором\n они предъявлялись.\n По истечении 3-х секунд ряд чисел исчезает с экрана,\n а два квадрата подсвечиваются красным цветом.\n Необходимо вспомнить ряд чисел и сложить в уме два\n числа, которые отображались в помеченных квадратах.\n При помощи клавиатуры записать получившуюся сумму в\n поле ввода и нажать ENTER.\n На обдумывание и запись отводится 30 секунд. Если\n запись за это время не состоялась, то на экране\n появится следующий ряд чисел, а предыдущее задание\n считается невыполненным.\n  ";

        private static LinkedList<Label> listLabels;

        //Function to get random number
        private static readonly Random getrandom = new Random();

        public static int GetRandomNumber(int min, int max) // включительно
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max + 1);
            }
        }

        public Form1()
        {
            InitializeComponent();

            timer1.Tick += new EventHandler(timer1_Tick);
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Tick += new EventHandler(timer3_Tick);
            timer1.Interval = 3000; // интервал показа 6 чисел
            timer2.Interval = 30000; // интервал обдумывания и ввода суммы чисел
            timer3.Interval = 1000; // интервал отображения 30 сек --
            timer1.Enabled = false; // выключены
            timer2.Enabled = false;
            timer3.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listLabels = new LinkedList<Label>();

            listLabels.AddLast(label1);
            listLabels.AddLast(label2);
            listLabels.AddLast(label3);
            listLabels.AddLast(label4);
            listLabels.AddLast(label5);
            listLabels.AddLast(label6);

            int i = 1;

            for (LinkedListNode<Label> rn = listLabels.First; rn != null; rn = rn.Next)
            {
                var Val = rn.Value;
                Val.Name = "L" + i;
                rn.Value = Val;
                i++;
            }

            label7.Text = String.Empty; // очищаем вывод результатов
            label8.Text = String.Empty; // очищаем вывод отсчета 30 сек

            //----------------------------------------------------------------------------------

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 30000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 100;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(this.button1, rules1);

            labelRules.Text = rules1;
        }

        private void GenData() // основной раунд
        {
            timer1.Enabled = false;
            timer2.Enabled = false;

            foreach (var v in listLabels)
            {
                int x = GetRandomNumber(0, 9);
                v.Text = x.ToString();
                v.BackColor = Color.White; // цвет фона белый у всех квадратов
                v.ForeColor = Color.Black; // цвет шрифта черный у всех квадратов
            }

            // получаем два случайных значения от 0 до 5
            rnd1 = GetRandomNumber(0, 5);
            int x1 = Int32.Parse(listLabels.ElementAt(rnd1).Text);

            do
            {
                rnd2 = GetRandomNumber(0, 5);
            }
            while (rnd1 == rnd2);

            int x2 = Int32.Parse(listLabels.ElementAt(rnd2).Text);

            sum = x1 + x2; // сумма чисел в двух случайных квадратах

            timer1.Enabled = true; // начало ожидания 3 сек
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false; // скрываем кнопку запуска
            GenData();
            labelRules.Visible = false;
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text.Equals(sum.ToString()))
                {
                    label7.Text += "1"; // введен правильный ответ
                }
                else
                {
                    label7.Text += "0"; // введен неправильный ответ
                }
                timer2.Enabled = false; // остан. 30 сек интервал т.к. введено значение раньше срабатывания таймера
                timer3.Enabled = false;
                textBox1.Text = String.Empty; // очистка поля ввода
                GenData();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;  // чтобы не проигрывался звук предупреждения в винде
                e.SuppressKeyPress = true;
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.BackColor = Color.Yellow;
        }

        private void timer1_Tick(object sender, EventArgs e) // 3 секунды истекло, запуск 30 сек
        {
            timer1.Enabled = false; // останавливаем 1 таймер

            foreach (var v in listLabels)
            {
                v.ForeColor = Color.White; // шрифт белый, чтобы скрыть
            }
            listLabels.ElementAt(rnd1).BackColor = Color.Red; // подсвечиваем красным рандомные квадраты
            listLabels.ElementAt(rnd1).ForeColor = Color.Red;
            listLabels.ElementAt(rnd2).BackColor = Color.Red;
            listLabels.ElementAt(rnd2).ForeColor = Color.Red;

            timer2.Enabled = true; // отсчет 30 секунд
            t3iter = 30;
            timer3.Enabled = true;
        }

        private void timer2_Tick(object sender, EventArgs e) // 30 секунд истекло
        {

            if (timer2.Enabled == true) // почему-то он иногда срабатывает 2 раза
            {
                timer2.Enabled = false; // останавливаем 2 таймер
                timer3.Enabled = false; // останавливаем 3 таймер
                label7.Text += "0"; // invalid answer
                GenData();
            }
            
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (timer2.Enabled == true)
            {
                label8.Text = t3iter.ToString();
                t3iter--;
            }
        }

    }
}
