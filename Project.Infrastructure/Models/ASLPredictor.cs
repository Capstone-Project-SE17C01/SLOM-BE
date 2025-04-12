using Microsoft.ML.OnnxRuntime;
using OpenCvSharp;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Project.Infrastructure.Model.ASLPredictor {
    public class ASLPredictor : IDisposable {
        private readonly InferenceSession _session;
        private readonly Dictionary<int, string> _labels;
        private readonly List<Mat> _frameBuffer = new List<Mat>();
        private readonly int _frameCount = 10;
        private readonly float _threshold = 0.50f;

        public ASLPredictor(string modelPath) {
            _session = new InferenceSession(modelPath);

            _labels = new Dictionary<int, string>
            {
                {0, "book"}, {1, "chair"}, {2, "clothes"}, {3, "computer"},
                {4, "drink"}, {5, "drum"}, {6, "family"}, {7, "football"},
                {8, "go"}, {9, "hat"}, {10, "hello"}, {11, "kiss"},
                {12, "like"}, {13, "play"}, {14, "school"}, {15, "street"},
                {16, "table"}, {17, "university"}, {18, "violin"}, {19, "wall"}
            };
        }

        public void AddFrame(Mat frame) {
            var resizedFrame = new Mat();
            Cv2.Resize(frame, resizedFrame, new Size(224, 224));

            resizedFrame.ConvertTo(resizedFrame, MatType.CV_32F, 1.0 / 255.0);

            _frameBuffer.Add(resizedFrame.Clone());

            if (_frameBuffer.Count > _frameCount)
                _frameBuffer.RemoveAt(0);
        }

        public (string word, float confidence) Predict() {
            if (_frameBuffer.Count < _frameCount)
                return ("none", 0);

            var shape = new int[] { 1, _frameCount, 224, 224, 3 };
            var tensorSize = shape.Aggregate(1, (acc, dim) => acc * dim);
            var tensorData = new float[tensorSize];

            int index = 0;
            for (int b = 0; b < 1; b++) {
                for (int f = 0; f < _frameCount; f++) {
                    for (int h = 0; h < 224; h++) {
                        for (int w = 0; w < 224; w++) {
                            for (int c = 0; c < 3; c++) {
                                var pixel = _frameBuffer[f].At<Vec3f>(h, w);
                                tensorData[index++] = pixel[c];
                            }
                        }
                    }
                }
            }

            var inputTensor = new DenseTensor<float>(tensorData, shape);

            var inputName = _session.InputMetadata.Keys.First();
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor<float>(inputName, inputTensor) };

            using var results = _session.Run(inputs);
            var output = results.First().AsEnumerable<float>().ToArray();

            int bestIndex = 0;
            float bestValue = 0;

            for (int i = 0; i < output.Length; i++) {
                if (output[i] > bestValue) {
                    bestValue = output[i];
                    bestIndex = i;
                }
            }

            if (bestValue > _threshold)
                return (_labels[bestIndex], bestValue);
            else
                return ("none", bestValue);
        }

        public void Dispose() {
            _session?.Dispose();
            foreach (var frame in _frameBuffer)
                frame.Dispose();
        }
    }
}
