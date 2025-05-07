using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;  // Đảm bảo bạn đã cài Dapper qua NuGet
using System.Threading.Tasks;
using System.Linq;
using HotelManagementAPI.Models;
using System.Text.RegularExpressions;

namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhongWithTienNghiQueryController : ControllerBase
    {
        private readonly IDbConnection _db;

        // Constructor nhận vào IDbConnection để kết nối cơ sở dữ liệu
        public PhongWithTienNghiQueryController(IDbConnection db)
        {
            _db = db;
        }

        // Endpoint để xử lý truy vấn phòng
        [HttpPost("process")]
        public async Task<IActionResult> ProcessQuery([FromBody] QueryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest(new { error = "Câu hỏi không được để trống." });
            }

            // Xử lý câu hỏi và trích xuất thông tin
            var extractedInfo = ExtractRoomQuery(request.Question);

            if (extractedInfo.RoomCode == null)
            {
                return Ok(new { message = "Không tìm thấy mã phòng hợp lệ trong câu hỏi." });
            }

            // Lấy thông tin phòng từ cơ sở dữ liệu
            var roomDetails = await GetRoomDetails(extractedInfo.RoomCode);

            if (roomDetails == null)
            {
                return Ok(new { message = $"Không tìm thấy thông tin cho phòng {extractedInfo.RoomCode}." });
            }

            // Lọc chỉ các trường thông tin yêu cầu từ extractedInfo.Fields
            var filteredRoomDetails = FilterRoomDetails(roomDetails, extractedInfo.Fields);
            string responseText = GenerateResponseText(extractedInfo.RoomCode, filteredRoomDetails);
            // Trả về các trường yêu cầu
            return Ok(new
            {
                roomCode = extractedInfo.RoomCode,
                fields = extractedInfo.Fields.Any() ? extractedInfo.Fields.ToArray() : new[] { "Không xác định được trường phụ nào." },
                roomDetails = filteredRoomDetails,
                 response = responseText
            });
        }

        // Hàm trích xuất mã phòng và các trường thông tin từ câu hỏi
        private RoomQueryResult ExtractRoomQuery(string query)
        {
            // Biểu thức chính quy để tìm mã phòng
            string roomCodePattern = @"\bP\d{3}\b"; // Đảm bảo mã phòng có định dạng "Pxxx"
            // Biểu thức chính quy để tìm các trường thông tin
            string fieldPattern = @"loại phòng|giá phòng|tình trạng|mô tả|tiện nghi|hình ảnh|sao trung bình|tầng|kiểu giường";

            // Tìm mã phòng
            var roomMatch = Regex.Match(query, roomCodePattern, RegexOptions.IgnoreCase);
            // Loại bỏ mã phòng khỏi câu hỏi để tìm các trường thông tin
            string queryWithoutRoomCode = Regex.Replace(query, roomCodePattern, "", RegexOptions.IgnoreCase).Trim();
            var fieldMatches = Regex.Matches(queryWithoutRoomCode, fieldPattern, RegexOptions.IgnoreCase)
                                     .Cast<Match>()
                                     .Select(m => m.Value)
                                     .ToList();

            return new RoomQueryResult
            {
                RoomCode = roomMatch.Success ? roomMatch.Value : null,
                Fields = fieldMatches
            };
        }

        // Hàm lấy thông tin phòng từ cơ sở dữ liệu sử dụng Dapper
        private async Task<PhongWithTienNghi> GetRoomDetails(string roomCode)
        {
            // Sử dụng Dapper để truy vấn dữ liệu từ cơ sở dữ liệu
            var sql = "SELECT * FROM PhongWithTienNghi WHERE maPhong = @RoomCode";  // Sửa lại thành "maPhong"
            var roomDetails = await _db.QueryFirstOrDefaultAsync<PhongWithTienNghi>(sql, new { RoomCode = roomCode });

            return roomDetails;
        }

        // Hàm lọc các trường yêu cầu từ thông tin phòng
        private object FilterRoomDetails(PhongWithTienNghi roomDetails, List<string> fields)
        {
            // Dùng DynamicObject để tạo đối tượng trả về linh động
            var result = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

            // Kiểm tra nếu có yêu cầu trường nào thì trả về giá trị của trường đó
            if (fields.Contains("loại phòng"))
                result["LoaiPhong"] = roomDetails.LoaiPhong;
            if (fields.Contains("giá phòng"))
                result["GiaPhong"] = roomDetails.GiaPhong;
            result["TinhTrang"] = (roomDetails.TinhTrang switch
{
    "1" => "Trống",
    "0" => "Đã có người đặt",
    "3" => "Đang bảo trì",
    _ => "Không xác định"
}).ToString();
            if (fields.Contains("mô tả"))
                result["MoTa"] = roomDetails.MoTa;
            if (fields.Contains("tiện nghi"))
                result["TienNghi"] = roomDetails.TienNghi;
            if (fields.Contains("hình ảnh"))
                result["HinhAnh"] = roomDetails.UrlAnhChinh;
            if (fields.Contains("số lượng phòng"))
                result["SoLuongPhong"] = roomDetails.SoLuongPhong;
            if (fields.Contains("sao trung bình"))
                result["SaoTrungBinh"] = roomDetails.SoSaoTrungBinh;
            if (fields.Contains("tầng"))
                result["Tang"] = roomDetails.Tang;
            if (fields.Contains("kiểu giường"))
                result["KieuGiuong"] = roomDetails.KieuGiuong;

            return result;
        }
        private string GenerateResponseText(string roomCode, object filteredDetails)
{
    var detailsDict = filteredDetails as IDictionary<string, object>;
    if (detailsDict == null || !detailsDict.Any())
        return $"Dạ bạn ơi, hiện mình chưa tìm thấy thông tin chi tiết cho phòng {roomCode} ạ.";

    var parts = new List<string>();

    foreach (var item in detailsDict)
    {
        var label = item.Key;
        var value = item.Value?.ToString() ?? "chưa có thông tin";

        switch (label)
        {
            case "TinhTrang":
                parts.Add($"hiện trạng phòng là đang {value.ToLower()}");
                break;
            case "TienNghi":
                try
                {
                    var list = System.Text.Json.JsonSerializer.Deserialize<List<string>>(value);
                    parts.Add($"tiện nghi gồm: {string.Join(", ", list)}");
                }
                catch
                {
                    parts.Add($"tiện nghi gồm: {value}");
                }
                break;
            case "LoaiPhong":
                parts.Add($"loại phòng là {value}");
                break;
            case "GiaPhong":
                parts.Add($"giá phòng là {value} VNĐ");
                break;
            case "MoTa":
                parts.Add($"mô tả: {value}");
                break;
            case "HinhAnh":
                parts.Add($"hình ảnh phòng: {value}");
                break;
            case "SoLuongPhong":
                parts.Add($"số lượng phòng còn lại là {value}");
                break;
            case "SaoTrungBinh":
                parts.Add($"phòng được đánh giá trung bình {value} sao");
                break;
            case "Tang":
                parts.Add($"phòng nằm ở tầng {value}");
                break;
            case "KieuGiuong":
                parts.Add($"kiểu giường là : {value}");
                break;
            default:
                parts.Add($"{label}: {value}");
                break;
        }
    }

    return $"Dạ bạn ơi, thông tin phòng {roomCode} như sau: {string.Join(", ", parts)} ạ.";
}

    }
     
    public class RoomQueryResult
    {
        public string RoomCode { get; set; }
        public List<string> Fields { get; set; }
    }

    public class QueryRequest
    {
        public string Question { get; set; }
    }
}
