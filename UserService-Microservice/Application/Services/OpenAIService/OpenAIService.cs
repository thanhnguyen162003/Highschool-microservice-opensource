using Application.Common.Models.Common;
using Application.Common.Models.Settings;
using Domain.Enumerations;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly ChatClient _client;
        private readonly AISetting _aiSetting;
        private const int MAX_SUMMARY_WORD_COUNT = 20;
        private const int MAX_DESCRIPTION_WORD_COUNT = 80;
        private const int MAX_OVERALL_SUMMARY_WORD_COUNT = 50;

        public OpenAIService(IOptions<AISetting> options)
        {
            _aiSetting = options.Value;
            _client = new ChatClient(model: "gpt-4o", apiKey: _aiSetting.OpenAIKey);
        }

        public async Task<ResponseModel> WriteContent(TypeHelperContent type, string text)
        {
            var chatMessages = new List<ChatMessage>();

            switch (type)
            {
                case TypeHelperContent.Improve:
                    chatMessages.Add(new SystemChatMessage("Bạn là một Biên Tập Viên chuyên nghiệp, một người đánh giá nội dung, chuyên sửa chữa và hướng dẫn để giúp các bài viết tốt hơn."));
                    chatMessages.Add(new UserChatMessage("Tôi đã viết đoạn văn sau bằng tiếng Việt, nhưng muốn bạn giúp cải thiện để nó trở nên trau chuốt, " +
                        "tự nhiên và chuyên nghiệp hơn. Vui lòng tập trung vào ngữ pháp, từ vựng, cấu trúc câu và sự mạch lạc tổng thể. " +
                        "Đây là bài viết mang lại thông tin với đối tượng là dành cho học sinh, phụ huynh và thầy cô giáo, những người liên quan tới giáo dục. " +
                        "Hãy thay thế các cụm từ, câu văn phù hợp hơn với ngữ cảnh nhưng vẫn giữ nguyên ý nghĩa ban đầu. " +
                        "Hãy định dạng kết quả bằng HTML đẹp mắt với style inline, chỉ trả về nội dung bên trong thẻ `<body>` mà không chứa ký tự xuống dòng ('\\n')." +
                        "Nội dung bài viết như sau: \n" +
                        $"{text}"));
                    break;
                case TypeHelperContent.Generate:
                    chatMessages.Add(new SystemChatMessage("Bạn là một Biên Tập Viên và Nhà Sáng Tạo Nội Dung chuyên nghiệp. Hãy tạo ra một bài viết sáng tạo, hấp dẫn và phù hợp với ngữ cảnh được cung cấp."));
                    chatMessages.Add(new UserChatMessage("Tôi muốn bạn viết một bài hoàn toàn mới, chủ đề xoay quanh giáo dục, phù hợp với học sinh, phụ huynh và thầy cô giáo. " +
                        "Bài viết cần mang tính sáng tạo, cung cấp thông tin giá trị, và có thể sử dụng trong các tài liệu truyền thông. " +
                        "Hãy định dạng kết quả bằng HTML đẹp mắt với style inline, chỉ trả về nội dung bên trong thẻ `<body>` mà không chứa ký tự xuống dòng ('\\n')." +
                        "Đây là yêu cầu cụ thể: \n" +
                        $"{text}"));
                    break;
                case TypeHelperContent.MoreConent:
                    chatMessages.Add(new SystemChatMessage("Bạn là một Biên Tập Viên và Nhà Phát Triển Nội Dung. Hãy mở rộng bài viết hiện tại, thêm các ý tưởng sáng tạo hoặc thông tin bổ ích để làm phong phú nội dung."));
                    chatMessages.Add(new UserChatMessage("Đây là một bài viết gốc cần được mở rộng. Vui lòng phát triển thêm nội dung, đảm bảo bài viết trở nên toàn diện và hấp dẫn hơn. " +
                        "Hãy giữ giọng văn phù hợp với đối tượng mục tiêu và thêm những ý tưởng sáng tạo phù hợp. " +
                        "Hãy định dạng kết quả bằng HTML đẹp mắt với style inline, chỉ trả về nội dung bên trong thẻ `<body>` mà không chứa ký tự xuống dòng ('\\n')." +
                        "Nội dung hiện tại như sau: \n" +
                        $"{text}"));
                    break;
                default:
                    return new ResponseModel
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = "Type helper not found."
                    };
            }

            try
            {
                var response = await _client.CompleteChatAsync(chatMessages);

                return new ResponseModel
                {
                    Status = HttpStatusCode.OK,
                    Message = "Generate complete.",
                    Data = response.Value.Content.ElementAt(0).Text
                };
            } catch
            {
                return new ResponseModel
                {
                    Status = HttpStatusCode.InternalServerError,
                    Message = "Generate failed."
                };
            }

        }

        public async Task<(List<string>? features, List<string>? descriptions)> WriteBriefMBTI(MBTIType mbtiType)
        {
            var chatMessages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    $"Sử dụng tiếng Việt trong câu trả lời. Bạn là một nhà phân tích tâm lý học chuyên nghiệp, và nhiệm vụ của bạn là phân tích và giải thích các kiểu MBTI. Với mỗi kiểu MBTI được cung cấp, bạn cần tạo ra 3 đặc điểm chính và 3 mô tả tương ứng như sau:\n\n" +
                    $"1. **Đặc điểm:** Cung cấp mô tả ngắn gọn, rõ ràng và dễ hiểu (tối thiểu {MAX_SUMMARY_WORD_COUNT} từ). Đặc điểm này nên thể hiện bản chất hoặc hành vi đặc trưng của kiểu MBTI mà không nhắc đến thuật ngữ chuyên môn phức tạp.\n\n" +
                    $"2. **Mô tả:** Giải thích chi tiết từng đặc điểm với ít nhất {MAX_DESCRIPTION_WORD_COUNT} mỗi đoạn, có 2 đoạn rõ ràng. Mỗi đoạn phải cách nhau bằng ký tự \\n để xuống dòng, và cần cung cấp các ví dụ thực tế hoặc tình huống minh họa để làm nổi bật ý nghĩa của đặc điểm.\n\n" +
                    $"Kết quả phải được trả về theo định dạng sau:\n" +
                    $"Đặc điểm 1: [Nội dung đặc điểm, tối thiểu {MAX_SUMMARY_WORD_COUNT} từ]\n" +
                    $"Mô tả 1: [Nội dung mô tả, viết thành 2 đoạn, tối thiểu {MAX_DESCRIPTION_WORD_COUNT} từ mỗi đoạn]\n\n" +
                    $"Đặc điểm 2: [Nội dung đặc điểm, tối thiểu {MAX_SUMMARY_WORD_COUNT} từ]\n" +
                    $"Mô tả 2: [Nội dung mô tả, viết thành 2 đoạn, tối thiểu {MAX_DESCRIPTION_WORD_COUNT} từ mỗi đoạn]\n\n" +
                    $"Đặc điểm 3: [Nội dung đặc điểm, tối thiểu {MAX_SUMMARY_WORD_COUNT} từ]\n" +
                    $"Mô tả 3: [Nội dung mô tả, viết thành 2 đoạn, tối thiểu {MAX_DESCRIPTION_WORD_COUNT} từ mỗi đoạn]\n\n" +
                    $"Hãy đảm bảo rằng câu trả lời của bạn súc tích, có tính ứng dụng cao, và dễ hiểu đối với mọi đối tượng. Không sử dụng các kí hiệu đặc biệt như * @ # $ trong câu trả lời."
                ),
                new UserChatMessage($"Từ kết quả {mbtiType.ToString()}, bạn có thể tóm tắt được: ")
            };

            try
            {
                List<string>? features, descriptions;
                int retryCount = 0;
                do
                {
                    var response = await _client.CompleteChatAsync(chatMessages);

                    var responseText = response.Value.Content.ElementAt(0).Text;

                    (features, descriptions) = ParseResponseText(responseText);

                    retryCount++;
                }
                while ((!IsValidContentList(features) || !IsValidContentList(descriptions)) && retryCount < 3);

                return (features, descriptions);
            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        public bool IsValidContentList(List<string>? listString)
        {
            return listString == null || !listString.Any() || listString.Where(content => string.IsNullOrWhiteSpace(content)).FirstOrDefault() != null;
        }

        public async Task<(List<string>? features, List<string>? descriptions)> WriteBriefHolland(string hollandType)
        {
            var chatMessages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    $"Sử dụng tiếng Việt trong câu trả lời. Bạn là một nhà phân tích tâm lý học chuyên nghiệp, có nhiệm vụ phân tích và giải thích các kết quả Holland Code. Với mỗi kết quả Holland Code được cung cấp, bạn cần tạo ra 3 đặc điểm chính và 3 mô tả chi tiết tương ứng như sau:\n\n" +
                    $"1. **Đặc điểm:** Mô tả một cách ngắn gọn, rõ ràng và có tính miêu tả cụ thể (tối thiểu {MAX_SUMMARY_WORD_COUNT} từ). Đặc điểm này nên nhấn mạnh vào các phẩm chất hoặc hành vi nổi bật của kết quả Holland Code mà không sử dụng các thuật ngữ quá phức tạp.\n\n" +
                    $"2. **Mô tả:** Giải thích chi tiết từng đặc điểm với tối thiểu {MAX_DESCRIPTION_WORD_COUNT} từ mỗi đoạn, có 2 đoạn rõ ràng. Các đoạn phải cách nhau bằng ký tự \\n để xuống dòng, và nội dung cần tập trung vào việc cung cấp ví dụ thực tế hoặc tình huống minh họa phù hợp.\n\n" +
                    $"Câu trả lời cần được trình bày theo định dạng sau:\n\n" +
                    $"Đặc điểm 1: [Nội dung đặc điểm, tối thiểu {MAX_SUMMARY_WORD_COUNT} từ]\n" +
                    $"Mô tả 1: [Nội dung mô tả, viết thành 2 đoạn, tối thiểu {MAX_DESCRIPTION_WORD_COUNT} từ mỗi đoạn]\n\n" +
                    $"Đặc điểm 2: [Nội dung đặc điểm, tối thiểu {MAX_SUMMARY_WORD_COUNT} từ]\n" +
                    $"Mô tả 2: [Nội dung mô tả, viết thành 2 đoạn, tối thiểu {MAX_DESCRIPTION_WORD_COUNT} từ mỗi đoạn]\n\n" +
                    $"Đặc điểm 3: [Nội dung đặc điểm, tối thiểu {MAX_SUMMARY_WORD_COUNT} từ]\n" +
                    $"Mô tả 3: [Nội dung mô tả, viết thành 2 đoạn, tối thiểu {MAX_DESCRIPTION_WORD_COUNT} từ mỗi đoạn]\n\n" +
                    $"Hãy đảm bảo rằng câu trả lời súc tích, dễ hiểu và có tính ứng dụng cao. Nội dung phải phù hợp với bối cảnh tâm lý học và hướng đến người dùng phổ thông. Không sử dụng các kí hiệu đặc biệt như * @ # $ trong câu trả lời."
                ),
                new UserChatMessage($"Từ kết quả Holland: {hollandType}, bạn có thể kết luận được: ")
            };

            try
            {
                List<string>? features, descriptions;
                int retryCount = 0;
                do
                {
                    var response = await _client.CompleteChatAsync(chatMessages);

                    var responseText = response.Value.Content.ElementAt(0).Text;

                    (features, descriptions) = ParseResponseText(responseText);

                    retryCount++;
                }
                while ((!IsValidContentList(features) || !IsValidContentList(descriptions)) && retryCount < 3);

                return (features, descriptions);

            }
            catch (Exception ex)
            {
                return (null, null);
            }
        }

        public async Task<List<string>?> WriteBrief(MBTIType mbtiType, string hollandTrait)
        {
            var chatMessages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    $"Bạn là một nhà phân tích tâm lý học. Với mỗi kết quả MBTI và Holland được cung cấp, hãy viết ra 3 đặc điểm chính của người dùng theo định dạng sau:\n" +
                    $"Đặc điểm 1: [Mô tả ngắn gọn, tối thiểu {MAX_OVERALL_SUMMARY_WORD_COUNT} từ]\\n" +
                    $"Đặc điểm 2: [Mô tả ngắn gọn, tối thiểu {MAX_OVERALL_SUMMARY_WORD_COUNT} từ]\n" +
                    $"Đặc điểm 3: [Mô tả ngắn gọn, tối thiểu {MAX_OVERALL_SUMMARY_WORD_COUNT} từ]\n" +
                    "Tránh việc nhắc lại cụ thể các kết quả MBTI hoặc Holland trong đặc điểm. Tập trung vào việc miêu tả các tính cách nổi bật, phong cách giao tiếp, hoặc xu hướng hành vi của người dùng. Sử dụng tiếng Việt trong câu trả lời. Không sử dụng các kí hiệu đặc biệt như * @ # $ trong câu trả lời."
                ),
                new UserChatMessage($"Từ kết quả MBTI: {mbtiType.ToString()} và Holland: {hollandTrait}, bạn có thể tóm tắt được: ")
            };

            try
            {
                var response = await _client.CompleteChatAsync(chatMessages);

                var responseText = response.Value.Content.ElementAt(0).Text;

                var features = new List<string>();

                var lines = responseText.Split('\n');
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line)) continue;
                        features.Add(line.Split(':')[1].Trim());
                }

                return features;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<String> GenerateBio(DateTime Birthdate)
        {
            var chatMessages = new List<ChatMessage>();
            chatMessages.Add(new SystemChatMessage(
                "Bạn là một người chuyên viết các bio cho mọi người, " +
                "Dựa trên ngày, tháng, năm sinh của tôi, hãy tạo ra một đoạn Bio ngắn gọn về tôi từ 4-5 câu, chia thành 2-3 đoạn và 1 đoạn fun fact về tôi. Bio nên phản ánh cá tính, sở thích, hoặc tiềm năng của người đó dựa trên các đặc điểm chiêm tinh hoặc các yếu tố liên quan đến thời điểm sinh, nhưng cần sáng tạo, thú vị và không quá nghiêng về bói toán, viết theo phong cách tự nhiên, gần gũi. Dùng danh xưng tôi trong câu và bỏ nhắc lại ngày tháng năm sinh"
            ));

            chatMessages.Add(new UserChatMessage($"Tôi sinh vào ngày {Birthdate.Day} tháng {Birthdate.Month} năm {Birthdate.Year}"));

            try
            {
                var response = await _client.CompleteChatAsync(chatMessages);

                return response.Value.Content.ElementAt(0).Text;
            } catch
            {
                return "";
            }
        }

        private (List<string> features, List<string> descriptions) ParseResponseText(string responseText)
        {
            var features = new List<string>();
            var descriptions = new List<string>();

            var lines = responseText.Split('\n');
            bool isDescription = false;
            StringBuilder description = new StringBuilder();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                if (Regex.IsMatch(trimmedLine, @"^\s{0,20}-?\s*Đặc điểm\b"))
                {
                    isDescription = false;
                    if (description.Length > 0)
                    {
                        descriptions.Add(description.ToString().Trim());
                        description.Clear();
                    }
                    var parts = trimmedLine.Split(':', 2);
                    if (parts.Length > 1)
                    {
                        features.Add(parts[1].Trim());
                    }
                }
                else if (Regex.IsMatch(trimmedLine, @"^\s{0,20}-?\s*Mô tả\b"))
                {
                    isDescription = true;
                    var parts = trimmedLine.Split(':', 2);
                    if (parts.Length > 1)
                    {
                        description.Append(parts[1].Trim() + "\n");
                    }
                }
                else
                {
                    if (isDescription)
                    {
                        description.Append(trimmedLine + "\n");
                    }
                }
            }

            if (description.Length > 0)
            {
                descriptions.Add(description.ToString().Trim());
            }

            return (features, descriptions);
        }

    }
}
