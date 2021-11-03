using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace lab4
{
    internal class Window3D : GameWindow
    {

        const float rotation_speed = 180.0f;
        float angle;
        bool showCube = true;
        KeyboardState lastKeyPress;
        private const int XYZ_SIZE = 75;
        private bool axesControl = true;
        private int transStep = 0;
        private int radStep = 0;
        private int attStep = 0;

        private Color[] colour = new Color[] { Color.Cyan, Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Orange};
        private float value = 0;
        private float transpar = 0;

        private bool newStatus = false;

        private int[,] objVertices = {
            {5, 10, 5,
                10, 5, 10,
                5, 10, 5,
                10, 5, 10,
                5, 5, 5,
                5, 5, 5,
                5, 10, 5,
                10, 10, 5,
                10, 10, 10,
                10, 10, 10,
                5, 10, 5,
                10, 10, 5},
            {5, 5, 12,
                5, 12, 12,
                5, 5, 5,
                5, 5, 5,
                5, 12, 5,
                12, 5, 12,
                12, 12, 12,
                12, 12, 12,
                5, 12, 5,
                12, 5, 12,
                5, 5, 12,
                5, 12, 12},
            {6, 6, 6,
                6, 6, 6,
                6, 6, 12,
                6, 12, 12,
                6, 6, 12,
                6, 12, 12,
                6, 6, 12,
                6, 12, 12,
                6, 6, 12,
                6, 12, 12,
                12, 12, 12,
                12, 12, 12}};

        private Window3D() : base(800, 600, new GraphicsMode(32, 24, 0, 8))
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color.MidnightBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);

            double aspect_ratio = Width / (double)Height;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)aspect_ratio, 1, 64);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);

            Matrix4 lookat = Matrix4.LookAt(30, 30, 30, 0, 0, 0, 0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookat);

            showCube = true;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (keyboard[Key.Escape])
            {
                Exit();
                return;
            }

            if (keyboard[Key.P] && !keyboard.Equals(lastKeyPress))
            {
                if (showCube)
                {
                    showCube = false;
                }
                else
                {
                    showCube = true;
                }
            }
            if (keyboard[Key.R] && !keyboard.Equals(lastKeyPress))
            {
                if (newStatus)
                {
                    newStatus = false;
                }
                else
                {
                    newStatus = true;
                }
            }

            if (keyboard[Key.A])
            {
                transStep--;
            }
            if (keyboard[Key.D])
            {
                transStep++;
            }

            if (keyboard[Key.W])
            {
                radStep--;
            }
            if (keyboard[Key.S])
            {
                radStep++;
            }

            if (keyboard[Key.Up])
            {
                attStep++;
            }
            if (keyboard[Key.Down])
            {
                attStep--;
            }


            if (value < 1)
                if (keyboard[Key.Plus])
                {
                    value += 0.01f;
                }
            if(value > 0)
                if (keyboard[Key.Minus])
                {
                    value -= 0.01f;
                }
            Console.WriteLine("Colour:" + value.ToString());

            if (transpar < 1)
                if (keyboard[Key.BracketRight])
                {
                    transpar += 0.01f;
                }
            if (transpar > 0)
                if (keyboard[Key.BracketLeft])
                {
                    transpar -= 0.01f;
                }
            Console.WriteLine("Transparency:" + transpar.ToString());

            lastKeyPress = keyboard;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (newStatus)
            {
                DrawCube();
            }

            if (axesControl)
            {
                DrawAxes();
            }

            if (showCube == true)
            {
                GL.PushMatrix();
                GL.Translate(transStep, attStep, radStep);
                DrawCube();
                GL.PopMatrix();
            }

            SwapBuffers();
        }

        private void DrawAxes()
        {
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Red);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(XYZ_SIZE, 0, 0);
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Yellow);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, XYZ_SIZE, 0); ;
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            GL.Color3(Color.Green);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, XYZ_SIZE);
            GL.End();
        }

        private void DrawCube()
        {
            Random rand = new Random();
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < 35; i = i + 3)
            {
                if(i % 6 == 0 && i != 18)
                    GL.Color3(colour[i/6]);
                if(i == 18)
                    GL.Color4(0f,value,0f,transpar);
                GL.Vertex3(objVertices[0, i], objVertices[1, i], objVertices[2, i]);
                GL.Vertex3(objVertices[0, i + 1], objVertices[1, i + 1], objVertices[2, i + 1]);
                GL.Vertex3(objVertices[0, i + 2], objVertices[1, i + 2], objVertices[2, i + 2]);
            }
            GL.End();
        }

        [STAThread]
        static void Main(string[] args)
        {

            using (Window3D example = new Window3D())
            {
                example.Run(30.0, 0.0);
            }

        }
    }
}
