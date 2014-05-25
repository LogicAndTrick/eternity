using System.Drawing;
using Eternity.Graphics.Textures;

namespace Eternity.Graphics
{
    public interface IRenderContext
    {
        int ScreenWidth { get; }
        int ScreenHeight { get; }

        void Clear();

        void Perspective();
        void Orthographic();

        void StartLines();
        void StartTriangles();
        void StartQuads();
        void StartPolygon();

        void End();

        void SetColour(Color c);
        void SetTexture(ITexture tex);
        void ClearTexture();

        void EnableTextures();
        void DisableTextures();
        
        void Point2(double x, double y);
        void Point3(double x, double y, double z);

        void TexturePoint(double x, double y);

        ITexture CreateTextureFromFile(string file, string name);
        ITexture CreateTextureFromImage(Image image, string name);

        void Translate(double x, double y);

        void SetScissor(double x, double y, double width, double height);
        void RemoveScissor();
    }
}
