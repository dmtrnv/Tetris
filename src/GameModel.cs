using System;
using System.Collections.Generic;
using System.Drawing;
using Tetris.Shapes;


namespace Tetris
{
    class GameModel
    {
        private readonly int _modelWidth;
        private readonly int _modelHeight;
        private readonly Shape[] _shapes;
        private readonly Random _randomNumberGenerator = new Random();
        private Shape _currentShape;
        private Point _startLocation;
        private int _gameScore = 0;
        
        public BlockType[,] Model { get; }
        public Shape NextShape { get; private set; }
        
        public event EventHandler<GameScoreEventArgs> GameScoreChanged;
        public event EventHandler<GameScoreEventArgs> GameEnd;
        public event EventHandler NextShapeChanged;
       
        public GameModel(int height, int width)
        {
            _modelHeight = height;
            _modelWidth = width;
            Model = new BlockType[_modelHeight, _modelWidth];
            _startLocation = new Point(_modelWidth / 2 - 2, 0);

            _shapes = new Shape[] 
            {
                new I(_startLocation),
                new J(_startLocation),
                new L(_startLocation),
                new O(_startLocation),
                new S(_startLocation),
                new T(_startLocation),
                new Z(_startLocation)
            };

            _currentShape = _shapes[_randomNumberGenerator.Next(0, _shapes.Length)];
            NextShape = _shapes[_randomNumberGenerator.Next(0, _shapes.Length)];
            NextShape.CurrentState = ShapeState.Base;
            NextShapeChanged?.Invoke(this, EventArgs.Empty);
            PlaceCurrentShape(); 
        }
        
        public void Clear()
        {
            ClearGameModel();
            _gameScore = 0;
            GameScoreChanged?.Invoke(this, new GameScoreEventArgs(_gameScore));

            BringCurrentShapeToNextOne();
            PlaceCurrentShape();
        }

        public void MoveShapeDown()
        {
            // Check the shape for reaching model's bottom border.
            if (_currentShape.LocationY + _currentShape.BottomBorder == _modelHeight - 1) 
            {
                CountAndEraseCompletedRows();
                BringCurrentShapeToNextOne();
                PlaceCurrentShape();
                return;
            }

            // Check the shape for contact with other shapes.
            for (var i = _currentShape.TopBorder; i <= _currentShape.BottomBorder; i++)
            {
                for (var j = _currentShape.LeftBorder; j <= _currentShape.RightBorder; j++)
                {
                    if (Model[_currentShape.LocationY + i + 1, _currentShape.LocationX + j] != BlockType.Empty 
                        && _currentShape.States[_currentShape.CurrentState][i, j] != BlockType.Empty
                        && _currentShape.States[_currentShape.CurrentState][i + 1, j] == BlockType.Empty)
                    {
                        CountAndEraseCompletedRows();

                        if (_currentShape.LocationY + _currentShape.TopBorder <= 0)
                        {
                            EndOfGame();
                            return;
                        }

                        BringCurrentShapeToNextOne();
                        var rowCount = CountCleanRows();
                        PlaceCurrentShape((rowCount >= _currentShape.Height), rowCount);					
                        return;
                    }
                }
            }

            EraseCurrentShape();
            _currentShape.LocationY++;
            PlaceCurrentShape();
        }

