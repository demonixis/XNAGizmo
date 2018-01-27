// -------------------------------------------------------------
// -- XNA 3D Gizmo (Component)
// -------------------------------------------------------------
// -- open-source gizmo component for any 3D level editor.
// -- contains any feature you may be looking for in a transformation gizmo.
// -- 
// -- for additional information and instructions visit codeplex.
// --
// -- codeplex url: http://xnagizmo.codeplex.com/
// --
// -----------------Please Do Not Remove ----------------------
// -- Work by Tom Looman, licensed under Ms-PL
// -- My Blog: http://coreenginedev.blogspot.com
// -- My Portfolio: http://tomlooman.com
// -- You may find additional XNA resources and information on these sites.
// ------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNA3DGizmoExample
{
    public sealed class GridComponent
    {
        public bool Enabled = true;

        private int _spacing;
        public int GridSpacing
        {
            get { return _spacing; }
            set
            {
                _spacing = value;
                ResetLines();
            }
        }

        private int gridSize = 512;

        /// <summary>
        /// Number of lines in total.
        /// </summary>
        private int _nrOfLines;
        //public int NumberOfLines
        //{
        //    get { return nrOfLines; }
        //    set
        //    {
        //        nrOfLines = value;
        //        ResetLines();
        //    }
        //}

        private BasicEffect _effect;
        private GraphicsDevice _graphics;
        private VertexPositionColor[] _vertexData;

        private Color _lineColor = Color.DarkBlue;
        private Color _highlightColor = Color.Blue;


        public GridComponent(GraphicsDevice device, int gridspacing)
        {
            _effect = new BasicEffect(device);
            _effect.VertexColorEnabled = true;
            _effect.World = Matrix.Identity;

            _graphics = device;

            _spacing = gridspacing;

            ResetLines();
        }

        public void ResetLines()
        {
            // calculate nr of lines, +2 for the highlights, +12 for boundingbox
            _nrOfLines = ((gridSize / _spacing) * 4) + 2 + 12;

            List<VertexPositionColor> vertexList = new List<VertexPositionColor>(_nrOfLines);

            // fill array
            for (int i = 1; i < (gridSize / _spacing) + 1; i++)
            {
                vertexList.Add(new VertexPositionColor(new Vector3((i * _spacing), 0, gridSize), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3((i * _spacing), 0, -gridSize), _lineColor));

                vertexList.Add(new VertexPositionColor(new Vector3((-i * _spacing), 0, gridSize), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3((-i * _spacing), 0, -gridSize), _lineColor));

                vertexList.Add(new VertexPositionColor(new Vector3(gridSize, 0, (i * _spacing)), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3(-gridSize, 0, (i * _spacing)), _lineColor));

                vertexList.Add(new VertexPositionColor(new Vector3(gridSize, 0, (-i * _spacing)), _lineColor));
                vertexList.Add(new VertexPositionColor(new Vector3(-gridSize, 0, (-i * _spacing)), _lineColor));
            }

            // add highlights
            vertexList.Add(new VertexPositionColor(Vector3.Forward * gridSize, _highlightColor));
            vertexList.Add(new VertexPositionColor(Vector3.Backward * gridSize, _highlightColor));

            vertexList.Add(new VertexPositionColor(Vector3.Right * gridSize, _highlightColor));
            vertexList.Add(new VertexPositionColor(Vector3.Left * gridSize, _highlightColor));


            // add boundingbox
            BoundingBox box = new BoundingBox(new Vector3(-gridSize, -gridSize, -gridSize), new Vector3(gridSize, gridSize, gridSize));
            Vector3[] corners = new Vector3[8];

            box.GetCorners(corners);
            vertexList.Add(new VertexPositionColor(corners[0], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[1], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[0], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[3], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[0], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[4], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[1], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[2], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[1], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[5], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[2], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[3], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[2], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[6], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[3], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[7], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[4], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[5], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[4], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[7], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[5], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[6], _lineColor));

            vertexList.Add(new VertexPositionColor(corners[6], _lineColor));
            vertexList.Add(new VertexPositionColor(corners[7], _lineColor));


            // convert to array for drawing
            _vertexData = vertexList.ToArray();
        }

        public void Draw()
        {
            if (Enabled)
            {
                _graphics.DepthStencilState = DepthStencilState.Default;

                _effect.View = Engine.View;
                _effect.Projection = Engine.Projection;

                _effect.CurrentTechnique.Passes[0].Apply();
                _graphics.DrawUserPrimitives(PrimitiveType.LineList, _vertexData, 0, _nrOfLines);
            }
        }
    }
}
