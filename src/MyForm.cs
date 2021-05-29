using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Tetris.Shapes;


namespace Tetris
{
    class MyForm : Form
    {
        private const int BlockSize = 20;

        public static System.Windows.Forms.Timer Timer { get; set; }

        private readonly GameModel gameModel;
        private readonly int gameModelWidth = 240 / BlockSize;
        private readonly int gameModelHeight = 460 / BlockSize;
        private readonly int nextShapePositionX;
        private readonly int nextShapePositionY;
        private readonly BlockType[,] nextShapeArea = new BlockType[2, 5];
        private Shape nextShape;

        public MyForm()
        {
            Text = "Tetris";
            ClientSize = new Size(gameModelWidth * BlockSize + 120, gameModelHeight * BlockSize);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            DoubleBuffered = true;

            gameModel = new GameModel(gameModelHeight, gameModelWidth);

            var nextShapeLabel = new Label
            {
                Text = $"Next shape:", 
                Location = new Point(ClientSize.Width - 100, 20)
            };
            this.Controls.Add(nextShapeLabel);
            this.nextShapePositionX = nextShapeLabel.Location.X - BlockSize;
            this.nextShapePositionY = nextShapeLabel.Location.Y + 30;
            this.nextShape = gameModel.NextShape;
            this.PlaceNextShape();

            gameModel.NextShapeChanged += (sender, e) 
                =>
                {
                    this.ClearNextShapeArea();
                    nextShape = gameModel.NextShape;
                    this.PlaceNextShape();
                };

            var scoreLabel = new Label
            {
                Text = "Current score:\n\n0",
                Location = new Point(ClientSize.Width - 100, nextShapePositionY + 60),
                Height = 40
            };
            this.Controls.Add(scoreLabel);

            gameModel.GameScoreChanged += (sender, e) 
                => scoreLabel.Text = $"Current score:\n\n{e?.GameScore ?? -1}";

            gameModel.GameScoreChanged += ChangeTimerInterval;

            Timer = new System.Windows.Forms.Timer { Interval = 450 };
            Timer.Tick += (s, a) =>
            {
                gameModel.MoveShapeDown();
                Invalidate();
            };
            Timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Brush brush = default;
            
            PaintGameModel(brush, e);
            PaintNextShape(brush, e);
        }

        private void PaintGameModel(Brush brush, PaintEventArgs e)
        {
            var pen = new Pen(SystemColors.ButtonFace, 1) { Alignment = PenAlignment.Inset };

            for (var i = 0; i < gameModelHeight; i++)
            {
                for (var j = 0; j < gameModelWidth; j++)
                {
                    switch (gameModel.Model[i, j])
                    {
                        case BlockType.Empty: brush = Brushes.WhiteSmoke; break;
                        case BlockType.I: brush = Brushes.LightBlue; break;
                        case BlockType.J: brush = Brushes.Blue; break;
                        case BlockType.L: brush = Brushes.Orange; break;
                        case BlockType.O: brush = Brushes.Yellow; break;
                        case BlockType.S: brush = Brushes.Green; break;
                        case BlockType.T: brush = Brushes.Purple; break;
                        case BlockType.Z: brush = Brushes.Red; break;
                    }

                    var rectangle = new Rectangle(j * BlockSize, i * BlockSize, BlockSize, BlockSize);
                    e.Graphics.FillRectangle(brush, rectangle);
                    e.Graphics.DrawRectangle(pen, rectangle);
                }
            }
        }

        private void PaintNextShape(Brush brush, PaintEventArgs e)
        {
            for (var i = 0; i < nextShapeArea.GetLength(0); i++)
            {
                for (var j = 0; j < nextShapeArea.GetLength(1); j++)
                {
                    switch (nextShapeArea[i, j])
                    {
                        case BlockType.Empty: brush = new SolidBrush(SystemColors.ButtonFace); break;
                        case BlockType.I: brush = Brushes.LightBlue; break;
                        case BlockType.J: brush = Brushes.Blue; break;
                        case BlockType.L: brush = Brushes.Orange; break;
                        case BlockType.O: brush = Brushes.Yellow; break;
                        case BlockType.S: brush = Brushes.Green; break;
                        case BlockType.T: brush = Brushes.Purple; break;
                        case BlockType.Z: brush = Brushes.Red; break;
                    }

                    e.Graphics.FillRectangle(brush, nextShapePositionX + (j * BlockSize), nextShapePositionY + (i * BlockSize), BlockSize, BlockSize);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up: gameModel.RotateShape(); break;
                case Keys.Left: gameModel.MoveShapeLeft(); break;
                case Keys.Right: gameModel.MoveShapeRight(); break;
                case Keys.Down: gameModel.MoveShapeDown(); break;
                default: break;
            }
        }

        private void ClearNextShapeArea()
        {
            for (var i = 0; i < nextShapeArea.GetLength(0); i++)
            {
                for (var j = 0; j < nextShapeArea.GetLength(1); j++)
                {
                    nextShapeArea[i, j] = BlockType.Empty;
                }
            }
        }

        private void PlaceNextShape()
        {
            for (var i = nextShape.TopBorder; i <= nextShape.BottomBorder; i++)
            {
                for (var j = nextShape.LeftBorder; j <= nextShape.RightBorder; j++)
                {
                    if (nextShape.States[nextShape.CurrentState][i, j] != BlockType.Empty)
                    {
                        nextShapeArea[i, j] = nextShape.States[nextShape.CurrentState][i, j];
                    }
                }
            }
        }

        private void ChangeTimerInterval(object sender, GameScoreEventArgs e)
        {
            switch (e.GameScore)
            {
                case 0: Timer.Interval = 450; break;
                case 50: Timer.Interval = 325; break;
                case 100: Timer.Interval = 200; break;
                case 200: Timer.Interval = 100; break;
                default: break;
            }
        }
    }
}