        public void RotateShape()
        {
            if (ShapeIsOutOfTopOrBottomModelBorders()) 
            {
                return;
            }

            var nextStateOfCurrentShape = _currentShape.Clone() as Shape;
            nextStateOfCurrentShape.CurrentState = _currentShape.CurrentState == ShapeState.DegreeRotation270 ? ShapeState.Base : _currentShape.CurrentState + 1;
            
            // Check the next shape state for out of model borders.
            if (_currentShape.LocationY + nextStateOfCurrentShape.BottomBorder > _modelHeight - 1 
                || _currentShape.LocationX + nextStateOfCurrentShape.LeftBorder < 0
                || _currentShape.LocationX + nextStateOfCurrentShape.RightBorder > _modelWidth - 1)
            {
                return;
            }
            
            // Check the next shape state for contact with other shapes.
            for (var i = nextStateOfCurrentShape.TopBorder; i <= nextStateOfCurrentShape.BottomBorder; i++)
                for (var j = nextStateOfCurrentShape.LeftBorder; j <= nextStateOfCurrentShape.RightBorder; j++)
                {
                    if (Model[nextStateOfCurrentShape.LocationY + i, nextStateOfCurrentShape.LocationX + j] != BlockType.Empty
                        && nextStateOfCurrentShape.States[nextStateOfCurrentShape.CurrentState][i, j] != BlockType.Empty
                        && _currentShape.States[_currentShape.CurrentState][i, j] == BlockType.Empty)
                    {
                        return;
                    }
                }

            EraseCurrentShape();
            _currentShape.CurrentState = nextStateOfCurrentShape.CurrentState;
            PlaceCurrentShape();
        }

        public void MoveShapeLeft()
        {
            // Check the shape for out of model borders.
            if (ShapeIsOutOfTopOrBottomModelBorders()) 
            {
                return;
            }
            if (_currentShape.LocationX + _currentShape.LeftBorder == 0) 
            {
                return;
            }
            
            // Check the shape for contact with other shapes.
            for (var i = _currentShape.TopBorder; i <= _currentShape.BottomBorder; i++)
            {
                for (var j = _currentShape.LeftBorder; j <= _currentShape.RightBorder; j++)
                {
                    if (Model[_currentShape.LocationY + i, _currentShape.LocationX + j - 1] != BlockType.Empty
                        && _currentShape.States[_currentShape.CurrentState][i, j] != BlockType.Empty
                        && _currentShape.States[_currentShape.CurrentState][i, j - 1] == BlockType.Empty)
                    {
                        return;
                    }
                }
            }

            EraseCurrentShape();
            _currentShape.LocationX--;
            PlaceCurrentShape();
        }

        public void MoveShapeRight()
        {
            // Check the shape for out of model borders.
            if (ShapeIsOutOfTopOrBottomModelBorders()) 
            {
                return;
            }
            if (_currentShape.LocationX + _currentShape.RightBorder == _modelWidth - 1) 
            {
                return;
            }

            // Check the shape for contact with other shapes.
            for (var i = _currentShape.TopBorder; i <= _currentShape.BottomBorder; i++)
            {
                for (var j = _currentShape.RightBorder; j >= _currentShape.LeftBorder; j--)
                {
                    if (Model[_currentShape.LocationY + i, _currentShape.LocationX + j + 1] != BlockType.Empty
                        && _currentShape.States[_currentShape.CurrentState][i, j] != BlockType.Empty
                        && _currentShape.States[_currentShape.CurrentState][i, j + 1] == BlockType.Empty)
                    {
                        return;
                    }
                }
            }

            EraseCurrentShape();
            _currentShape.LocationX++;
            PlaceCurrentShape();
        }

        private void PlaceCurrentShape(bool enoughSpace = true, int rowCount = default)
        {
            if (enoughSpace)
            {
                for (var i = _currentShape.TopBorder; i <= _currentShape.BottomBorder; i++)
                {
                    for (var j = _currentShape.LeftBorder; j <= _currentShape.RightBorder; j++)
                    {
                        if (_currentShape.States[_currentShape.CurrentState][i, j] != BlockType.Empty)
                        {
                            Model[_currentShape.LocationY + i, _currentShape.LocationX + j] = _currentShape.States[_currentShape.CurrentState][i, j];
                        }
                    }
                }
            }
            else
            {
                for (var i = _currentShape.BottomBorder; rowCount > 0; i--, rowCount--)
                {
                    for (var j = _currentShape.LeftBorder; j <= _currentShape.RightBorder; j++)
                    {
                        if (_currentShape.States[_currentShape.CurrentState][i, j] != BlockType.Empty)
                        {
                            Model[0 + rowCount - 1, _currentShape.LocationX + j] = _currentShape.States[_currentShape.CurrentState][i, j];
                        }
                    }
                }
            }
        }

