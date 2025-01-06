using System;
using System.Net.Http;
using System.Threading.Tasks;

public class TelegramNotifier
{
    private readonly string _botToken;
    private readonly string _chatId;

    // Constructor cho phép truyền giá trị tùy chỉnh
    public TelegramNotifier(string botToken = "7553997923:AAFBabzEfRLdluri42vy3VixZwrLfv2BHPs", string chatId = "589101034")
    {
        _botToken = botToken;
        _chatId = chatId;
    }

    public async Task SendMessageAsync(string message)
    {
        using (HttpClient client = new HttpClient())
        {
            string url = $"https://api.telegram.org/bot{_botToken}/sendMessage?chat_id={_chatId}&text={Uri.EscapeDataString(message)}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Telegram API error: {response.StatusCode} - {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to send Telegram message: {ex.Message}");
            }
        }
    }
}
