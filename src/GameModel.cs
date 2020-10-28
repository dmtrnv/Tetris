using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Tetris.Shapes;


namespace Tetris
{
    class GameModel
    {
        private readonly int modelWidth;
        private readonly int modelHeight;
        private Shape[] shapes;					
        private Shape currentShape;
        private Point startLocation;
        private Random randomNumberGenerator = new Random();
        private int gameScore = 0;

        public BlockType[,] Model { get; set; }
        
        public GameModel(int height, int width)
        {
            modelHeight = height;
            modelWidth = width;
            Model = new BlockType[modelHeight, modelWidth];
            startLocation = new Point(modelWidth / 2 - 2, 0);

            shapes = new Shape[] 
            {
                new I(startLocation),
                new J(startLocation),
                new L(startLocation),
                new O(startLocation),
                new S(startLocation),
                new T(startLocation),
                new Z(startLocation)
            };

            currentShape = shapes[randomNumberGenerator.Next(0, shapes.Length)];
            PlaceCurrentShape(); 
        }

        public void MoveShapeDown()
        {
            // Check the shape for reaching model's bottom border.
            if(currentShape.LocationY + currentShape.BottomBorder == modelHeight - 1) 
            {
                CountAndEraseCompletedRows();
                BringCurrentShapeToNextOne();
                PlaceCurrentShape();
                return;
            }

            // Check the shape for contact with other shapes.
            for(var i = currentShape.TopBorder; i <= currentShape.BottomBorder; i++)
                for(var j = currentShape.LeftBorder; j <= currentShape.RightBorder; j++)
                {
                    if(Model[currentShape.LocationY + i + 1, currentShape.LocationX + j] != BlockType.Empty 
                        && currentShape.States[currentShape.CurrentState][i, j] != BlockType.Empty
                        && currentShape.States[currentShape.CurrentState][i + 1, j] == BlockType.Empty)
                    {
                        CountAndEraseCompletedRows();

                        if(currentShape.LocationY + currentShape.TopBorder <= 0)
                        {
                            EndOfGame();
                            return;
                        }

                        BringCurrentShapeToNextOne();
                        var rowCount = CountCleanRows();
                        PlaceCurrentShape((rowCount >= currentShape.Height), rowCount);					
                        return;
                    }
                }

            EraseCurrentShape();
            currentShape.LocationY++;
            PlaceCurrentShape();
        }

        public void RotateShape()
        {
            if(ShapeIsOutOfTopOrBottomModelBorders()) 
                return;

            var nextFigure = currentShape.Clone() as Shape;
            nextFigure.CurrentState = currentShape.CurrentState == ShapeState.DegreeRotation270 ? ShapeState.Base : currentShape.CurrentState + 1;
            
            // Check the next shape state for out of model borders.
            if(currentShape.LocationY + nextFigure.BottomBorder > modelHeight - 1 
                || currentShape.LocationX + nextFigure.LeftBorder < 0
                || currentShape.LocationX + nextFigure.RightBorder > modelWidth - 1)
            {
                return;
            }
            
            // Check the next shape state for contact with other shapes.
            for(var i = nextFigure.TopBorder; i <= nextFigure.BottomBorder; i++)
                for(var j = nextFigure.LeftBorder; j <= nextFigure.RightBorder; j++)
                {
                    if(Model[nextFigure.LocationY + i, nextFigure.LocationX + j] != BlockType.Empty
                        && nextFigure.States[nextFigure.CurrentState][i, j] != BlockType.Empty
                        && currentShape.States[currentShape.CurrentState][i, j] == BlockType.Empty)
                    {
                        return;
                    }
                }

            EraseCurrentShape();
            currentShape.CurrentState = nextFigure.CurrentState;
            PlaceCurrentShape();
        }

        public void MoveShapeLeft()
        {
            // Check the shape for out of model borders.
            if(ShapeIsOutOfTopOrBottomModelBorders()) 
                return;
            if(currentShape.LocationX + currentShape.LeftBorder == 0) 
                return;
            
            // Check the shape for contact with other shapes.
            for(var i = currentShape.TopBorder; i <= currentShape.BottomBorder; i++)
                for(var j = currentShape.LeftBorder; j <= currentShape.RightBorder; j++)
                {
                    if(Model[currentShape.LocationY + i, currentShape.LocationX + j - 1] != BlockType.Empty
                        && currentShape.States[currentShape.CurrentState][i, j] != BlockType.Empty
                        && currentShape.States[currentShape.CurrentState][i, j - 1] == BlockType.Empty)
                    {
                        return;
                    }
                }

            EraseCurrentShape();
            currentShape.LocationX--;
            PlaceCurrentShape();
        }

