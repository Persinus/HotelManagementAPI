using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using HotelManagementAPI.DTOs.KhachHang;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using SentimentAnalysisExample.Data;
using Microsoft.ML;
using Microsoft.Extensions.Configuration;


namespace HotelManagementAPI.Controllers
{
    [ApiController]
    [Route("api/khachhang")]
    [Authorize(Roles = "KhachHang")]
    public class KhachHangController : ControllerBase
    {
        private readonly IDbConnection _db;

        public KhachHangController(IDbConnection db)
        {
            _db = db;
        }

        // ----------- ĐẶT PHÒNG -----------
        /// <summary>
        /// Đặt phòng mới cho khách hàng.
        /// </summary>
        /// <remarks>
        /// Tạo đơn đặt phòng mới, kiểm tra trạng thái phòng và dịch vụ đi kèm.
        /// </remarks>
        // Đặt phòng mới
        [HttpPost("datphong")]
        [SwaggerOperation(
            Summary = "Đặt phòng mới",
            Description = "Tạo đơn đặt phòng mới cho khách hàng, kiểm tra trạng thái phòng và dịch vụ đi kèm."
)]
[SwaggerResponse(200, "Đặt phòng thành công, trả về mã đặt phòng và tổng tiền tạm tính.")]
[SwaggerResponse(400, "Dữ liệu không hợp lệ hoặc phòng/dịch vụ không hợp lệ.")]
[SwaggerResponse(401, "Không xác định được người dùng.")]
public async Task<IActionResult> TaoDonDatPhong([FromBody] KhachHangDatPhongDTO datPhongDTO)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(maNguoiDung))
        return Unauthorized(new { Message = "Không xác định được người dùng." });

    datPhongDTO.MaNguoiDung = maNguoiDung;
    datPhongDTO.MaDatPhong = await GenerateUniqueMaDatPhong();
    datPhongDTO.NgayDat = DateTime.Now;

    // Kiểm tra trạng thái phòng
    const string checkPhongQuery = "SELECT TinhTrang FROM Phong WHERE MaPhong = @MaPhong";
    var tinhTrang = await _db.ExecuteScalarAsync<byte?>(checkPhongQuery, new { datPhongDTO.MaPhong });
    if (tinhTrang != 1)
        return BadRequest(new { Message = "Phòng này không còn trống để đặt." });

    // Kiểm tra trạng thái đặt phòng FE gửi lên
    if (datPhongDTO.TinhTrangDatPhong != 1)
        return BadRequest(new { Message = "Phòng này bạn đã đặt sang hóa đơn để xem." });

    // Kiểm tra dịch vụ đi kèm (nếu có)
    if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
    {
        foreach (var dv in datPhongDTO.DichVuDiKem)
        {
            if (dv.SoLuong <= 0)
                return BadRequest(new { Message = $"Số lượng dịch vụ {dv.MaDichVu} phải lớn hơn 0." });

            const string checkDichVuQuery = "SELECT SoLuong FROM DichVu WHERE MaDichVu = @MaDichVu";
            var dichVuSoLuong = await _db.ExecuteScalarAsync<int?>(checkDichVuQuery, new { dv.MaDichVu });
            if (dichVuSoLuong == null)
                return NotFound(new { Message = $"Mã dịch vụ {dv.MaDichVu} không tồn tại." });

            if (dichVuSoLuong == 0)
                return BadRequest(new { Message = $"Dịch vụ {dv.MaDichVu} đã hết hàng." });

            if (dv.SoLuong > dichVuSoLuong)
                return BadRequest(new { Message = $"Số lượng dịch vụ {dv.MaDichVu} yêu cầu vượt quá số lượng còn lại." });
        }
    }

    // Lấy thông tin giá phòng và giá ưu đãi (nếu có)
    const string getGiaPhongQuery = "SELECT GiaPhong, GiaUuDai FROM Phong WHERE MaPhong = @MaPhong";
    var phong = await _db.QueryFirstOrDefaultAsync<(decimal GiaPhong, decimal? GiaUuDai)>(getGiaPhongQuery, new { MaPhong = datPhongDTO.MaPhong });

