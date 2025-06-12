namespace ImageWebApi.Request.Images;

public class ResizeImageRequest
{
    public string? TypeOfImageToResize { get; set; }
    public int DesiredHeight { get; set; }
}
