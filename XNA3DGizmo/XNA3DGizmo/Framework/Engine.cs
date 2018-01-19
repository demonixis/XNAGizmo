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
using Microsoft.Xna.Framework;

namespace XNA3DGizmoExample
{
    public static class Engine
    {
        public static List<SceneEntity> Entities = new List<SceneEntity>();

        public static Matrix View;
        public static Matrix Projection;
        public static Vector3 CameraPosition;

        public static void Update()
        {
            foreach (SceneEntity entity in Entities)
                entity.Update();
        }

        public static void Draw()
        {
            foreach (SceneEntity entity in Entities)
                entity.Draw();
        }
    }
}