    // Chọn giá thanh toán: nếu có giảm giá thì lấy GiaUuDai, không thì lấy GiaPhong
    decimal giaThanhToan = phong.GiaUuDai.HasValue && phong.GiaUuDai.Value < phong.GiaPhong
        ? phong.GiaUuDai.Value
        : phong.GiaPhong;

    // Tính số ngày ở (tối thiểu 1 ngày)
    var soNgay = (datPhongDTO.NgayCheckOut.Date - datPhongDTO.NgayCheckIn.Date).Days;
    if (soNgay < 1) soNgay = 1;

    // Tính tổng tiền dịch vụ
    decimal tongTienDichVu = 0;
    if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
    {
        foreach (var dv in datPhongDTO.DichVuDiKem)
        {
            const string getDonGiaQuery = "SELECT DonGia FROM DichVu WHERE MaDichVu = @MaDichVu";
            var donGia = await _db.ExecuteScalarAsync<decimal>(getDonGiaQuery, new { dv.MaDichVu });
            tongTienDichVu += donGia * dv.SoLuong;
        }
    }

    var tongTien = giaThanhToan * soNgay + tongTienDichVu;

    // Thêm đơn đặt phòng
    const string insertQuery = @"
        INSERT INTO DatPhong (MaDatPhong, MaNguoiDung, MaPhong, NgayDat, NgayCheckIn, NgayCheckOut, TinhTrangDatPhong)
        VALUES (@MaDatPhong, @MaNguoiDung, @MaPhong, @NgayDat, @NgayCheckIn, @NgayCheckOut, @TinhTrangDatPhong)";
    await _db.ExecuteAsync(insertQuery, datPhongDTO);

    // Ngay sau khi insert, cập nhật trạng thái đặt phòng sang 2
    const string updateTinhTrangDatPhong = "UPDATE DatPhong SET TinhTrangDatPhong = 2 WHERE MaDatPhong = @MaDatPhong";
    await _db.ExecuteAsync(updateTinhTrangDatPhong, new { datPhongDTO.MaDatPhong });

    // **Cập nhật trạng thái phòng sang 2 (đã đặt)**
    const string updateTinhTrangPhong = "UPDATE Phong SET TinhTrang = 2 WHERE MaPhong = @MaPhong";
    await _db.ExecuteAsync(updateTinhTrangPhong, new { datPhongDTO.MaPhong });

    // Thêm dịch vụ đi kèm (nếu có)
    if (datPhongDTO.DichVuDiKem != null && datPhongDTO.DichVuDiKem.Any())
    {
        foreach (var dv in datPhongDTO.DichVuDiKem)
        {
            dv.MaDatDichVu = await GenerateUniqueMaDatDichVu();
            dv.MaDatPhong = datPhongDTO.MaDatPhong;
            const string insertDichVu = @"
                INSERT INTO DatDichVu (MaDatDichVu, MaDatPhong, MaDichVu, SoLuong)
                VALUES (@MaDatDichVu, @MaDatPhong, @MaDichVu, @SoLuong)";
            await _db.ExecuteAsync(insertDichVu, dv);

            const string updateDichVuQuery = "UPDATE DichVu SET SoLuong = SoLuong - @SoLuong WHERE MaDichVu = @MaDichVu";
            await _db.ExecuteAsync(updateDichVuQuery, new { dv.SoLuong, dv.MaDichVu });
        }
    }

