using Microsoft.ML.Data;

namespace SentimentAnalysisExample.Data // Hoặc Models
{
    public class SentimentData
    {
        public string SentimentText { get; set; }
    }

    public class SentimentPrediction : SentimentData
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }

        [ColumnName("Probability")]
        public float Probability { get; set; }
    }
}