        public void MoveShapeRight()
        {
            // Check the shape for out of model borders.
            if(ShapeIsOutOfTopOrBottomModelBorders()) 
                return;
            if(currentShape.LocationX + currentShape.RightBorder == modelWidth - 1) 
                return;

            // Check the shape for contact with other shapes.
            for(var i = currentShape.TopBorder; i <= currentShape.BottomBorder; i++)
                for(var j = currentShape.RightBorder; j >= currentShape.LeftBorder; j--)
                {
                    if(Model[currentShape.LocationY + i, currentShape.LocationX + j + 1] != BlockType.Empty
                        && currentShape.States[currentShape.CurrentState][i, j] != BlockType.Empty
                        && currentShape.States[currentShape.CurrentState][i, j + 1] == BlockType.Empty)
                    {
                        return;
                    }
                }

            EraseCurrentShape();
            currentShape.LocationX++;
            PlaceCurrentShape();
        }

        private void PlaceCurrentShape(bool enoughSpace = true, int rowCount = default)
        {
            if(enoughSpace)
            {
                for(var i = currentShape.TopBorder; i <= currentShape.BottomBorder; i++)
                    for(var j = currentShape.LeftBorder; j <= currentShape.RightBorder; j++)
                    {
                        if(currentShape.States[currentShape.CurrentState][i, j] != BlockType.Empty)
                            Model[currentShape.LocationY + i, currentShape.LocationX + j] = currentShape.States[currentShape.CurrentState][i, j];
                    }
            }
            else
            {
                for(var i = currentShape.BottomBorder; rowCount > 0; i--, rowCount--)
                    for(var j = currentShape.LeftBorder; j <= currentShape.RightBorder; j++)
                    {
                        if(currentShape.States[currentShape.CurrentState][i, j] != BlockType.Empty)
                            Model[0 + rowCount - 1, currentShape.LocationX + j] = currentShape.States[currentShape.CurrentState][i, j];
                    }
            }
        }

        private void EraseCurrentShape()
        {
            for(var i = currentShape.TopBorder; i <= currentShape.BottomBorder; i++)
                for(var j = currentShape.LeftBorder; j <= currentShape.RightBorder; j++)
                {
                    if(currentShape.States[currentShape.CurrentState][i, j] != BlockType.Empty)
                        Model[currentShape.LocationY + i, currentShape.LocationX + j] = BlockType.Empty;
                }
        }

        private void BringCurrentShapeToNextOne()
        {
            currentShape = shapes[randomNumberGenerator.Next(0, shapes.Length )];
            currentShape.LocationX = startLocation.X;
            currentShape.LocationY = startLocation.Y;
            currentShape.CurrentState = ShapeState.Base;
        }

        private void CountAndEraseCompletedRows()
        {
            var completedRows = CountCompletedRows();
            if(completedRows.Count != 0)
                EraseCompletedRows(completedRows);
        }

        private List<int> CountCompletedRows()
        {
            var completedRows = new List<int>();
            bool isRowCompleted;

            for(var i = currentShape.TopBorder; i <= currentShape.BottomBorder; i++)
            {
                isRowCompleted = true;

                for(var j = 0; j < Model.GetLength(1); j++)
                    if(Model[currentShape.LocationY + i, j] == BlockType.Empty)
                    {
                        isRowCompleted = false;
                        break;
                    }

                if(isRowCompleted) 
                    completedRows.Add(currentShape.LocationY + i);
            }

            return completedRows;
        }

        private void EraseCompletedRows(List<int> completedRows)
        {
            for(int i = completedRows.Count - 1, offset = 0; i >= 0; i--)
            {
                for(var j = completedRows[i]; j > 0; j--)
                {
                    for(var k = 0; k < Model.GetLength(1); k++)
                    {
                        Model[j + offset, k] = Model[j + offset - 1, k];
                    }
                }

                offset++;
            }

            gameScore += 10 * completedRows.Count;
        }

        private void Clear()
        {
            for(var i = 0; i < modelHeight; i++)
                for(var j = 0; j < modelWidth; j++)
                {
                    Model[i, j] = BlockType.Empty;
                }
        }

        private void EndOfGame()
        {
            MyForm.Timer.Stop();
            var dialogResult = MessageBox.Show($"Your score is: {gameScore}.", "Gamve over", MessageBoxButtons.OK);
            if(dialogResult == DialogResult.OK)
            {
                Clear();
                gameScore = 0;

                BringCurrentShapeToNextOne();
                PlaceCurrentShape();

                MyForm.Timer.Start();
            }
        }

        private int CountCleanRows()
        {
            var rowCount = 0;
            bool isRowClean;

            for(var i = 0; i < modelHeight; i++)
            {
                isRowClean = true;

                for(var j = currentShape.LeftBorder; j <= currentShape.RightBorder; j++)
                {
                    if(Model[i, currentShape.LocationX + j] != BlockType.Empty) 
                    {
                        isRowClean = false;
                        break;
                    }
                }
                if(!isRowClean) 
                    break;

                rowCount++;
            }

            return rowCount;
        }

        private bool ShapeIsOutOfTopOrBottomModelBorders()
        {
            if(currentShape.LocationY + currentShape.TopBorder < 0
                || currentShape.LocationY + currentShape.BottomBorder == modelHeight - 1) 
            {
                return true;
            }

            return false;
        }
    }
}