        private void EraseCurrentShape()
        {
            for (var i = _currentShape.TopBorder; i <= _currentShape.BottomBorder; i++)
            {
                for (var j = _currentShape.LeftBorder; j <= _currentShape.RightBorder; j++)
                {
                    if (_currentShape.States[_currentShape.CurrentState][i, j] != BlockType.Empty)
                    {
                        Model[_currentShape.LocationY + i, _currentShape.LocationX + j] = BlockType.Empty;
                    }
                }
            }
        }

        private void BringCurrentShapeToNextOne()
        {
            _currentShape = NextShape;
            _currentShape.LocationX = _startLocation.X;
            _currentShape.LocationY = _startLocation.Y;

            NextShape = _shapes[_randomNumberGenerator.Next(0, _shapes.Length)];
            NextShape.CurrentState = ShapeState.Base;
            NextShapeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void CountAndEraseCompletedRows()
        {
            var completedRows = CountCompletedRows();
            if (completedRows.Count != 0)
            {
                EraseCompletedRows(completedRows);
            }
        }

        private List<int> CountCompletedRows()
        {
            var completedRows = new List<int>();
            bool isRowCompleted;

            for (var i = _currentShape.TopBorder; i <= _currentShape.BottomBorder; i++)
            {
                isRowCompleted = true;

                for (var j = 0; j < Model.GetLength(1); j++)
                {
                    if (Model[_currentShape.LocationY + i, j] == BlockType.Empty)
                    {
                        isRowCompleted = false;
                        break;
                    }
                }

                if (isRowCompleted) 
                {
                    completedRows.Add(_currentShape.LocationY + i);
                }
            }

            return completedRows;
        }

        private void EraseCompletedRows(List<int> completedRows)
        {
            for (int i = completedRows.Count - 1, offset = 0; i >= 0; i--)
            {
                for (var j = completedRows[i]; j > 0; j--)
                {
                    for (var k = 0; k < Model.GetLength(1); k++)
                    {
                        Model[j + offset, k] = Model[j + offset - 1, k];
                    }
                }

                offset++;
            }

            _gameScore += (completedRows.Count == 1) ? 10 : 15 * completedRows.Count;
            GameScoreChanged?.Invoke(this, new GameScoreEventArgs(_gameScore));
        }

        private void ClearGameModel()
        {
            for (var i = 0; i < _modelHeight; i++)
            {
                for (var j = 0; j < _modelWidth; j++)
                {
                    Model[i, j] = BlockType.Empty;
                }
            }
        }

        private void EndOfGame()
        {
            GameEnd?.Invoke(this, new GameScoreEventArgs(_gameScore));
        }

        private int CountCleanRows()
        {
            var rowCount = 0;
            bool isRowClean;

            for (var i = 0; i < _modelHeight; i++)
            {
                isRowClean = true;

                for (var j = _currentShape.LeftBorder; j <= _currentShape.RightBorder; j++)
                {
                    if (Model[i, _currentShape.LocationX + j] != BlockType.Empty) 
                    {
                        isRowClean = false;
                        break;
                    }
                }

                if (!isRowClean) 
                {
                    break;
                }

                rowCount++;
            }

            return rowCount;
        }

        private bool ShapeIsOutOfTopOrBottomModelBorders()
        {
            if (_currentShape.LocationY + _currentShape.TopBorder < 0
                || _currentShape.LocationY + _currentShape.BottomBorder == _modelHeight - 1) 
            {
                return true;
            }

            return false;
        }
    }

    internal class GameScoreEventArgs : EventArgs
    {
        internal readonly int GameScore;

        internal GameScoreEventArgs(int score)
        {
           GameScore = score;
        }
    }
}