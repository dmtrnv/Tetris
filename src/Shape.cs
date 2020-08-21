namespace Tetris
{
	class Shape : System.ICloneable
	{
		public System.Collections.Generic.Dictionary<ShapeState, BlockType [,]> States { get; protected set; }

		public ShapeState CurrentState { get; set; }

		public int LocationX { get; set; }

		public int LocationY { get; set; }

		public int Height 
		{
			get => BottomBorder - TopBorder + 1;
		}

		public virtual int TopBorder 
		{ 
			get => 0; 
		}

		public virtual int BottomBorder 
		{
			get => States[CurrentState].GetLength(0) - 2; 
		}

		public virtual int LeftBorder 
		{ 
			get => 1; 
		}

		public virtual int RightBorder 
		{ 
			get => States[CurrentState].GetLength(1) - 2; 
		}

		public Shape(System.Drawing.Point startLocation)
		{
			LocationX = startLocation.X;
			LocationY = startLocation.Y;
			CurrentState = ShapeState.Base;
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
