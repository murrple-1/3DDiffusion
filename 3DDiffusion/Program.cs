using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Roadrunner._3DDiffusion
{
    class Program
    {
        private Vector3 initialEyePosition = new Vector3(0.0f, 0.0f, 1.6f);
        private static readonly Vector3 changeEyePosition = new Vector3(0.0f, 0.0f, 0.2f);
        private static readonly Vector3 minEyePosition = new Vector3(0.0f, 0.0f, 1.0f);
        private static readonly Vector3 maxEyePosition = new Vector3(0.0f, 0.0f, 4.0f);
        private static readonly Vector3 lookAtPosition = new Vector3(0.0f, 0.0f, 0.0f);
        private static readonly Vector3 upVector = new Vector3(0.0f, 1.0f, 0.0f);
        private static Matrix4 initialTranslateMatrix = Matrix4.CreateTranslation(-0.5f, -0.5f, -0.5f);

        private float pitch = 0.0f;
        private float yaw = 0.0f;
        private float roll = 0.0f;

        private int selectedX = 0;
        private int selectedY = 0;
        private int selectedZ = 0;

        private float insertion = 10.0f;

        private static float[] vertices = {
                                    // Front
                                    0.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,

                                    0.0f, 1.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 1.0f, 0.0f,

                                    // Back
                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 1.0f, 1.0f,
                                    1.0f, 0.0f, 1.0f,

                                    0.0f, 1.0f, 1.0f,
                                    1.0f, 1.0f, 1.0f,
                                    1.0f, 0.0f, 1.0f,

                                    // Left
                                    0.0f, 0.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f,

                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 1.0f,

                                    // Right
                                    1.0f, 1.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 1.0f,

                                    1.0f, 1.0f, 0.0f,
                                    1.0f, 0.0f, 1.0f,
                                    1.0f, 1.0f, 1.0f,

                                    // Bottom
                                    0.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 1.0f,
                                    1.0f, 0.0f, 1.0f,

                                    1.0f, 0.0f, 0.0f,
                                    0.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 1.0f,

                                    // Top
                                    0.0f, 1.0f, 1.0f,
                                    0.0f, 1.0f, 0.0f,
                                    1.0f, 1.0f, 1.0f,

                                    0.0f, 1.0f, 0.0f,
                                    1.0f, 1.0f, 0.0f,
                                    1.0f, 1.0f, 1.0f
                                   };

        private static int verticesCount = vertices.Length / 3;

        private static float[] normals = {
                                    // Front
                                    0.0f, 0.0f, -1.0f,
                                    0.0f, 0.0f, -1.0f,
                                    0.0f, 0.0f, -1.0f,

                                    0.0f, 0.0f, -1.0f,
                                    0.0f, 0.0f, -1.0f,
                                    0.0f, 0.0f, -1.0f,

                                    // Back
                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f,

                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f,

                                    // Left
                                    -1.0f, 0.0f, 0.0f,
                                    -1.0f, 0.0f, 0.0f,
                                    -1.0f, 0.0f, 0.0f,

                                    -1.0f, 0.0f, 0.0f,
                                    -1.0f, 0.0f, 0.0f,
                                    -1.0f, 0.0f, 0.0f,

                                    // Right
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,

                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,

                                    // Bottom
                                    0.0f, -1.0f, 0.0f,
                                    0.0f, -1.0f, 0.0f,
                                    0.0f, -1.0f, 0.0f,

                                    0.0f, -1.0f, 0.0f,
                                    0.0f, -1.0f, 0.0f,
                                    0.0f, -1.0f, 0.0f,

                                    // Top
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,

                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f
                                  };

        private LiquidModel liquidModel;

        public Program(uint width, uint height, uint depth, float consistency)
        {
            liquidModel = new LiquidModel(width, height, depth, consistency);
        }

        private static Color4 Color4FromValue(float value, float alpha)
        {
            float r = ((value * 0.25f) % 1.0f) * 0.5f + 0.5f;
            float g = ((value * 0.5f) % 1.0f) * 0.5f + 0.5f;
            float b = ((value * 0.75f) % 1.0f) * 0.5f + 0.5f;
            Color4 color = new Color4(r, g, b, alpha);
            return color;
        }

        public static Matrix4 MatrixFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            return Matrix4.CreateFromQuaternion(QuaternionFromYawPitchRoll(yaw, pitch, roll));
        }

        public static Quaternion QuaternionFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            Quaternion result = Quaternion.Identity;
            float num9 = roll * 0.5f;
            float num6 = (float)Math.Sin((double)num9);
            float num5 = (float)Math.Cos((double)num9);
            float num8 = pitch * 0.5f;
            float num4 = (float)Math.Sin((double)num8);
            float num3 = (float)Math.Cos((double)num8);
            float num7 = yaw * 0.5f;
            float num2 = (float)Math.Sin((double)num7);
            float num = (float)Math.Cos((double)num7);
            result.X = ((num * num4) * num5) + ((num2 * num3) * num6);
            result.Y = ((num2 * num3) * num5) - ((num * num4) * num6);
            result.Z = ((num * num3) * num6) - ((num2 * num4) * num5);
            result.W = ((num * num3) * num5) + ((num2 * num4) * num6);
            return result;
        }

        public void Run()
        {
            using (GameWindow game = new GameWindow(600, 400, GraphicsMode.Default, "3D Diffusion"))
            {
                game.Load += (sender, e) =>
                {
                    game.VSync = VSyncMode.On;
                };

                game.Resize += (sender, e) =>
                {
                    GL.Viewport(0, 0, game.Width, game.Height);
                };

                game.UpdateFrame += (sender, e) =>
                {
                    if (game.Keyboard[Key.Space])
                    {
                        liquidModel.SetState((uint)selectedX, (uint)selectedY, (uint)selectedZ, liquidModel.GetState((uint)selectedX, (uint)selectedY, (uint)selectedZ) + insertion);
                    }

                    liquidModel.Update(e.Time);
                };

                game.Keyboard.KeyDown += (sender, e) =>
                {
                    Key key = e.Key;
                    switch (key)
                    {
                        case Key.Escape:
                            {
                                game.Exit();
                                break;
                            }
                        case Key.W:
                            {
                                selectedX += 1;
                                if (selectedX >= liquidModel.Width)
                                {
                                    selectedX = (int)liquidModel.Width - 1;
                                }
                                break;
                            }
                        case Key.S:
                            {
                                selectedX -= 1;
                                if (selectedX < 0)
                                {
                                    selectedX = 0;
                                }
                                break;
                            }
                        case Key.E:
                            {
                                selectedY += 1;
                                if (selectedY >= liquidModel.Height)
                                {
                                    selectedY = (int)liquidModel.Height - 1;
                                }
                                break;
                            }
                        case Key.Q:
                            {
                                selectedY -= 1;
                                if (selectedY < 0)
                                {
                                    selectedY = 0;
                                }
                                break;
                            }
                        case Key.D:
                            {
                                selectedZ += 1;
                                if (selectedZ >= liquidModel.Depth)
                                {
                                    selectedZ = (int)liquidModel.Depth - 1;
                                }
                                break;
                            }
                        case Key.A:
                            {
                                selectedZ -= 1;
                                if (selectedZ < 0)
                                {
                                    selectedZ = 0;
                                }
                                break;
                            }
                    }
                };

                game.Mouse.Move += (sender, e) =>
                {
                    yaw += ((float)Math.PI / 180.0f) * e.XDelta;
                    pitch += ((float)Math.PI / 180.0f) * e.YDelta;
                    if (pitch < -(float)Math.PI * 0.25f)
                    {
                        pitch = -(float)Math.PI * 0.25f;
                    }
                    else if (pitch > (float)Math.PI * 0.25f)
                    {
                        pitch = (float)Math.PI * 0.25f;
                    }
                };

                game.Mouse.WheelChanged += (sender, e) =>
                {
                    if (e.Delta > 0)
                    {
                        initialEyePosition = Vector3.Subtract(initialEyePosition, changeEyePosition);
                        if (initialEyePosition.Length < minEyePosition.Length)
                        {
                            initialEyePosition = minEyePosition;
                        }
                    }
                    else
                    {
                        initialEyePosition = Vector3.Add(initialEyePosition, changeEyePosition);
                        if (initialEyePosition.Length > maxEyePosition.Length)
                        {
                            initialEyePosition = maxEyePosition;
                        }
                    }
                };

                game.RenderFrame += (sender, e) =>
                {
                    if (game.Width < 1 || game.Height < 1)
                    {
                        return;
                    }
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    GL.MatrixMode(MatrixMode.Projection);
                    Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI * 0.5f, (float)game.Width / (float)game.Height, 0.1f, 100.0f);
                    GL.LoadMatrix(ref perspective);

                    GL.MatrixMode(MatrixMode.Modelview);
                    
                    Matrix4 cameraMoveMatrix = MatrixFromYawPitchRoll(yaw, pitch, roll);
                    Matrix4 lookAtMatrix = Matrix4.LookAt(Vector3.Transform(initialEyePosition, cameraMoveMatrix), lookAtPosition, upVector);

                    GL.Enable(EnableCap.CullFace);
                    GL.CullFace(CullFaceMode.Back);
                    GL.FrontFace(FrontFaceDirection.Cw);

                    GL.Enable(EnableCap.Blend);
                    GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.EnableClientState(ArrayCap.NormalArray);

                    GL.VertexPointer<float>(3, VertexPointerType.Float, 0, vertices);
                    GL.NormalPointer<float>(NormalPointerType.Float, 0, normals);


                    float scaleX = 1.0f / (float)liquidModel.Width;
                    float scaleY = 1.0f / (float)liquidModel.Height;
                    float scaleZ = 1.0f / (float)liquidModel.Depth;
                    Matrix4 scaleMatrix = Matrix4.CreateScale(scaleX, scaleY, scaleZ);

                    float minAlpha = 1.0f / (float)Math.Max(liquidModel.Width, Math.Max(liquidModel.Height, liquidModel.Depth));

                    for (uint i = 0; i < liquidModel.Width; i++)
                    {
                        for (uint j = 0; j < liquidModel.Height; j++)
                        {
                            for (uint k = 0; k < liquidModel.Depth; k++)
                            {
                                bool isSelected = i == selectedX && j == selectedY && k == selectedZ;
                                Color4 color;
                                if (!isSelected)
                                {
                                    float value = liquidModel.GetState(i, j, k);
                                    color = Color4FromValue(value, minAlpha);
                                }
                                else
                                {
                                    color = Color4.White;
                                }
                                GL.Color4(color);

                                Matrix4 translateMatrix = Matrix4.CreateTranslation((float)i * scaleX, (float)j * scaleY, (float)k * scaleZ);
                                GL.LoadMatrix(ref lookAtMatrix);
                                GL.MultMatrix(ref initialTranslateMatrix);
                                GL.MultMatrix(ref translateMatrix);
                                GL.MultMatrix(ref scaleMatrix);
                                GL.DrawArrays(PrimitiveType.Triangles, 0, verticesCount);
                            }
                        }
                    }

                    GL.DisableClientState(ArrayCap.VertexArray);
                    GL.DisableClientState(ArrayCap.NormalArray);

                    GL.Disable(EnableCap.Blend);
                    GL.Disable(EnableCap.CullFace);

                    game.SwapBuffers();
                };

                game.Run(60.0);
            }
        }

        public static void Main()
        {
            Program prog = new Program(16, 16, 16, 40.0f);
            prog.Run();
        }
    }
}