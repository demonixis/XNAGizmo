using System;

namespace XNA3DGizmoExample
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GizmoSampleGame game = new GizmoSampleGame())
            {
                game.Run();
            }
        }
    }
#endif
}

