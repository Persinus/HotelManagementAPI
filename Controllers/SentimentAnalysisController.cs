using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; 
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Threading.Tasks;
using SentimentAnalysisExample.Data; // Thêm dòng này

namespace SentimentAnalysisExample
{
    [Route("api/[controller]")]
    [ApiController]
    public class SentimentAnalysisController : ControllerBase
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly PredictionEngine<SentimentData, SentimentPrediction> _predictionEngine;

        public SentimentAnalysisController(IConfiguration configuration)
        {
            _mlContext = new MLContext();
            
            string modelPath = configuration["SentimentModelPath"]; 
            _model = _mlContext.Model.Load(modelPath, out var schema);

            _predictionEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
        }

        [HttpPost]
        public async Task<ActionResult<SentimentPrediction>> AnalyzeSentiment([FromBody] SentimentData input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.SentimentText))
            {
                return BadRequest("Invalid input data.");
            }

            var result = _predictionEngine.Predict(input);

            return Ok(new
            {
                Sentiment = result.Prediction ? " 😊 Đánh giá tích cực " : " 😞 đánh giá tiêu cực",
                
                Probability = result.Probability
            });
        }
    }
}