    // Đặt phòng thành công
    return Ok(new
    {
        Message = $"🎉 Đặt phòng thành công! Tổng tiền tạm tính là: {tongTien:N0} VNĐ. Hãy sang phần tạo hóa đơn để hoàn tất thanh toán.",
        MaDatPhong = datPhongDTO.MaDatPhong,
        TongTienTamTinh = tongTien
    });
}

        /// <summary>
        /// Lấy lịch sử đặt phòng của khách hàng.
        /// </summary>
      
        // Lấy lịch sử đặt phòng
        [HttpGet("datphong/lichsu")]
        [SwaggerOperation(
            Summary = "Lấy lịch sử đặt phòng",
            Description = "Trả về danh sách các đơn đặt phòng của khách hàng hiện tại."
)]
[SwaggerResponse(200, "Danh sách lịch sử đặt phòng.")]
[SwaggerResponse(401, "Không xác định được người dùng.")]
public async Task<IActionResult> LichSuDatPhong()
{
    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(maNguoiDung))
        return Unauthorized(new { Message = "Không xác định được người dùng." });

    const string query = @"
        SELECT dp.MaDatPhong, dp.MaNguoiDung, dp.MaPhong, dp.NgayDat, dp.NgayCheckIn, dp.NgayCheckOut, dp.TinhTrangDatPhong,
               p.LoaiPhong, p.GiaPhong
        FROM DatPhong dp
        INNER JOIN Phong p ON dp.MaPhong = p.MaPhong
        WHERE dp.MaNguoiDung = @MaNguoiDung";

    var result = await _db.QueryAsync(query, new { MaNguoiDung = maNguoiDung });

    // Lấy lịch sử đặt phòng
    if (result == null || !result.Any())
        return NotFound(new { Message = "❌ Xin lỗi, bạn chưa có lịch sử đặt phòng nào." });
    return Ok(new { Message = "✅ Lấy lịch sử đặt phòng thành công.", Data = result });
}

        
        
        // ----------- HÓA ĐƠN -----------
        /// <summary>
        /// Lấy hóa đơn theo mã đặt phòng.
        /// </summary>
        /// <param name="maDatPhong">Mã đặt phòng</param>
        [HttpGet("hoadon/by-madatphong/{maDatPhong}")]
        [SwaggerOperation(
            Summary = "Lấy hóa đơn theo mã đặt phòng",
            Description = "Trả về hóa đơn tương ứng với mã đặt phòng."
        )]
        [SwaggerResponse(200, "Trả về hóa đơn.")]
        [SwaggerResponse(401, "Không xác định được người dùng hoặc không có quyền.")]
        [SwaggerResponse(404, "Không tìm thấy hóa đơn.")]
        
public async Task<IActionResult> GetHoaDonByMaDatPhong([FromRoute] string maDatPhong)
{
    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(maNguoiDung))
        return Unauthorized(new { Message = "Không xác định được người dùng." });

    const string checkQuery = "SELECT MaNguoiDung FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
    var owner = await _db.ExecuteScalarAsync<string>(checkQuery, new { MaDatPhong = maDatPhong });
    if (owner != maNguoiDung)
        return Forbid("Bạn không có quyền xem hóa đơn này.");

    const string query = @"SELECT * FROM HoaDon WHERE MaDatPhong = @MaDatPhong";
    var hoaDon = await _db.QueryFirstOrDefaultAsync<KhachHangHoaDonDTO>(query, new { MaDatPhong = maDatPhong });

    // Lấy hóa đơn theo mã đặt phòng
    if (hoaDon == null)
        return NotFound(new { Message = "❌ Không tìm thấy hóa đơn cho mã đặt phòng này." });
    return Ok(new { Message = "✅ Lấy hóa đơn thành công.", Data = hoaDon });
}

        /// <summary>
        /// Tạo hóa đơn mới cho đơn đặt phòng.
        /// </summary>
        [HttpPost("hoadon/tao")]
        [SwaggerOperation(
            Summary = "Tạo hóa đơn mới",
            Description = "Tạo hóa đơn mới cho đơn đặt phòng đã đặt."
        )]
        [SwaggerResponse(200, "Tạo hóa đơn thành công.")]
        [SwaggerResponse(401, "Không xác định được người dùng hoặc không có quyền.")]
        [SwaggerResponse(404, "Không tìm thấy mã đặt phòng.")]
        
