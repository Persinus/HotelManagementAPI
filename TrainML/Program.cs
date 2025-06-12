using Microsoft.ML;
using Microsoft.ML.Data;
using System;

namespace SentimentAnalysisExample
{
    // ==================== GIẢI THÍCH ====================
    // Ví dụ dữ liệu trong file data.csv:
    // "Phòng chưa được dọn khi tôi nhận phòng.",0     → Tiêu cực
    // "Cửa phòng chắc chắn an toàn và cách âm tốt.",1 → Tích cực
    // "Phòng chưa đáp ứng yêu cầu của tôi.",0
    // "Không gian phòng đẹp sạch và dễ chịu.",1
    // "Phòng có gián bò trong tủ quần áo.",0
    // "Phòng ấm cúng nội thất gỗ đẹp mắt.",1
    // ====================================================

    // Dữ liệu đầu vào để huấn luyện/dự đoán



    /* 
    Thuật toán: SdcaLogisticRegression 
    (Stochastic Dual Coordinate Ascent cho hồi quy logistic)

    Đây là một mô hình phân loại nhị phân mạnh mẽ, phù hợp với dữ liệu văn bản 
    sau khi đã chuyển thành vector số.

    Các bước chính để phân biệt cảm xúc:

    🔹 Bước 1 – FeaturizeText:
    - Văn bản được chuyển thành vector số (sử dụng Bag-of-Words hoặc TF-IDF).
    - Ví dụ: "Phòng sạch" → [0.12, 0.03, 0, ..., 0.5]

   🔹 Bước 2 – Hồi quy Logistic:
    - Mô hình học mối liên hệ giữa từ/cụm từ và cảm xúc.
    - Từ tích cực như "sạch", "tiện nghi", "hài lòng" → tăng xác suất true.
    - Từ tiêu cực như "bẩn", "ồn", "tệ" → tăng xác suất false.

   🔹 Bước 3 – Dự đoán:
   - Khi nhập bình luận mới như "Phòng rất ồn ào và dơ", mô hình:
  1. Chuyển thành vector đặc trưng.
  2. Tính xác suất → nếu < 0.5 → Negative, nếu ≥ 0.5 → Positive.

  để biết thêm vui lòng tham khảo tài liệu chính thức của Microsoft ML.NET tại:

  https://github.com/dotnet/machinelearning
  
    // Mô hình này có thể được sử dụng trong các ứng dụng như chatbot, phân tích cảm xúc khách hàng, v.v.
    // ==================================================== 

*/

    public class SentimentData
    {
        [LoadColumn(0)] public string? SentimentText { get; set; }  // Câu đánh giá
        [LoadColumn(1)] public bool Sentiment { get; set; }         // Nhãn: true = tích cực, false = tiêu cực
    }

    // Kết quả trả ra từ mô hình dự đoán
    public class SentimentPrediction
    {
        [ColumnName("PredictedLabel")] public bool Prediction { get; set; } // Nhãn dự đoán
        public float Probability { get; set; }  // Xác suất dự đoán đúng
        public float Score { get; set; }        // Điểm số (logistic regression)
    }

    class Program
    {
        static void Main(string[] args)
        {
            var context = new MLContext();

            // 1. Load dữ liệu từ file CSV để huấn luyện
            // Dữ liệu gồm 2 cột: câu đánh giá và nhãn (0 = tiêu cực, 1 = tích cực)
            var data = context.Data.LoadFromTextFile<SentimentData>(
                path: "data.csv",
                separatorChar: ',',
                hasHeader: true
            );

            // 2. Tạo pipeline xử lý văn bản và huấn luyện mô hình
            var pipeline = context.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",  // Cột đầu ra là đặc trưng dạng số
                    inputColumnName: nameof(SentimentData.SentimentText) // Cột đầu vào là văn bản
                )
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(SentimentData.Sentiment) // Sử dụng nhãn làm cột mục tiêu
                ));

            // 3. Huấn luyện mô hình từ dữ liệu đã xử lý
            var model = pipeline.Fit(data);

            // 4. Tạo 1 câu ví dụ để dự đoán cảm xúc
            var newSentiment = new SentimentData { SentimentText = "Phòng này sạch và tiện nghi." };

            // 5. Dự đoán bằng mô hình đã huấn luyện
            var predictionFunction = context.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            var result = predictionFunction.Predict(newSentiment);

            // 6. In kết quả dự đoán ra màn hình
            Console.WriteLine($"Sentiment: {(result.Prediction ? "Positive" : "Negative")} with probability of {result.Probability}");

            // 7. Lưu mô hình để sử dụng sau (trong API chẳng hạn)
            context.Model.Save(model, data.Schema, "sentimentModel.zip");
        }
    }
}
