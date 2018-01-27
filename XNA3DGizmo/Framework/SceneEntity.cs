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
// -- uncomment the define statements below to choose your desired rotation method.

//#define USE_QUATERNION
//#define USE_ROTATIONMATRIX

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAGizmo;
#endregion

namespace XNA3DGizmoExample
{
    public class SceneEntity : ITransformable
    {

        #region Fields & Properties
        private Vector3 _position = Vector3.Zero;

        public string Name
        {
            get;
            set;
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private Vector3 _scale = Vector3.One;
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

#if USE_QUATERNION
    private Quaternion orientation = Quaternion.Identity;
    public Quaternion Orientation
    {
      get { return orientation; }
      set
      {
        orientation = value;
        orientation.Normalize();
      }
    }
#elif USE_ROTATIONMATRIX
        private Matrix rotation = Matrix.Identity;
        public Matrix Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
#endif
        private Vector3 _forward = Vector3.Forward;
        private Vector3 _up = Vector3.Up;

        public Vector3 Forward
        {
            get { return _forward; }
            set
            {
                _forward = value;
                _forward.Normalize();
            }
        }

        public Vector3 Up
        {
            get { return _up; }
            set
            {
                _up = value;
                _up.Normalize();
            }
        }

        private Model _model;
        private Matrix _world;

        const float LENGTH = 5f;
        public BoundingBox BoundingBox
        {
            get { return new BoundingBox(Position - (Vector3.One * LENGTH) * Scale, Position + (Vector3.One * LENGTH) * Scale); }
        }
        #endregion

        public SceneEntity(Model model)
        {
            _model = model;
        }

        public void Update()
        {
#if USE_QUATERNION

      world = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(orientation) * Matrix.CreateTranslation(position);
#elif USE_ROTATIONMATRIX
            world = Matrix.CreateScale(scale) * rotation * Matrix.CreateTranslation(position);
#else
            _world = Matrix.CreateScale(_scale) * Matrix.CreateWorld(_position, _forward, _up);
#endif
        }

        public float? Select(Ray selectionRay)
        {
            return selectionRay.Intersects(BoundingBox);
        }

        public void Draw()
        {
            foreach (ModelMesh modelmesh in _model.Meshes)
            {
                foreach (ModelMeshPart meshpart in modelmesh.MeshParts)
                {
                    BasicEffect effect = (BasicEffect)meshpart.Effect;

                    effect.World = _world;
                    effect.View = Engine.View;
                    effect.Projection = Engine.Projection;

                    effect.EnableDefaultLighting();
                }
                modelmesh.Draw();
            }
        }
    }
}
