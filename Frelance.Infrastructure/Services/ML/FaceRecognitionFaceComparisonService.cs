using Frelance.Application.Repositories.ML;
using Frelance.Contracts.Exceptions;
using Frelance.Contracts.Responses;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace Frelance.Infrastructure.Services.ML;

public class FaceRecognitionFaceComparisonService : IFaceComparisionService
{
    private const double ThresholdDistance = 0.8;
    private readonly CascadeClassifier _faceDetector;
    private readonly Net _faceRecognitionNet;

    public FaceRecognitionFaceComparisonService()
    {
        var baseDir = AppContext.BaseDirectory;
        var modelDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "models"));

        var faceCascadePath = Path.Combine(modelDir, "haarcascade_frontalface_default.xml");
        if (!File.Exists(faceCascadePath))
            throw new FileNotFoundException("Face cascade not found", faceCascadePath);
        _faceDetector = new CascadeClassifier(faceCascadePath);

        var faceRecognitionModelPath = Path.Combine(modelDir, "openface.nn4.small2.v1.t7");
        if (!File.Exists(faceRecognitionModelPath))
            throw new FileNotFoundException("Face recognition model not found", faceRecognitionModelPath);

        _faceRecognitionNet = CvDnn.ReadNetFromTorch(faceRecognitionModelPath)
                              ?? throw new Exception("Failed to load face recognition model.");
    }

    public async Task<FaceComparisonResult> CompareFacesAsync(string base64Face1, string base64Face2)
    {
        using var mat1 = Base64ToMat(base64Face1);
        using var mat2 = Base64ToMat(base64Face2);

        using var face1 = PreprocessFace(mat1);
        using var face2 = PreprocessFace(mat2);

        if (face1.Empty() || face2.Empty())
            throw new NotFoundException("Face not detected in one or both images");

        var inputSize = new Size(96, 96);
        using var resized1 = new Mat();
        using var resized2 = new Mat();
        Cv2.Resize(face1, resized1, new Size(face1.Width, face1.Height));
        Cv2.Resize(face2, resized2, new Size(face2.Width, face2.Height));
        Cv2.Resize(resized1, resized1, inputSize);
        Cv2.Resize(resized2, resized2, inputSize);

        var embedding1 = ComputeFaceEmbedding(resized1);
        var embedding2 = ComputeFaceEmbedding(resized2);

        var distance = embedding1.Select((t, i) => t - embedding2[i]).Sum(diff => diff * (double)diff);
        distance = Math.Sqrt(distance);

        var isMatch = distance < ThresholdDistance;
        var confidence = Math.Max(0, 1 - distance / ThresholdDistance);

        return await Task.FromResult(new FaceComparisonResult(isMatch, confidence));
    }

    private float[] ComputeFaceEmbedding(Mat face)
    {
        using var blob = CvDnn.BlobFromImage(face, 1.0 / 255.0, new Size(face.Width, face.Height), new Scalar(0, 0, 0),
            true, false);
        _faceRecognitionNet.SetInput(blob);
        using var prob = _faceRecognitionNet.Forward();

        prob.GetArray(out float[] embedding);
        return embedding;
    }

    private static Mat Base64ToMat(string base64)
    {
        if (base64.StartsWith("data:image", StringComparison.OrdinalIgnoreCase))
            base64 = base64[(base64.IndexOf(',') + 1)..];
        var data = Convert.FromBase64String(base64);
        return Cv2.ImDecode(data, ImreadModes.Color);
    }

    private Mat PreprocessFace(Mat image)
    {
        using var gray = new Mat();
        Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
        var faces = _faceDetector.DetectMultiScale(gray);
        if (faces.Length == 0)
            return new Mat();
        var faceRect = faces.OrderByDescending(r => r.Width * r.Height).First();
        var faceRoi = new Mat(image, faceRect);
        return faceRoi;
    }
}