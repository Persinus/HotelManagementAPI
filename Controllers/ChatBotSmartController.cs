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
    public class ChatbotSmartController : ControllerBase
    {
        private readonly IDbConnection _db;

        // Constructor nhận vào IDbConnection để kết nối cơ sở dữ liệu
        public ChatbotSmartController(IDbConnection db)
        {
            _db = db;
        }

        // Endpoint để xử lý truy vấn phòng
        /// <summary>
        /// Xử lý câu hỏi từ người dùng.
        /// </summary>
       
        /// <remarks>
        /// <para><b>Mô tả:</b> API này nhận câu hỏi từ người dùng và trả về thông tin liên quan đến phòng hoặc dịch vụ.</para>
        /// <para><b>Từ khóa hỗ trợ:</b></para>
        /// <list type="bullet">
        /// <item>
        /// <term>Phòng</term>
        /// <description>"loại phòng", "giá phòng", "tình trạng", "mô tả", "tiện nghi", "hình ảnh", "số lượng phòng", "sao trung bình", "tầng", "kiểu giường".</description>
        /// </item>
        /// <item>
        /// <term>Dịch vụ</term>
        /// <description>"tên dịch vụ", "đơn giá", "mô tả", "hình ảnh", "trạng thái".</description>
        /// </item>
        /// <item>
        /// <term>Từ khóa đặc biệt</term>
        /// <description>"tất cả", "thông tin".</description>
        /// </item>
        /// </list>
        /// <para><b>Ví dụ câu hỏi:</b></para>
        /// <list type="bullet">
        /// <item>
        /// <term>Câu hỏi về phòng</term>
        /// <description>
        /// "Tôi muốn biết thông tin về phòng P201."<br />
        /// "Phòng P202 có những tiện nghi gì?"<br />
        /// "Giá phòng P303 là bao nhiêu?"<br />
        /// "Tôi muốn biết tất cả phòng."<br />
        /// "Khách sạn có bao nhiêu phòng và giá phòng dao động như thế nào?"<br />
        /// </description>
        /// </item>
        /// <item>
        /// <term>Câu hỏi về dịch vụ</term>
        /// <description>
        /// "Tôi muốn biết thông tin về dịch vụ DV001."<br />
        /// "Dịch vụ DV002 có giá bao nhiêu?"<br />
        /// "Tôi muốn biết tất cả dịch vụ."<br />
        /// "Khách sạn có những dịch vụ nào và giá dao động ra sao?"<br />
        /// </description>
        /// </item>
        /// <item>
        /// <term>Câu hỏi kết hợp</term>
        /// <description>
        /// "Tôi muốn biết thông tin về phòng P101 và dịch vụ DV001."<br />
        /// "Phòng P202 và dịch vụ DV002 có gì đặc biệt?"<br />
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="request">Câu hỏi từ người dùng.</param>
        /// <returns>Thông tin liên quan đến phòng hoặc dịch vụ.</returns>
        [HttpPost("process")]
        public async Task<IActionResult> ProcessQuery([FromBody] QueryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return Ok(new
                {
                    message = "🤔 Bạn hãy hỏi gì đó đi, nhớ phải kèm theo từ khóa nhé! Ví dụ: 'Tôi muốn biết thông tin về phòng P201.' hoặc 'Tôi muốn biết tất cả dịch vụ.'"
                });
            }

            // Kiểm tra nếu câu hỏi chứa từ khóa "tất cả"
            if (request.Question.Contains("tất cả", StringComparison.OrdinalIgnoreCase))
            {
                if (request.Question.Contains("phòng", StringComparison.OrdinalIgnoreCase))
                {
                    var hotelOverview = await GetHotelOverview();
                    return Ok(new { message = hotelOverview });
                }
                else if (request.Question.Contains("dịch vụ", StringComparison.OrdinalIgnoreCase))
                {
                    var serviceOverview = await GetServiceOverview();
                    return Ok(new { message = serviceOverview });
                }
            }

            // Xử lý câu hỏi và trích xuất thông tin
            var extractedInfo = ExtractQuery(request.Question);

            if (extractedInfo.RoomCode == null)
            {
                return Ok(new
                {
                    message = "🤔 Không tìm thấy mã hợp lệ trong câu hỏi. Hãy thử hỏi như: 'Tôi muốn biết thông tin về phòng P201.' hoặc 'Tôi muốn biết tất cả dịch vụ.'"
                });
            }

            if (extractedInfo.RoomCode.StartsWith("P", StringComparison.OrdinalIgnoreCase))
            {
                var roomDetails = await GetRoomDetails(extractedInfo.RoomCode);

                if (roomDetails == null)
                {
                    return Ok(new { message = $"😔 Không tìm thấy thông tin cho phòng {extractedInfo.RoomCode}." });
                }

                var filteredRoomDetails = FilterRoomDetails(roomDetails, extractedInfo.Fields);
                string responseText = GenerateResponseText(extractedInfo.RoomCode, filteredRoomDetails);

                return Ok(new { message = responseText });
            }
            else if (extractedInfo.RoomCode.StartsWith("DV", StringComparison.OrdinalIgnoreCase))
            {
                var serviceDetails = await GetServiceDetails(extractedInfo.RoomCode);

                if (serviceDetails == null)
                {
                    return Ok(new { message = $"😔 Không tìm thấy thông tin cho dịch vụ {extractedInfo.RoomCode}." });
                }

                var filteredServiceDetails = FilterServiceDetails(serviceDetails, extractedInfo.Fields);
                string responseText = GenerateServiceResponseText(extractedInfo.RoomCode, filteredServiceDetails);

                return Ok(new { message = responseText });
            }

            return Ok(new { message = "🤔 Không xác định được loại câu hỏi. Hãy thử hỏi như: 'Tôi muốn biết thông tin về phòng P201.' hoặc 'Tôi muốn biết tất cả dịch vụ.'" });
        }

        // Hàm trích xuất mã và các trường thông tin từ câu hỏi
        private RoomQueryResult ExtractQuery(string query)
        {
            // Biểu thức chính quy để tìm mã phòng hoặc mã dịch vụ
            string codePattern = @"\b(P\d{3}|DV\d{3})\b"; // Định dạng "Pxxx" hoặc "DVxxx"
            string fieldPattern = @"tên dịch vụ|đơn giá|mô tả|hình ảnh|trạng thái|loại phòng|giá phòng|tình trạng|tiện nghi|sao trung bình|tầng|kiểu giường|thông tin";

            // Tìm mã phòng hoặc mã dịch vụ
            var codeMatch = Regex.Match(query, codePattern, RegexOptions.IgnoreCase);
            string queryWithoutCode = Regex.Replace(query, codePattern, "", RegexOptions.IgnoreCase).Trim();
            var fieldMatches = Regex.Matches(queryWithoutCode, fieldPattern, RegexOptions.IgnoreCase)
                                     .Cast<Match>()
                                     .Select(m => m.Value)
                                     .ToList();

            // Nếu từ khóa "Thông tin" xuất hiện, thêm tất cả các thuộc tính
            if (fieldMatches.Contains("thông tin", StringComparer.OrdinalIgnoreCase))
            {
                if (codeMatch.Value.StartsWith("P", StringComparison.OrdinalIgnoreCase))
                {
                    // Thêm tất cả thuộc tính của phòng
                    fieldMatches = new List<string>
                    {
                        "loại phòng", "giá phòng", "tình trạng", "mô tả", "tiện nghi", "hình ảnh", "số lượng phòng", "sao trung bình", "tầng", "kiểu giường"
                    };
                }
                else if (codeMatch.Value.StartsWith("DV", StringComparison.OrdinalIgnoreCase))
                {
                    // Thêm tất cả thuộc tính của dịch vụ
                    fieldMatches = new List<string>
                    {
                        "tên dịch vụ", "đơn giá", "mô tả", "hình ảnh", "trạng thái"
                    };
                }
            }

            return new RoomQueryResult
            {
                RoomCode = codeMatch.Success ? codeMatch.Value : null,
                Fields = fieldMatches
            };
        }

        // Hàm lấy thông tin phòng từ cơ sở dữ liệu sử dụng Dapper
        private async Task<PhongWithTienNghi> GetRoomDetails(string roomCode)
        {
            var sql = "SELECT * FROM PhongWithTienNghi WHERE maPhong = @RoomCode";
            var roomDetails = await _db.QueryFirstOrDefaultAsync<PhongWithTienNghi>(sql, new { RoomCode = roomCode });

            return roomDetails;
        }

        // Hàm lấy thông tin dịch vụ từ cơ sở dữ liệu sử dụng Dapper
        private async Task<DichVu> GetServiceDetails(string serviceCode)
        {
            // Sử dụng Dapper để truy vấn dữ liệu từ cơ sở dữ liệu
            var sql = "SELECT * FROM DichVu WHERE MaDichVu = @ServiceCode";
            var serviceDetails = await _db.QueryFirstOrDefaultAsync<DichVu>(sql, new { ServiceCode = serviceCode });

            return serviceDetails;
        }

        // Hàm lấy thông tin tổng quan về khách sạn
        private async Task<string> GetHotelOverview()
        {
            var sql = "SELECT * FROM PhongWithTienNghi";
            var allRooms = (await _db.QueryAsync<PhongWithTienNghi>(sql)).ToList();

            if (!allRooms.Any())
            {
                return "😔 Dạ thưa quý khách, hiện tại khách sạn Ocean của chúng tôi không có phòng nào khả dụng.";
            }

            int totalRooms = allRooms.Count;
            decimal minPrice = allRooms.Min(r => r.GiaPhong);
            decimal maxPrice = allRooms.Max(r => r.GiaPhong);

            var allAmenities = allRooms
                .Where(r => !string.IsNullOrEmpty(r.TienNghi))
                .SelectMany(r => System.Text.Json.JsonSerializer.Deserialize<List<string>>(r.TienNghi))
                .Distinct()
                .ToList();

            string amenitiesList = string.Join(", ", allAmenities);

            var response = new List<string>
            {
                "😊 Dạ thưa quý khách, khách sạn Ocean của chúng tôi hiện có các phòng như sau:",
                $"- Tổng cộng có {totalRooms} phòng.",
                $"- Giá phòng dao động từ {minPrice:N0} VND đến {maxPrice:N0} VND mỗi đêm.",
                $"- Các tiện nghi bao gồm: {amenitiesList}."
            };

            return string.Join("\n", response);
        }

        // Hàm lấy thông tin tổng quan về dịch vụ
        private async Task<string> GetServiceOverview()
        {
            // Lấy danh sách tất cả dịch vụ từ cơ sở dữ liệu
            var sql = "SELECT * FROM DichVu";
            var allServices = (await _db.QueryAsync<DichVu>(sql)).ToList();

            if (!allServices.Any())
            {
                return "😔 Dạ thưa quý khách, hiện tại khách sạn Ocean của chúng tôi không có dịch vụ nào khả dụng.";
            }

            // Tính số lượng dịch vụ
            int totalServices = allServices.Count;

            // Tính giá dao động (bỏ qua các dịch vụ miễn phí)
            var paidServices = allServices.Where(s => s.DonGia > 0).ToList();
            decimal minPrice = paidServices.Any() ? paidServices.Min(s => s.DonGia) : 0;
            decimal maxPrice = paidServices.Any() ? paidServices.Max(s => s.DonGia) : 0;

            // Tạo danh sách tên dịch vụ
            var freeServices = allServices.Where(s => s.DonGia == 0).Select(s => s.TenDichVu).ToList();
            var paidServiceNames = paidServices.Select(s => s.TenDichVu).ToList();

            // Tạo câu trả lời tổng quan
            var response = new List<string>
            {
                "😊 Dạ thưa quý khách, khách sạn Ocean của chúng tôi hiện có các dịch vụ như sau:",
                $"- Tổng cộng có {totalServices} dịch vụ.",
                paidServices.Any()
                    ? $"- Giá dịch vụ dao động từ {minPrice:N0} VND đến {maxPrice:N0} VND."
                    : "- Hiện tại tất cả các dịch vụ đều miễn phí! 🎉",
                freeServices.Any()
                    ? $"- Các dịch vụ miễn phí bao gồm: {string.Join(", ", freeServices)}."
                    : "- Không có dịch vụ miễn phí nào hiện tại.",
                paidServiceNames.Any()
                    ? $"- Các dịch vụ có tính phí bao gồm: {string.Join(", ", paidServiceNames)}."
                    : "- Không có dịch vụ tính phí nào hiện tại."
            };

            return string.Join("\n", response);
        }

        // Hàm lọc các trường yêu cầu từ thông tin phòng
        private object FilterRoomDetails(PhongWithTienNghi roomDetails, List<string> fields)
        {
            var result = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

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

        // Hàm lọc các trường yêu cầu từ thông tin dịch vụ
        private object FilterServiceDetails(DichVu serviceDetails, List<string> fields)
        {
            var result = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

            if (fields.Contains("tên dịch vụ"))
                result["TenDichVu"] = serviceDetails.TenDichVu;
            if (fields.Contains("đơn giá"))
                result["DonGia"] = serviceDetails.DonGia;
            if (fields.Contains("mô tả"))
                result["MoTaDichVu"] = serviceDetails.MoTaDichVu;
            if (fields.Contains("hình ảnh"))
                result["UrlAnh"] = serviceDetails.UrlAnh;
            if (fields.Contains("trạng thái"))
                result["TrangThai"] = serviceDetails.TrangThai switch
                {
                    1 => "Hoạt động",
                    0 => "Ngừng hoạt động",
                    _ => "Không xác định"
                };

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

        private string GenerateServiceResponseText(string serviceCode, object filteredDetails)
        {
            var detailsDict = filteredDetails as IDictionary<string, object>;
            if (detailsDict == null || !detailsDict.Any())
                return $"Dạ bạn ơi, hiện mình chưa tìm thấy thông tin chi tiết cho dịch vụ {serviceCode} ạ.";

            var parts = new List<string>();

            foreach (var item in detailsDict)
            {
                var label = item.Key;
                var value = item.Value?.ToString() ?? "chưa có thông tin";

                switch (label)
                {
                    case "TenDichVu":
                        parts.Add($"tên dịch vụ là {value}");
                        break;
                    case "DonGia":
                        parts.Add($"đơn giá là {value} VNĐ");
                        break;
                    case "MoTaDichVu":
                        parts.Add($"mô tả: {value}");
                        break;
                    case "UrlAnh":
                        parts.Add($"hình ảnh dịch vụ: {value}");
                        break;
                    case "TrangThai":
                        parts.Add($"trạng thái: {value}");
                        break;
                    default:
                        parts.Add($"{label}: {value}");
                        break;
                }
            }

            return $"Dạ bạn ơi, thông tin dịch vụ {serviceCode} như sau: {string.Join(", ", parts)} ạ.";
        }
    }

    public class QueryResult
    {
        public string Code { get; set; }
        public List<string> Fields { get; set; }
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
