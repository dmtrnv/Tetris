using System.Windows.Forms;
using System.Drawing;


namespace Tetris
{
    class MyForm : Form
    {
        private GameModel gameModel;
        private readonly int blockSize = 20;

        public static System.Windows.Forms.Timer Timer { get; set; }

        public MyForm()
        {
            Text = "Tetris";
            ClientSize = new Size(240, 460);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            DoubleBuffered = true;
            
            gameModel = new GameModel(ClientSize.Height / blockSize, ClientSize.Width / blockSize);

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

            for(var i = 0; i < ClientSize.Height / blockSize; i++)
                for(var j = 0; j < ClientSize.Width / blockSize; j++)
                {
                    switch(gameModel.Model[i, j])
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

                    e.Graphics.FillRectangle(brush, j * blockSize, i * blockSize, blockSize, blockSize);
                }
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Up: gameModel.RotateShape(); break;
                case Keys.Left: gameModel.MoveShapeLeft(); break;
                case Keys.Right: gameModel.MoveShapeRight(); break;
                case Keys.Down: gameModel.MoveShapeDown(); break;
                default: break;
            }
        }
    }
}
