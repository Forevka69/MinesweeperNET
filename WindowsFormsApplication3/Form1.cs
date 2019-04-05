﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        public Game g;
        public Form1()
        {
            InitializeComponent();
        }

        public void calc_size(int f_w, int f_h, int f_h_off, int f_v_off, int b_size)
        {
            this.Size = new Size(4+(b_size) * (f_w+1), f_v_off*2 + ((b_size) * f_h));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            g = new Game(this, 10, 10, 25);
        }
    }

    public class Game : Form
    {
        private int time_clock;
        public int field_width;
        public int field_height;
        public static bool end = false;
        public static bool start = false;
        public int opened = 0;
        public int bombs_count;
        public GameTimer my_timer;
        public Point[] with_bombs;
        public MyButton[,] field_stats;
        public MyButton smiley_button;
        public static Image img_untouched;
        public static Image img_flagged;
        public static Image img_bombed;
        public static Image img_bomb_win;
        public static Image[] smiley;
        public static Image[] numbers;
        public static Image[] my_font;
        public static MoveCounter move_counter;
        public PictureBox[] timer_pic;
        public PictureBox[] move_count;
        public Form1 father;


        public Game(Form1 father, int width, int height, int bomb_count)
        {
            field_width = width;
            field_height = height;
            this.bombs_count = bomb_count;
            this.father = father;



            with_bombs = new Point[bomb_count];
            field_stats = new MyButton[field_width, field_height];
            load_assets();
            
            
            timer_pic = new PictureBox[3];
            move_count = new PictureBox[3];
            my_timer = new GameTimer(this.timer_pic);

            start_game();
            foreach (Point p in this.with_bombs)
            {
                Console.WriteLine("x {0} y {1}", p.X, p.Y);
            }
            //this.smiley_button.Focus();
            
        }

        public void start_game()
        {
            int button_size = 16;
            int field_horizontal_offset = 2;
            int field_vertical_offset = 50;

            Random rand = new Random();
            Form1 father_form = this.father;

            for (int i = 0; i < 3; i++)
            {
                timer_pic[i] = new PictureBox();
                timer_pic[i].Location = new Point(button_size / 2 + i * 14, 13);
                timer_pic[i].Size = new Size(14, 23);
                timer_pic[i].Image = my_font[0];
                father_form.Controls.Add(timer_pic[i]);

                move_count[i] = new PictureBox();
                move_count[i].Location = new Point(field_width * button_size - button_size / 2 - (3 - i) * 14, 13);
                move_count[i].Size = new Size(14, 23);
                move_count[i].Image = my_font[0];
                father_form.Controls.Add(move_count[i]);
            }
            move_counter = new MoveCounter(move_count);
            for (int i = 0; i < this.bombs_count; i++)
            {
                Point t = new Point(rand.Next(field_width), rand.Next(field_height));
                while (with_bomb(t))
                {
                    t = new Point(rand.Next(field_width), rand.Next(field_height));
                }
                this.with_bombs[i] = t;
            }

            this.smiley_button = new MyButton(this, -1, -1);
            this.smiley_button.Image = Game.smiley[0];
            this.smiley_button.BackColor = Color.Transparent;
            this.smiley_button.Size = new Size(26, 26);
            this.smiley_button.Location = new Point(field_width * button_size / 2 - 13, 13);
            this.smiley_button.MouseUp += new MouseEventHandler(this.smiley_button.on_click_up_smile);
            this.smiley_button.MouseDown += new MouseEventHandler(this.smiley_button.on_click_down_smile);
            father_form.Controls.Add(this.smiley_button);

            for (int i = 0; i < field_width; i++)
            {
                for (int j = 0; j < field_height; j++)
                {
                    MyButton newButton = new MyButton(this, i, j);
                    newButton.Image = img_untouched;
                    newButton.BackColor = Color.Transparent;
                    newButton.Size = new Size(button_size, button_size);
                    newButton.MouseUp += new MouseEventHandler(newButton.on_click_up);
                    newButton.MouseDown += new MouseEventHandler(newButton.on_click_down);
                    newButton.Location = new Point(field_horizontal_offset + i * button_size,
                                                    field_vertical_offset + j * button_size);
                    father_form.Controls.Add(newButton);

                    field_stats[i, j] = newButton;
                }
            }
            father_form.calc_size(field_width, field_height, field_horizontal_offset, field_vertical_offset, button_size);
            //my_timer = new GameTimer(this.timer_pic);
        }

        public void delete_game()
        {
            foreach (MyButton btn_row in field_stats)
            {
                Console.WriteLine(btn_row);
                father.Controls.Remove(btn_row);
            }
            for (int i = 0; i < 3; i++)
            {
                father.Controls.Remove(timer_pic[i]);
                father.Controls.Remove(move_count[i]);
            }
            father.Controls.Remove(this.smiley_button);
            my_timer.restart();// = false;
            Game.end = false;
            Game.start = false;
        }

        /*public void start_timer()
        {
            //my_timer.Enabled = false;
            my_timer = new GameTimer(this.timer_pic);
        }*/

        public bool with_bomb(Point t)
        {
            Console.WriteLine("x {0} y {1}", t.X, t.Y);
            foreach(Point p in this.with_bombs)
            {
                if (p == t)
                    return true;
            }
            return false;
        }

        public int bomb_count(MyButton btn)
        {
            //Console.WriteLine("OCLOCK {0}",my_timer.get_time());
            //num_to_font(my_timer.get_time());
            int start_x = btn.pos.X;
            int start_y = btn.pos.Y;
            int count = 0;
            this.Opened += 1;
            for (int i = -1 + start_x; i < 2 + start_x; i++)
            {
                for (int j = -1 + start_y; j < 2 + start_y; j++)
                {
                    Point t = new Point(i, j);
                    if (with_bomb(t))
                    {
                        count++;
                    }
                }
            }
            if (count == 0)
            {
                //btn.Image = Game.numbers[count];
                //btn.Enabled = false;
                List<MyButton> b_list = new List<MyButton>();
                b_list.Add(btn);
                for (int i = -1 + start_x; i < 2 + start_x; i++)
                {
                    for (int j = -1 + start_y; j < 2 + start_y; j++)
                    {
                        if (i >= 0 && i < this.field_width && j >= 0 && j < this.field_height)
                        {
                            MyButton neigh_btn = this.field_stats[i, j];
                            this.open_if_no_bomb(neigh_btn, b_list);
                        }
                    }
                }
            }
            return count;
        }

        public int open_if_no_bomb(MyButton btn, List<MyButton> except_button)
        {
            int start_x = btn.pos.X;
            int start_y = btn.pos.Y;
            int count = 0;

            for (int i = -1 + start_x; i < 2 + start_x; i++)
            {
                for (int j = -1 + start_y; j < 2 + start_y; j++)
                {
                    Point t = new Point(i, j);
                    if (with_bomb(t))
                    {
                        count++;
                    }
                }
            }
            
            if (count == 0)
            {
                //this.Opened += 1;
                btn.Image = Game.numbers[count];
                btn.Enabled = false;
                for (int i = -1 + start_x; i < 2 + start_x; i++)
                {
                    for (int j = -1 + start_y; j < 2 + start_y; j++)
                    {
                        if (i >= 0 && i < this.field_width && j >= 0 && j < this.field_height)
                        {
                            MyButton neigh_btn = this.field_stats[i, j];
                            if (!except_button.Contains(neigh_btn))
                            {
                                this.Opened += 1;
                                except_button.Add(neigh_btn);
                                this.open_if_no_bomb(neigh_btn, except_button);
                            }
                        }
                    }
                }
            }else
            {
                //this.Opened += 1;
                btn.Image = Game.numbers[count];
                btn.Enabled = false;
            }
            
            if (!have_opened())
            {
                smiley_button.Image = Game.smiley[2];
                blow_up_win();
                Console.WriteLine("END ONLY BOMBS");
            }
            return count;
        }

        public void blow_up()
        {
            Game.end = true;
            foreach (Point p in this.with_bombs)
            {
                MyButton mb = this.field_stats[p.X, p.Y];
                mb.explode();
            }
        }

        public void blow_up_win()
        {
            Game.end = true;
            foreach (Point p in this.with_bombs)
            {
                Console.WriteLine("blowing bomb");
                MyButton mb = this.field_stats[p.X, p.Y];
                mb.explode_win();
            }
        }

        public bool have_opened()
        {
            //int o = (this.field_width * this.field_height - this.Opened);
            Console.WriteLine("HAVE OPENED {0} bombs count {1}", this.Opened, this.bombs_count);
            if (this.Opened + this.bombs_count == this.field_width * this.field_height)
                return false;
            return true;
        }

        public int Opened
        {
            get { return opened; }
            set { opened = value; }
        }

        public static List<Image> num_to_font(int number)
        {
            string s_n = number.ToString();
            List<Image> font_list = new List<Image>();
            foreach (char i in s_n)
            {
                //Console.WriteLine(i);
                font_list.Add(Game.my_font[i - '0']);
            }
            for(int i = font_list.Count; i < 3;i++)
                font_list.Insert(0, Game.my_font[0]);
            return font_list;
        }

        private void load_assets()
        {
            img_untouched = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\untouched.png");
            img_flagged = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\flag.png");
            img_bombed = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\bomb.png");
            img_bomb_win = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\bomb_win.png");
            Game.numbers = new Image[9];
            Game.numbers[0] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\zero.png");
            Game.numbers[1] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\one.png");
            Game.numbers[2] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\two.png");
            Game.numbers[3] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\three.png");
            Game.numbers[4] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\four.png");
            Game.numbers[5] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\five.png");
            Game.numbers[6] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\six.png");
            Game.numbers[7] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\seven.png");
            Game.numbers[8] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\eight.png");
            Game.smiley = new Image[4];
            Game.smiley[0] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\smiley.png");
            Game.smiley[1] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\smiley_ou.png");
            Game.smiley[2] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\smiley_win.png");
            Game.smiley[3] = Image.FromFile("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\smiley_defeat.png");
            Game.my_font = new Image[10];
            for(int i=0;i<10;i++)
            {
                string load_path = String.Format("F:\\csharp\\WindowsFormsApplication3\\WindowsFormsApplication3\\assets\\font\\{0}.png", i);
                Game.my_font[i] = Image.FromFile(load_path);
            }
        }
    }
}
