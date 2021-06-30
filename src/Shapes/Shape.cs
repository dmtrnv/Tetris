namespace Tetris.Shapes
{
    class Shape : System.ICloneable
    {
        public System.Collections.Generic.Dictionary<ShapeState, BlockType [,]> States { get; protected set; }

        public ShapeState CurrentState { get; set; }

        public int LocationX { get; set; }

        public int LocationY { get; set; }

        public int Height 
            => BottomBorder - TopBorder + 1;

        public virtual int TopBorder 
            => 0;

        public virtual int BottomBorder 
            => States[CurrentState].GetLength(0) - 2;

        public virtual int LeftBorder 
            => 1;

        public virtual int RightBorder 
            => States[CurrentState].GetLength(1) - 2;

        protected Shape(System.Drawing.Point startLocation)
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
