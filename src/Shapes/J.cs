namespace Tetris.Shapes
{
	class J : Shape
	{
		public J(System.Drawing.Point startLocation) : base(startLocation)
		{
			States = new System.Collections.Generic.Dictionary<ShapeState, BlockType[,]>();

			States[ShapeState.Base] = new BlockType[,] { { BlockType.Empty, BlockType.J, BlockType.Empty, BlockType.Empty, BlockType.Empty },
														 { BlockType.Empty, BlockType.J, BlockType.J, BlockType.J, BlockType.Empty },
														 { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };

			States[ShapeState.DegreeRotation90] = new BlockType[,] { { BlockType.Empty, BlockType.Empty, BlockType.J, BlockType.J, BlockType.Empty },
																	 { BlockType.Empty, BlockType.Empty, BlockType.J, BlockType.Empty, BlockType.Empty }, 
																	 { BlockType.Empty, BlockType.Empty, BlockType.J, BlockType.Empty, BlockType.Empty },
																	 { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };
			
			States[ShapeState.DegreeRotation180] = new BlockType[,] { { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
																	  { BlockType.Empty, BlockType.J, BlockType.J, BlockType.J, BlockType.Empty },
																	  { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.J, BlockType.Empty },
																	  { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };
			
			States[ShapeState.DegreeRotation270] = new BlockType[,] { { BlockType.Empty, BlockType.Empty, BlockType.J, BlockType.Empty },
																	  { BlockType.Empty, BlockType.Empty, BlockType.J, BlockType.Empty }, 
																	  { BlockType.Empty, BlockType.J, BlockType.J, BlockType.Empty },
																	  { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty } };
		}

		public override int TopBorder 
			=> CurrentState == ShapeState.DegreeRotation180 ? base.TopBorder + 1 : base.TopBorder;

		public override int LeftBorder 
			=> CurrentState == ShapeState.DegreeRotation90 ? base.LeftBorder + 1 : base.LeftBorder;
	}
}
