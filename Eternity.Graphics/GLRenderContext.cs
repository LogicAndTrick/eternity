using System;
using System.Drawing;
using System.Drawing.Imaging;
using Eternity.Graphics.Textures;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Eternity.Graphics
{
    public class GLRenderContext : IRenderContext
    {
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public GLRenderContext(int screenWidth, int screenHeight)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
        }

        public void Clear()
        {
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);
        }

        public void Perspective()
        {
            GL.Viewport(0, 0, 640, 480);
            GL.MatrixMode(MatrixMode.Projection);
            var m4 = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), 4f / 3f, 0, 1000);
            GL.LoadMatrix(ref m4);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void Orthographic()
        {
            GL.Viewport(0, 0, 640, 480);
            GL.MatrixMode(MatrixMode.Projection);
            var m4 = Matrix4.CreateOrthographicOffCenter(0, 640, 480, 0, -1, 1);
            GL.LoadMatrix(ref m4);
            GL.MatrixMode(MatrixMode.Modelview);
        }

        public void StartLines()
        {
            GL.Begin(BeginMode.Lines);
        }

        public void StartTriangles()
        {
            GL.Begin(BeginMode.Triangles);
        }

        public void StartQuads()
        {
            GL.Begin(BeginMode.Quads);
        }

        public void StartPolygon()
        {
            GL.Begin(BeginMode.Polygon);
        }

        public void End()
        {
            GL.End();
        }

        public void SetColour(Color c)
        {
            GL.Color4(c);
        }

        public void SetTexture(ITexture tex)
        {
            SetTexture(tex.TextureReference);
        }

        public void EnableTextures()
        {
            GL.Enable(EnableCap.Texture2D);
        }

        public void DisableTextures()
        {
            GL.Disable(EnableCap.Texture2D);
        }

        public void SetTexture(int tref)
        {
            GL.BindTexture(TextureTarget.Texture2D, tref);
        }

        public void Point2(double x, double y)
        {
            GL.Vertex2(x, y);
        }

        public void Point3(double x, double y, double z)
        {
            GL.Vertex3(x, y, z);
        }

        public void TexturePoint(double x, double y)
        {
            GL.TexCoord2(x, y);
        }

        public ITexture CreateTextureFromFile(string file, string name)
        {
            var bitmap = new Bitmap(file);
            return CreateTextureFromImage(bitmap, name);
        }

        public ITexture CreateTextureFromImage(Image image, string name)
        {
            var bitmap = new Bitmap(image);
            var data = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );
            var id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                data.Width, data.Height,
                0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                data.Scan0
            );
            bitmap.UnlockBits(data);
            var w = bitmap.Width;
            var h = bitmap.Height;
            bitmap.Dispose();
            return new GLTexture(id, name, w, h);
        }

        public void Translate(int x, int y)
        {
            GL.Translate(x, y, 0);
        }
    }
}
