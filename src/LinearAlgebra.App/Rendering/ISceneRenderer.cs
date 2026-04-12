using SkiaSharp;

namespace LinearAlgebra.App.Rendering;

public interface ISceneRenderer
{
    void Render(SKCanvas canvas, SKSize size);
}
