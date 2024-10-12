using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pyramid_XO
{
    public partial class Form1 : Form
    {
        private bool turn= false; /// assume x will start
        private bool winner = false;
        private int player1Pointes = 0;
        private int player2Pointes = 0;
        private int num_of_played_buttons = 0;
        private string wrong_sound_path = "wrong-sound.wav";
        private string win_sound_path = "winning-sound.wav";
        private string button_click_sound_path = "button-click.wav";
        Stack<Button> history = new Stack<Button>();
        
        public Form1()
        {
            InitializeComponent();
            this.button1.Click += (s, ev) => play(this.button1);
            this.button2.Click += (s, ev) => play(this.button2);
            this.button3.Click += (s, ev) => play(this.button3);
            this.button4.Click += (s, ev) => play(this.button4);
            this.button5.Click += (s, ev) => play(this.button5);
            this.button6.Click += (s, ev) => play(this.button6);
            this.button7.Click += (s, ev) => play(this.button7);
            this.button8.Click += (s, ev) => play(this.button8);
            this.button9.Click += (s, ev) => play(this.button9);

            this.winner_is_label.Visible = false;
            this.winner_label.Visible = false;
            this.play_again_button.Visible = false;
            this.exit_button.Visible = false;
            this.hard_trackBar1.Visible = false;
            this.hard_button_checkbox.Visible = false;
        }
        private async void play(Button clicked_button)
        {
            if(winner)
                return;

            if(num_of_played_buttons == 9)
            {
                play_sound();
                await Task.Run(() => {
                    AnimateButtonColorChange(reset_button, Color.White, Color.Blue,50,25);
                    Task.Delay(100);
                    AnimateButtonColorChange(reset_button, Color.Blue, Color.White, 50, 25);
                });
                return;
            }

            if (!is_avilable(clicked_button))
            {
                button_warning(clicked_button);
                return;
            }

            play_sound("click");

            System.Threading.Thread.Sleep(100);


            if (turn == false)
            {
                turn_of_label2.Text =  "O";
                clicked_button.Text = "X";
            }else
            {
                turn_of_label2.Text = "X";
                clicked_button.Text="O";
            }

            history.Push(clicked_button);
            check_if_winning();

            if (!winner)
            {
                num_of_played_buttons += 1;
                turn = !turn;


                if (turn == true && this.ai_radioButton.Checked && num_of_played_buttons < 9)
                {
                    await Task.Delay(500);
                    get_ai_move();
                }
            }

        }



        void button_warning(Button button)
        {
            play_sound("wrong");
            button_wrong_effict(button);
        }
        async void button_wrong_effict(Button button)
        {

            if (button.InvokeRequired)
            {
                // If the method is called from a different thread, use Invoke to execute it on the UI thread
                button.Invoke(new Action(() => button_wrong_effict(button)));
                return;
            }


            button.BackColor = Color.Red;
            AnimateButtonColorChange(button, Color.White , Color.Red);
            await Task.Run(async () =>
            {
                await Task.Delay(300);
                button.Invoke(new Action(() =>button.Location = new Point(button.Location.X, button.Location.Y -1)));
                button.Invoke(new Action(() =>button.Location = new Point(button.Location.X +5, button.Location.Y)));
                await Task.Delay(70);
                button.Invoke(new Action(() => button.Location = new Point(button.Location.X - 10, button.Location.Y)));
                await Task.Delay(70);
                button.Invoke(new Action(() => button.Location = new Point(button.Location.X + 10, button.Location.Y)));
                await Task.Delay(70);
                button.Invoke(new Action(() => button.Location = new Point(button.Location.X - 5, button.Location.Y)));
                button.Invoke(new Action(() =>button.Location = new Point(button.Location.X, button.Location.Y+1)));
                AnimateButtonColorChange(button, Color.Red , Color.White);
            });
        }

        void play_sound(string type="")
        {
            string soundFilePath = wrong_sound_path;
            if (type == "win")
                soundFilePath = win_sound_path;
            else if (type == "click")
                soundFilePath = button_click_sound_path;
            if (System.IO.File.Exists(soundFilePath))
            {
                using (var player = new SoundPlayer(soundFilePath))
                {
                    player.Play();
                }
            }
        }

        bool is_avilable(Button clicked_button)
        {
            return (clicked_button.Text != "X" && clicked_button.Text != "O"); 
        }

        private void AnimateButtonColorChange(Button button, Color startColor, Color endColor,int duration = 10,int steps = 10)
        {
/*            const int steps = 10; // Adjust as needed*/
/*            const int duration = 10; // milliseconds
*/
            for (int i = 0; i <= steps; i++)
            {
                float ratio = (float)i / steps;
                int r = (int)(startColor.R + (endColor.R - startColor.R) * ratio);
                int g = (int)(startColor.G + (endColor.G - startColor.G) * ratio);
                int b = (int)(startColor.B + (endColor.B - startColor.B) * ratio);

                button.BackColor = Color.FromArgb(r, g, b);

                System.Threading.Thread.Sleep(duration / steps);
                Application.DoEvents(); 
            }
        }


        bool check_if_winning(bool test=false)
        {
            bool b1_b2_b5= winning_game(button1, button2, button5);
            bool b1_b4_b9= winning_game(button1, button4, button9);
            bool b1_b3_b7= winning_game(button1, button3, button7);

            bool b2_b3_b4= winning_game(button2, button3, button4);

            bool b5_b6_b7= winning_game(button5, button6, button7);
            bool b6_b7_b8= winning_game(button6, button7, button8);
            bool b7_b8_b9= winning_game(button7, button8, button9);

            bool winning_status = b1_b2_b5|b1_b4_b9|b1_b3_b7|b2_b3_b4|b5_b6_b7|b6_b7_b8|b7_b8_b9;

            /// for computer player
            if (test && winning_status)
            {
                return true;
            }
            if(!winning_status)
                return false;

            Button b1=new Button(),b2 = new Button(), b3 = new Button();
            if (b1_b2_b5)
            {
                b1 = button1;
                b2 = button2;
                b3 = button5;
            }
            if (b1_b4_b9)
            {
                b1 = button1;
                b2 = button4;
                b3 = button9;
            }
            if (b1_b3_b7)
            {
                b1 = button1;
                b2 = button3;
                b3 = button7;
            }
            if (b2_b3_b4)
            {
                b1 = button2;
                b2 = button3;
                b3 = button4;
            }
            if (b5_b6_b7)
            {
                b1 = button5;
                b2 = button6;
                b3 = button7;
            }
            if (b6_b7_b8)
            {
                b1 = button6;
                b2 = button7;
                b3 = button8;
            }
            if (b7_b8_b9)
            {
                b1 = button7;
                b2 = button8;
                b3 = button9;
            }
            
            if(winning_status)
            {
                perform_winning_game(b1,b2,b3);
            }

            return winning_status;

        }

        bool winning_game(Button mybutton1, Button mybutton2, Button mybutton3)
        {
            if (mybutton1.Text == "" || mybutton2.Text == "" || mybutton3.Text==""||winner)
                return false;

            if((mybutton1.Text == mybutton2.Text && mybutton2.Text == mybutton3.Text))
            {
                return true;
            }
            /////// for game is end and no one is win
            else if(num_of_played_buttons == 8)
            {
                show_finished_game_buttons();
            }
            return false;
        }

        void show_finished_game_buttons()
        {
            this.play_again_button.Invoke(new Action(() => play_again_button.Visible = true));
            this.exit_button.Invoke(new Action(() => exit_button.Visible = true));

        }

        async void perform_winning_game(Button mybutton1, Button mybutton2, Button mybutton3)
        {
            winner = true;
            this.winner_is_label.Visible = true;
            this.winner_label.Text = turn ? "O" : "X";
            this.winner_label.Visible = true;

            this.turn_of_label.Visible = false;
            this.turn_of_label2.Visible = false;

            await Task.Run(() =>
            {
                Task.Delay(200);
                show_finished_game_buttons();
            });
            play_sound("win");
            change_winning_buttons_colors(mybutton1, mybutton2, mybutton3);
        }

        async void change_winning_buttons_colors(Button mybutton1, Button mybutton2, Button mybutton3)
        {
            await Task.Run(() =>{
                AnimateButtonColorChange(mybutton1, Color.White, Color.LightGreen);
                Task.Delay(200);
                AnimateButtonColorChange(mybutton2, Color.White, Color.LightGreen);
                Task.Delay(200);
                AnimateButtonColorChange(mybutton3, Color.White, Color.LightGreen);
            });
        }


        private void exit_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// reset the game
        void set_players_points()
        {
            if (turn == false && winner)
                player1Pointes += 1;
            else if (turn == true && winner)
                player2Pointes += 1;
            else
            {
                player1Pointes = 0;
                player2Pointes = 0;
            }
            player1_pointes_label.Text = player1_pointes_label.Text.Substring(0, 17) + player1Pointes.ToString("D");
            player2_pointes_label.Text = player2_pointes_label.Text.Substring(0, 17) + player2Pointes.ToString("D");
        }

        void rest_buttons()
        {
            button1.Text = ""; 
            button1.BackColor = Color.White; 
            button2.Text = ""; 
            button2.BackColor = Color.White; 
            button3.Text = ""; 
            button3.BackColor = Color.White; 
            button4.Text = ""; 
            button4.BackColor = Color.White; 
            button5.Text = ""; 
            button5.BackColor = Color.White; 
            button6.Text = ""; 
            button6.BackColor = Color.White; 
            button7.Text = ""; 
            button7.BackColor = Color.White; 
            button8.Text = ""; 
            button8.BackColor = Color.White; 
            button9.Text = ""; 
            button9.BackColor = Color.White; 
        }

        void reset_all_data()
        {
            set_players_points();
            rest_buttons();
            winner = false;
            this.winner_is_label.Visible = false;
            this.winner_label.Visible = false;
            this.play_again_button.Visible = false;
            this.exit_button.Visible = false;

            turn = false;

            this.turn_of_label.Visible = true;
            this.turn_of_label2.Visible = true;
            turn_of_label2.Text = "X";
            num_of_played_buttons = 0;
        }

        private void play_again_button_Click(object sender, EventArgs e)
        {
            reset_all_data();
            history.Clear();
        }

        private void reset_button_Click(object sender, EventArgs e)
        {
            winner=false;
            reset_all_data();
            history.Clear();
        }

        private void ai_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(ai_radioButton.Checked)
            {
                this.hard_button_checkbox.Visible= true;
                this.hard_button_checkbox.Checked= false;

                //this.hard_trackBar1.Visible= true;

                if (this.turn)
                    get_ai_move();
            }
            else
            {
                this.hard_button_checkbox.Visible = false;
                this.hard_trackBar1.Visible = false;
            }
        }
        ///// pc player
        private void MakeRandomMove()
        {
            var availableButtons = GetAvailableButtons();

            Random random = new Random();
            var randomIndex = random.Next(availableButtons.Count);
            availableButtons[randomIndex].PerformClick();
        }

        private List<Button> GetAvailableButtons()
        {
            return Controls.OfType<Button>().Where(b => b.Text == "").ToList();
        }


        private async void bestMove()
        {
            List<Button> availableButtons = GetAvailableButtons().OrderBy(b=>b.Name).ToList();
            int bestval =  int.MinValue ;
            int bestIndex = 0;
            await Task.Run(async () =>
            {
                for (int i = 0; i < availableButtons.Count; i++)
                {
                    int steps = 50;
                    availableButtons[i].Invoke(new Action(() => availableButtons[i].Text = "O"));
                    hard_trackBar1.Invoke(new Action(() => steps *= int.Parse(hard_trackBar1.Value.ToString()) *5));
                    int temp = miniMax(availableButtons,ref steps, true, true);
                    if (temp > bestval)
                    {
                        bestval = temp;
                        bestIndex = i;
                    }
                    await Task.Delay(10);
                    availableButtons[i].Invoke(new Action(() => availableButtons[i].Text = ""));

                }

                availableButtons[bestIndex].Invoke(new Action(() => availableButtons[bestIndex].PerformClick())) ;
            });
            
        }

        private int miniMax(List<Button> availableButtons,ref int depth,bool min = true, bool algo_turn = true,int steps_passed = 1)
        {
            List<Button> availableButtons2= availableButtons.Where(b => b.Text == "").ToList(); ;

            if (check_if_winning(true))
            {
                if (algo_turn == false)
                    return -10 + steps_passed; 
                return 10- steps_passed;

            }else if (availableButtons2.Count == 0 || depth <=0)
            {
                return 0;
            }

            int bestValue = min ? int.MaxValue : int.MinValue;

            for (int i = 0; i < availableButtons2.Count; i++)
            {

                availableButtons2[i].Invoke(new Action(() => availableButtons2[i].Text = algo_turn ? "X" : "O"));
                int val=0;
                hard_trackBar1.Invoke(new Action(() => val = int.Parse(hard_trackBar1.Value.ToString())));
                if(val != 5)
                    depth--;
                int currentValue = miniMax(availableButtons2,ref depth,!min, !algo_turn, steps_passed+1);
                if (min)
                    bestValue = Math.Min(bestValue, currentValue);
                else
                    bestValue = Math.Max(bestValue, currentValue);
                availableButtons2[i].Invoke(new Action(() => availableButtons2[i].Text = ""));

            }

            return bestValue;
        }

        void get_ai_move()
        {
            if(this.hard_button_checkbox.Checked)
            {
                bestMove();
            }else
                MakeRandomMove();
        }

        private void hard_button_checkbox_CheckedChanged(object sender, EventArgs e)
        {
            if(hard_button_checkbox.Checked)
            {
                hard_trackBar1.Visible = true;
            }else
                hard_trackBar1.Visible=false;
        }

        private void step_back_button_Click(object sender, EventArgs e)
        {
            if(history.Count > 0 && num_of_played_buttons > 0 && !winner)
            {
                history.Pop().Text = "";
                turn = !turn;
                turn_of_label2.Text = turn_of_label2.Text == "X" ? "O" : "X";
                num_of_played_buttons -= 1;
                winner_label.Visible = false;
                winner_is_label.Visible = false;
            }
        }
    }
}
