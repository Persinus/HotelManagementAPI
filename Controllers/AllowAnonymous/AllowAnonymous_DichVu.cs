using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagementAPI.Controllers.AllowAnonymous
{
    /// <summary>
    /// Controller cho phép xem danh sách dịch vụ (không yêu cầu đăng nhập).
    /// </summary>
    [ApiController]
    [Route("api/dichvu")]
    public class AllowAnonymous_DichVuController : ControllerBase
    {
        private readonly IDbConnection _db;

        /// <summary>
        /// Khởi tạo controller với kết nối cơ sở dữ liệu.
        /// </summary>
        /// <param name="db">Kết nối cơ sở dữ liệu.</param>
        public AllowAnonymous_DichVuController(IDbConnection db)
        {
            _db = db;
        }

        /// <summary>
        /// Lấy danh sách toàn bộ dịch vụ.
        /// </summary>
        /// <returns>Danh sách dịch vụ.</returns>
        [HttpGet]
        [AllowAnonymous] // Cho phép truy cập mà không cần đăng nhập
        public async Task<IActionResult> GetAllDichVu()
        {
            const string query = @"
                SELECT 
                    MaDichVu, 
                    TenDichVu, 
                    DonGia, 
                    MoTaDichVu, 
                    HinhAnhDichVu, 
                    SoLuong, 
                    TrangThai, 
                    LoaiDichVu, 
                    DonViTinh
                FROM DichVu";

            var dichVuList = await _db.QueryAsync<DichVuDTO>(query);

            return Ok(dichVuList);
        }
    }
}