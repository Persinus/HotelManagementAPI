using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace SentimentAnalysisExample
{
    public class SentimentData
    {
        [LoadColumn(0)]
        public string? SentimentText { get; set; }  

        [LoadColumn(1)]
        public bool Sentiment { get; set; }
    }

    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
        public float Probability { get; set; }
        public float Score { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var context = new MLContext();

            // Đọc dữ liệu từ file CSV
            var data = context.Data.LoadFromTextFile<SentimentData>("data.csv", separatorChar: ',', hasHeader: true);

            // Tạo pipeline cho việc phân loại cảm xúc
            var pipeline = context.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.SentimentText))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: nameof(SentimentData.Sentiment))); // Chỉ định cột nhãn

            // Huấn luyện mô hình
            var model = pipeline.Fit(data);

            // Dữ liệu mới để dự đoán
            var newSentiment = new SentimentData { SentimentText = "Phòng này sạch và tiện nghi." };
            var predictionFunction = context.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            var result = predictionFunction.Predict(newSentiment);

            // Kết quả
            Console.WriteLine($"Sentiment: {(result.Prediction ? "Positive" : "Negative")} with probability of {result.Probability}");
            context.Model.Save(model, data.Schema, "sentimentModel.zip");
        }
    }
}