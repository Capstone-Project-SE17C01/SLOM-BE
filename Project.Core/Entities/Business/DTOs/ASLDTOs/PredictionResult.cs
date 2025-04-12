namespace Project.Core.Entities.Business.DTOs.ASLDTOs {
    public class PredictionResult {
        public string Word { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public bool IsRecognized => Word != "none";
    }
}
