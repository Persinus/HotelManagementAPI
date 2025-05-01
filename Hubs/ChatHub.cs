using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace HotelManagementAPI.Hus
{
    public class ChatHub : Hub
    {
        // Lưu ánh xạ người dùng với ConnectionId
        private static readonly ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();

        // Lưu trữ tin nhắn cho khách hàng khi họ offline
        private static readonly ConcurrentDictionary<string, List<string>> OfflineMessages = new ConcurrentDictionary<string, List<string>>();

        // Khi người dùng kết nối
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // Xác định người dùng từ UserIdentifier
            if (!string.IsNullOrEmpty(userId))
            {
                // Lưu ánh xạ kết nối người dùng
                UserConnections[userId] = Context.ConnectionId;

                // Kiểm tra và gửi lại tin nhắn đã lưu khi người dùng online
                SendOfflineMessagesToCustomer(userId, Context.ConnectionId);
            }
            return base.OnConnectedAsync();
        }

        // Khi người dùng ngắt kết nối
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.TryRemove(userId, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }

        // Phương thức khách hàng gửi tin nhắn tới quản trị viên
        public async Task SendMessageToAdmin(string maKhachHang, string message)
        {
            const string adminId = "QT001"; // Mã quản trị viên
            if (UserConnections.TryGetValue(adminId, out var adminConnectionId))
            {
                // Gửi tin nhắn tới quản trị viên khi họ online
                await Clients.Client(adminConnectionId).SendAsync("ReceiveMessage", maKhachHang, message);
            }
            else
            {
                // Nếu quản trị viên không online, gửi thông báo cho khách hàng
                await Clients.Caller.SendAsync("AdminNotAvailable", "Quản trị viên hiện không online.");
            }
        }

        // Phương thức quản trị viên gửi tin nhắn tới khách hàng
        public async Task SendMessageToCustomer(string maKhachHang, string message)
        {
            if (UserConnections.TryGetValue(maKhachHang, out var customerConnectionId))
            {
                // Gửi tin nhắn tới khách hàng khi họ online
                await Clients.Client(customerConnectionId).SendAsync("ReceiveMessage", "Admin", message);
            }
            else
            {
                // Nếu khách hàng không online, lưu tin nhắn vào bộ nhớ hoặc cơ sở dữ liệu
                SaveMessageForOfflineCustomer(maKhachHang, message);
                await Clients.Caller.SendAsync("CustomerOffline", "Khách hàng hiện không online, tin nhắn sẽ được gửi khi họ kết nối lại.");
            }
        }

        // Lưu tin nhắn cho khách hàng offline
        private void SaveMessageForOfflineCustomer(string maKhachHang, string message)
        {
            if (!OfflineMessages.ContainsKey(maKhachHang))
            {
                OfflineMessages[maKhachHang] = new List<string>();
            }

            OfflineMessages[maKhachHang].Add(message);
        }

        // Gửi lại tin nhắn đã lưu cho khách hàng khi họ online
        private void SendOfflineMessagesToCustomer(string maKhachHang, string connectionId)
        {
            if (OfflineMessages.TryGetValue(maKhachHang, out var messages))
            {
                foreach (var message in messages)
                {
                    Clients.Client(connectionId).SendAsync("ReceiveMessage", "Admin", message);
                }

                // Sau khi gửi hết tin nhắn, xóa các tin nhắn đã gửi
                OfflineMessages.TryRemove(maKhachHang, out _);
            }
        }
    }
}