public async Task<IActionResult> TaoHoaDon([FromBody] TaoHoaDonRequestDTO request)
{
    const string datPhongQuery = @"
        SELECT dp.MaDatPhong, dp.MaNguoiDung, dp.MaPhong, dp.NgayCheckIn, dp.NgayCheckOut
        FROM DatPhong dp
        WHERE dp.MaDatPhong = @MaDatPhong";
    var datPhong = await _db.QueryFirstOrDefaultAsync<KhachHangDatPhongDTO>(datPhongQuery, new { request.MaDatPhong });

    if (datPhong == null)
        return NotFound(new { Message = "Không tìm thấy mã đặt phòng." });

    // Kiểm tra quyền sở hữu
    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (datPhong.MaNguoiDung != maNguoiDung)
        return Forbid("Bạn không có quyền tạo hóa đơn cho mã đặt phòng này.");

    const string getGiaPhongQuery = "SELECT GiaPhong FROM Phong WHERE MaPhong = @MaPhong";
    var giaPhong = await _db.ExecuteScalarAsync<decimal>(getGiaPhongQuery, new { datPhong.MaPhong });

    // Tính số ngày ở (tối thiểu 1 ngày)
    var soNgay = (datPhong.NgayCheckOut.Date - datPhong.NgayCheckIn.Date).Days;
    if (soNgay < 1) soNgay = 1;

    // Tính tổng tiền dịch vụ
    const string getDichVuQuery = @"
        SELECT ddv.SoLuong, dv.DonGia
        FROM DatDichVu ddv
        INNER JOIN DichVu dv ON ddv.MaDichVu = dv.MaDichVu
        WHERE ddv.MaDatPhong = @MaDatPhong";
    var dichVus = await _db.QueryAsync<(int SoLuong, decimal DonGia)>(getDichVuQuery, new { datPhong.MaDatPhong });
    decimal tongTienDichVu = dichVus.Sum(x => x.SoLuong * x.DonGia);

    // Tổng tiền hóa đơn
    decimal tongTien = giaPhong * soNgay + tongTienDichVu;

    // Không áp dụng giảm giá từ request nữa
    decimal tongTienSauGiamGia = tongTien; // Giữ nguyên tổng tiền

    string maHoaDon = await GenerateUniqueMaHoaDon();

    const string insertHoaDonQuery = @"
        INSERT INTO HoaDon (MaHoaDon, MaNguoiDung, MaDatPhong, TongTien, NgayTaoHoaDon, NgayThanhToan, TinhTrangHoaDon)
        VALUES (@MaHoaDon, @MaNguoiDung, @MaDatPhong, @TongTien, @NgayTaoHoaDon, @NgayThanhToan, @TinhTrangHoaDon)";
    var hoaDonDTO = new KhachHangHoaDonDTO
    {
        MaHoaDon = maHoaDon,
        MaNguoiDung = datPhong.MaNguoiDung!,
        MaDatPhong = datPhong.MaDatPhong!,
        TongTien = tongTienSauGiamGia,
        NgayTaoHoaDon = DateTime.Now,
        NgayThanhToan = null,
        TinhTrangHoaDon = 1
    };
    await _db.ExecuteAsync(insertHoaDonQuery, hoaDonDTO);

    // Tạo hóa đơn thành công
    return Ok(new { Message = "🎉 Tạo hóa đơn thành công! Vui lòng tiến hành thanh toán.", Data = hoaDonDTO });
}

        // ----------- THANH TOÁN -----------
        /// <summary>
        /// Thanh toán hóa đơn.
        /// </summary>
        [HttpPost("thanhtoan")]
        [SwaggerOperation(
            Summary = "Thanh toán hóa đơn",
            Description = "Thực hiện thanh toán cho hóa đơn của khách hàng."
        )]
        [SwaggerResponse(200, "Thanh toán thành công.")]
        [SwaggerResponse(401, "Không xác định được người dùng hoặc không có quyền.")]
