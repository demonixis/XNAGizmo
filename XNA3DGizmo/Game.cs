using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework.Input;
using XNAGizmo;

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

namespace XNA3DGizmoExample
{
    public class GizmoSampleGame : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        GizmoComponent _gizmo;
        GridComponent _grid;

        StringBuilder _helpTextBuilder;
        string _helpText;
        SpriteFont _font;

        public GizmoSampleGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferMultiSampling = true;

            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            SpriteFont font = Content.Load<SpriteFont>("GizmoFont");
            _gizmo = new GizmoComponent(_graphics.GraphicsDevice, _spriteBatch, font);
            _gizmo.SetSelectionPool(Engine.Entities);

            _gizmo.TranslateEvent += GizmoTranslateEvent;
            _gizmo.RotateEvent += GizmoRotateEvent;
            _gizmo.ScaleEvent += GizmoScaleEvent;

            _grid = new GridComponent(GraphicsDevice, 8);

            Model boxModel = Content.Load<Model>("box");
            // add entity
            SceneEntity entity1 = new SceneEntity(boxModel) { Position = new Vector3(0, 0, 20) };
            Engine.Entities.Add(entity1);
            // and another
            SceneEntity entity2 = new SceneEntity(boxModel) { Position = new Vector3(0, 0, -20) };
            Engine.Entities.Add(entity2);

            #region Hotkey Explanation
            _helpTextBuilder = new StringBuilder();
            _helpTextBuilder.AppendLine("Hotkeys:");

            _helpTextBuilder.AppendLine("1,2,3,4 to switch Transformation Modes");
            _helpTextBuilder.AppendLine("O = Switch space (Local/World)");
            _helpTextBuilder.AppendLine("I = Toggle Snapping");
            _helpTextBuilder.AppendLine("P = Switch PivotTypes");
            _helpTextBuilder.AppendLine("Hold Control = Add to selection");
            _helpTextBuilder.AppendLine("Hold Shift = Precision Mode");
            _helpTextBuilder.AppendLine("Hold Alt = Remove from selection");

            _helpText = _helpTextBuilder.ToString();

            #endregion

            _font = Content.Load<SpriteFont>("SpriteFont");
        }

        #region Gizmo Event Hooks
        private void GizmoTranslateEvent(ITransformable transformable, TransformationEventArgs e)
        {
            transformable.Position += (Vector3)e.Value;
        }

        private void GizmoRotateEvent(ITransformable transformable, TransformationEventArgs e)
        {
            _gizmo.RotationHelper(transformable, e);
        }

        private void GizmoScaleEvent(ITransformable transformable, TransformationEventArgs e)
        {
            Vector3 delta = (Vector3)e.Value;
            if (_gizmo.ActiveMode == GizmoMode.UniformScale)
                transformable.Scale *= 1 + ((delta.X + delta.Y + delta.Z) / 3);
            else
                transformable.Scale += delta;
            transformable.Scale = Vector3.Clamp(transformable.Scale, Vector3.Zero, transformable.Scale);
        }
        #endregion

        private KeyboardState _previousKeys;
        private MouseState _previousMouse;
        private MouseState _currentMouse;
        private KeyboardState _currentKeys;

        protected override void Update(GameTime gameTime)
        {
            Engine.CameraPosition = new Vector3(50, 50, 50);
            Engine.View = Matrix.CreateLookAt(Engine.CameraPosition, Vector3.Zero, Vector3.Up);
            Engine.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)_graphics.GraphicsDevice.Viewport.Width / (float)_graphics.GraphicsDevice.Viewport.Height, 1f, 1000f);

            _currentMouse = Mouse.GetState();
            _currentKeys = Keyboard.GetState();

            // update camera properties for rendering and ray-casting.
            _gizmo.UpdateCameraProperties(Engine.View, Engine.Projection, Engine.CameraPosition);

            // select entities with your cursor (add the desired keys for add-to / remove-from -selection)
            if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
                _gizmo.SelectEntities(new Vector2(_currentMouse.X, _currentMouse.Y),
                                      _currentKeys.IsKeyDown(Keys.LeftControl) || _currentKeys.IsKeyDown(Keys.RightControl),
                                      _currentKeys.IsKeyDown(Keys.LeftAlt) || _currentKeys.IsKeyDown(Keys.RightAlt));

            // set the active mode like translate or rotate
            if (IsNewButtonPress(Keys.D1))
                _gizmo.ActiveMode = GizmoMode.Translate;
            if (IsNewButtonPress(Keys.D2))
                _gizmo.ActiveMode = GizmoMode.Rotate;
            if (IsNewButtonPress(Keys.D3))
                _gizmo.ActiveMode = GizmoMode.NonUniformScale;
            if (IsNewButtonPress(Keys.D4))
                _gizmo.ActiveMode = GizmoMode.UniformScale;

            // toggle precision mode
            if (_currentKeys.IsKeyDown(Keys.LeftShift) || _currentKeys.IsKeyDown(Keys.RightShift))
                _gizmo.PrecisionModeEnabled = true;
            else
                _gizmo.PrecisionModeEnabled = false;

            // toggle active space
            if (IsNewButtonPress(Keys.O))
                _gizmo.ToggleActiveSpace();

            // toggle snapping
            if (IsNewButtonPress(Keys.I))
                _gizmo.SnapEnabled = !_gizmo.SnapEnabled;

            // select pivot types
            if (IsNewButtonPress(Keys.P))
                _gizmo.NextPivotType();

            // clear selection
            if (IsNewButtonPress(Keys.Escape))
                _gizmo.Clear();

            _gizmo.Update(gameTime);
            Engine.Update();

            _previousKeys = _currentKeys;
            _previousMouse = _currentMouse;
            base.Update(gameTime);
        }

        private bool IsNewButtonPress(Keys key)
        {
            return _currentKeys.IsKeyDown(key) && _previousKeys.IsKeyUp(key);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _grid.Draw();
            Engine.Draw();
            _gizmo.Draw();

            // -- Draw Help Info -- //
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, _helpText, new Vector2(5), Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
