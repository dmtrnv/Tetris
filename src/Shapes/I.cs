namespace Tetris.Shapes
{
    class I : Shape
    {
        public I(System.Drawing.Point startLocation) : base(startLocation)
        {
            States = new System.Collections.Generic.Dictionary<ShapeState, BlockType[,]>
            {

                [ShapeState.Base] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.I, BlockType.I, BlockType.I, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation90] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation180] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.I, BlockType.I, BlockType.I, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                [ShapeState.DegreeRotation270] = new BlockType[,] 
                { 
                    { BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.I, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                }
            };
        }

        public override int TopBorder 
            => CurrentState == ShapeState.DegreeRotation180 ? base.TopBorder + 1 : base.TopBorder;

        public override int BottomBorder
        {
            get
            {
                if (CurrentState == ShapeState.Base)
                {
                    return base.BottomBorder - 3;
                }

                if (CurrentState == ShapeState.DegreeRotation180)
                {
                    return base.BottomBorder - 2;
                }

                return base.BottomBorder;
            }
        }

        public override int LeftBorder
        {
            get
            {
                if (CurrentState == ShapeState.DegreeRotation90) 
                {
                    return base.LeftBorder + 2;
                }

                if (CurrentState == ShapeState.DegreeRotation270) 
                {
                    return base.LeftBorder + 1;
                }
                
                return base.LeftBorder;
            }
        }
    }
}