public async Task<IActionResult> ThanhToan([FromBody] KhachHangThanhToanDTO request)
{
    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(maNguoiDung))
        return Unauthorized(new { Message = "Không xác định được người dùng." });

    // Lấy hóa đơn
    const string hoaDonQuery = "SELECT * FROM HoaDon WHERE MaHoaDon = @MaHoaDon";
    var hoaDon = await _db.QueryFirstOrDefaultAsync<KhachHangHoaDonDTO>(hoaDonQuery, new { request.MaHoaDon });
    if (hoaDon == null)
        return NotFound(new { Message = "Không tìm thấy hóa đơn." });

    if (hoaDon.MaNguoiDung != maNguoiDung)
        return Forbid("Bạn không có quyền thanh toán hóa đơn này.");

    if (hoaDon.TinhTrangHoaDon == 2)
        return BadRequest(new { Message = "Hóa đơn này đã được thanh toán." });

    decimal soTienPhaiTra = hoaDon.TongTien;
    decimal soTienKhachTra = request.SoTienThanhToan;

    if (soTienKhachTra < soTienPhaiTra)
    {
        decimal thieu = soTienPhaiTra - soTienKhachTra;
        return BadRequest(new { Message = $"❌ Vui lòng trả đủ số tiền. Bạn còn thiếu {thieu:N0} đồng." });
    }

    // Sinh mã thanh toán tự động
    const string getMaxSql = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaThanhToan, 3, LEN(MaThanhToan)-2) AS INT)), 0) + 1 FROM ThanhToan";
    var nextId = await _db.ExecuteScalarAsync<int>(getMaxSql);
    var maThanhToan = $"TT{nextId:D3}";

    var now = DateTime.Now;

    // Cập nhật hóa đơn: đã thanh toán
    const string updateHoaDonQuery = @"
        UPDATE HoaDon
        SET NgayThanhToan = @NgayThanhToan, TinhTrangHoaDon = 2
        WHERE MaHoaDon = @MaHoaDon";
    await _db.ExecuteAsync(updateHoaDonQuery, new { NgayThanhToan = now, MaHoaDon = request.MaHoaDon });

    // Thêm bản ghi thanh toán
    const string insertThanhToanQuery = @"
        INSERT INTO ThanhToan (MaThanhToan, MaHoaDon, SoTienThanhToan, NgayThanhToan, PhuongThucThanhToan, TinhTrangThanhToan)
        VALUES (@MaThanhToan, @MaHoaDon, @SoTienThanhToan, @NgayThanhToan, @PhuongThucThanhToan, @TinhTrangThanhToan)";
    await _db.ExecuteAsync(insertThanhToanQuery, new
    {
        MaThanhToan = maThanhToan,
        MaHoaDon = request.MaHoaDon,
        SoTienThanhToan = soTienKhachTra,
        NgayThanhToan = now,
        PhuongThucThanhToan = request.PhuongThucThanhToan,
        TinhTrangThanhToan = 2 // 1: Đã thanh toán
    });

    if (soTienKhachTra == soTienPhaiTra)
    {
        return Ok(new { Message = "🎉 Thanh toán thành công! Cảm ơn bạn đã sử dụng dịch vụ.", MaThanhToan = maThanhToan, NgayThanhToan = now });
    }
    else // Khách trả thừa
    {
        decimal tienThua = soTienKhachTra - soTienPhaiTra;
        return Ok(new
        {
            Message = $"🎉 Thanh toán thành công! Bạn đã trả thừa {tienThua:N0} đồng, số tiền này sẽ được nhân viên hoàn lại.",
            MaThanhToan = maThanhToan,
            NgayThanhToan = now
        });
    }
}

        /// <summary>
        /// Lấy lịch sử thanh toán của khách hàng.
        /// </summary>
        [HttpGet("thanhtoan/lichsu")]
        [SwaggerOperation(
            Summary = "Lấy lịch sử thanh toán",
            Description = "Trả về danh sách các giao dịch thanh toán của khách hàng."
        )]
        [SwaggerResponse(200, "Danh sách lịch sử thanh toán.")]
        [SwaggerResponse(401, "Không xác định được người dùng.")]
        [SwaggerResponse(404, "Không tìm thấy lịch sử thanh toán.")]
