namespace Tetris.Shapes
{
    class O : Shape
    {
        public O(System.Drawing.Point startLocation) : base(startLocation)
        {
            States = new System.Collections.Generic.Dictionary<ShapeState, BlockType[,]>
            {
                [ShapeState.Base] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation90] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation180] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }

                },
                [ShapeState.DegreeRotation270] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                }
            };
        }
    }
}
