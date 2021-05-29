namespace Tetris.Shapes
{
    class Z : Shape
    {
        public Z(System.Drawing.Point startLocation) : base(startLocation)
        {
            States = new System.Collections.Generic.Dictionary<ShapeState, BlockType[,]>
            {
                [ShapeState.Base] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.Z, BlockType.Z, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Z, BlockType.Z, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation90] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Z, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Z, BlockType.Z, BlockType.Empty }, 
                    { BlockType.Empty, BlockType.Empty, BlockType.Z, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation180] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Z, BlockType.Z, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Z, BlockType.Z, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation270] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.Empty, BlockType.Z, BlockType.Empty },
                    { BlockType.Empty, BlockType.Z, BlockType.Z, BlockType.Empty }, 
                    { BlockType.Empty, BlockType.Z, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }

                }
            };
        }

        public override int TopBorder 
            => CurrentState == ShapeState.DegreeRotation180 ? base.TopBorder + 1 : base.TopBorder;

        public override int LeftBorder 
            => CurrentState == ShapeState.DegreeRotation90 ? base.LeftBorder + 1 : base.LeftBorder;
    }
}