public async Task<IActionResult> LichSuThanhToan()
{
    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(maNguoiDung))
        return Unauthorized(new { Message = "Không xác định được người dùng." });

    const string query = @"
        SELECT t.*
        FROM ThanhToan t
        INNER JOIN HoaDon h ON t.MaHoaDon = h.MaHoaDon
        WHERE h.MaNguoiDung = @MaNguoiDung
        ORDER BY t.NgayThanhToan DESC";

    var lichSu = await _db.QueryAsync<KhachHangThanhToanDTO>(query, new { MaNguoiDung = maNguoiDung });
    // Lịch sử thanh toán
    if (lichSu == null || !lichSu.Any())
        return NotFound(new { Message = "❌ Xin lỗi, bạn chưa có lịch sử thanh toán nào." });
    return Ok(new { Message = "✅ Lấy lịch sử thanh toán thành công.", Data = lichSu });
}

        /// <summary>
        /// Lấy tổng hợp lịch sử giao dịch của khách hàng.
        /// </summary>
        [HttpGet("lichsu")]
        [SwaggerOperation(
            Summary = "Lấy tổng hợp lịch sử giao dịch",
            Description = "Trả về tổng hợp lịch sử đặt phòng, hóa đơn, thanh toán của khách hàng."
        )]
        [SwaggerResponse(200, "Tổng hợp lịch sử giao dịch.")]
        [SwaggerResponse(401, "Không xác định được người dùng.")]
        // Lấy tổng hợp lịch sử giao dịch
        
public async Task<IActionResult> GetLichSuGiaoDich()
{
    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(maNguoiDung))
        return Unauthorized(new { Message = "Không xác định được người dùng." });

    const string datPhongQuery = @"
        SELECT MaDatPhong, MaPhong, NgayDat, NgayCheckIn, NgayCheckOut, TinhTrangDatPhong
        FROM DatPhong
        WHERE MaNguoiDung = @MaNguoiDung";
    var datPhongs = await _db.QueryAsync(datPhongQuery, new { MaNguoiDung = maNguoiDung });

    const string hoaDonQuery = @"
        SELECT MaHoaDon, MaDatPhong, TongTien, NgayTaoHoaDon, NgayThanhToan, TinhTrangHoaDon
        FROM HoaDon
        WHERE MaNguoiDung = @MaNguoiDung";
    var hoaDons = await _db.QueryAsync(hoaDonQuery, new { MaNguoiDung = maNguoiDung });

    const string thanhToanQuery = @"
        SELECT t.MaThanhToan, t.MaHoaDon, t.SoTienThanhToan, t.NgayThanhToan, t.PhuongThucThanhToan, t.TinhTrangThanhToan
        FROM ThanhToan t
        INNER JOIN HoaDon h ON t.MaHoaDon = h.MaHoaDon
        WHERE h.MaNguoiDung = @MaNguoiDung";
    var thanhToans = await _db.QueryAsync(thanhToanQuery, new { MaNguoiDung = maNguoiDung });

    // Tổng hợp lịch sử giao dịch
    return Ok(new
    {
        Message = "✅ Lấy tổng hợp lịch sử giao dịch thành công.",
        DatPhongs = datPhongs,
        HoaDons = hoaDons,
        ThanhToans = thanhToans
    });
}

        /// <summary>
        /// Gửi feedback từ khách hàng.
        /// </summary>
        [HttpPost("feedback")]
        [SwaggerOperation(Summary = "Gửi feedback", Description = "Khách hàng gửi feedback, hệ thống tự phân loại tích cực/tiêu cực.")]
        [SwaggerResponse(200, "Gửi feedback thành công.")]
        [SwaggerResponse(401, "Không xác định được người dùng.")]
    public async Task<IActionResult> GuiFeedback([FromBody] KhachHangFeedBackDTO dto, [FromServices] SentimentModelConfig sentimentConfig)
{
    var maNguoiDung = User.FindFirstValue("sub") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrEmpty(maNguoiDung))
        return Unauthorized(new { Message = "Không xác định được người dùng." });

    // Phân tích cảm xúc bình luận
    string phanLoai = "Tiêu cực"; // Giá trị mặc định hợp lệ

