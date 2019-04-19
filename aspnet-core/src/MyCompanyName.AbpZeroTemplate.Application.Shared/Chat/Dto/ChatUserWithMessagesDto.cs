using System.Collections.Generic;

namespace MyCompanyName.AbpZeroTemplate.Chat.Dto
{
    public class ChatUserWithMessagesDto : ChatUserDto
    {
        public List<ChatMessageDto> Messages { get; set; }
    }
}