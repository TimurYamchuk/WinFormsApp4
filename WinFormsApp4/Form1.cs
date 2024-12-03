using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer; // ���������� Timer �� System.Windows.Forms

namespace NumberGame
{
    public partial class Form1 : Form
    {
        private Button[] buttons = new Button[16];
        private List<int> buttonValues = new List<int>(); // ������ �������� �� �������
        private int nextNumber = 0; // ��������� ��������� �����
        private Timer gameTimer; // ������ ����
        private int timeLimit; // ����� ���� � ��������
        private NumericUpDown timeInput;
        private Label timerLabel;
        private Label currentNumberLabel;
        private Label messageLabel;
        private ProgressBar progressBar;
        private Random random = new Random();
        private Button startButton;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            // ��������� ������� �����
            this.Text = "Number Game";
            this.Size = new Size(450, 550);
            this.StartPosition = FormStartPosition.CenterScreen; // ���������� ����
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // ������������� ������ ����
            this.MaximizeBox = false; // ��������� ������ ������������ ����

            // �������� ���������� ��� �������
            var timeLabel = new Label
            {
                Text = "Set Time (sec):",
                Location = new Point(50, 20),
                Size = new Size(100, 30)
            };
            this.Controls.Add(timeLabel);

            timeInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 60,
                Value = 30,
                Location = new Point(160, 20),
                Size = new Size(50, 30)
            };
            this.Controls.Add(timeInput);

            startButton = new Button
            {
                Text = "Start Game",
                Location = new Point(230, 20),
                Size = new Size(100, 30)
            };
            startButton.Click += StartGame;
            this.Controls.Add(startButton);

            timerLabel = new Label
            {
                Text = "Time: 0 sec",
                Location = new Point(180, 60),
                Size = new Size(100, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(timerLabel);

            currentNumberLabel = new Label
            {
                Text = $"Next Number: {nextNumber}",
                Location = new Point(150, 100),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(currentNumberLabel);

            messageLabel = new Label
            {
                Text = "",
                Location = new Point(150, 130),
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Red
            };
            this.Controls.Add(messageLabel);

            progressBar = new ProgressBar
            {
                Maximum = 60,
                Value = 0,
                Location = new Point(50, 170),
                Size = new Size(350, 20)
            };
            this.Controls.Add(progressBar);

            // ������� � ��������� ������
            for (int i = 0; i < 16; i++)
            {
                buttons[i] = new Button
                {
                    Size = new Size(50, 50),
                    Location = new Point(50 + (i % 4) * 80, 210 + (i / 4) * 80),
                    Font = new Font("Arial", 12),
                    Enabled = false // ������ ����������� �� ������ ����
                };
                buttons[i].Click += Button_Click;
                this.Controls.Add(buttons[i]);
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            // ������������� ����� ����
            timeLimit = (int)timeInput.Value;
            progressBar.Maximum = timeLimit;
            progressBar.Value = 0;
            timerLabel.Text = $"Time: {timeLimit} sec";

            // ���������� ��������� ����� ��� ������
            buttonValues = Enumerable.Range(0, 101).OrderBy(x => random.Next()).Take(16).ToList();
            for (int i = 0; i < 16; i++)
            {
                buttons[i].Text = buttonValues[i].ToString();
                buttons[i].BackColor = SystemColors.Control;
                buttons[i].Enabled = true;
            }

            // ���������� ��������� ��������
            nextNumber = 0;
            currentNumberLabel.Text = $"Next Number: {nextNumber}";
            messageLabel.Text = "";

            // ��������� ������
            gameTimer = new Timer
            {
                Interval = 1000 // 1 �������
            };
            gameTimer.Tick += TimerElapsed;
            gameTimer.Start();
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            if (timeLimit > 0)
            {
                timeLimit--;
                timerLabel.Text = $"Time: {timeLimit} sec";
                progressBar.Value = progressBar.Maximum - timeLimit;
            }
            else
            {
                gameTimer.Stop();
                MessageBox.Show("Time's up! You didn't finish in time.");
                foreach (var button in buttons)
                {
                    button.Enabled = false;
                }
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                int number = int.Parse(clickedButton.Text);

                // ���������, ������ �� �����, ��� ���������, � ���������� �� ����� ����������
                if (number > nextNumber && buttonValues.Where(n => n > nextNumber).Min() == number)
                {
                    nextNumber = number;
                    currentNumberLabel.Text = $"Next Number: {nextNumber}";
                    clickedButton.BackColor = Color.Green;
                    clickedButton.Enabled = false;
                    messageLabel.Text = "";

                    // ���������, ��������� �� ����
                    if (buttonValues.All(b => !buttons.Any(btn => btn.Text == b.ToString() && btn.Enabled)))
                    {
                        gameTimer.Stop();
                        MessageBox.Show("Congratulations! You completed the game.");
                    }
                }
                else
                {
                    clickedButton.BackColor = Color.Red;
                    messageLabel.Text = $"Incorrect! Expected: > {nextNumber}";

                    // ���������� ��������� ����� �������� ���������� �������
                    Timer resetTimer = new Timer
                    {
                        Interval = 500 // 0.5 �������
                    };
                    resetTimer.Tick += (s, args) =>
                    {
                        clickedButton.BackColor = SystemColors.Control;
                        resetTimer.Stop();
                    };
                    resetTimer.Start();
                }
            }
        }
    }
}