if (!string.IsNullOrWhiteSpace(dto.BinhLuan))
{
    var mlContext = new MLContext();
    var model = mlContext.Model.Load(sentimentConfig.ModelPath, out var schema);
    var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

    var result = predictionEngine.Predict(new SentimentData { SentimentText = dto.BinhLuan });
    phanLoai = result.Prediction ? "Tích cực" : "Tiêu cực";
}

    // Sinh mã feedback tự động FBxxx
    const string getMaxSql = "SELECT ISNULL(MAX(CAST(SUBSTRING(MaFeedback, 3, LEN(MaFeedback)-2) AS INT)), 0) + 1 FROM Feedback";
    var nextId = await _db.ExecuteScalarAsync<int>(getMaxSql);
    var maFeedback = $"FB{nextId:D3}";

    // Lưu feedback vào DB
    const string insertQuery = @"
        INSERT INTO Feedback (MaFeedback, MaNguoiDung, MaPhong, SoSao, BinhLuan, PhanLoai, NgayFeedback)
        VALUES (@MaFeedback, @MaNguoiDung, @MaPhong, @SoSao, @BinhLuan, @PhanLoai, @NgayFeedback)";
    await _db.ExecuteAsync(insertQuery, new
    {
        MaFeedback = maFeedback,
        MaNguoiDung = maNguoiDung,
        MaPhong = dto.MaPhong,
        SoSao = dto.SoSao,
        BinhLuan = dto.BinhLuan,
        PhanLoai = phanLoai,
        NgayFeedback = DateTime.Now
    });

    // Gửi feedback thành công
    return Ok(new { Message = "🎉 Gửi feedback thành công! Cảm ơn bạn đã đóng góp ý kiến.", MaFeedback = maFeedback, PhanLoai = phanLoai });
}

        // ----------- Helper methods -----------
        private async Task<string> GenerateUniqueMaDatPhong()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatPhong, 3, LEN(MaDatPhong) - 2) AS INT)), 0) + 1
                FROM DatPhong";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"DP{nextId:D3}";
        }

        private async Task<string> GenerateUniqueMaDatDichVu()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaDatDichVu, 4, LEN(MaDatDichVu) - 3) AS INT)), 0) + 1
                FROM DatDichVu";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"DDV{nextId:D3}";
        }

        private async Task<string> GenerateUniqueMaHoaDon()
        {
            const string query = @"
                SELECT ISNULL(MAX(CAST(SUBSTRING(MaHoaDon, 3, LEN(MaHoaDon) - 2) AS INT)), 0) + 1
                FROM HoaDon";
            var nextId = await _db.ExecuteScalarAsync<int>(query);
            return $"HD{nextId:D3}";
        }
