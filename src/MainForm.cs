using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Microsoft.Data.Sqlite;
using Tetris.Shapes;


namespace Tetris
{
    class MainForm : Form
    {
        public static System.Windows.Forms.Timer Timer { get; private set; }

        private const int BlockSize = 20;
        
        private const string ConnectionString = @"Data Source=.\tetris.db";

        private readonly GameModel _gameModel;
        private readonly int _gameModelWidth = 240 / BlockSize;
        private readonly int _gameModelHeight = 460 / BlockSize;
        private readonly int _nextShapePositionX;
        private readonly int _nextShapePositionY;
        private readonly BlockType[,] _nextShapeArea = new BlockType[2, 5];
        private Shape _nextShape;
        private string _playerName;

        
        public MainForm()
        {
            Text = "Tetris";
            ClientSize = new Size(_gameModelWidth * BlockSize + 120, _gameModelHeight * BlockSize);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            DoubleBuffered = true;

            _gameModel = new GameModel(_gameModelHeight, _gameModelWidth);

            var nextShapeLabel = new Label
            {
                Text = $"Next shape:", 
                Location = new Point(ClientSize.Width - 100, 20)
            };
            this.Controls.Add(nextShapeLabel);
            this._nextShapePositionX = nextShapeLabel.Location.X - BlockSize;
            this._nextShapePositionY = nextShapeLabel.Location.Y + 30;
            this._nextShape = _gameModel.NextShape;
            this.PlaceNextShape();

            _gameModel.NextShapeChanged += (sender, e) 
                =>
                {
                    this.ClearNextShapeArea();
                    _nextShape = _gameModel.NextShape;
                    this.PlaceNextShape();
                };

            var scoreLabel = new Label
            {
                Text = "Current score:\n\n0",
                Location = new Point(ClientSize.Width - 100, _nextShapePositionY + 60),
                Height = 40
            };
            this.Controls.Add(scoreLabel);

            _gameModel.GameScoreChanged += (sender, e) 
                => scoreLabel.Text = $"Current score:\n\n{e?.GameScore ?? -1}";

            _gameModel.GameScoreChanged += ChangeTimerInterval;

            _gameModel.GameEnd += OnGameEnd;

            var getNameForm = new GetNameForm();
            getNameForm.NameChanged += (sender, e) 
                => _playerName = e.Name;

            getNameForm.Show();
            
            Timer = new System.Windows.Forms.Timer { Interval = 450 };
            Timer.Tick += (s, a) =>
            {
                _gameModel.MoveShapeDown();
                Invalidate();
            };
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

            for (var i = 0; i < _gameModelHeight; i++)
            {
                for (var j = 0; j < _gameModelWidth; j++)
                {
                    switch (_gameModel.Model[i, j])
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
            for (var i = 0; i < _nextShapeArea.GetLength(0); i++)
            {
                for (var j = 0; j < _nextShapeArea.GetLength(1); j++)
                {
                    switch (_nextShapeArea[i, j])
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

                    e.Graphics.FillRectangle(brush, _nextShapePositionX + (j * BlockSize), _nextShapePositionY + (i * BlockSize), BlockSize, BlockSize);
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up: _gameModel.RotateShape(); break;
                case Keys.Left: _gameModel.MoveShapeLeft(); break;
                case Keys.Right: _gameModel.MoveShapeRight(); break;
                case Keys.Down: _gameModel.MoveShapeDown(); break;
                default: break;
            }
        }

        private void ClearNextShapeArea()
        {
            for (var i = 0; i < _nextShapeArea.GetLength(0); i++)
            {
                for (var j = 0; j < _nextShapeArea.GetLength(1); j++)
                {
                    _nextShapeArea[i, j] = BlockType.Empty;
                }
            }
        }

        private void PlaceNextShape()
        {
            for (var i = _nextShape.TopBorder; i <= _nextShape.BottomBorder; i++)
            {
                for (var j = _nextShape.LeftBorder; j <= _nextShape.RightBorder; j++)
                {
                    if (_nextShape.States[_nextShape.CurrentState][i, j] != BlockType.Empty)
                    {
                        _nextShapeArea[i, j] = _nextShape.States[_nextShape.CurrentState][i, j];
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
        
        private void OnGameEnd(object sender, GameScoreEventArgs e)
        {
            Timer.Stop();

            var topOfPlayers = new List<string>(10);

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                var insertCommand = connection.CreateCommand();
                insertCommand.CommandText = 
                @"
                    INSERT INTO records (playerName, score)
                    VALUES ($playerName, $score);
                ";
                insertCommand.Parameters.AddWithValue("$playerName", _playerName);
                insertCommand.Parameters.AddWithValue("$score", e.GameScore);
                
                insertCommand.ExecuteNonQuery();

                var selectCommand = connection.CreateCommand();
                selectCommand.CommandText = 
                @"
                    SELECT playerName, score
                    FROM records
                    ORDER BY score DESC
                    LIMIT 10; 
                ";
                
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topOfPlayers.Add($"{reader.GetString(0)} - {reader.GetString(1)}");
                    }
                }
            }

            MessageBox.Show($"Your score is: {e.GameScore}.", "Game over", MessageBoxButtons.OK);

            var dialogResult = MessageBox.Show(string.Join("\n", topOfPlayers), "Player's top", MessageBoxButtons.OK);
            if (dialogResult == DialogResult.OK)
            {
                _gameModel.Clear();
                Timer.Start();
            }
        }
    }
}
