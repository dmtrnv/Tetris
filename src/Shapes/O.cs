namespace Tetris.Shapes
{
    class O : Shape
    {
        public O(System.Drawing.Point startLocation) : base(startLocation)
        {
            States = new System.Collections.Generic.Dictionary<ShapeState, BlockType[,]>();

            States[ShapeState.Base] = new BlockType[,] { { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                         { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                         { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };

            States[ShapeState.DegreeRotation90] = new BlockType[,] { { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                                     { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                                     { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };
            
            States[ShapeState.DegreeRotation180] = new BlockType[,] { { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                                      { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                                      { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };
            
            States[ShapeState.DegreeRotation270] = new BlockType[,] { { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                                      { BlockType.Empty, BlockType.O, BlockType.O, BlockType.Empty },
                                                                      { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };
        }
    }
}