/// <summary>
/// Hủy thanh toán: xóa thanh toán, xóa hóa đơn, phục hồi trạng thái phòng và trạng thái đặt phòng.
/// </summary>
[HttpDelete("thanhtoan/huy/{maThanhToan}")]
[SwaggerOperation(
    Summary = "Hủy thanh toán",
    Description = "Xóa thanh toán, xóa hóa đơn liên quan, phục hồi trạng thái phòng và trạng thái đặt phòng về 1."
)]
[SwaggerResponse(200, "Hủy thanh toán thành công.")]
[SwaggerResponse(404, "Không tìm thấy mã thanh toán.")]
public async Task<IActionResult> HuyThanhToan([FromRoute] string maThanhToan)
{
    // Lấy mã hóa đơn từ thanh toán
    const string getHoaDonQuery = "SELECT MaHoaDon FROM ThanhToan WHERE MaThanhToan = @MaThanhToan";
    var maHoaDon = await _db.ExecuteScalarAsync<string>(getHoaDonQuery, new { MaThanhToan = maThanhToan });
    if (string.IsNullOrEmpty(maHoaDon))
        return NotFound(new { Message = "Không tìm thấy mã thanh toán." });

    // Lấy mã đặt phòng từ hóa đơn
    const string getDatPhongQuery = "SELECT MaDatPhong FROM HoaDon WHERE MaHoaDon = @MaHoaDon";
    var maDatPhong = await _db.ExecuteScalarAsync<string>(getDatPhongQuery, new { MaHoaDon = maHoaDon });
    if (string.IsNullOrEmpty(maDatPhong))
        return NotFound(new { Message = "Không tìm thấy hóa đơn liên quan." });

    // Lấy mã phòng từ đặt phòng
    const string getMaPhongQuery = "SELECT MaPhong FROM DatPhong WHERE MaDatPhong = @MaDatPhong";
    var maPhong = await _db.ExecuteScalarAsync<string>(getMaPhongQuery, new { MaDatPhong = maDatPhong });

    // Xóa thanh toán
    const string deleteThanhToan = "DELETE FROM ThanhToan WHERE MaThanhToan = @MaThanhToan";
    await _db.ExecuteAsync(deleteThanhToan, new { MaThanhToan = maThanhToan });

    // Xóa hóa đơn
    const string deleteHoaDon = "DELETE FROM HoaDon WHERE MaHoaDon = @MaHoaDon";
    await _db.ExecuteAsync(deleteHoaDon, new { MaHoaDon = maHoaDon });

    // Phục hồi trạng thái phòng về 1 (chưa đặt)
    const string updatePhong = "UPDATE Phong SET TinhTrang = 1 WHERE MaPhong = @MaPhong";
    await _db.ExecuteAsync(updatePhong, new { MaPhong = maPhong });

    // Phục hồi trạng thái đặt phòng về 1 (chưa đặt)
    const string updateDatPhong = "UPDATE DatPhong SET TinhTrangDatPhong = 1 WHERE MaDatPhong = @MaDatPhong";
    await _db.ExecuteAsync(updateDatPhong, new { MaDatPhong = maDatPhong });

    // Hủy thanh toán thành công
    return Ok(new { Message = "✅ Hủy thanh toán thành công. Phòng đã được mở lại cho khách khác." });
}

        /// <summary>
        /// Phân loại bình luận.
        /// </summary>
        /// <param name="binhLuan">Nội dung bình luận</param>
        [HttpPost("phanloai-binhluan")]
        [SwaggerOperation(Summary = "Phân loại bình luận", Description = "Nhập bình luận, trả về phân loại tích cực/tiêu cực.")]
public IActionResult PhanLoaiBinhLuan([FromBody] string binhLuan, [FromServices] SentimentModelConfig sentimentConfig)
{
    if (string.IsNullOrWhiteSpace(binhLuan))
        return BadRequest(new { Message = "❌ Bình luận không được để trống." });

    var mlContext = new MLContext();
    var model = mlContext.Model.Load(sentimentConfig.ModelPath, out var schema);
    var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

    var result = predictionEngine.Predict(new SentimentData { SentimentText = binhLuan });
    var phanLoai = result.Prediction ? "Tích cực" : "Tiêu cực";

    return Ok(new { Message = "✅ Phân loại bình luận thành công.", PhanLoai = phanLoai });
}
    }
}